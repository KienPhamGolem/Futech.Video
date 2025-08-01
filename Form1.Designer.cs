
namespace Futech.Video
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.axIPSViewer1 = new AxIPSLIB3.AxIPSViewer();
            ((System.ComponentModel.ISupportInitialize)(this.axIPSViewer1)).BeginInit();
            this.SuspendLayout();
            // 
            // axIPSViewer1
            // 
            this.axIPSViewer1.Enabled = true;
            this.axIPSViewer1.Location = new System.Drawing.Point(12, 12);
            this.axIPSViewer1.Name = "axIPSViewer1";
            this.axIPSViewer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axIPSViewer1.OcxState")));
            this.axIPSViewer1.Size = new System.Drawing.Size(129, 117);
            this.axIPSViewer1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.axIPSViewer1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.axIPSViewer1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxIPSLIB3.AxIPSViewer axIPSViewer1;
    }
}