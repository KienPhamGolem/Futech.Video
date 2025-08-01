using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using AForge;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.VFW;
using AForge.Video.DirectShow;
using AForge.Controls;
using System.Net;
using System.Net.NetworkInformation;

namespace Futech.Video
{
    public partial class CameraAForgeSDK : UserControl, ICameraWindow
    {
        private string cameraname = "Camera AForge";
        private string source = "";
        private int httpPort = 80;
        private int rtspPort = 554;
        private string login = null;
        private string password = null;
        private string mediatype = "MJPEG";
        private string resolution = "640x480";
        private string cgi = "";
        private int channel = 1;
        private int status = 0;

        private Bitmap lastFrame = null;

        private bool displayCameraTitle = true;

        // new frame event
        public event NewFrameHandler NewFrame;

        private Rectangle[] userWindows = null;

        private Color borderColor = Color.Black;

        private Rectangle[] motionZones = null;

        string VideoPath = @"C:\VideoRecord";
        Process ffmpeg = new Process();

        public CameraAForgeSDK()
        {
            InitializeComponent();

            // Events
            this.videoSourcePlayer1.NewFrame += new VideoSourcePlayer.NewFrameHandler(videoSourcePlayer1_NewFrame);

           
        }

        // Name property
        public string CameraName
        {
            set { cameraname = value; }
        }

        // VideoSource property
        public string VideoSource
        {
            get { return source; }
            set
            {
                source = value;
            }
        }
        // Http Port
        public int HttpPort
        {
            get { return httpPort; }
            set { httpPort = value; }
        }

        // Rtsp Port
        public int RtspPort
        {
            get { return rtspPort; }
            set { rtspPort = value; }
        }

        // Login property
        public string Login
        {
            get { return login; }
            set { login = value; }
        }

        // Password property
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        // MediaType property
        public string MediaType
        {
            get { return mediatype; }
            set { mediatype = value; }
        }

        // Resolution property
        public string Resolution
        {
            get { return resolution; }
            set { resolution = value; }
        }

        // Cgi property
        public string Cgi
        {
            get { return cgi; }
            set { cgi = value; }
        }

        // Channel property
        public int Channel
        {
            get { return channel; }
            set
            {
                channel = value;
            }
        }

        // Status property
        public int Status
        {
            get
            {
                if (videoSourcePlayer1.Connected)
                    status = 1;
                else
                    status = 0;
                return status;
            }
        }

        // LastFrame property
        public Bitmap LastFrame
        {
            get { return lastFrame; }
        }

        // DisplayCameraTitle property
        public bool DisplayCameraTitle
        {
            set
            {
                displayCameraTitle = value;
                if (displayCameraTitle)
                    panel1.Visible = true;
                else
                    panel1.Visible = false;
            }
        }

        // Fps property
        public float Fps
        {
            get { return videoSourcePlayer1.Fps; }
        }

