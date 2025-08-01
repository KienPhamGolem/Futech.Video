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
using AxNVRMEDIACLIENTAXCTRLLib;
using System.Timers;

namespace Futech.Video
{
    public partial class CameraKztek3 : UserControl,ICameraWindow
    {
        public CameraKztek3()
        {
            InitializeComponent();
            this.axNVRMediaViewer1.EnableReconnect = true;
            this.axNVRMediaViewer1.EnablePopup = false;
            this.axNVRMediaViewer1.EnablePopupRecord = false;
            this.axNVRMediaViewer1.EnablePopupSetting = false;
            this.axNVRMediaViewer1.EnablePopupSnapshot = false;
            this.axNVRMediaViewer1.EnablePopupFishEye = false;
            this.axNVRMediaViewer1.EnableGetAudio = false;
            this.axNVRMediaViewer1.EnableGetVideo = false;
            this.axNVRMediaViewer1.EnableGetEvent = false;
            this.axNVRMediaViewer1.Mute = true;
            this.axNVRMediaViewer1.EnableOverlay = false;
            this.axNVRMediaViewer1.ForceGDI = true;
            this.axNVRMediaViewer1.MediaFileFormat = 2;
            this.axNVRMediaViewer1.MediaInfoEnabled = true;
            this.axNVRMediaViewer1.OnMediaInfo += new _IMediaViewerEvents_OnMediaInfoEventHandler(this.axNVRMediaViewer1_OnMediaInfo);
            this.axNVRMediaViewer1.OnNewImage += new EventHandler(this.axNVRMediaViewer1_OnNewImage);
            this.axNVRMediaViewer1.OnRTPVideoFrame += new _IMediaViewerEvents_OnRTPVideoFrameEventHandler(this.axNVRMediaViewer1_OnRTPVideoFrame);
            this.axNVRMediaViewer1.OnError += new _IMediaViewerEvents_OnErrorEventHandler(this.axNVRMediaViewer1_OnError);
            this.axNVRMediaViewer1.OnReconnect += new _IMediaViewerEvents_OnReconnectEventHandler(this.axNVRMediaViewer1_OnReconnect);
            this.axNVRMediaViewer1.OnMouseDown += new _IMediaViewerEvents_OnMouseDownEventHandler(this.axNVRMediaViewer1_OnMouseDown);
            this.axNVRMediaViewer1.OnMouseUp += new _IMediaViewerEvents_OnMouseUpEventHandler(this.axNVRMediaViewer1_OnMouseUp);
            this.axNVRMediaViewer1.OnMouseMove += new _IMediaViewerEvents_OnMouseMoveEventHandler(this.axNVRMediaViewer1_OnMouseMove);
            this.axNVRMediaViewer1.OnMouseWheel += new _IMediaViewerEvents_OnMouseWheelEventHandler(this.axNVRMediaViewer1_OnMouseWheel);
            this.axNVRMediaViewer1.OnDoubleClick += new _IMediaViewerEvents_OnDoubleClickEventHandler(this.axNVRMediaViewer1_OnDoubleClick);

            this.videoLengthTimer.Interval = 60000.0;
            this.videoLengthTimer.Elapsed += new ElapsedEventHandler(this.videoLengthTimer_Tick);
            this.videoLengthTimer.AutoReset = true;
            GC.KeepAlive(this.videoLengthTimer);
        }

        #region Control Function
        private void axNVRMediaViewer1_OnDoubleClick(object sender, _IMediaViewerEvents_OnDoubleClickEvent e)
        {
            //throw new NotImplementedException();
        }

        private void axNVRMediaViewer1_OnMouseWheel(object sender, _IMediaViewerEvents_OnMouseWheelEvent e)
        {
            //throw new NotImplementedException();
        }

        private void axNVRMediaViewer1_OnMouseMove(object sender, _IMediaViewerEvents_OnMouseMoveEvent e)
        {
            //throw new NotImplementedException();
        }

