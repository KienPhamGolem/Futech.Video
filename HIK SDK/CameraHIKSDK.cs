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
using HIKSDK;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Futech.Video
{
    public partial class CameraHIKSDK : UserControl, ICameraWindow
    {
        private string cameraname = "Camera HIKvision";
        private string source = "";
        private string resolution = "";
        private int httpPort = 80;
        private int rtspPort = 554;
        private string login = null;
        private string password = null;
        private string mediatype = "H264";
        private string cgi = "";
        private int channel = 1;
        private int status = 0;
        private CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo;
        private int pRealPlayHandle = -1;
        private int pLoginID = -1;

        private Bitmap lastFrame = null;

        private bool displayCameraTitle = true;
        private int fps = 0;

        // new frame event
        public event NewFrameHandler NewFrame;

        private Rectangle[] userWindows = null;

        private Color borderColor = Color.Black;

        private Rectangle[] motionZones = null;

        public CameraHIKSDK()
        {
            InitializeComponent();
            CHCNetSDK.NET_DVR_Init();
            CHCNetSDK.NET_DVR_SetReconnect(30000u, 1);
            // Create Folder
            string path = Application.StartupPath + "\\img_tmp_hik";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
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

        // Resolution property
        public string Resolution
        {
            get { return resolution; }
            set { resolution = value; }
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
                    lbTitle.Visible = true;
                else
                    lbTitle.Visible = false;
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
            try
            {
                string DVRIPAddress = this.source;
                int DVRPortNumber = this.httpPort;
                if (this.source.Contains("hik-online.com") || this.source.Contains("hiddns.com"))
                {
                    int ddns_index = this.source.IndexOf("hik-online.com");
                    int ddns_length = 15;
                    if (this.source.Contains("hiddns.com"))
                    {
                        ddns_index = this.source.IndexOf("hiddns.com");
                        ddns_length = 11;
                    }
                    string ddns;
                    if (ddns_index == 0)
                    {
                        ddns = this.source.Substring(ddns_length);
                    }
                    else
                    {
                        ddns = this.source.Substring(0, ddns_index - 1);
                    }
                    byte[] HiDDNSName = Encoding.Default.GetBytes(ddns);
                    byte[] GetIPAddress = new byte[16];
                    uint dwPort = 8000u;
                    if (CHCNetSDK.NET_DVR_GetDVRIPByResolveSvr_EX("www.hik-online.com", 80, HiDDNSName, (ushort)HiDDNSName.Length, null, 0, GetIPAddress, ref dwPort))
                    {
                        string arg_EF_0 = Encoding.UTF8.GetString(GetIPAddress);
                        char[] trimChars = new char[1];
                        DVRIPAddress = arg_EF_0.TrimEnd(trimChars);
                        DVRPortNumber = (int)((short)dwPort);
                    }
                    else
                    {
                        DVRIPAddress = this.source;
                        DVRPortNumber = this.httpPort;
                    }
                }

                this.pLoginID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, this.login, this.password, ref this.m_struDeviceInfo);
                if (this.pLoginID != -1)
                {
                    CHCNetSDK.NET_DVR_CLIENTINFO lpClientInfo = default(CHCNetSDK.NET_DVR_CLIENTINFO);
                    lpClientInfo.lChannel = this.channel;
                    lpClientInfo.lLinkMode = 0;
                    lpClientInfo.sMultiCastIP = "";
                    lpClientInfo.hPlayWnd = this.panel1.Handle;
                    this.pRealPlayHandle = CHCNetSDK.NET_DVR_RealPlay_V30(this.pLoginID, ref lpClientInfo, null, IntPtr.Zero, 0u);
                    if (this.pRealPlayHandle >= 0)
                    {
                        this.status = 1;
                    }
                }
                else
                {
                }

                // delete temp folder
                Thread thread = new Thread(DeleteImgTemp);
                thread.Start();

            }
            catch (Exception ex)
            {

            }
            finally
            {
                GC.Collect();
            }
        }

        private void DeleteImgTemp()
        {
            string imgPath_Temp = Application.StartupPath + "\\img_tmp_hik";              
            try
            {
                if (Directory.Exists(imgPath_Temp))
                {
                    DirectoryInfo directory = new DirectoryInfo(imgPath_Temp);
                    FileInfo[] fiArr = directory.GetFiles();
                    if (fiArr.Length > 0)
                    {
                        foreach (FileInfo fInfo in fiArr)
                        {
                            System.IO.File.Delete(fInfo.FullName);
                            System.Threading.Thread.Sleep(1);
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(imgPath_Temp);
                }    
            }
            catch
            { }
        }

        // Abort camera
        public void Stop()
        {
            lbTitle.Text = "Stop to " + cameraname + "...";
            status = 0;
        }

        // Luu hinh anh camera
        private List<string> listFileDelete = new List<string>();
        public void SaveCurrentImage(string theFile)
        {
            if (this.status == 1)
            {
                try
                {
                    CHCNetSDK.NET_DVR_JPEGPARA lpJpegPara = default(CHCNetSDK.NET_DVR_JPEGPARA);
                    lpJpegPara.wPicQuality = 0;
                    lpJpegPara.wPicSize = 255;

                    if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(this.pLoginID, this.channel, ref lpJpegPara, theFile))
                    {
                        if (theFile.Contains(".jpg"))
                        {
                            string newpath = theFile.Replace(".jpg", ".bmp");

                            if (CHCNetSDK.NET_DVR_CapturePicture(this.pLoginID, newpath))
                            {
                                System.Threading.Thread.Sleep(10);
                                int i = 0;
                                while (!System.IO.File.Exists(newpath) && i < 100)
                                {
                                    i += 1;
                                    System.Threading.Thread.Sleep(50);
                                }
                                if (System.IO.File.Exists(newpath))
                                {
                                    using (Bitmap bitmap = (Bitmap)System.Drawing.Image.FromFile(newpath))
                                    {
                                        bitmap.Save(theFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        bitmap.Dispose();
                                        // Delete bmp file
                                        CancellationToken cancellationToken = new CancellationToken();
                                        listFileDelete.Add(newpath);
                                        Task task = new Task(DeleteFile, cancellationToken);
                                        task.Start();

                                        // Delete bmp file
                                        //System.IO.File.Delete(newpath);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    GC.Collect();
                    Application.DoEvents();
                }
            }
        }
        private void DeleteFile()
        {
            try
            {
                while (listFileDelete.Count > 0)
                {
                    System.IO.File.Delete(listFileDelete[0]);
                    listFileDelete.RemoveAt(0);
                }
            }
            catch
            { }
        }

        // get bitmap image from bitmap data
        bool readyToCapture = true;
        DateTime lastTimeSnapshot = DateTime.Now;
        private object sync = new object();
        private int captureIndex = 1;
        public Bitmap GetCurrentImage()
        {
            System.Drawing.Bitmap bmp = null;
            if (this.status == 1 && this.readyToCapture)
            {
                this.readyToCapture = false;
                try
                {
                    TimeSpan tsFg = new TimeSpan(DateTime.Now.Ticks - this.lastTimeSnapshot.Ticks);
                    if (tsFg.TotalMilliseconds < 500.0)
                    {
                        lock (this.sync)
                        {
                            bmp = ((this.lastFrame != null) ? new System.Drawing.Bitmap(this.lastFrame) : null);
                            goto Flag;
                        }
                    }
                    lock (this.sync)
                    {
                        if (this.lastFrame != null)
                        {
                            this.lastFrame.Dispose();
                            this.lastFrame = null;
                        }
                        string fileName = string.Concat(new object[]
                        {
                            Application.StartupPath,
                            "\\img_tmp_hik\\tmp_",
                            this.source,
                            "_",
                            this.httpPort,
                            "_",
                            this.channel,
                            "_",
                            this.pLoginID,
                            "_",
                            this.pRealPlayHandle,
                            "_",
                            this.captureIndex,
                            ".bmp"
                        });
                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }
                        this.captureIndex++;
                        if (this.captureIndex >= 200)
                        {
                            this.captureIndex = 1;
                        }
                        {
                            if (CHCNetSDK.NET_DVR_CapturePicture(this.pRealPlayHandle, fileName))
                            {
                                this.lastTimeSnapshot = DateTime.Now;
                                int index = 0;
                                while (!File.Exists(fileName) && index < 200)
                                {
                                    index += 1;
                                    Thread.Sleep(20);
                                    GC.Collect();
                                }
                                if (File.Exists(fileName))
                                {
                                    Thread.Sleep(100);
                                    this.lastFrame = new System.Drawing.Bitmap(System.Drawing.Image.FromFile(fileName));
                                    GC.Collect();
                                }
                                else
                                {
                                    Thread.Sleep(100);
                                    if (File.Exists(fileName))
                                        this.lastFrame = new System.Drawing.Bitmap(System.Drawing.Image.FromFile(fileName));
                                    GC.Collect();
                                }
                            }
                            else
                            {
                                //if (this.VideoSourceError != null)
                                //{
                                //    this.VideoSourceError(this, new VideoSourceErrorEventArgs("Error while get current image from camera.\nError Code: " + CHCNetSDK.NET_DVR_GetLastError()));
                                //}
                            }
                        }
                    }
                    bmp = ((this.lastFrame != null) ? new System.Drawing.Bitmap(this.lastFrame) : null);
                Flag:;
                }
                catch (Exception ex)
                {
                    //if (this.VideoSourceError != null)
                    //{
                    //    this.VideoSourceError(this, new VideoSourceErrorEventArgs("Error while get current image from camera.\n" + ex.ToString()));
                    //}
                }
                finally
                {
                    GC.Collect();
                }
                this.readyToCapture = true;
            }
            return bmp;
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
