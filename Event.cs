using System;
using System.Collections.Generic;
using System.Text;

namespace Futech.Video
{
    // Motion Event
    public delegate void MotionEventHandler(object sender, MotionEventArgs e);

    /// <summary>
    /// Camera event arguments
    /// </summary>
    public class MotionEventArgs : EventArgs
    {
        private System.Drawing.Bitmap bmp;
        private float motionLevel;

        // Constructor
        public MotionEventArgs(System.Drawing.Bitmap bmp, float motionLevel)
        {
            this.bmp = bmp;
            this.motionLevel = motionLevel;
        }

        // Bitmap property
        public System.Drawing.Bitmap Bitmap
        {
            get { return bmp; }
        }

        // Motion Level
        public float MotionLevel
        {
            get { return motionLevel; }
        }
    }

    // NewFrame delegate
    public delegate void NewFrameHandler(object sender, NewFrameArgs e);

    /// <summary>
    /// Camera event arguments
    /// </summary>
    public class NewFrameArgs : EventArgs
    {
        private System.Drawing.Bitmap bmp;

        // Constructor
        public NewFrameArgs(System.Drawing.Bitmap bmp)
        {
            this.bmp = bmp;
        }

        // Bitmap property
        public System.Drawing.Bitmap Bitmap
        {
            get { return bmp; }
        }
    }

    public delegate void VideoInputEventHandler(object sender, VideoInputEventArgs e);
    /// <summary>
    /// Camera InputEventArgs
    /// </summary>
    public class VideoInputEventArgs : EventArgs
    {
        public VideoInputEventArgs()
        {
        }

        public VideoInputEventArgs(System.Drawing.Bitmap bmp)
        {
            this.bmp = bmp;
        }

        private System.Drawing.Bitmap bmp;
        public System.Drawing.Bitmap Bitmap
        {
            get { return bmp; }
            set { bmp = value; }
        }

        private string inputtype = "";
        public string InputType
        {
            get { return inputtype; }
            set { inputtype = value; }
        }

        private bool motiondected = false;
        public bool MotionDetected
        {
            get { return motiondected; }
            set { motiondected = value; }
        }

        private string input1 = "";
        public string Input1
        {
            get { return input1; }
            set { input1 = value; }
        }

        private string input2 = "";
        public string Input2
        {
            get { return input2; }
            set { input2 = value; }
        }

        private string input3 = "";
        public string Input3
        {
            get { return input3; }
            set { input3 = value; }
        }
    }
}