        // LastMessage property
        public string LastMessage
        {
            get { return videoSourcePlayer1.LastMessage; }
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

        // Start video source
        public void Start()
        {
            IVideoSource iSource = null;
            switch (mediatype)
            {
                case "JPEG":
                    JPEGStream jpegSource = new JPEGStream("http://" + source + cgi);
                    jpegSource.Login = login;
                    jpegSource.Password = password;
                    iSource = jpegSource;
                    break;
                case "MJPEG":
                    MJPEGStream mjpegSource = new MJPEGStream("http://" + source + cgi);
                    mjpegSource.Login = login;
                    mjpegSource.Password = password;
                    iSource = mjpegSource;
                    break;
                case "Local Video Capture Device":
                    // create video source
                    FilterInfoCollection videoDevices;
                    //string device = "";
                    // enumerate video devices
                    videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                    if (videoDevices.Count == 0)
                        throw new ApplicationException();

                    // add all devices to combo
                    foreach (FilterInfo device in videoDevices)
                    {
                        if (cgi == device.Name)
                        {
                            VideoCaptureDevice videoSource = new VideoCaptureDevice(device.MonikerString);
                            iSource = videoSource;
                            break;
                        }
                    }
                    break;
                case "PlayFile":
                    if (File.Exists(cgi))
                    {
                        FileVideoSource fileSource = new FileVideoSource(cgi);
                        iSource = fileSource;
                    }
                    break;
            }

            // open it
            if (iSource != null)
            {
                // close previous video source
                Stop();

                // start new video source
                videoSourcePlayer1.VideoSource = new AsyncVideoSource(iSource);
                videoSourcePlayer1.Start();
            }

            if (enablerecording)
            {
                VideoPath = this.VideoFolder;
                //StartRecord();
            }

            lbTitle.Text = "Connecting to " + cameraname + "...";
        }

        // Abort camera
        public void Stop()
        {
            try
            {
                // stop current video source
                videoSourcePlayer1.SignalToStop();

                //StopRecord();
                // wait 2 seconds until camera stops
                for (int i = 0; (i < 2) && (videoSourcePlayer1.IsRunning); i++)
                {
                    Thread.Sleep(100);
                }
                if (videoSourcePlayer1.IsRunning)
                    videoSourcePlayer1.Stop();

                lbTitle.Text = "Stop to " + cameraname + "...";
                lastFrame = null;
            }
            catch
            { }
        }

        // Luu hinh anh camera
        public void SaveCurrentImage(string theFile)
        {
            if (videoSourcePlayer1.VideoSource != null && videoSourcePlayer1.IsRunning)
                lastFrame = videoSourcePlayer1.GetCurrentVideoFrame();
            if (lastFrame != null)
                lastFrame.Save(theFile, ImageFormat.Jpeg);
        }

        // get bitmap image from bitmap data
        public Bitmap GetCurrentImage()
        {
            if (videoSourcePlayer1.VideoSource != null && videoSourcePlayer1.IsRunning)
                return videoSourcePlayer1.GetCurrentVideoFrame();
            else
                return null;
        }

        // Start Record Video
        public void StartRecord()
        {
            WriteVideo();
        }

        // Stop Record Video
        public void StopRecord()
        {
            this.enablerecording = false;
        }

        private void videoSourcePlayer1_NewFrame(object sender, ref Bitmap image)
        {
            lastFrame = image;
            if (NewFrame != null)
                NewFrame(this, new NewFrameArgs(image));
        }


        string previousState = "0";//off
        string currentState = "0";//on
        private void timer1_Tick(object sender, EventArgs e)
        {
            // update label
            previousState = currentState;
            lbTitle.Text = cameraname + " - " + videoSourcePlayer1.Fps.ToString("F2") + " fps";
            //if (videoSourcePlayer1 != null && videoSourcePlayer1.Connected)
            //    currentState = "1";
            //else
            //    currentState = "0";

            //if (currentState == "0" && previousState == "1")
            //{
            //    timerState.Start();
            //}

            //if (videoSourcePlayer1 != null && videoSourcePlayer1.Connected == false)
            if (isVideoLoss == true)
            {
                if (istimerstateRunning == false)
                    timerState.Start();
            }
            
        }

        /// <summary>
        /// UserWindow
        /// </summary>
        public Rectangle[] UserWindows
        {
            get { return userWindows; }
            set 
            { 
                userWindows = value;
                videoSourcePlayer1.UserWindows = value;
            }
        }

        /// <summary>
        /// BorderColor
        /// </summary>
        [DefaultValue( typeof( Color ), "Black" )]
        public Color BorderColor
        {
            get { return borderColor; }
            set 
            { 
                borderColor = value;
                videoSourcePlayer1.BorderColor = borderColor;
            }
        }

        /// <summary>
        /// MotionZones
        /// </summary>
        public Rectangle[] MotionZones
        {
            get { return motionZones; }
            set
            {
                motionZones = value;
                videoSourcePlayer1.MotionZones = value;
            }
        }

        public uint Get_BRIGHTNESS()
        {
            return 0;
        }
        public void Set_BRIGHTNESS(uint pbrightness)
        {

        }
        //contrast
        public uint Get_CONTRAST()
        {
            return 0;
        }
        public void Set_CONTRAST(uint pcontrast)
        {

        }

        //hue
        public uint Get_HUE()
        {
            return 0;
        }
        public void Set_HUE(uint phue)
        {

        }

        //saturation
        public uint Get_SATURATION()
        {
            return 0;
        }
        public void Set_SATURATION(uint psaturation)
        {

        }

        //sharpness
        public uint Get_SHARPNESS()
        {
            return 0;
        }
        public void Set_SHARPNESS(uint psharpness)
        {

        }

        private string cameratype = "";
        public string CameraType
        {
            get { return cameratype; }
            set { cameratype = value; }
        }


        bool isVideoLoss = false;
        private void WriteVideo()
        {
            try
            {
                //if (CheckConnect(source) == false)
                //{
                //    isVideoLoss = true;
                //    return;
                //}

               

                string path = VideoPath + "\\CAM" + source + "\\" + DateTime.Now.ToString("dd-MM-yyyy");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                string videofile = path + "\\" + DateTime.Now.ToString("HHmmss") + ".mp4";
              
                videofile = "\"" + videofile + "\"";

                string ffmpegPath = "";

                ffmpegPath = Application.StartupPath.Substring(0, 1) + @":\CRVideo\ffmpeg.exe -i";
                // "http://192.168.1.100/cgi-bin/video.cgi?camera=1";
                //streamsource = "http://192.168.1.30/cgi-bin/image/mjpeg.cgi?id=admin&passwd=admin";
                //streamsource = "rtsp://admin:admin@192.168.1.30/stream1";
                string streamsource = "";
                streamsource = CameraWindow.GetStreamSource(cameratype, source, login, password);

                string ffmpegParams = streamsource + " -r 5 -s 320x180 -c:v libx264 -t 00:05:00" + " " + videofile;

                ffmpeg.StartInfo.FileName = "cmd.exe";

                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.CreateNoWindow = true;

                ffmpeg.StartInfo.Arguments = "/c " + ffmpegPath + " " + ffmpegParams;//zenzen OK

                ffmpeg.Start();
            }
            catch
            {
                //WriteVideo();
                //StartRecord();
            }
        }

       

        void ffmpeg_Exited(object sender, EventArgs e)
        {
            try
            {
                if (this.enablerecording)
                {
                    string path = VideoPath + "\\CAM" + source + "\\" + DateTime.Now.AddDays(-30).ToString("dd-MM-yyyy");

                    if (Directory.Exists(path))
                    {
                        // Directory.Delete(path,true);
                        DirectoryInfo dir = new DirectoryInfo(path);
                        foreach (FileInfo file in dir.GetFiles())
                        {
                            file.Delete();
                        }

                        Directory.Delete(path);
                    }
                    GC.Collect();
                    Application.DoEvents();
                    Thread.Sleep(300);
                    StartRecord();
                }
            }
            catch
            {
                //StartRecord();
            }
        }

        private void CameraAForgeSDK_Load(object sender, EventArgs e)
        {

            ffmpeg.EnableRaisingEvents = true;
            ffmpeg.Exited += new EventHandler(ffmpeg_Exited);

        }

        private bool CheckConnect(string _ip)
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = null;
                reply = ping.Send(_ip, 200);
                if (reply != null && reply.Status == IPStatus.Success)
                    return true;
                else
                    return false;
            }
            catch
            { }

            return false;
        }
        bool istimerstateRunning = false;
        private void timerState_Tick(object sender, EventArgs e)
        {
            if (CheckConnect(source) == true)
            {
                isVideoLoss = false;
                timerState.Stop();
                istimerstateRunning = false;
                StartRecord();
            }
            else
                istimerstateRunning = true;
        }

       
     }
}
