using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using System.Runtime.InteropServices;
using System.Threading;

namespace Futech.Video
{
    public partial class CameraGeoSDK : UserControl,ICameraWindow
    {
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

        private Bitmap lastFrame = null;

        private bool displayCameraTitle = true;
        private float fps = 0;

        // new frame event
        public event NewFrameHandler NewFrame;

        private Rectangle[] userWindows = null;

        private Color borderColor = Color.Black;

        private Rectangle[] motionZones = null;

        public CameraGeoSDK()
        {
            InitializeComponent();

            // Thay doi kich thuoc hinh anh camera vua voi man hinh
            this.Resize += new System.EventHandler(this.CameraGeoSDK_Resize);

            // Events
            this.axLiveX1.OnConnect += new System.EventHandler(this.axLiveX1_OnConnect);
            this.axLiveX1.OnGetPicture += new AxLIVEXLib._DLiveXEvents_OnGetPictureEventHandler(this.axLiveX1_OnGetPicture);
            this.axLiveX1.OnDisconnect += new System.EventHandler(this.axLiveX1_OnDisconnect);
        }

        // Name property
        public string CameraName
        {
            set
            {
                cameraname = value;
                lbTitle.Text = value;
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
            get { return axLiveX1.DefaultCam; }
            set 
            { 
                channel = value;
                axLiveX1.DefaultCam = (short)channel;
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
            get { return fps; }
        }

        // LastMessage property
        public string LastMessage
        {
            get { return lbTitle.Text; }
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
            //if (axLiveX1.IsPlay())
            axLiveX1.StopX();

            //Thread.Sleep(500);
            axLiveX1.Dock = DockStyle.Fill;

            axLiveX1.UserName = login;
            axLiveX1.Password = password;
            axLiveX1.IpAddress = source;
            axLiveX1.DefaultCam = (short)channel;

            axLiveX1.DisablePWD = true;
            axLiveX1.SetCntDeviceType(0);
            //axLiveX1.SetGUIMode(3, 0, 0);
            axLiveX1.AutoReConnect = true;
            axLiveX1.RetryInterval = 20; // ~ 10-40s
            axLiveX1.FixSize = true;
            axLiveX1.FixHeight = this.Height;
            axLiveX1.FixWidth = this.Width;
            //axLiveX1.AutoLogin = true;
            axLiveX1.MaxRetries = -1; // unlimit
            //axLiveX1.WelcomeBMP = "Hello";
            //axLiveX1.ChangeSizeX(640, 480);
            axLiveX1.ChangeQualityX(129);
            axLiveX1.EnableAutoScreenSize(true);

            // Start the streaming
            axLiveX1.PlayX();
            axLiveX1.StartGrapImage(true);
            lbTitle.Text = "Connecting to " + cameraname + "...";

            axLiveX1.Dock = DockStyle.Fill;
        }

        // Abort camera
        public void Stop()
        {
            //if (axLiveX1.IsPlay())
            //{
            axLiveX1.StopX();
            axLiveX1.StartGrapImage(false);
            //}
            lbTitle.Text = "Stop to " + cameraname + "...";
            status = 0;
        }

        // Luu hinh anh camera
        public void SaveCurrentImage(string theFile)
        {
            if (axLiveX1.IsPlay())
                axLiveX1.SnapShotToFile(theFile); // jpeg
        }

        // get bitmap image from bitmap data
        public Bitmap GetCurrentImage()
        {
            return lastFrame;
        }

        // Start Record Video
        public void StartRecord()
        {

        }

        // Stop Record Video
        public void StopRecord()
        {

        }

        private void axLiveX1_OnGetPicture(object sender, AxLIVEXLib._DLiveXEvents_OnGetPictureEvent e)
        {
            try
            {
                fps = axLiveX1.GetFrameRate();
                lastFrame = Bitmap.FromHbitmap(new System.IntPtr(e.mPict.Handle));
                lbTitle.Text = cameraname + @"\" + axLiveX1.DefaultCam + " - " + (int)axLiveX1.GetFrameRate() + " fps";
                status = 1;
                if (NewFrame != null)
                    NewFrame(this, new NewFrameArgs(lastFrame));
            }
            catch
            {
            }
        }

        private void axLiveX1_OnConnect(object sender, EventArgs e)
        {
            axLiveX1.SetReceiveImageSize(4);
            status = 1;
            lbTitle.Text = "Connected to " + cameraname + "...";
        }

        private void axLiveX1_OnDisconnect(object sender, EventArgs e)
        {
            lastFrame = null;
            status = 0;
            lbTitle.Text = "Disconnect to " + cameraname + "...";
        }

        private void CameraGeoSDK_Resize(object sender, EventArgs e)
        {
            axLiveX1.FixSize = true;
            axLiveX1.FixHeight = this.Height;
            axLiveX1.FixWidth = this.Width;
        }

        /// <summary>
        /// UserWindow
        /// </summary>
        public Rectangle[] UserWindows
        {
            get { return userWindows; }
            set { userWindows = value; }
        }

        /// <summary>
        /// BorderColor
        /// </summary>
        [DefaultValue( typeof( Color ), "Black" )]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }

        /// <summary>
        /// MotionZones
        /// </summary>
        public Rectangle[] MotionZones
        {
            get { return motionZones; }
            set { motionZones = value; }
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
    }
}
