using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using AForge.Vision.Motion;
using AForge.Video.DirectShow;


namespace Futech.Video
{
    public partial class CameraWindow : UserControl
    {
        private Control camera;
        private ICameraWindow icamera;

        // new frame event
        public event NewFrameHandler NewFrame;

        // motion event
        public event MotionEventHandler MotionEvent;

        public event VideoInputEventHandler VideoInput;

        private IVideoInput ivideo;

        // motion detection and processing algorithm
        private int motionDetectionType = 1;
        private int motionProcessingType = 1;

        // motion detector
        private MotionDetector detector = null;// new MotionDetector(null, null);
        //new TwoFramesDifferenceDetector(),
        //new MotionAreaHighlighting());

        // motion alarm level
        private float motionAlarmLevel = 0.05f;

        // Chu ky phat hien chuyen dong
        private int motionDetectionInterval = 0;

        // So chuyen dong phat hien duoc
        private int motionDetectionCount = 0;

        private bool startMotionDetector = false;

        // capture worker
        private BackgroundWorker captureWorker = null;

        //private NVRWebServer webServer = new NVRWebServer();
        private bool enableWebServer = false;

        public CameraWindow()
        {
            InitializeComponent();
           // Graphics.DrawRectangle(Pens.Red, new Rectangle(10,10,200,200));
        }

        public static string[] LoadSDKList()
        {
            return new string[] { "AForgeSDK", "AxisSDK", "GeoSDK", "EtroSDK", "VLCSDK", "ScSDK", "FFMPEG","VLC", "KztekSDK" };
        }

        public static string[] LoadMediaTypeList()
        {
            return new string[] { "MJPEG", "PlayFile", "Local Video Capture Device", "JPEG", "MPEG4", "H264" };
        }

        public static string[] LoadCameraType()
        {
            return new string[] {"Vantech","Secus" };
        }

        public static string[] LoadCgiList()
        {
            return new string[] 
                { 
                    " ----------Panasonic iPro-----------------------------------------------",
                    "/cgi-bin/mjpeg?framerate=30&resolution=640x360",  
                    "/cgi-bin/mjpeg?framerate=30&resolution=640x480",  
                    "/cgi-bin/mjpeg?framerate=15&resolution=640x480",  
                    "/cgi-bin/mjpeg?framerate=30&resolution=800x600",  
                    "/cgi-bin/mjpeg?framerate=15&resolution=800x600",  
                    "/cgi-bin/mjpeg?framerate=30&resolution=1280x720",
                    "/cgi-bin/mjpeg?framerate=15&resolution=1280x720",
                    "/cgi-bin/mjpeg?framerate=30&resolution=1280x960",
                    "/cgi-bin/mjpeg?framerate=15&resolution=1280x960",
                    "/cgi-bin/mjpeg?framerate=30&resolution=1920x1080",
                    "/cgi-bin/mjpeg?framerate=15&resolution=1920x1080",
                    "/cgi-bin/mjpeg?framerate=30&resolution=2048x1536",
                    "/cgi-bin/mjpeg?framerate=15&resolution=2048x1536",
                    "/cgi-bin/mjpeg?connect=start&resolution=640&UID=263&page=20040830203157", 
                    " ----------Axis---------------------------------------------------------",
                    "/axis-cgi/mjpg/video.cgi?resolution = 640x400", 
                    "/axis-cgi/mjpg/video.cgi?camera=1", 
                    " ----------Futech-------------------------------------------------------",
                    "/FutechCGI?camera=0",
                    " ----------Lilin--------------------------------------------------------",
                    "/getimage?camera=1&fmt=qsif", // 192x144
                    "/getimage?camera=1&fmt=sif", // 320x240
                    "/getimage?camera=1&fmt=full", // full
                    " ----------Sony---------------------------------------------------------",
                    "/image",
                    " ----------VIVOTEK------------------------------------------------------",
                    "video.mjpg",
                    "/cgi-bin/viewer/video.jpg",
                    "/cgi-bin/viewer/video.jpg?resolution=640x480",
                    "/cgi-bin/viewer/video.jpg?resolution=800x600",
                    "/cgi-bin/viewer/video.jpg?resolution=1280x720",
                    "/cgi-bin/viewer/video.jpg?resolution=1280x960",
                    "/cgi-bin/viewer/video.jpg?resolution=1280x1080",
                    "/cgi-bin/viewer/video.jpg?resolution=1920x1280",
                    " ----------Arecont Vision-----------------------------------------------",
                    "/img.jpg", // Jpeg
                    "/image?res=full&x0=0&y0=0&x1=1600&y1=1200&quality=12&doublescan=0", // Jpeg
                    "/mjpeg?res=full&x0=0&y0=0&x1=1600&y1=1200&quality=12&doublescan=0", // MJPEG
                    "/h264f?res=full&x0=0&y0=0&x1=1600&y1=1200&quality=12&doublescan=0&bitrate=2048&ssn=1", // Individual H264
                    "/h264stream?res=full&x0=0&y0=0&x1=1600&y1=1200&qp=26&ssn=1&doublescan=0&bitrate=2048", // Continuous H.264
                    " ----------MESSOA-------------------------------------------------------",
                    "/cgi-bin/videoconfiguration.cgi", // MJPEG
                    "-----------SHANY---------------------------------------------------------",
                    "/live/stream2.cgi",
                     "----------Bosch----------",
                    "/snap.jpg?",
                    "Vantech",
                    "Secus",
                    "/cgi-bin/video.cgi?camera=1",
                    "/cgi-bin/image/mjpeg.cgi",
                    "Surveon",
                    "Shany",
                    "Dahua"
                };
        }

