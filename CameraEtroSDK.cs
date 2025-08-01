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
    public partial class CameraEtroSDK : UserControl, ICameraWindow
    {
        private string cameraname = "Camera Etrovision";
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
        private int fps = 0;

        // new frame event
        public event NewFrameHandler NewFrame;

        private Rectangle[] userWindows = null;

        private Color borderColor = Color.Black;

        private Rectangle[] motionZones = null;

        public CameraEtroSDK()
        {
            InitializeComponent();

            // Events
            this.axIPSViewer1.NoSignal += new AxIPSLIB3._DIPSViewerEvents_NoSignalEventHandler(this.axIPSViewer1_NoSignal);
            this.axIPSViewer1.LoginSuccess += new System.EventHandler(this.axIPSViewer1_LoginSuccess);
            this.axIPSViewer1.StreamStopped += new System.EventHandler(this.axIPSViewer1_StreamStopped);
            this.axIPSViewer1.FPSChanged += new AxIPSLIB3._DIPSViewerEvents_FPSChangedEventHandler(this.axIPSViewer1_FPSChanged);
            this.axIPSViewer1.MPEG4BitRateChanged += new AxIPSLIB3._DIPSViewerEvents_MPEG4BitRateChangedEventHandler(this.axIPSViewer1_MPEG4BitRateChanged);
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
            get { return axIPSViewer1.Channel; }
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
            axIPSViewer1.Stop();
            axIPSViewer1.Dock = DockStyle.Fill;

            //axIPSViewer1.OutputType = 1; // 1-mjpeg, 8-h264
            //axIPSViewer1.PutFrameResolution(640, 480);
            axIPSViewer1.PutAddress(source, 1852, "");
            axIPSViewer1.PutAccount(login, password);
            axIPSViewer1.Preview();
            lbTitle.Text = "Connecting to " + cameraname + "...";
        }

        // Abort camera
        public void Stop()
        {
            axIPSViewer1.Stop();
            lbTitle.Text = "Stop to " + cameraname + "...";
            status = 0;
        }

        // Luu hinh anh camera
        public void SaveCurrentImage(string theFile)
        {
            //if (status == 1)
            axIPSViewer1.FileSnapShot(theFile); // jpeg
        }

        // get bitmap image from bitmap data
        public Bitmap GetCurrentImage()
        {
            return null;
        }

        // Start Record Video
        public void StartRecord()
        {

        }

        // Stop Record Video
        public void StopRecord()
        {

        }

        // Ket noi lai den camera
        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (status == 0)
            //    Start();
        }

        private void axIPSViewer1_NoSignal(object sender, AxIPSLIB3._DIPSViewerEvents_NoSignalEvent e)
        {
            lastFrame = null;
            status = 0;
            lbTitle.Text = "No Signal to " + cameraname + "...";
        }

        private void axIPSViewer1_LoginSuccess(object sender, EventArgs e)
        {
            status = 1;
            lbTitle.Text = "Connected to " + cameraname + "...";
        }

        private void axIPSViewer1_StreamStopped(object sender, EventArgs e)
        {
            lastFrame = null;
            status = 0;
            lbTitle.Text = "Disconnect to " + cameraname + "...";
        }

        private void axIPSViewer1_FPSChanged(object sender, AxIPSLIB3._DIPSViewerEvents_FPSChangedEvent e)
        {
            fps = e.fPS;
            lbTitle.Text = cameraname + " - " + e.fPS + " fps";
        }

        private void axIPSViewer1_MPEG4BitRateChanged(object sender, AxIPSLIB3._DIPSViewerEvents_MPEG4BitRateChangedEvent e)
        {

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
        [DefaultValue(typeof(Color), "Black")]
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
