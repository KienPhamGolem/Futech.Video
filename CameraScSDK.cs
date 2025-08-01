using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using QCAP.NET;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
namespace Futech.Video
{
    //[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public partial class CameraScSDK : UserControl,ICameraWindow
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);
        private string cameraname = "Camera SC330";
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
        public event NewFrameHandler NewFrame = null;

        private Rectangle[] userWindows = null;

        private Color borderColor = Color.Black;

        private Rectangle[] motionZones = null;



        string m_strChipName = "TW6802 PCI";

        // DEVICE PROPERTY

        EXPORTS.PF_NO_SIGNAL_DETECTED_CALLBACK m_pNoSignalDetectedCB;
        EXPORTS.PF_FORMAT_CHANGED_CALLBACK m_pFormatChangedCB;
        EXPORTS.PF_VIDEO_PREVIEW_CALLBACK m_pPreviewVideoCB;
        //
        public uint[] m_hCapDev = new uint[8];

        public bool[] m_bNoSignal = new bool[8];

        unsafe public byte* m_pBuffer;                            // MEMORY BUFFER

        //public byte[] bArr = new byte[1920 * 1080 * 4];

        int videowidth = 720;
        int videoheight = 480;

        public byte[] bArr = null;//new byte[720 * 480 * 4];

       // public byte[] bArr = new byte[videow * 480 * 4];

        public Bitmap m_BmpSnapshot = null;          // C# BITMAP OBJECT

        private Object thisLock = new Object();

        string VideoPath = @"C:\VideoRecord";

        public CameraScSDK()
        {
            InitializeComponent();

           
        }

        // Name property
        public string CameraName
        {
            set
            {
                //cameraname = value;
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
            set 
            {
                cgi = value;
                string[] temp = cgi.Split('x');
                if (temp != null && temp.Length == 2)
                {
                    videowidth = Convert.ToInt32(temp[0]);
                    videoheight = Convert.ToInt32(temp[1]);
                }
            }
        }

        // Channel property
        public int Channel
        {
            get { return channel; }
            set
            {
                channel = value - 1;
                if (channel < 0)
                    channel = 0;
                if (channel > 7)
                    channel = 7;
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

        //Start
        public void Start()
        {
            bArr = new byte[videowidth * videoheight * 4];
           
            unsafe
            {
                m_pBuffer = (byte*)memory.Alloc(videowidth * videoheight * 4);
            }
           
            for (int i = 0; i < 8; i++) { m_hCapDev[i] = 0x00000000; }
            EXPORTS.QCAP_CREATE(ref m_strChipName, (uint)channel, (uint)myChanelControl.Handle.ToInt32(), ref m_hCapDev[channel],1, 0);

            m_pNoSignalDetectedCB = new EXPORTS.PF_NO_SIGNAL_DETECTED_CALLBACK(on_process_no_signal_detected);

            EXPORTS.QCAP_REGISTER_NO_SIGNAL_DETECTED_CALLBACK(m_hCapDev[channel], m_pNoSignalDetectedCB, 0);

            m_pFormatChangedCB = new EXPORTS.PF_FORMAT_CHANGED_CALLBACK(on_process_format_changed);

            EXPORTS.QCAP_REGISTER_FORMAT_CHANGED_CALLBACK(m_hCapDev[channel], m_pFormatChangedCB, 0);

            m_pPreviewVideoCB = new EXPORTS.PF_VIDEO_PREVIEW_CALLBACK(on_process_preview_video_buffer);
            EXPORTS.QCAP_REGISTER_VIDEO_PREVIEW_CALLBACK(m_hCapDev[channel], m_pPreviewVideoCB, 0);

            uint nInput = (uint)EXPORTS.InputVideoSourceEnum.QCAP_INPUT_TYPE_SDI;
            EXPORTS.QCAP_SET_VIDEO_INPUT(m_hCapDev[channel], nInput);
            EXPORTS.QCAP_SET_VIDEO_DEINTERLACE(m_hCapDev[channel], 0);

            EXPORTS.QCAP_SET_VIDEO_DEFAULT_OUTPUT_FORMAT(m_hCapDev[channel], (uint)videowidth, (uint)videoheight, 0, 25);

            EXPORTS.QCAP_SET_AUDIO_RECORD_PROPERTY(m_hCapDev[channel], 0, (uint)EXPORTS.EncoderTypeEnum.QCAP_ENCODER_TYPE_SOFTWARE, (uint)EXPORTS.AudioEncoderFormatEnum.QCAP_ENCODER_FORMAT_AAC);
            EXPORTS.QCAP_SET_VIDEO_RECORD_PROPERTY(m_hCapDev[channel], 0, (uint)EXPORTS.EncoderTypeEnum.QCAP_ENCODER_TYPE_SOFTWARE, (uint)EXPORTS.VideoEncoderFormatEnum.QCAP_ENCODER_FORMAT_H264, (uint)EXPORTS.RecordModeEnum.QCAP_RECORD_MODE_CBR, 1000, 1024 * 1024 / 4, 30, 0, 0, (uint)EXPORTS.DownScaleModeEnum.QCAP_DOWNSCALE_MODE_1_4);
           
            EXPORTS.QCAP_RUN(m_hCapDev[channel]);

            if (this.enablerecording)
            {
                VideoPath = this.videofolder;
             
                timerVideoLength.Start();
            }
        }

        //stop
        public void Stop()
        {
            if (m_hCapDev[channel] != 0)
            {
                StopRecord();
                EXPORTS.QCAP_STOP(m_hCapDev[channel]);
                EXPORTS.QCAP_DESTROY(m_hCapDev[channel]);
            }

            unsafe
            {
                // RELEASE MEMORY BUFFER
                //
                memory.free(m_pBuffer);
            }
           
        }

        // Luu hinh anh camera
        public void SaveCurrentImage(string theFile)
        {
            EXPORTS.QCAP_SNAPSHOT_JPG(m_hCapDev[channel], ref theFile, 100, 0, 0);
        }

     
        public Bitmap GetCurrentImage()
        {
            try
            {
                unsafe
                {
                   

                    Bitmap bmp = null;
                    lock (this)
                    {
                        EXPORTS.QCAP_COLORSPACE_YUY2_TO_ABGR32(pBuffer, (uint)videowidth, (uint)videoheight, (uint)videowidth * 2, (uint)m_pBuffer, (uint)videowidth, (uint)videoheight, (uint)videowidth * 4, 0, 0, 0);
                    }
                    fixed (byte* pArr = bArr)
                    {
                        memory.Copy(m_pBuffer, pArr, videowidth * videoheight * 4);
                    }

                    // CAPTURE TO BITMAP OBJECT FROM MEMORY
                   
                    int nStride = 4 * ((32 * (int)videowidth + 31) / 32);
                    m_BmpSnapshot = new Bitmap((int)videowidth, (int)videoheight, nStride, PixelFormat.Format32bppPArgb, (IntPtr)m_pBuffer);


                    using (MemoryStream ms = new MemoryStream())
                    {
                        m_BmpSnapshot.Save(ms, ImageFormat.Bmp);
                        bmp = new Bitmap(new Bitmap(ms));//(Bitmap)Image.FromStream(ms);
                        ms.Dispose();
                    }
                    GC.Collect();
               
                    return bmp;
                }
            }
            catch
            {
                return null;
            }
          
            
        }

        // Start Record Video
        public void StartRecord()
        {
            try
            {
                string path = VideoPath + "\\CAM" + channel.ToString() + "\\" + DateTime.Now.ToString("dd-MM-yyyy");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                string videofile = path + "\\" + DateTime.Now.ToString("HHmmss") + ".mp4";

                EXPORTS.QCAP_START_RECORD(m_hCapDev[channel], 0, ref videofile);
                timerVideoLength.Start();
            }
            catch
            { }
        }

        // Stop Record Video
        public void StopRecord()
        {
            try
            {
               
                if (m_hCapDev[channel] != 0)
                {
                    EXPORTS.QCAP_STOP_RECORD(m_hCapDev[channel], 0);

                    System.Threading.Thread.Sleep(100);
                    Application.DoEvents();
                }

                string path = VideoPath + "\\CAM" + channel.ToString() + "\\" + DateTime.Now.AddDays(-7).ToString("dd-MM-yyyy");

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

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
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


        EXPORTS.ReturnOfCallbackEnum on_process_no_signal_detected(uint pDevice, uint nVideoInput, uint nAudioInput, uint pUserData)
        {
            uint nCH = pUserData;
            m_bNoSignal[nCH] = true;

            return EXPORTS.ReturnOfCallbackEnum.QCAP_RT_OK;
        }

        EXPORTS.ReturnOfCallbackEnum on_process_format_changed(uint pDevice, uint nVideoInput, uint nAudioInput, uint nVideoWidth, uint nVideoHeight, uint bVideoIsInterleaved, uint nVideoFrameRate, uint nAudioChannels, uint nAudioBitsPerSample, uint nAudioSampleFrequency, uint pUserData)
        {
            uint nCH = pUserData;

                 
            fps = (float)nVideoFrameRate;
            //       
            if (nVideoWidth == 0 && nVideoHeight == 0 && nVideoFrameRate == 0 && nAudioChannels == 0 && nAudioBitsPerSample == 0 && nAudioSampleFrequency == 0)
            {
                m_bNoSignal[nCH] = true;
            }
            else
            {
                m_bNoSignal[nCH] = false;

            }

            return EXPORTS.ReturnOfCallbackEnum.QCAP_RT_OK;
        }
       // public uint isOn = 1;
        uint pBuffer = 0;
        EXPORTS.ReturnOfCallbackEnum on_process_preview_video_buffer(uint pDevice, double dSampleTime, uint pFrameBuffer, uint nFrameBufferLen, uint pUserData)
        {

            if (nFrameBufferLen > 0)
            {

                pBuffer = pFrameBuffer;
            }

            return EXPORTS.ReturnOfCallbackEnum.QCAP_RT_OK;
        }

        //brightness
        public uint Get_BRIGHTNESS()
        {
            uint pbrightness = 0;
            EXPORTS.QCAP_GET_VIDEO_BRIGHTNESS(m_hCapDev[channel], ref pbrightness);
            return pbrightness;
        }
        public void Set_BRIGHTNESS(uint pbrightness)
        {
            EXPORTS.QCAP_SET_VIDEO_BRIGHTNESS(m_hCapDev[channel], pbrightness);
        }

        //contrast
        public uint Get_CONTRAST()
        {
            uint pcontrast = 0;
            EXPORTS.QCAP_GET_VIDEO_CONTRAST(m_hCapDev[channel],ref pcontrast);
            return pcontrast;
        }
        public void Set_CONTRAST(uint pcontrast)
        {
            EXPORTS.QCAP_SET_VIDEO_CONTRAST(m_hCapDev[channel], pcontrast);
        }

        //hue
        public uint Get_HUE()
        {
            uint phue = 0;
            EXPORTS.QCAP_GET_VIDEO_HUE(m_hCapDev[channel], ref phue);
            return phue;
        }
        public void Set_HUE(uint phue)
        {
            EXPORTS.QCAP_SET_VIDEO_HUE(m_hCapDev[channel], phue);
        }

        //saturation
        public uint Get_SATURATION()
        {
            uint psaturation = 0;
            EXPORTS.QCAP_GET_VIDEO_SATURATION(m_hCapDev[channel], ref psaturation);
            return psaturation;
        }
        public void Set_SATURATION(uint psaturation)
        {
            EXPORTS.QCAP_SET_VIDEO_SATURATION(m_hCapDev[channel], psaturation);
        }

        //sharpness
        public uint Get_SHARPNESS()
        {
            uint psharpness = 0;
            EXPORTS.QCAP_GET_VIDEO_SHARPNESS(m_hCapDev[channel], ref psharpness);
            return psharpness;
        }
        public void Set_SHARPNESS(uint psharpness)
        {
            EXPORTS.QCAP_SET_VIDEO_SHARPNESS(m_hCapDev[channel], psharpness);
        }

        private string cameratype = "";
        public string CameraType
        {
            get { return cameratype; }
            set { cameratype = value; }
        }


        int videolengthindex = 0;
        bool isFirstTime = true;
        private void timerVideoLength_Tick(object sender, EventArgs e)
        {
            videolengthindex++;
            if (isFirstTime == true)
            {
                if (videolengthindex >= 10 + channel*3)
                {
                    isFirstTime = false;
                    StartRecord();
                }
            }
            if (videolengthindex >= 300 + channel*3)
            {
                timerVideoLength.Stop();
                videolengthindex = 0;
                StopRecord();
                if (this.enablerecording)
                {
                    StartRecord();
                }
            }
        }
        
    }
}
