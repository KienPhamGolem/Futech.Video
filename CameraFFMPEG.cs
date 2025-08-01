using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using WebEye;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace Futech.Video
{
    public partial class CameraFFMPEG : UserControl, ICameraWindow
    {
        public CameraFFMPEG()
        {
            InitializeComponent();

            ffmpeg.EnableRaisingEvents = true;
            ffmpeg.Exited += new EventHandler(ffmpeg_Exited);
            this.timerState.Tick += new EventHandler(timerState_Tick);
        }

        // new frame event
        public event NewFrameHandler NewFrame;

        private Rectangle[] userWindows = null;

        private Color borderColor = Color.Black;

        private Rectangle[] motionZones = null;

        private string cameraname = "Camera Geovision";
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
        private Bitmap lastFrame = null;

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
            get { return fps; }
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

        private StreamPlayerProxy _player;

        private StreamPlayerProxy Player
        {
            get
            {
                if (_player == null)
                {
                    _player = CreateAndInitializePlayer();
                }

                return _player;
            }
        }


        /// <summary>
        /// Asynchronously plays a stream.
        /// </summary>
        /// <param name="uri">The uri of a stream to play.</param>
        /// <param name="connectionTimeout"></param>
        /// <exception cref="Win32Exception">Failed to load the FFmpeg facade dll.</exception>
        /// <exception cref="StreamPlayerException">Failed to play the stream.</exception>
        /// <param name="transport">RTSP transport protocol.</param>
        /// <param name="flags">RTSP flags.</param>
        public void StartPlay(Uri uri, TimeSpan connectionTimeout,
            RtspTransport transport, RtspFlags flags)
        {
            if (IsPlaying)
            {
                Stop();
            }

            Player.StartPlay(uri.IsFile ? uri.LocalPath : uri.ToString(),
                connectionTimeout, transport, flags);
        }

        /// <summary>
        /// Asynchronously plays a stream.
        /// </summary>
        /// <param name="uri">The uri of a stream to play.</param>
        /// <exception cref="Win32Exception">Failed to load the FFmpeg facade dll.</exception>
        /// <exception cref="StreamPlayerException">Failed to play the stream.</exception>
        public void StartPlay(Uri uri)
        {
            //StartPlay(uri, TimeSpan.FromSeconds(5.0), RtspTransport.Undefined, RtspFlags.None);
            StartPlay(uri, TimeSpan.FromMilliseconds(1300), RtspTransport.Udp, RtspFlags.None);
        }

        /// <summary>
        /// Retrieves the image being played.
        /// </summary>
        /// <returns>The current image.</returns>
        /// <exception cref="InvalidOperationException">The control is not playing a video stream.</exception>
        /// <exception cref="StreamPlayerException">Failed to get the current image.</exception>
        public Bitmap GetCurrentFrame()
        {
            try
            {
                if (!IsPlaying)
                {
                    return null;
                }

                return Player.GetCurrentFrame();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Stops a stream.
        /// </summary>
        /// <exception cref="InvalidOperationException">The control is not playing a stream.</exception>
        /// <exception cref="StreamPlayerException">Failed to stop a stream.</exception>
        public void Stop()
        {
            try
            {
                timer1.Stop();
                timer1.Dispose();
                Thread.Sleep(100);
                
                if (IsPlaying)
                {
                  
                    Player.Stop();
                }

                IsPlaying = false;
                StopThread();
                StopRecord();
            }
            catch
            { }
        }

        /// <summary>
        /// Gets a value indicating whether the control is playing a video stream.
        /// </summary>
        [Browsable(false)]
        public Boolean IsPlaying { get; private set; }

        /// <summary>
        /// Gets the unstretched frame size.
        /// </summary>
        [Browsable(false)]
        public Size VideoSize
        {
            get { return IsPlaying ? Player.GetFrameSize() : new Size(0, 0); }
        }

        private bool _disposed;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing && (_player != null))
            {
                if (IsPlaying)
                {
                    Stop();
                }

                _player.Uninitialize();
                _player.Dispose();
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            _disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Specifies a set of values that are used when you start the player.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct StreamPlayerParams
        {
            internal IntPtr window;
            internal IntPtr streamStartedCallback;
            internal IntPtr streamStoppedCallback;
            internal IntPtr streamFailedCallback;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        private delegate void CallbackDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate void ErrorCallbackDelegate(string error);

        private Delegate _streamStartedCallback;
        private Delegate _streamStoppedCallback;
        private Delegate _streamFailedCallback;

        private StreamPlayerProxy CreateAndInitializePlayer()
        {
            var player = new StreamPlayerProxy();

            _streamStartedCallback = new CallbackDelegate(RaiseStreamStartedEvent);
            _streamStoppedCallback = new CallbackDelegate(RaiseStreamStoppedEvent);
            _streamFailedCallback = new ErrorCallbackDelegate(RaiseStreamFailedEvent);

            var playerParams = new StreamPlayerParams
            {
                window = Handle,
                streamStartedCallback = Marshal.GetFunctionPointerForDelegate(_streamStartedCallback),
                streamStoppedCallback = Marshal.GetFunctionPointerForDelegate(_streamStoppedCallback),
                streamFailedCallback = Marshal.GetFunctionPointerForDelegate(_streamFailedCallback)
            };

            player.Initialize(playerParams);

            return player;
        }

        /// <summary>
        /// Occurs when the first frame is read from a stream.
        /// </summary>
        public event EventHandler StreamStarted;

        private void RaiseStreamStartedEvent()
        {
            IsPlaying = true;
            
            if (StreamStarted != null)
            {
                StreamStarted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when there are no more frames to read from a stream.
        /// </summary>
        public event EventHandler StreamStopped;

        private void RaiseStreamStoppedEvent()
        {
            IsPlaying = false;
          
            if (StreamStopped != null)
            {
                StreamStopped(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when the player fails to play a stream.
        /// </summary>
        public event EventHandler<WebEye.StreamFailedEventArgs> StreamFailed;

        private void RaiseStreamFailedEvent(string error)
        {
            IsPlaying = false;

            if (StreamFailed != null)
            {
                StreamFailed(this, new StreamFailedEventArgs(error));
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

        public Rectangle[] MotionZones
        {
            get { return motionZones; }
            set { motionZones = value; }
        }

        public void Start()
        {
            switch (cgi)
            {
                case "Vantech":
                    url = "rtsp://" + source + "//user=" + login + "_password=" + password + "_channel=1_stream=0.sdp";
                    break;
                case "Secus":
                    url = "rtsp://" + login + ":" + password + "@" + source + "/stream1";
                    break;
                case "Surveon":
                    url = "rtsp://" + login + ":" + password + "@" + source + "/stream1";
                    break;
                case "Shany":
                    url = "rtsp://" + source + ":8557" + "/PSIA/Streaming/channels/2?videoCodecType=H.264";
                    break;
                case "CNB":
                    //url = "rtsp://" + login + ":" + password + "@" + source;
                    url = "rtsp://" + login + ":" + password + "@" + source + "/media/video2";
                    break;
                case "Dahua":
                    url = "rtsp://" + login + ":" + password + "@" + source + "/cam/realmonitor?channel=1&subtype=" + channel;
                    break;
                case "HIK":
                    url = "rtsp://" + login + ":" + password + "@" + source + "/streaming/channels/"+ (channel == 2 ? 2 : 1);
                    break;
                case "Enster":
                    url = "rtsp://" + login + ":" + password + "@" + source + "/11";
                    break;
                case "Afidus":
                    url = "rtsp://" + login + ":" + password + "@" + source + "/media/media.amp?streamprofile=Profile1";
                    break;
                case "ITX":
                    url = "rtsp://" + login + ":" + password + "@" + source + "/live/main";
                    break;
                case "Samsung":
                    url = "rtsp://" + login + ":" + password + "@" + source + "/onvif/profile2/media.smp";
                    break;
                default:
                    url = "rtsp://" + login + ":" + password + "@" + source + cgi;
                    break;
            }
           
            StartPlay(new Uri(url));


            if (enablerecording)
            {
                VideoPath = this.VideoFolder;
                //StartRecord();
            }
           
            //StartThread();
        }

        public void StartRecord()
        {
            WriteVideo();
        }

        /// <summary>
        /// Stop Record
        /// </summary>
        public void StopRecord()
        { }

        public Bitmap GetCurrentImage()
        {
            return GetCurrentFrame();
        }

        public void SaveCurrentImage(string filename)
        {
            Bitmap bm = GetCurrentFrame();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (IsPlaying == false)
                {
                    System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                    System.Net.NetworkInformation.PingReply reply = null;
                    reply = ping.Send(source, 200);

                    if (reply != null && reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        this.Start();
                    }
                }

                if (isVideoLoss == true)
                {
                    if (istimerstateRunning == false)
                        timerState.Start();
                }
            }
            catch
            { }
        }

        private string cameratype = "";
        public string CameraType
        {
            get { return cameratype; }
            set { cameratype = value; }
        }



        private Thread thread = null;
        private ManualResetEvent stopEvent = null;
        public bool IsRunning
        {
            get
            {
                if (thread != null)
                {
                    // check thread status
                    if (thread.Join(0) == false)
                        return true;

                    // the thread is not running, free resources
                    Free();
                }
                return false;
            }
        }
        public void StartThread()
        {
            if (!IsRunning)
            {


                // create events
                stopEvent = new ManualResetEvent(false);

                // create and start new thread
                thread = new Thread(new ThreadStart(WorkerThread));
                thread.Name = source; // mainly for debugging
                thread.Start();
            }
        }

        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals video source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        /// 
        public void SignalToStop()
        {
            // stop thread
            if (thread != null)
            {
                // signal to stop
                stopEvent.Set();
            }
        }

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for source stopping after it was signalled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
        public void WaitForStop()
        {
            if (thread != null)
            {
                // wait for thread stop
                thread.Join(500);

                Free();
            }
        }

        /// <summary>
        /// Stop video source.
        /// </summary>
        /// 
        /// <remarks><para>Stops video source aborting its thread.</para>
        /// 
        /// <para><note>Since the method aborts background thread, its usage is highly not preferred
        /// and should be done only if there are no other options. The correct way of stopping camera
        /// is <see cref="SignalToStop">signaling it stop</see> and then
        /// <see cref="WaitForStop">waiting</see> for background thread's completion.</note></para>
        /// </remarks>
        /// 
        public void StopThread()
        {
            //if (this.IsRunning)
            //{
            //    stopEvent.Set();
            //    thread.Abort();
            //    WaitForStop();
            //}
            if (this.IsRunning)
            {
                SignalToStop();
                while (thread.IsAlive)
                {
                    if (WaitHandle.WaitAll(
                        (new ManualResetEvent[] { stopEvent }),
                        100,
                        true))
                    {
                        WaitForStop();
                        break;
                    }

                    Application.DoEvents();
                }
            }
        }

        /// <summary>
        /// Free resource.
        /// </summary>
        /// 
        private void Free()
        {
            thread = null;

            // release events
            stopEvent.Close();
            stopEvent = null;
        }

        // Worker thread
        private void WorkerThread()
        {

            while (!stopEvent.WaitOne(0, true))
            {


                try
                {
                    Bitmap bmp = GetCurrentImage();
                    if (bmp != null)
                    {
                        NewFrameArgs farg = new NewFrameArgs(bmp);
                        NewFrame(this, farg);

                        bmp.Dispose();
                    }

                }
                catch
                { }
                
                Thread.Sleep(50);
                GC.Collect();
               // Application.DoEvents();
            }
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

                // ffmpegParams = @"E:\1.mp4" + " " + filename;//OK neu la tu file

                ffmpeg.StartInfo.FileName = "cmd.exe";

                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.CreateNoWindow = true;

                ffmpeg.StartInfo.Arguments = "/c " + ffmpegPath + " " + ffmpegParams;//zenzen OK

                ffmpeg.Start();


            }
            catch
            {
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
        void timerState_Tick(object sender, EventArgs e)
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