        public static string[] LoadDeviceList()
        {
            List<string> deviceList = new List<string>();
            // show device list
            try
            {
                FilterInfoCollection videoDevices;
                //string device = "";
                // enumerate video devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                // add all devices to combo
                foreach (FilterInfo device in videoDevices)
                {
                    deviceList.Add(device.Name);
                }
            }
            catch (ApplicationException)
            {
                deviceList.Add("No local capture devices");
            }

            string[] devices = new string[deviceList.Count];
            deviceList.CopyTo(devices);
            return devices;
        }

        private string cameraName = "";
        public string CameraName
        {
            get { return cameraName; }
        }

        private string cameratype = "";
        public string CameraType
        {
            get { return cameratype; }
            set { cameratype = value; }
        }

        private string resolution = "640x480";
        public string Resolution
        {
            get { return resolution; }
            set { resolution = value; }
        }

        public ICameraWindow iCamera
        {
            get { return icamera; }
           
        }


        // Tao cua so camera
        public void CreateCameraControl(string sdk)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Futech.Video.CameraAForgeSDK ||
                    control is Futech.Video.CameraAxisSDK ||
                    control is Futech.Video.CameraEtroSDK ||
                    control is Futech.Video.CameraFFMPEG ||
                    control is Futech.Video.CameraGeoSDK ||
                    control is Futech.Video.CameraVLC||
                    control is Futech.Video.CameraScSDK||
                    control is Futech.Video.CameraKztek ||
                    control is Futech.Video.CameraHIKSDK ||
                    control is Futech.Video.CameraTiandySDK||
                    control is Futech.Video.CameraKztek2 ||
                    control is Futech.Video.CameraKztek3
                    )
                    this.Controls.Remove(control);
            }
            switch (sdk)
            {
                case "AxisSDK": // Axis
                    camera = new Futech.Video.CameraAxisSDK();
                    break;
                case "AForgeSDK": // AForgeSDK
                    camera = new Futech.Video.CameraAForgeSDK();
                    break;
                case "GeoSDK": // Geovision
                    camera = new Futech.Video.CameraGeoSDK();
                    break;
                case "EtroSDK": // Etrovision
                    camera = new Futech.Video.CameraEtroSDK();
                    break;
                case "VLCSDK": // VLC
                               //camera = new Futech.Video.CameraGeoSDK();
                    break;
                case "ScSDK":
                    camera = new Futech.Video.CameraScSDK();
                    break;
                case "FFMPEG":
                    camera = new Futech.Video.CameraFFMPEG();
                    break;
                case "VLC":
                    camera = new Futech.Video.CameraVLC();
                    break;
                case "KztekSDK":
                    camera = new Futech.Video.CameraKztek();
                    break;
                case "HIKSDK":
                    camera = new Futech.Video.CameraHIKSDK();
                    break;
                case "TiandySDK":
                    camera = new Futech.Video.CameraTiandySDK();
                    break;
                case "KztekSDK2":
                    camera = new Futech.Video.CameraKztek2();
                    break;
                case "KztekSDK3":
                    camera = new Futech.Video.CameraKztek3();
                    break;
            }
            if (camera != null)
            
            {
                this.Controls.Add(camera);
                camera.Dock = DockStyle.Fill;
                // Thiet lap tham so cho camera
                icamera = ((ICameraWindow)camera);
            }
        }

        // Huy dieu khien
        public void DisposeCamera()
        {
            if (camera != null)
            {
                camera.Dispose();
            }
        }

        public void Start(string name, string source, int httpPort, int rtspPort, string login, string password, string mediatype, int channel, string cgi, bool displayCameraTitle)
        {
            try
            {
                if (camera != null)
                {
                    icamera.CameraName = cameraName = name;
                    icamera.VideoSource = source;
                    icamera.Resolution = resolution;
                    icamera.HttpPort = httpPort;
                    icamera.RtspPort = rtspPort;
                    icamera.Login = login;
                    icamera.Password = password;
                    icamera.MediaType = mediatype;
                    icamera.Cgi = cgi;
                    icamera.Channel = channel;
                    icamera.DisplayCameraTitle = displayCameraTitle;
                    icamera.CameraType = cameratype;
                    icamera.EnableRecording = enablerecording;
                    icamera.VideoFolder = videofolder;
                    if (NewFrame != null)
                        icamera.NewFrame += new NewFrameHandler(video_NewFrame);
                    icamera.Start();
                    GC.KeepAlive(this.icamera);

                    // initial capture worker
                    InitializeCaptureWorker();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Stop()
        {
            try
            {
                // reset motion detector
                if (detector != null)
                    detector.Reset();
                detector = null;

                if (icamera != null)
                {
                    icamera.Stop();

                    if (captureWorker != null)
                    {
                        int i = 0;
                        while (captureWorker.IsBusy)
                        {
                            i = i + 1;
                            Thread.Sleep(50);
                            if (i <= 20)
                                Application.DoEvents();
                            else
                                break;
                        }
                    }
                }
                // close capture worker
                CloseCaptureWorker();

                if (ivideo != null)
                {
                    ivideo.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ReConnect()
        {
            if (icamera != null)
            {
                icamera.Stop();
                Thread.Sleep(500);
                icamera.Start();
            }            
        }

        // Luu hinh anh camera
        public void SaveCurrentImage(string theFile)
        {
            if (icamera != null)
                icamera.SaveCurrentImage(theFile);
        }

        // Get anh
        public Bitmap GetCurrentImage()
        {
            if (icamera != null)
            {
                return icamera.GetCurrentImage();
            }
            else
                return null;
        }

        public float Fps()
        {
            if (icamera != null)
                return icamera.Fps;
            else
                return 0;
        }

        public int Channel()
        {
            if (icamera != null)
                return icamera.Channel;
            else
                return 0;
        }

        public bool EnableWebServer
        {
            get { return enableWebServer; }
            set { enableWebServer = value; }
        }

        public int Status
        {
            get { return icamera.Status; }
        }

        //public NVRWebServer WebServer
        //{
        //    get { return webServer; }
        //    set { webServer = value; }
        //}

        public Rectangle[] UserWindows
        {
            get
            {
                if (icamera != null)
                    return icamera.UserWindows;
                else
                    return null;
            }
            set
            {
                if (icamera != null)
                    icamera.UserWindows = value;
            }
        }

        /// <summary>
        /// BorderColor
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get
            {
                if (icamera != null) return icamera.BorderColor;
                else return Color.Black;
            }
            set
            {
                if (icamera != null)
                    icamera.BorderColor = value;
            }
        }

        // Start Motion Detector
        public bool StartMotionDetector
        {
            set { startMotionDetector = value; }
        }

        // Set MotionZone
        public Rectangle[] MotionZones
        {
            get
            {
                if (icamera != null)
                    return icamera.MotionZones;
                else
                    return null;
            }
            set
            {
                if (icamera != null)
                {
                    icamera.MotionZones = value;
                    if (detector != null)
                        detector.MotionZones = value;
                }
            }
        }

        public void SetMotionDetector(Rectangle[] motionZones, int _motionDetectionType, int _motionProcessingType, float _motionAlarmLevel, int _motionDetectionInterval)
        {
            try
            {
                detector = new MotionDetector(null, null);

                // Set new motion processing algorithm
                motionProcessingType = _motionProcessingType; // 3;
                switch (motionProcessingType)
                {
                    case 0: // Turn off motion processing
                        SetMotionProcessingAlgorithm(null, ref detector);
                        break;
                    case 1: // Set motion area highlighting
                        SetMotionProcessingAlgorithm(new MotionAreaHighlighting(), ref detector);
                        break;
                    case 2: // Set motion borders highlighting
                        SetMotionProcessingAlgorithm(new MotionBorderHighlighting(), ref detector);
                        break;
                    case 3: // Set objects' counter
                        SetMotionProcessingAlgorithm(new BlobCountingObjectsProcessing(), ref detector);
                        break;
                    case 4: // Set grid motion processing
                        SetMotionProcessingAlgorithm(new GridMotionAreaProcessing(32, 32), ref detector);
                        break;
                }

                // Set new motion detection algorithm
                motionDetectionType = _motionDetectionType; // 2
                switch (motionDetectionType)
                {
                    case 0:  // Turn off motion detection
                        SetMotionDetectionAlgorithm(null, ref detector);
                        break;
                    case 1: // Set Two Frames Difference motion detection algorithm
                        SetMotionDetectionAlgorithm(new TwoFramesDifferenceDetector(), ref detector);
                        break;
                    case 2: // Set Simple Background Modeling motion detection algorithm
                        SetMotionDetectionAlgorithm(new SimpleBackgroundModelingDetector(true, true), ref detector);
                        break;
                }

                detector.MotionZones = motionZones;
                MotionZones = motionZones;

                motionAlarmLevel = _motionAlarmLevel;
                motionDetectionInterval = _motionDetectionInterval;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Set new motion detection algorithm
        private void SetMotionDetectionAlgorithm(IMotionDetector detectionAlgorithm, ref MotionDetector detector)
        {
            lock (this)
            {
                detector.MotionDetectionAlgorithm = detectionAlgorithm;

                if (detectionAlgorithm is TwoFramesDifferenceDetector)
                {
                    if (
                        (detector.MotionProcessingAlgorithm is MotionBorderHighlighting) ||
                        (detector.MotionProcessingAlgorithm is BlobCountingObjectsProcessing))
                    {
                        motionProcessingType = 1;
                        SetMotionProcessingAlgorithm(new MotionAreaHighlighting(), ref detector);
                    }
                }
            }
        }

        // Set new motion processing algorithm
        private void SetMotionProcessingAlgorithm(IMotionProcessing processingAlgorithm, ref MotionDetector detector)
        {
            lock (this)
            {
                detector.MotionProcessingAlgorithm = processingAlgorithm;
            }
        }

        // On new frame
        private void video_NewFrame(object sender, NewFrameArgs e)
        {
            if (captureWorker != null && !captureWorker.IsBusy)
            {
                captureWorker.RunWorkerAsync();
            }
        }

        #region capture worker
        // Init capture background worker
        private void InitializeCaptureWorker()
        {
            try
            {
                this.captureWorker = new BackgroundWorker();
                this.captureWorker.WorkerSupportsCancellation = true;
                this.captureWorker.WorkerReportsProgress = true;
                this.captureWorker.DoWork += new DoWorkEventHandler(this.captureWorker_DoWork);
                this.captureWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.captureWorker_RunWorkerCompleted);
                this.captureWorker.ProgressChanged += new ProgressChangedEventHandler(this.captureWorker_ProgressChanged);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error while initial capture worker " + cameraWindowC1.Name + "\n" + ex.ToString());
            }
        }
        // capture image
        private void captureWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                if (!worker.CancellationPending)
                {
                    worker.ReportProgress(0, GetCurrentImage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in capture worker Do_Work: " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        // capture worker progress change
        private void captureWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                Bitmap lastFrame = (Bitmap)e.UserState;
                if (lastFrame != null)
                {
                    if (NewFrame != null)
                        NewFrame(this, new NewFrameArgs(lastFrame));

                    // WebServer
                    //if (enableWebServer)
                    //{
                    //    if (!NVRWebServer.CurrentFrames.Contains(Channel().ToString()))
                    //        NVRWebServer.CurrentFrames.Add(Channel().ToString(), new Bitmap(lastFrame));
                    //    else
                    //        NVRWebServer.CurrentFrames[Channel().ToString()] = new Bitmap(lastFrame);
                    //}

                    // Motion Detector
                    if (detector != null && startMotionDetector)
                    {
                        float motionLevel = detector.ProcessFrame(lastFrame);

                        if (motionLevel >= motionAlarmLevel)
                        {
                            if (motionDetectionCount == -1 || motionDetectionInterval == 0 || (motionDetectionInterval > 0 && motionDetectionCount % motionDetectionInterval == 0))
                            {
                                motionDetectionCount = 0;
                                icamera.BorderColor = Color.Red;
                                // Motion Event
                                if (MotionEvent != null)
                                {
                                    MotionEventArgs e1 = new MotionEventArgs(lastFrame, motionLevel);
                                    MotionEvent(this, e1);
                                }
                            }
                            motionDetectionCount = motionDetectionCount + 1;
                        }
                        else
                        {
                            motionDetectionCount = -1;
                            icamera.BorderColor = Color.Transparent;
                        }
                    }
                    else icamera.BorderColor = Color.Transparent;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // capture worker complete
        private void captureWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        // close capture worker
        public void CloseCaptureWorker()
        {
            try
            {
                if (captureWorker != null)
                {
                    this.captureWorker.DoWork -= new DoWorkEventHandler(this.captureWorker_DoWork);
                    this.captureWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.captureWorker_RunWorkerCompleted);
                    this.captureWorker.ProgressChanged -= new ProgressChangedEventHandler(this.captureWorker_ProgressChanged);
                    captureWorker.CancelAsync();
                    captureWorker = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion        

        public uint Get_BRIGHTNESS()
        {

            if (icamera != null)
                return icamera.Get_BRIGHTNESS();
            return 0;
        }

        public void Set_BRIGHTNESS(uint pbrightness)
        {

            if (icamera != null)
                icamera.Set_BRIGHTNESS(pbrightness);
        }

        //contrast
        public uint Get_CONTRAST()
        {
            if (icamera != null)
                return icamera.Get_CONTRAST();
            return 0;
        }
        public void Set_CONTRAST(uint pcontrast)
        {
            if (icamera != null)
                icamera.Set_CONTRAST(pcontrast);
        }

        //hue
        public uint Get_HUE()
        {
            if (icamera != null)
                return icamera.Get_HUE();
            return 0;
        }
        public void Set_HUE(uint phue)
        {
            if (icamera != null)
                icamera.Set_HUE(phue);
        }

        //saturation
        public uint Get_SATURATION()
        {
            if (icamera != null)
                return icamera.Get_SATURATION();
            return 0;
        }
        public void Set_SATURATION(uint psaturation)
        {
            if (camera != null)
                icamera.Set_SATURATION(psaturation);
        }

        //sharpness
        public uint Get_SHARPNESS()
        {
            if (icamera != null)
                return icamera.Get_SHARPNESS();
            return 0;
        }
        public void Set_SHARPNESS(uint psharpness)
        {
            if (icamera != null)
                icamera.Set_SHARPNESS(psharpness);
        }

        private void tsmSnapshot_Click(object sender, EventArgs e)
        {

        }

        private void tsmView_Click(object sender, EventArgs e)
        {
            frmView frm = new frmView();
            frm.Source = icamera.VideoSource;
            frm.Login = icamera.Login;
            frm.Password = icamera.Password;
            frm.Channel = icamera.Channel;
            frm.MediaType = icamera.MediaType;
            frm.Cgi = icamera.Cgi;

            icamera.Stop();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                icamera.Start();
            }
        }

        private bool enablerecording = false;
        public bool EnableRecording
        {
            get { return enablerecording; }
            set { enablerecording = value; }
        }

        private string videofolder = "";
        public string VideoFolder
        {
            get { return videofolder; }
            set { videofolder = value; }
        }

        public static string GetStreamSource(string _cameratype, string _source, string _login, string _password)
        {
            switch (_cameratype)
            {
                case "Secus":
                case "SecusFFMPEG":
                case "Hanse":
                    return "rtsp://" + _login + ":" + _password + "@" + _source + "/stream2";

                case "Secus HIP-SB3":
                    return "http://" + _source + "/cgi-bin/video.cgi?camera=1";

                case "Shany":
                    return "rtsp://" + _source + ":8557/PSIA/Streaming/channels/2?videoCodecType=H.264";

                case "Axis":
                    return "rtsp://" + _login + ":" + _password + "@" + _source + "/mpeg4/media.amp";

                case "Panasonic":
                case "Panasonic i-Pro":
                    return "rtsp://" + _login + ":" + _password + "@" + _source + "/MediaInput/h264";//ONVIF/MediaInput";
                // MediaInput/h264
                case "Vantech":
                    return "rtsp://" + _source + "//user=" + _login + "_password=" + _password + "_channel=1_stream=0.sdp";
                case "Samsung":
                    return "rtsp://" + _login + ":" + _password + "@" + _source + "/onvif/profile2/media.smp";
                case "Dahua":
                    return "rtsp://" + _login + ":" + _password + "@" + _source + "/cam/realmonitor?channel=1&subtype=1";
                case "HIK":
                    return "rtsp://" + _login + ":" + _password + "@" + _source + "/streaming/channels/2";
            }


            return "";
        }

        public void StartRecord()
        {
            try
            {
                if (icamera != null)
                    icamera.StartRecord();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void StopRecord()
        {
            if (icamera != null)
                icamera.StopRecord();
        }

        public void SetVideoLength(int minute)
        {
            if (camera != null)
            {
                if (camera is CameraKztek3)
                {
                    ((CameraKztek3)camera).RecordingVideoLength = minute;
                }
            }
        }

        public void StartVideoInput(string name)
        {
            switch (name)
            {

                default:
                    ivideo = new iProPanasonic();
                    break;
            }
            if (ivideo != null)
            {
                ivideo.HttpUrl = icamera.VideoSource;
                ivideo.Username = icamera.Login;
                ivideo.Password = icamera.Password;
                ivideo.VideoInput += new VideoInputEventHandler(ivideo_VideoInput);
                ivideo.PollingStart();
            }

        }

        void ivideo_VideoInput(object sender, VideoInputEventArgs e)
        {

            if (VideoInput != null)
            {
                //e.Bitmap = GetCurrentImage();
                if (e.MotionDetected == true)
                    this.icamera.BorderColor = Color.Red;
                else
                    this.icamera.BorderColor = Color.Transparent;
              
                {
                    VideoInput(this, e);
                  
                }
            }
        }

        private bool IsMouseDown = false;
        private Point StartLocation;
        private Point EndLocation;
        public Rectangle Rect;

        private void CameraWindow_MouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDown = true; // If This Event Is Occured So This Variable Is True.

            StartLocation = e.Location; // Get The Starting Location Of Point X and Y.
        }

        private void CameraWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown == true) // This Block Is Not Execute Until Mouse Down Event Is Not a Fire.
            {
                EndLocation = e.Location; // Get The Current Location Of Point X and Y.

                Refresh(); // Refresh the form.
            }
        }

        private void CameraWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsMouseDown == true) // This Block Is Not Execute Until Mouse Down Event Is Not a Fire.
            {
                EndLocation = e.Location; // Get The Ending Point of X and Y.

                IsMouseDown = false; // false this..
            }
        }

        private void CameraWindow_Paint(object sender, PaintEventArgs e)
        {
            if (Rect != null) // Check If Rectangle Is Not a null.
            {
                e.Graphics.DrawRectangle(Pens.Red, GetRect()); // GetRect() Is a Function, Now Creates this function.
            }
        }
        private Rectangle GetRect()
        {
            //Create Object Of rect. we define rect at TOP.
            Rect = new Rectangle();

            //The x-value of our Rectangle should be the minimum between the start x-value and the current x-position.
            Rect.X = Math.Min(StartLocation.X, EndLocation.X);

            //same as above x-value. The y-value of our Rectangle should be the minimum between the start y-value and the current y-position.
            Rect.Y = Math.Min(StartLocation.Y, EndLocation.Y);

            //the width of our rectangle should be the maximum between the start x-position and current x-position MINUS.
            Rect.Width = Math.Abs(StartLocation.X - EndLocation.X);

            Rect.Height = Math.Abs(StartLocation.Y - EndLocation.Y);

            return Rect;
        }

    }
}
