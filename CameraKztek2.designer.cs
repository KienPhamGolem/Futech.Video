﻿namespace Futech.Video
{
    partial class CameraKztek2
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rtspPlayerControl1 = new Gts.RtspPlayer.RtspPlayerControl();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // rtspPlayerControl1
            // 
            this.rtspPlayerControl1.BackColor = System.Drawing.SystemColors.Desktop;
            this.rtspPlayerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtspPlayerControl1.Location = new System.Drawing.Point(0, 0);
            this.rtspPlayerControl1.Login = "";
            this.rtspPlayerControl1.Name = "rtspPlayerControl1";
            this.rtspPlayerControl1.Password = "";
            this.rtspPlayerControl1.Resolution_Height = 1080;
            this.rtspPlayerControl1.Resolution_Width = 1920;
            this.rtspPlayerControl1.Size = new System.Drawing.Size(246, 224);
            this.rtspPlayerControl1.TabIndex = 5;
            this.rtspPlayerControl1.URI = "";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(246, 224);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // CameraKztek2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtspPlayerControl1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "CameraKztek2";
            this.Size = new System.Drawing.Size(246, 224);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private Gts.RtspPlayer.RtspPlayerControl rtspPlayerControl1;
    }
}
