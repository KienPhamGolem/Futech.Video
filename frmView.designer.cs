namespace Futech.Video
{
    partial class frmView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmView));
            this.cameraWindow1 = new Futech.Video.CameraWindow();
            this.btConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cameraWindow1
            // 
            this.cameraWindow1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.cameraWindow1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cameraWindow1.BackgroundImage")));
            this.cameraWindow1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cameraWindow1.EnableWebServer = false;
            this.cameraWindow1.Location = new System.Drawing.Point(12, 12);
            this.cameraWindow1.MotionZones = null;
            this.cameraWindow1.Name = "cameraWindow1";
            this.cameraWindow1.Size = new System.Drawing.Size(640, 480);
            this.cameraWindow1.TabIndex = 0;
            this.cameraWindow1.UserWindows = null;
            // 
            // btConnect
            // 
            this.btConnect.Location = new System.Drawing.Point(12, 498);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(75, 27);
            this.btConnect.TabIndex = 1;
            this.btConnect.Text = "Connect";
            this.btConnect.UseVisualStyleBackColor = true;
            this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
            // 
            // frmView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 575);
            this.Controls.Add(this.btConnect);
            this.Controls.Add(this.cameraWindow1);
            this.Name = "frmView";
            this.Text = "frmView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmView_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private CameraWindow cameraWindow1;
        private System.Windows.Forms.Button btConnect;
    }
}