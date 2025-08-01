namespace Futech.Video
{
    partial class CameraScSDK
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lbTitle = new System.Windows.Forms.Label();
            this.myChanelControl = new System.Windows.Forms.Panel();
            this.timerVideoLength = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lbTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(194, 25);
            this.panel1.TabIndex = 88;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 1);
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
            this.lbTitle.Size = new System.Drawing.Size(194, 25);
            this.lbTitle.TabIndex = 84;
            this.lbTitle.Text = "Camera";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // myChanelControl
            // 
            this.myChanelControl.BackColor = System.Drawing.SystemColors.Highlight;
            this.myChanelControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.myChanelControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myChanelControl.Location = new System.Drawing.Point(0, 25);
            this.myChanelControl.Name = "myChanelControl";
            this.myChanelControl.Size = new System.Drawing.Size(194, 161);
            this.myChanelControl.TabIndex = 89;
            // 
            // timerVideoLength
            // 
            this.timerVideoLength.Interval = 1000;
            this.timerVideoLength.Tick += new System.EventHandler(this.timerVideoLength_Tick);
            // 
            // CameraScSDK
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.myChanelControl);
            this.Controls.Add(this.panel1);
            this.Name = "CameraScSDK";
            this.Size = new System.Drawing.Size(194, 186);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Panel myChanelControl;
        private System.Windows.Forms.Timer timerVideoLength;

    }
}
