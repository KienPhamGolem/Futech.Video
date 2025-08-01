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
    public partial class CameraKztek2 : UserControl,ICameraWindow
    {
        public CameraKztek2()
        {
            InitializeComponent();
        }

        // new frame event
        public event NewFrameHandler NewFrame;

        private Rectangle[] userWindows = null;

        private Color borderColor = Color.Black;

        private Rectangle[] motionZones = null;

        private string cameraname = "Camera Kztek";
        private string source = "";
        private int httpPort = 80;
        private int rtspPort = 554;
        private string login = "";
        private string password = "";
        private string mediatype = "H264";
        private string cgi = "";
        private int channel = 1;
        private int status = 0;
        private float fps = 0;
        private string url = "";
        private string urlimage = "";
        private Bitmap lastFrame = null;
        string VideoPath = "";

        private int resolution_Width = 1920;
        private int resolution_Height = 1080;

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
            get
            {
                return this.resolution_Height + "x" + this.resolution_Height;
            }
            set
            {
                string resolution = value;
                if (resolution.Contains(("x")))
                {
                    string[] resols = resolution.Split('x');
                    if (resols.Length == 2)
                    {
                        int.TryParse(resols[0], out resolution_Width);
                        int.TryParse(resols[1], out resolution_Height);
                        if (resolution_Width < 640) resolution_Width = 640;
                        if (resolution_Height < 480) resolution_Height = 480;
                    }
                }
            }
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
                string url = RtspTools.GetRTSP(cameratype, source, rtspPort, login, password, channel);
                rtspPlayerControl1.Stop();
                rtspPlayerControl1.SetURL(url, this.login, this.password);
                rtspPlayerControl1.Resolution_Width = this.resolution_Width;
                rtspPlayerControl1.Resolution_Height = this.resolution_Height;
                rtspPlayerControl1.StartPlay();
                this.status = 1;
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

        public void Stop()
        {
            try
            {
                if (this.enablerecording)
                {
                    this.StopRecord();
                }
                rtspPlayerControl1.Stop();
                this.status = 0;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                GC.Collect();
            }
        }

        public void StartRecord()
        {
            this.enablerecording = true;
        }

        /// <summary>
        /// Stop Record
        /// </summary>
        public void StopRecord()
        {
            this.enablerecording = false;
        }

        // get bitmap image from bitmap data
        bool readyToCapture = true;
        DateTime lastTimeSnapshot = DateTime.Now;
        private object sync = new object();
        private int captureIndex = 1;
        public Bitmap GetCurrentImage()
        {
            try
            {
                if (this.status == 1) return rtspPlayerControl1.GetCurrentFrame();
            }
            catch
            {
            }
            return null;
        }

        public void SaveCurrentImage(string filename)
        {
            if (this.status == 1)
            {
                try
                {
                    Bitmap currentFrame = rtspPlayerControl1.GetCurrentFrame();
                    if (currentFrame != null)
                    {
                        using (Bitmap bmp = new Bitmap(currentFrame))
                        {
                            bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                            bmp.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Tools.SystemUI.SaveLogFile($"Get bitmap Exepetion. Ex: {ex.Message}");
                }
                finally
                {
                    GC.Collect();
                }
            }
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
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            { }

            return false;
        }
    }

}
