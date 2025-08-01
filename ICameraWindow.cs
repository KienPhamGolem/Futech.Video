using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Futech.Video
{
    public interface ICameraWindow
    {
        // Events
        /// <summary>
        /// New frame event - notify client about the new frame
        /// </summary>
        event NewFrameHandler NewFrame;

        // Properties
        /// <summary>
        /// CameraName property
        /// </summary>
        string CameraName { set;}

        /// <summary>
        /// Video source property
        /// </summary>
        string VideoSource { get; set;}

        /// <summary>
        /// Http Port property
        /// </summary>
        int HttpPort { get; set; }

        /// <summary>
        /// Rtsp Port property
        /// </summary>
        int RtspPort { get; set; }

        /// <summary>
        /// Login property
        /// </summary>
        string Login { get; set;}

        /// <summary>
        /// Password property
        /// </summary>
        string Password { get; set;}

        /// <summary>
        /// MediaType property
        /// </summary>
        string MediaType { get; set;}

        /// <summary>
        /// Resolution property
        /// </summary>
        string Resolution { get; set; }

        /// <summary>
        /// Cgi property
        /// </summary>
        string Cgi { get; set;}

        /// <summary>
        /// Channel property
        /// </summary>
        int Channel { get; set;}

        /// <summary>
        /// Status property
        /// </summary>
        int Status { get;}

        /// <summary>
        /// LastFrame property
        /// </summary>
        Bitmap LastFrame { get;}

        /// <summary>
        /// DisplayCameraTitle property
        /// </summary>
        bool DisplayCameraTitle { set;}

        /// <summary>
        /// Fps property
        /// </summary>
        float Fps { get;}

        /// <summary>
        /// LastMessage property
        /// </summary>
        string LastMessage { get;}

        // Methods
        /// <summary>
        /// Start receiving video frames
        /// </summary>
        void Start();

        /// <summary>
        /// Stop work
        /// </summary>
        void Stop();

        /// <summary>
        /// SaveCurrentImage method
        /// </summary>
        void SaveCurrentImage(string theFile);

        /// <summary>
        /// GetCurrentImage method
        /// </summary>
        Bitmap GetCurrentImage();

        /// <summary>
        /// Start Record
        /// </summary>
        void StartRecord();

        /// <summary>
        /// Stop Record
        /// </summary>
        void StopRecord();

        /// <summary>
        /// UserWindows
        /// </summary>
        Rectangle[] UserWindows { get; set;}

        /// <summary>
        /// BorderColor
        /// </summary>
        Color BorderColor { get; set;}

        /// <summary>
        /// MotionZones
        /// </summary>
        Rectangle[] MotionZones { get; set;}

        uint Get_BRIGHTNESS();
        void Set_BRIGHTNESS(uint pbrightness);

        //contrast
        uint Get_CONTRAST();

        void Set_CONTRAST(uint pcontrast);


        //hue
        uint Get_HUE();

        void Set_HUE(uint phue);


        //saturation
        uint Get_SATURATION();

        void Set_SATURATION(uint psaturation);


        //sharpness
        uint Get_SHARPNESS();

        void Set_SHARPNESS(uint psharpness);

        string CameraType { get; set; }

        bool EnableRecording { get; set; }
        string VideoFolder { get; set; }
    }
}
