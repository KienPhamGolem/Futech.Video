using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;

namespace Futech.Video
{
    public partial class CameraAxisSDK : UserControl, ICameraWindow
    {
        // Status Property
        private const int AMC_STATUS_INITIALIZED = 1;
        private const int AMC_STATUS_FLAG_PLAYING = 2;
        private const int AMC_STATUS_FLAG_PAUSED = 4;
        private const int AMC_STATUS_FLAG_RECORDING = 8;
        private const int AMC_STATUS_FLAG_OPENING = 16;
        private const int AMC_STATUS_FLAG_RECONNECTING = 32;

        private string cameraname = "Camera Axis";
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

        // new frame event
        public event NewFrameHandler NewFrame;

        private int keepAliveInterval = 0;

        private int framesReceived = 0;
        // calculate frame rate
        private const int statLength = 5;
        private int statIndex = 0, statReady = 0;
        private int[] statCount = new int[statLength];
        // frame rate
        private float fps = 0;

        private Rectangle[] userWindows = null;

        private Color borderColor = Color.Black;

        private int width = 640;//640; // Chieu rong khung anh
        private int height = 480;//480; // Chieu cao khung anh

        private Rectangle[] motionZones = null;

        string VideoPath = "";

        Process ffmpeg = new Process();

        public CameraAxisSDK()
        {
            InitializeComponent();

            // Timer KeepAlive
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

            // Events
            this.axAxisMediaControl1.OnNewImage += new System.EventHandler(axAxisMediaControl1_OnNewImage);
            this.axAxisMediaControl1.OnStatusChange += new AxAXISMEDIACONTROLLib._IAxisMediaControlEvents_OnStatusChangeEventHandler(this.axAxisMediaControl1_OnStatusChange);
            this.axAxisMediaControl1.OnError += new AxAXISMEDIACONTROLLib._IAxisMediaControlEvents_OnErrorEventHandler(this.axAxisMediaControl1_OnError);

            ffmpeg.EnableRaisingEvents = true;
            ffmpeg.Exited += new EventHandler(ffmpeg_Exited);
            this.timerState.Tick += new EventHandler(timerState_Tick);
        }

       

        // Name property
        public string CameraName
        {
            set { cameraname = value; }
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
            get { return status; }
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
            //axAxisMediaControl1.Name = this.Name;
            axAxisMediaControl1.EnableReconnect = true;
            axAxisMediaControl1.MaintainAspectRatio = false;
            axAxisMediaControl1.StretchToFit = true;
            axAxisMediaControl1.EnableContextMenu = false;
            //axAxisMediaControl1.DisplayMessages = false;
            //axAxisMediaControl.ShowStatusBar = false;
            axAxisMediaControl1.ShowToolbar = false;
            axAxisMediaControl1.NetworkTimeout = 5000;

            //axAxisMediaControl1.Dock = DockStyle.Fill;

            // start new media
            axAxisMediaControl1.Stop();
            axAxisMediaControl1.MediaUsername = login;
            axAxisMediaControl1.MediaPassword = password;
            axAxisMediaControl1.MediaType = mediatype.ToUpper();
            switch (mediatype)
            {
                case "JPEG":
                    axAxisMediaControl1.MediaURL = "http://" + source + cgi;
                    break;
                case "MJPEG":
                    axAxisMediaControl1.MediaURL = "http://" + source + cgi;
                    break;
                case "MPEG4":
                    axAxisMediaControl1.MediaURL = "axrtsphttp://" + source + "/mpeg4/media.amp"; // only for axis camera
                    break;
                case "H264":
                    axAxisMediaControl1.MediaURL = "axrtsphttp://" + source + "/axis-media/media.amp?videocodec=h264"; // only for axis camera
                    break;
                case "PlayFile":
                    if (File.Exists(cgi))
                        axAxisMediaControl1.MediaFile = cgi;
                    break;
            }

            //axAxisMediaControl1.SetReconnectionStrategy(60000, 20000, 300000, 25000, 0, 30000, true);

            //if (source.Contains("640x360"))
            if (cgi.Contains("640x360"))//sua lai cho nay 
            {
                width = 640;
                height = 360;
            }
            else if (cgi.Contains("640x480"))
            {
                width = 640;
                height = 480;
            }
            else if (cgi.Contains("800x600"))
            {
                width = 800;
                height = 600;
            }
            else if(cgi.Contains("1280x720"))
            {
                width = 1280;
                height = 720;
            }
            else if(cgi.Contains("1280x960"))
            {
                width = 1280;
                height = 960;
            }
            else if (cgi.Contains("1920x1080"))
            {
                width = 1920;
                height = 1080;
            }
            else if (cgi.Contains("2048x1536"))
            {
                width = 2048;
                height = 1536;
            }

            axAxisMediaControl1.Play();
            lbTitle.Text = "Connecting to " + cameraname + "...";
            timer1.Start();

            if (enablerecording)
            {
                VideoPath = this.VideoFolder;
                //StartRecord();
            }
        }