        private void axNVRMediaViewer1_OnMouseUp(object sender, _IMediaViewerEvents_OnMouseUpEvent e)
        {
            //throw new NotImplementedException();
        }

        private void axNVRMediaViewer1_OnMouseDown(object sender, _IMediaViewerEvents_OnMouseDownEvent e)
        {
            //throw new NotImplementedException();
        }

        private void axNVRMediaViewer1_OnReconnect(object sender, _IMediaViewerEvents_OnReconnectEvent e)
        {
            //throw new NotImplementedException();
        }

        private void axNVRMediaViewer1_OnError(object sender, _IMediaViewerEvents_OnErrorEvent e)
        {
            //throw new NotImplementedException();
        }

        private void axNVRMediaViewer1_OnRTPVideoFrame(object sender, _IMediaViewerEvents_OnRTPVideoFrameEvent e)
        {
            //throw new NotImplementedException();
        }

        private void axNVRMediaViewer1_OnNewImage(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void axNVRMediaViewer1_OnMediaInfo(object sender, _IMediaViewerEvents_OnMediaInfoEvent e)
        {
            //throw new NotImplementedException();
        }
        #endregion

        // new frame event
        public event NewFrameHandler NewFrame;

        private Rectangle[] userWindows = null;

        private Color borderColor = Color.Black;

        private Rectangle[] motionZones = null;

        private string cameraname = "Camera Kztek";
        private string source = "";
        private string resolution = "";
        private int httpPort = 80;
        private int rtspPort = 554;
        private string login = null;
        private string password = null;
        private string mediatype = "MJPEG";
        private string cgi = "";
        private int channel = 1;
        private int status = 0;
        private float fps = 0;
        private string url = "rtsp://192.168.1.10//user=admin_password=_channel=1_stream=0.sdp";
        private string urlimage = "";
        private Bitmap lastFrame = null;

        private int videoLengthIndex = 0;
        private int recordingVideoLength = 30;
        string VideoPath = "";
        private System.Timers.Timer videoLengthTimer = new System.Timers.Timer();


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
                url = RtspTools.GetRTSP(cameratype, source, rtspPort, login, password, channel);
                //if (CheckConnect(source))
                {
                    lock (this.sync)
                    {
                        this.axNVRMediaViewer1.StopRecord();
                        this.axNVRMediaViewer1.Stop();
                        this.axNVRMediaViewer1.MediaUsername = this.login;
                        this.axNVRMediaViewer1.MediaPassword = this.password;
                        this.axNVRMediaViewer1.MediaType = "rtsp";
                        this.axNVRMediaViewer1.MediaURL = this.url;
                        this.axNVRMediaViewer1.EnableReconnect = true;
                        this.axNVRMediaViewer1.SetBackground(false);
                        this.axNVRMediaViewer1.Mute = true;
                        if (this.axNVRMediaViewer1.Play())
                            this.status = 1;
                       
                    }
                }
                // delete temp folder
                Thread thread = new Thread(DeleteImgTemp);
                thread.Start();


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

        private void DeleteImgTemp()
        {
            string imgPath_Temp = Application.StartupPath + "\\img_tmp_kzsdk2";
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

        public void Stop()
        {
            try
            {
                if (this.enablerecording)
                {
                    this.StopRecord();
                }
                this.axNVRMediaViewer1.Stop();
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

        private string currentVideoFile = "";
        private bool isRecording = false;
        public void StartRecord()
        {
            if (this.status == 1)
            {
                try
                {
                    lock (this.sync)
                    {
                        string path = string.Concat(new object[]
                        {
                            this.videofolder,
                            "\\Nam_",
                            DateTime.Now.Year,
                            "\\Thang_",
                            DateTime.Now.Month,
                            "\\Ngay_",
                            DateTime.Now.ToString("dd_MM_yyyy")
                        });
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        this.currentVideoFile = path + "\\" + this.cameraname + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".avi";                       
                        if (this.axNVRMediaViewer1.StartRecord(this.currentVideoFile))
                        {
                            this.isRecording = true;
                            this.videoLengthIndex = 0;
                            this.videoLengthTimer.Start();
                        }
                    }
                }
                catch (Exception )
                {
                    throw;
                }
            }
        }

        public void StopRecord()
        {
            try
            {
                lock (this.sync)
                {
                    this.isRecording = false;
                    this.videoLengthTimer.Stop();
                    this.videoLengthIndex = 0;
                    this.axNVRMediaViewer1.StopRecord();
                    this.currentVideoFile = "";
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                GC.Collect();
            }
        }


        private void videoLengthTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.videoLengthIndex++;
                if (this.videoLengthIndex >= this.recordingVideoLength)
                {
                    this.videoLengthIndex = 0;
                    this.StopRecord();
                    this.StartRecord();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                GC.Collect();
            }
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
                    if (tsFg.TotalMilliseconds < 300.0)
                    {
                        lock (this.sync)
                        {
                            try
                            {
                                bmp = ((this.lastFrame != null) ? new System.Drawing.Bitmap(this.lastFrame) : null);
                            }
                            catch (Exception ex)
                            {
                                Tools.SystemUI.SaveLogFile($"Get bitmap Exepetion. Ex: {ex.Message}");
                            }
                            goto IL_209;
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
                        "\\img_tmp_kzsdk2\\tmp_",
                        Tools.SystemUI.RemoveUseLess(this.source),
                        "_",
                        DateTime.Now.ToString("yyyyMMddhhmmss"),
                        //DateTime.Now.ToString("yyyyMMddhhmmss") + DateTime.Now.Millisecond.ToString(),
                        "_",
                        this.captureIndex,
                        ".bmp"
                        });
                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }
                        this.captureIndex++;
                        if (this.captureIndex >= 50)
                        {
                            this.captureIndex = 1;
                        }

                        if (this.axNVRMediaViewer1.SaveCurrentImage(0, fileName))
                        {
                            Thread.Sleep(20);
                            this.lastTimeSnapshot = DateTime.Now;
                            int index = 0;
                            while (!File.Exists(fileName) && index < 100)
                            {
                                index++;
                                Thread.Sleep(30);
                                Application.DoEvents();
                            }
                            if (File.Exists(fileName))
                            {
                                Thread.Sleep(20);
                                try
                                {
                                    this.lastFrame = new System.Drawing.Bitmap(System.Drawing.Image.FromFile(fileName));
                                }
                                catch
                                {
                                    this.lastFrame = null;
                                }
                            }
                        }

                        if (this.lastFrame == null)
                        {
                            if (File.Exists(fileName))
                            {
                                File.Delete(fileName);
                            }
                            this.axNVRMediaViewer1.SaveCurrentImage(0, fileName);
                            Thread.Sleep(300);
                            if (File.Exists(fileName))
                            {
                                try
                                {
                                    this.lastFrame = new System.Drawing.Bitmap(System.Drawing.Image.FromFile(fileName));
                                }
                                catch { }
                            }
                        }

                        bmp = ((this.lastFrame != null) ? new System.Drawing.Bitmap(this.lastFrame) : null);
                    }
                IL_209:;
                }
                catch (Exception ex)
                {
                    Tools.SystemUI.SaveLogFile($"Get bitmap Exepetion. Ex: {ex.Message}");
                }
                finally
                {
                    GC.Collect();
                    Application.DoEvents();
                }
                this.readyToCapture = true;
            }
            return bmp;
        }

        public void SaveCurrentImage(string filename)
        {
            if (this.status == 1)
            {
                try
                {
                    this.axNVRMediaViewer1.SaveCurrentImage(0, filename);
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

        public int RecordingVideoLength { get => recordingVideoLength; set => recordingVideoLength = value; }

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
