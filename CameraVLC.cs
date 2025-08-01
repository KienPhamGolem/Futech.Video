using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Declarations;
using Declarations.Events;
using Declarations.Media;
using Declarations.Players;
using Implementation;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.IO;

namespace Futech.Video
{
    public partial class CameraVLC : UserControl, ICameraWindow
    {

        // new frame event
        public event NewFrameHandler NewFrame;

        private Rectangle[] userWindows = null;

        private Color borderColor = Color.Black;

        private Rectangle[] motionZones = null;

        private string cameraname = "Camera VLC";
        private string source = "";
        private int httpPort = 80;
        public int rtspPort = 554;
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
            get 
            {
                return m_memoryrender.ActualFrameRate;
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
        IMediaPlayerFactory m_factory_video;
        IDiskPlayer m_player_video;
        IMedia m_media_video;

        //for image
        IMediaPlayerFactory m_factory;
        IDiskPlayer m_player;
        IMedia m_media;
        IMemoryRenderer m_memoryrender;
        int width = 704;
        int height = 576;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public CameraVLC()
        {
            InitializeComponent();
            m_factory = new MediaPlayerFactory(true);
            m_player = m_factory.CreatePlayer<IDiskPlayer>();

            

            m_factory_video = new MediaPlayerFactory(true);
            m_player_video = m_factory_video.CreatePlayer<IDiskPlayer>();
            m_player_video.WindowHandle = panel1.Handle;
            m_memoryrender = m_player.CustomRenderer;

            //UISync.Init(this);
            timer.Interval = 5000;
            timer.Tick += Timer_Tick;
            timer.Start();

            ffmpeg.EnableRaisingEvents = true;
            ffmpeg.Exited += new EventHandler(ffmpeg_Exited);
        }

        int timeout = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (m_player.IsPlaying == false || m_player_video.IsPlaying == false)
            {
                timeout++;
                if (timeout > 3)
                {
                    if (CheckConnect(source) == true)
                    {
                        Start();
                        timeout = 0;
                    }
                    timeout = 0;
                }
            }
            //throw new NotImplementedException();
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

            //cgi=Secus?resolution=704x576

            int index = cgi.LastIndexOf("=") + 1;
            string[] temp = cgi.Substring(index).Split('x');
            if (temp != null && temp.Length == 2)
            {
                width = int.Parse(temp[0]);
                height = int.Parse(temp[1]);
            }

            if (cgi.Contains("Vantech"))
            {

                url = "rtsp://" + source + "//user=" + login + "_password=" + password + "_channel=1_stream=0.sdp";

            }
            else if (cgi.Contains("Secus"))
            {

                url = "rtsp://" + login + ":" + password + "@" + source + "/stream1";

            }

            else if (cgi.Contains("Surveon"))
            {
                url = "rtsp://" + login + ":" + password + "@" + source + "/stream1";
            }

            else if (cgi.Contains("Shany"))
            {
                url = "rtsp://" + source + ":8557" + "/PSIA/Streaming/channels/2?videoCodecType=H.264";
            }
            else if (cgi.Contains("CNB"))
            {
               // url = "rtsp://" + login + ":" + password + "@" + source;
                url = "rtsp://" + login + ":" + password + "@" + source + "/media/video2";
            }
            else if(cgi.Contains("Dahua"))
            {

                url = "rtsp://" + login + ":" + password + "@" + source + "/cam/realmonitor?channel=1&subtype=" + channel;
            }
            else if (cgi.Contains("HIK"))
            {
                url = "rtsp://" + login + ":" + password + "@" + source + "/streaming/channels/"+ (channel == 2 ? 2 : 1);
            }
            else if (cgi.Contains("Enster"))
            {
                url = "rtsp://" + login + ":" + password + "@" + source + "/11";
            }
            else if(cgi.Contains("ITX"))
            {
                url = "rtsp://" + login + ":" + password + "@" + source + "/live/main";
            }
            else if(cgi.Contains("Samsung"))
            {
                url = "rtsp://" + login + ":" + password + "@" + source + "/onvif/profile2/media.smp";
            }
            else
            {
                if (cgi.Contains("?resolution="))
                    cgi = cgi.Substring(0, cgi.LastIndexOf("?resolution="));
                url = "rtsp://" + login + ":" + password + "@" + source + cgi;

            }

            //url ="rtsp://184.72.239.149/vod/mp4:BigBuckBunny_115k.mov";// @"D:\sougo\mov.mp4";


            m_memoryrender.SetFormat(new BitmapFormat(width, height, ChromaType.RV32));
            m_media = m_factory.CreateMedia<IMedia>(url, new string[] { ":network-caching=300" });

            m_player.Open(m_media);
            m_media.Parse(true);
            m_player.Play();
            Thread.Sleep(50);

            m_media_video = m_factory_video.CreateMedia<IMedia>(url, new string[] { ":network-caching=250" });

            m_player_video.Open(m_media_video);
            m_media_video.Parse(true);
            m_player_video.Play();

            //m_memoryrender.SetFormat(new BitmapFormat(width, height, ChromaType.RV24));
            //m_media = m_factory.CreateMedia<IMediaFromFile>(url);
           
            //m_player.Open(m_media);
            //m_player.Play();

            //System.Threading.Thread.Sleep(10);

            //m_media_video = m_factory_video.CreateMedia<IMediaFromFile>(url);
          
            //m_player_video.Open(m_media_video);
            //m_player_video.Play();


            if (enablerecording)
            {
                VideoPath = this.VideoFolder;
                //StartRecord();
            }


        }

        public void Stop()
        {
            try
            {
                try
                {

                    m_player.Stop();
                    Thread.Sleep(50);
                }
                catch
                { }
              //  if (m_player_video.IsPlaying)
                {
                    m_player_video.Stop();
                    Thread.Sleep(50);
                }
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
            return m_memoryrender.CurrentFrame;
            
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


    }

    internal class UISync
    {
        private static ISynchronizeInvoke Sync;

        public static void Init(ISynchronizeInvoke sync)
        {
            Sync = sync;
        }

        public static void Execute(Action action)
        {
            Sync.BeginInvoke(action, null);
        }
    }
}