        // Abort camera
        public void Stop()
        {
            try
            {
                timer1.Stop();
                axAxisMediaControl1.Stop();
                lbTitle.Text = "Stop to " + cameraname + "...";
                lastFrame = null;
                framesReceived = 0;
                StopRecord();
            }
            catch
            { }
        }

        // Luu hinh anh camera
        public void SaveCurrentImage(string theFile)
        {
            axAxisMediaControl1.SaveCurrentImage(0, theFile); // 0-jpeg, 1-bmp
        }

        // Start Record Video
        public void StartRecord()
        {
            WriteVideo();
        }

        // Stop Record Video
        public void StopRecord()
        {
            //this.enablerecording = false;
        }

        private void axAxisMediaControl1_OnNewImage(object sender, EventArgs e)
        {
            if (NewFrame != null)
                NewFrame(this, new NewFrameArgs(lastFrame));
            framesReceived++;
        }

        // get bitmap image from bitmap data
        public Bitmap GetCurrentImage()
        {
            try
            {
                if (axAxisMediaControl1.MediaFile == "")
                {
                    object buf = null;
                    int len = 0;
                    axAxisMediaControl1.GetCurrentImage(1, out buf, out len);

                    if (buf != null && len > 0)
                    {
                        Bitmap frame = GetCurrentFrame(Marshal.UnsafeAddrOfPinnedArrayElement((Array)buf, 0));
                        return new Bitmap(frame);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            return null;
        }

        private Bitmap GetCurrentFrame(IntPtr DIB)
        {
            lock (this)
            {
                if (DIB == IntPtr.Zero)
                    throw new ApplicationException("Failed getting frame");

                Win32.BITMAPINFOHEADER bitmapInfoHeader;

                // copy BITMAPINFOHEADER from unmanaged memory
                bitmapInfoHeader = (Win32.BITMAPINFOHEADER)Marshal.PtrToStructure(DIB, typeof(Win32.BITMAPINFOHEADER));

                int width = bitmapInfoHeader.width;
                int height = bitmapInfoHeader.height;
                if (height < 0)
                    height = -height;

                // create new bitmap
                Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                // lock bitmap data
                BitmapData imageData = image.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                // copy image data
                int srcStride = imageData.Stride;
                int dstStride = imageData.Stride;

                // check image direction
                if (bitmapInfoHeader.height > 0)
                {
                    // it's a bottom-top image
                    int dst = imageData.Scan0.ToInt32() + dstStride * (height - 1);
                    int src = DIB.ToInt32() + Marshal.SizeOf(typeof(Win32.BITMAPINFOHEADER));

                    for (int y = 0; y < height; y++)
                    {
                        Win32.memcpy(dst, src, srcStride);
                        dst -= dstStride;
                        src += srcStride;
                    }
                }
                else
                {
                    // it`s a top bottom image
                    int dst = imageData.Scan0.ToInt32();
                    int src = DIB.ToInt32() + Marshal.SizeOf(typeof(Win32.BITMAPINFOHEADER));

                    // copy the whole image
                    Win32.memcpy(dst, src, srcStride * height);
                }

                // unlock bitmap data
                image.UnlockBits(imageData);

                if (image != null&& (image.Width != this.width || image.Height != height))
                {
                    this.width = image.Width;
                    this.height = image.Height;

                    if (userWindows != null)
                    {
                        short i = 1;
                        foreach (Rectangle rec in userWindows)
                        {
                            i++;
                            short theID = i;
                            string theName = "";
                            int theXPos = rec.X * 9999 / this.width;
                            int theYPos = rec.Y * 9999 / this.height;
                            int theWidth = rec.Width * 9999 / this.width;
                            int theHeight = rec.Height * 9999 / this.height;
                            Color color = Color.Green;

                            int theColor = color.B << 16 | color.G << 8 | color.R;
                            int theOpacity = 100;
                            int theFlags = 8;
                            axAxisMediaControl1.SetUserWindow(theID, theName, theXPos, theYPos, theWidth, theHeight, theColor, theOpacity, theFlags);
                        }                     

                    }

                    if (motionZones != null)
                    {
                        short i = 100;
                        foreach (Rectangle rec in motionZones)
                        {
                            i++;
                            short theID = i;
                            string theName = "";
                            int theXPos = rec.X * 9999 / this.width;
                            int theYPos = rec.Y * 9999 / this.height;
                            int theWidth = rec.Width * 9999 / this.width;
                            int theHeight = rec.Height * 9999 / this.height;

                            Color color = Color.Magenta;
                            int theColor = color.B << 16 | color.G << 8 | color.R;
                            int theOpacity = 100;
                            int theFlags = 8;
                            axAxisMediaControl1.SetUserWindow(theID, theName, theXPos, theYPos, theWidth, theHeight, theColor, theOpacity, theFlags);
                        }
                    }

                }

                return image;
            }
        }

        private void axAxisMediaControl1_OnStatusChange(object sender, AxAXISMEDIACONTROLLib._IAxisMediaControlEvents_OnStatusChangeEvent e)
        {
            if ((e.theNewStatus & AMC_STATUS_FLAG_PLAYING) == AMC_STATUS_FLAG_PLAYING)
            {
                lbTitle.Text = "Connecting to " + cameraname + "...";
                status = 1;
            }
            else
            {
                lbTitle.Text = "Stop to " + cameraname + "...";
                status = 0;
                lastFrame = null;
                framesReceived = 0;

                //if (istimerstateRunning == false)
                //    timerState.Start();
            }
        }

        private void axAxisMediaControl1_OnError(object sender, AxAXISMEDIACONTROLLib._IAxisMediaControlEvents_OnErrorEvent e)
        {
            //MessageBox.Show(e.theErrorCode + ":" + e.theErrorInfo);
        }

        // Send Keep-Alive command to camera
        private void KeepAlive()
        {
            ExecuteCommand exe = new ExecuteCommand("http://" + source + "/cgi-bin/keep_alive?mode=mjpeg&protocol=http&UID=263&page=20040830203157", login, password, "Futech.Video");
        }


    
    
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (cgi.Contains("/cgi-bin/mjpeg?"))
                {
                    keepAliveInterval++;
                    if (keepAliveInterval >= 15)
                    {
                        KeepAlive();
                        keepAliveInterval = 0;
                    }
                }

                if (framesReceived > 0)
                {
                    // get number of frames for the last second
                    statCount[statIndex] = framesReceived;

                    // increment indexes
                    if (++statIndex >= statLength)
                        statIndex = 0;
                    if (statReady < statLength)
                        statReady++;

                    fps = 0;

                    // calculate average value
                    for (int i = 0; i < statReady; i++)
                    {
                        fps += statCount[i];
                    }
                    fps /= statReady;

                    if (fps > 30)
                        fps = 30;

                    statCount[statIndex] = 0;
                    statCount[statIndex] = 0;
                    framesReceived = 0;

                    // update label
                    lbTitle.Text = cameraname + " - " + fps.ToString("F2") + " fps";

                    

                }

                if (isVideoLoss == true)
                {
                    if (istimerstateRunning == false)
                        timerState.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// UserWindow
        /// </summary>
        public Rectangle[] UserWindows
        {
            get { return userWindows; }
            set
            {
                userWindows = value;
                if (userWindows != null)
                {
                    short i = 1;
                    foreach (Rectangle rec in userWindows)
                    {
                        i++;
                        short theID = i;
                        string theName = "";
                        int theXPos = rec.X * 9999 / width;
                        int theYPos = rec.Y * 9999 / height;
                        int theWidth = rec.Width * 9999 / width;
                        int theHeight = rec.Height * 9999 / height;

                        Color color = Color.Green;
                        int theColor = color.B << 16 | color.G << 8 | color.R;
                        int theOpacity = 100;
                        int theFlags = 8;
                        axAxisMediaControl1.SetUserWindow(theID, theName, theXPos, theYPos, theWidth, theHeight, theColor, theOpacity, theFlags);
                    }
                }
            }
        }

        /// <summary>
        /// BorderColor
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get { return borderColor; }
            set 
            { 
                borderColor = value;
                if (borderColor == Color.Transparent)
                    borderColor = Color.Black;
                Graphics g = panel2.CreateGraphics();
                g.DrawRectangle(new Pen(borderColor), 0, 0, panel2.Width-1, panel2.Height-1);
            }
        }

        /// <summary>
        /// MotionZones
        /// </summary>
        public Rectangle[] MotionZones
        {
            get { return motionZones; }
            set 
            { 
                motionZones = value;
                if (motionZones != null)
                {
                    short i = 100;
                    foreach (Rectangle rec in motionZones)
                    {
                        i++;
                        short theID = i;
                        string theName = "";
                        int theXPos = rec.X * 9999 / width;
                        int theYPos = rec.Y * 9999 / height;
                        int theWidth = rec.Width * 9999 / width;
                        int theHeight = rec.Height * 9999 / height ;

                        Color color = Color.Magenta;
                        int theColor = color.B << 16 | color.G << 8 | color.R;
                        int theOpacity = 100;
                        int theFlags = 8;
                        axAxisMediaControl1.SetUserWindow(theID, theName, theXPos, theYPos, theWidth, theHeight, theColor, theOpacity, theFlags);
                    }
                }
            }
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
            axAxisMediaControl1.Width = panel2.Width - 2;
            axAxisMediaControl1.Height = panel2.Height - 2;
            axAxisMediaControl1.Location = new Point(1, 1);
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


        bool isVideoLoss = false;
        private void WriteVideo()
        {
            try
            {
                if (CheckConnect(source) == false)
                {
                    isVideoLoss = true;
                    return;
                }

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

            }
        }

        void ffmpeg_Exited(object sender, EventArgs e)
        {

            if (this.enablerecording)
            {
                string path = VideoPath + "\\CAM" + source + "\\" + DateTime.Now.AddDays(-7).ToString("dd-MM-yyyy");

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

        //private void timerState_Tick(object sender, EventArgs e)
        //{
        //     if (CheckConnect(source) == true)
        //     {
        //         timerState.Stop();
        //         istimerstateRunning = false;
        //         StartRecord();
        //     }
        //     else
        //         istimerstateRunning = true;
        //}
        //private void timerState_Tick(object sender, EventArgs e)
        //{
        //    if (CheckConnect(source) == true)
        //    {
        //        timerState.Stop();
        //        istimerstateRunning = false;
        //        StartRecord();
        //    }
        //    else
        //        istimerstateRunning = true;
        //}

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
