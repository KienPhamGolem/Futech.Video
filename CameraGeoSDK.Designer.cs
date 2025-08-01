namespace Futech.Video
{
    partial class CameraGeoSDK
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CameraGeoSDK));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lbTitle = new System.Windows.Forms.Label();
            this.axLiveX1 = new AxLIVEXLib.AxLiveX();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axLiveX1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lbTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(150, 25);
            this.panel1.TabIndex = 87;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 1);
            this.label1.TabIndex = 86;
            this.label1.Text = "label1";
            // 
            // lbTitle
            // 
            this.lbTitle.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.lbTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lbTitle.ForeColor = System.Drawing.Color.Red;
            this.lbTitle.Location = new System.Drawing.Point(0, 0);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(150, 25);
            this.lbTitle.TabIndex = 84;
            this.lbTitle.Text = "Camera";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // axLiveX1
            // 
            this.axLiveX1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axLiveX1.Enabled = true;
            this.axLiveX1.Location = new System.Drawing.Point(0, 25);
            this.axLiveX1.Name = "axLiveX1";
            this.axLiveX1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axLiveX1.OcxState")));
            this.axLiveX1.Size = new System.Drawing.Size(150, 150);
            this.axLiveX1.TabIndex = 89;
            // 
            // CameraGeoSDK
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.axLiveX1);
            this.Controls.Add(this.panel1);
            this.Name = "CameraGeoSDK";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axLiveX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbTitle;
        private AxLIVEXLib.AxLiveX axLiveX1;

    }
}
