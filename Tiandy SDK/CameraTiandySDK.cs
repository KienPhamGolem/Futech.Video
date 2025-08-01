using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using TiandyNetClient;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Futech.Video
{
    public partial class CameraTiandySDK : UserControl, ICameraWindow
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
        CLIENTINFO m_cltInfo;
        CONNECT_STATE m_conState;
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


        private Rectangle SnapshotCut;

        #region Properties
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

        private bool _IsResize = false;

        public bool IsResize
        {
            get { return _IsResize; }
            set { _IsResize = value; }
        }

        #endregion

        public CameraTiandySDK()
        {
            InitializeComponent();
            Init_SDK();
        }

        private void Init_SDK()
        {
            m_cltInfo.m_iServerID = -1;
            NVSSDK.NetClient_SetPort(3000, 6000);
            NVSSDK.NetClient_Startup();
            m_conState = new CONNECT_STATE();
            m_conState.m_iChannelNO = -1;
            m_conState.m_iLogonID = -1;
            m_conState.m_uiConID = UInt32.MaxValue;
            // Create Folder
            string path = Application.StartupPath + "\\img_tmp";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        // Start video source
        public void Start()
        {
            try
            {
                string strProxy = "";
                string strProxyID = "";
                int iPort = 3000;
                int iRet_Logon;

                //Logon
                iRet_Logon = NVSSDK.NetClient_Logon(strProxy, this.source, this.login, this.password, strProxyID, iPort);
                if (iRet_Logon < 0)
                {
                    m_cltInfo.m_iServerID = -1;
                    return;
                }
                m_cltInfo.m_iServerID = iRet_Logon;

                // Connect
                m_cltInfo.m_iChannelNo = this.channel - 1;
                m_cltInfo.m_iNetMode = 1; //1-TCP 2-UDP 3-MULTICAST
                m_cltInfo.m_iStreamNO = 0; // 0

                m_cltInfo.m_cNetFile = new char[255];
                m_cltInfo.m_cRemoteIP = new char[16];
                Array.Copy(this.source.ToCharArray(), m_cltInfo.m_cRemoteIP, this.source.Length);
                UInt32 uiConID = m_conState.m_uiConID;

                //Get the video playback status corresponding to the current window
                int iRet = NVSSDK.NetClient_GetPlayingStatus(uiConID);

                //If the video is playing, do not connect
                if (iRet != SDKConstMsg.PLAYER_PLAYING)
                {
                    int iChannelNum = 0;

                    //Get the maximum number of channels of the network video server connected to the current window
                    NVSSDK.NetClient_GetChannelNum(m_cltInfo.m_iServerID, ref iChannelNum);

                    //Determine whether the maximum channel number is exceeded
                    if (m_cltInfo.m_iChannelNo >= iChannelNum)
                    {
                        return;
                    }
                    //Start to receive one channel of video data
                    iRet = NVSSDK.NetClient_StartRecv(ref uiConID, ref m_cltInfo, null);

                    //Operation failed, clear the information of structure m_conState
                    if (iRet < 0)
                    {
                        m_conState.m_iLogonID = -1;
                        m_conState.m_uiConID = UInt32.MaxValue;
                        m_conState.m_iChannelNO = -1;
                        return;
                    }
                    //The operation is successful, update the information of the structure m_conState
                    m_conState.m_iLogonID = m_cltInfo.m_iServerID;
                    m_conState.m_iChannelNO = m_cltInfo.m_iChannelNo;
                    m_conState.m_uiConID = uiConID;
                    m_conState.m_iStreamNO = m_cltInfo.m_iStreamNO;

                    //Start to export the received data
                    NVSSDK.NetClient_StartCaptureData(uiConID);
                    RECT rect = new RECT();
                    //Start playing a certain video
                    if (NVSSDK.NetClient_StartPlay(uiConID, this.panel1.Handle, rect, 0) >= 0)
                    {
                        this.status = 1;
                    }
                }
                // Delete temp folder
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
            string imgPath_Temp = Application.StartupPath + "\\img_tmp";
            try
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
            catch
            { }
        }

        // Abort camera
        public void Stop()
        {
            /*   NVSSDK.NetClient_StopCaptureData((UInt32)m_conState.m_uiConID);
               NVSSDK.NetClient_StopRecv((UInt32)m_conState.m_uiConID);
               NVSSDK.NetClient_Cleanup();
               NVSSDK.NetClient_Logoff(m_conState.m_iLogonID);*/

            lbTitle.Text = "Stop to " + cameraname + "...";
            NVSSDK.NetClient_StopRecv(m_conState.m_uiConID);//Stop one video receiving
            m_conState.m_iChannelNO = -1;//Modify the channel number and connection ID of the current window
            m_conState.m_uiConID = UInt32.MaxValue;
            this.panel1.Invalidate(true);//Refresh the current window and update its status information
            int iLogonID = m_conState.m_iLogonID;
            if (iLogonID < 0)//If the current window is not logged in, no operation is performed
            {
                return;
            }

            //Log out the user login corresponding to the current window
            NVSSDK.NetClient_Logoff(iLogonID);

            //Update the corresponding window information
            if (m_conState.m_iLogonID == iLogonID)
            {
                m_conState.m_iLogonID = -1;
                m_conState.m_iChannelNO = -1;
                m_conState.m_uiConID = UInt32.MaxValue;
            }
            status = 0;
        }

        // Luu hinh anh camera
        private List<string> listFileDelete = new List<string>();
        public void SaveCurrentImage(string theFile)
        {
            try
            {
                UInt32 uiConID1 = m_conState.m_uiConID;
                //Only record the connected windows
                if (uiConID1 == UInt32.MaxValue)
                {
                    return;
                }
                string strFileName = theFile.Replace(".jpg", ".bmp");
                //Start to write the received data to the file  
                if (NVSSDK.NetClient_CaptureBmpPic(uiConID1, strFileName) > 0)
                {
                    System.Threading.Thread.Sleep(100);
                    int i = 0;
                    while (!System.IO.File.Exists(strFileName) && i < 100)
                    {
                        i += 1;
                        System.Threading.Thread.Sleep(30);
                    }
                    if (System.IO.File.Exists(strFileName))
                    {
                        using (Bitmap bitmap = (Bitmap)System.Drawing.Image.FromFile(strFileName))
                        {
                            if (IsResize )
                            {
                                var CropedBitmap = Tools.SystemUI.CropImage(bitmap, SnapshotCut);
                                CropedBitmap.Save(theFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                                CropedBitmap.Dispose();
                            }
                            else
                            {
                                bitmap.Save(theFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                                bitmap.Dispose();
                            }
                        }
                        // Delete bmp file
                        CancellationToken cancellationToken = new CancellationToken();
                        listFileDelete.Add(strFileName);
                        Task task = new Task(DeleteFile, cancellationToken);
                        task.Start();
                    }
                }
                else
                {
                    Tools.SystemUI.SaveLogFile($"SaveCurrentImage Failed. File path: {theFile}");
                }
            }
            catch (Exception ex)
            {
                Tools.SystemUI.SaveLogFile($"SaveCurrentImage Exeption. Ex: {ex.Message}");
                throw;
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
                            goto IL_3D9;
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
                            "\\img_tmp\\tmp_",
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
                            UInt32 uiConID1 = m_conState.m_uiConID;

                            //Only record the connected windows
                            if (uiConID1 == UInt32.MaxValue)
                            {
                                return null;
                            }

                            //Start to write the received data to the file 
                            if (NVSSDK.NetClient_CaptureBmpPic(uiConID1, fileName) > 0)
                            {
                                this.lastTimeSnapshot = DateTime.Now;
                                int index = 0;
                                while (!File.Exists(fileName) && index < 200)
                                {
                                    index += 1;
                                    Thread.Sleep(20);
                                }
                                if (File.Exists(fileName))
                                {
                                    this.lastFrame = new System.Drawing.Bitmap(System.Drawing.Image.FromFile(fileName));

                                }
                                else
                                {
                                    Thread.Sleep(300);
                                    if (File.Exists(fileName))
                                        this.lastFrame = new System.Drawing.Bitmap(System.Drawing.Image.FromFile(fileName));
                                }
                            }
                            else
                            {
                            }
                        }
                        bmp = ((this.lastFrame != null) ? new System.Drawing.Bitmap(this.lastFrame) : null);
                    }
                    if (_IsResize && this.LastFrame != null)
                    {
                        var CropedBitmap = Tools.SystemUI.CropImage(this.LastFrame, SnapshotCut);
                        if(CropedBitmap != null)
                        {
                           bmp = CropedBitmap;
                        }
                    }
                IL_3D9:;
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
