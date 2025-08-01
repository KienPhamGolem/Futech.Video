using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Diagnostics;
using Emgu.CV.Util;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace Futech.Video
{
    public partial class CameraKztek : UserControl,ICameraWindow
    {
        public CameraKztek()
        {
            InitializeComponent();
            //Application.Idle += new EventHandler(Application_Idle);
            ffmpeg.EnableRaisingEvents = true;
            ffmpeg.Exited += new EventHandler(ffmpeg_Exited);
        }       

         // new frame event
        public event NewFrameHandler NewFrame ;

        private Rectangle[] userWindows = null;

        private Color borderColor = Color.Black;

        private Rectangle[] motionZones = null;

        private string cameraname = "Camera Kztek";
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
        private float fps = 0;

        string VideoPath = "";
        Process ffmpeg = new Process();

        private string url = "rtsp://192.168.1.10//user=admin_password=_channel=1_stream=0.sdp";
        private string urlimage = "";
        private Bitmap lastFrame = null;


        private VideoCapture _cameraCapture;
        private VideoCapture _imageCapture;

        private IBackgroundSubtractor _fgDetector;
        private Emgu.CV.Cvb.CvBlobDetector _blobDetector;
        private Emgu.CV.Cvb.CvTracks _tracker;

        private CancellationTokenSource cts;
        ManualResetEvent ForceLoopIteration;

        // Name property
        public string CameraName
        {
            set
            {
                cameraname = value;
                //lbTitle.Text = value;
            }
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
                return status;
            }
        }

        // Fps property
        public float Fps
        {
            get 
            {
                //if (_cameraCapture != null)
                //    return (float)_cameraCapture.GetCaptureProperty(CapProp.Fps);
                return fps;
            }
            set
            {
                fps = value;
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

        private string cameratype = "";
        public string CameraType
        {
            get { return cameratype; }
            set { cameratype = value; }
        }


        /// <summary>
        /// BorderColor
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }

        // LastFrame property
        public Bitmap LastFrame
        {
            get { return lastFrame; }
        }

        //for display video
        
       



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

        public Rectangle[] MotionZones
        {
            get { return motionZones; }
            set { motionZones = value; }
        }

        public void Start()
        {

            try
            {
                if (_cameraCapture != null)
                {
                    _cameraCapture.Stop();
                    _cameraCapture.Dispose();
                }

                switch (cgi)
                {
                    case "Vantech":
                        url = "rtsp://" + source + "//user=" + login + "_password=" + password + "_channel=1_stream=0.sdp";
                        urlimage = "rtsp://" + source + "//user=" + login + "_password=" + password + "_channel=1_stream=0.sdp";
                        break;
                    case "Secus":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/stream2";
                        urlimage = "rtsp://" + login + ":" + password + "@" + source + "/stream1";
                        break;
                    case "Surveon":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/stream1";
                        urlimage = "rtsp://" + login + ":" + password + "@" + source + "/stream1";
                        break;
                    case "Shany":
                        url = "rtsp://" + source + ":8557" + "/PSIA/Streaming/channels/2?videoCodecType=H.264";
                        urlimage= "rtsp://" + source + ":8557" + "/PSIA/Streaming/channels/2?videoCodecType=H.264";
                        break;
                    case "CNB":
                        //url = "rtsp://" + login + ":" + password + "@" + source;
                        url = "rtsp://" + login + ":" + password + "@" + source + "/media/video2";
                        urlimage = "rtsp://" + login + ":" + password + "@" + source + "/media/video2";
                        break;
                    case "Dahua":
                        //url = "rtsp://" + login + ":" + password + "@" + source + "/cam/realmonitor?channel=1&subtype=" + channel;
                        url = "rtsp://" + login + ":" + password + "@" + source + "/cam/realmonitor?channel=1&subtype=" + channel;
                        urlimage = url;
                        break;
                    case "HIK":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/streaming/channels/" + (channel == 2 ? 2 : 1);
                        urlimage = "rtsp://" + login + ":" + password + "@" + source + "/streaming/channels/" + (channel == 2 ? 2 : 1);
                        break;
                    case "Enster":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/11";
                        urlimage = url;
                        break;
                    case "Afidus":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/media/media.amp?streamprofile=Profile1";
                        urlimage = "rtsp://" + login + ":" + password + "@" + source + "/media/media.amp?streamprofile=Profile1";
                        break;
                    case "ITX":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/live/main";
                        urlimage = "rtsp://" + login + ":" + password + "@" + source + "/live/main";
                        break;
                    case "Samsung":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/onvif/profile2/media.smp";
                        urlimage = "rtsp://" + login + ":" + password + "@" + source + "/onvif/profile2/media.smp";
                        break;
                    case "Tiandy":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/" + channel + "/1";
                        urlimage = url;
                        break;
                    case "Camtron":
                        url = $"rtsp://{login}:{password}@{source}/H264?ch=1&subtype=1";
                        urlimage = $"rtsp://{login}:{password}@{source}/H264?ch=1&subtype=0";
                        break;
                    case "HIVIZ":
                        url = "rtsp://" + login + ":" + password + "@" + source + ":" + rtspPort + "/profile1";
                        urlimage = "rtsp://" + login + ":" + password + "@" + source + ":" + rtspPort + "/profile1";
                        break;
                    default:
                        url = "rtsp://" + login + ":" + password + "@" + source + cgi;
                        urlimage = "rtsp://" + login + ":" + password + "@" + source + cgi;
                        break;

                }

                //url = "";
                //url = "rtsp://184.72.239.149/vod/mp4:BigBuckBunny_115k.mov";
                if (url == "")
                {
                    _cameraCapture = new VideoCapture();
                }
                else
                {
                    if (CheckConnect(source))
                    {
                        _cameraCapture = new VideoCapture(url);
                        _cameraCapture.ImageGrabbed += _cameraCapture_ImageGrabbed;
                        _cameraCapture.Start();

                        //_imageCapture.ImageGrabbed += _imageCapture_ImageGrabbed;
                        //_imageCapture = new VideoCapture(urlimage);
                        //_imageCapture.Start();
                    }
                }

                cts = new CancellationTokenSource();
                ForceLoopIteration = new ManualResetEvent(false);

                Task.Run(() =>

                    DoWork(cts.Token), cts.Token

                );

                if (enablerecording)
                {
                    VideoPath = this.VideoFolder;
                    //StartRecord();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void _cameraCapture_ImageGrabbed(object sender, EventArgs e)
        {
            timeout = 0;
           
            Mat frame = new Mat();
           
            try
            {
                _cameraCapture.Retrieve(frame, 0);
                imageBox1.Image = frame;
                GC.Collect();
            }
            catch (Exception ex)
            {

            }

        }

        public void Stop()
        {
            try
            {
                _cameraCapture.Stop();
               
                cts.Cancel();
                WaitHandle.WaitAny(
                            new[] { cts.Token.WaitHandle, ForceLoopIteration },
                            TimeSpan.FromMilliseconds(50));
            }
            catch
            { }
        }

        public void StartRecord()
        {
            WriteVideo();
        }

        /// <summary>
        /// Stop Record
        /// </summary>
        public void StopRecord()
        {
            this.enablerecording = false;
        }

        public Bitmap GetCurrentImage()
        {
            //lock (this)
            //{
            //    if (lastFrame != null)
            //        return new Bitmap(lastFrame);
            //}

            //Mat frame = new Mat();
          
            try
            {
                //if (_imageCapture != null && _imageCapture.IsOpened)
                //    _imageCapture.Retrieve(frame, 0);

                //if (frame != null && timeout < 10)
                //    return new Bitmap(frame.Bitmap);
                //else
                //    frame = null;

                lock (this)
                {
                    if (imageBox1.Image != null && timeout < 10)
                    {
                        Bitmap bmp = new Bitmap(imageBox1.Image.Bitmap);
                        return bmp;
                    }
                }


            }
            catch (Exception ex)
            {

            }

            return null;
            
        }

        int timeout = 0;
        void Application_Idle(object sender, EventArgs e)
        {
            //var lockTaken = false;
            try
            {
                //Monitor.Enter(_cameraCapture, ref lockTaken);
                Mat frame = _cameraCapture.QueryFrame();

                if (frame == null)
                {
                    Thread.Sleep(300);
                    timeout++;
                    if (timeout > 10)
                    {
                        Start();
                        timeout = 0;
                    }
                    return;
                }
                timeout = 0;
                imageBox1.Image = frame;
               // pictureBox1.Image = frame.Bitmap;
                lastFrame = frame.Bitmap;
                double fr = _cameraCapture.GetCaptureProperty(CapProp.Fps);
            }
            catch
            { }
           
        }

        private async Task DoWork(CancellationToken token)
        {

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1000);

                timeout = timeout + 1;

                try
                {
                    if (timeout > 10)
                    {
                        imageBox1.Image = null;

                        if (_cameraCapture != null)
                        {
                            _cameraCapture.Stop();
                            _cameraCapture.ImageGrabbed -= _cameraCapture_ImageGrabbed;
                            _cameraCapture = null;
                        }
                       
                        if (CheckConnect(source))
                        {
                            _cameraCapture = new VideoCapture(url);
                           // _imageCapture = new VideoCapture(urlimage);
                            _cameraCapture.ImageGrabbed += _cameraCapture_ImageGrabbed;
                            // _imageCapture.ImageGrabbed += _cameraCapture_ImageGrabbed;

                            _cameraCapture.Start();
                           // _imageCapture.Start();
                            timeout = 0;
                        }
                    }
                    //Mat frame = null;

                    //try
                    //{
                    //    frame = _cameraCapture.QueryFrame();

                    //}
                    //catch(Exception ex)
                    //{

                    //}              
                    //if (frame == null)
                    //{
                    //    imageBox1.Image = null;
                    //    await Task.Delay(100);
                    //    timeout++;
                    //    if (timeout > 5)
                    //    {

                    //        if (url == "")
                    //        {
                    //            _cameraCapture = new VideoCapture();
                    //        }
                    //        else
                    //        {
                    //            if(CheckConnect(source))
                    //            { 
                    //                _cameraCapture = new VideoCapture(url);
                    //            }
                    //        }

                    //        timeout = 0;
                    //    }
                    //    continue;
                    //}
                    //timeout = 0;

                    //Mat framedisplay = frame.Clone();
                    //lastFrame = new Bitmap(frame.Bitmap);
                    //if (NewFrame != null)
                    //    NewFrame(this, new NewFrameArgs(lastFrame));
                    //if (userWindows != null)
                    //{
                    //    foreach (Rectangle rec in userWindows)
                    //    {

                    //        CvInvoke.Rectangle(framedisplay, rec, new MCvScalar(0.0, 255.0, 0.0), 2);
                    //    }
                    //}
                    //if (motionZones != null)
                    //{
                    //    foreach (Rectangle rec in motionZones)
                    //    {

                    //        CvInvoke.Rectangle(framedisplay, rec, new MCvScalar(255.0, 0.0, 255.0), 2);
                    //    }
                    //}

                    //imageBox1.Image = framedisplay;

                }
                catch (Exception ex)
                {
                   
                }
                GC.Collect();
            }

        }

        public void SaveCurrentImage(string filename)
        {
            Bitmap bm = GetCurrentImage();
            bm.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        /// <summary>
        /// UserWindow
        /// </summary>
        public Rectangle[] UserWindows
        {
            get { return userWindows; }
            set { userWindows = value; }
        }

        // LastMessage property
        public string LastMessage
        {
            get { return ""; }
        }

        // DisplayCameraTitle property
        public bool DisplayCameraTitle
        {
            set
            {
                
            }
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
                try
                {
                    if (Directory.Exists(path) == false)
                        Directory.CreateDirectory(path);
                }
                catch
                { }
                if (Directory.Exists(path))
                {
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

                    // ffmpegParams = @"E:\1.mp4" + " " + filename;//OK neu la tu file

                    ffmpeg.StartInfo.FileName = "cmd.exe";

                    ffmpeg.StartInfo.UseShellExecute = false;
                    ffmpeg.StartInfo.CreateNoWindow = true;

                    ffmpeg.StartInfo.Arguments = "/c " + ffmpegPath + " " + ffmpegParams;//zenzen OK

                    ffmpeg.Start();
                }


            }
            catch
            {
                //Application.DoEvents();
                //Thread.Sleep(5000);
                //StartRecord();
            }
        }

        void ffmpeg_Exited(object sender, EventArgs e)
        {

            if (this.enablerecording)
            {
                try
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
                }
                catch
                {
                    //StartRecord();
                }
                //if (CheckConnect(source) == false)
                //{
                //    if (istimerstateRunning == false)
                //        timerState.Start();

                //}
                //else
                StartRecord();

            }
        }


    }

}
