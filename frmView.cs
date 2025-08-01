using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Futech.Video
{
    public partial class frmView : Form
    {
        public frmView()
        {
            InitializeComponent();
        }

        private string sdk = "AForgeSDK";
        public string SDK
        {
            get { return sdk; }
            set { sdk = value; }
        }

        private string source = "";
        public string Source
        {
            get { return source; }
            set { source = value; }
        }
        private int httpPort = 80;
        public int HttpPort
        {
            get { return httpPort; }
            set { httpPort = value; }
        }

        // Rtsp Port
        private int rtspPort = 554;
        public int RtspPort
        {
            get { return rtspPort; }
            set { rtspPort = value; }
        }

        private string login = "";
        public string Login
        {
            get { return login; }
            set { login = value; }
        }

        private string password = "";
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        string mediatype = "";
        public string MediaType
        {
            get { return mediatype; }
            set { mediatype = value; }
        }

        private string cgi = "";
        public string Cgi
        {
            get { return cgi; }
            set { cgi = value; }
        }

        private int channel = 0;
        public int Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        private void btConnect_Click(object sender, EventArgs e)
        {
            try
            {
                cameraWindow1.CreateCameraControl(sdk);
                cameraWindow1.Start("CAM", source, httpPort, rtspPort, login, password, mediatype, channel, cgi, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmView_FormClosing(object sender, FormClosingEventArgs e)
        {
            cameraWindow1.Stop();
            this.DialogResult = DialogResult.OK;
        }

    }
}