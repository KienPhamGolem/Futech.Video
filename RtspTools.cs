using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futech.Video
{
    public static class RtspTools
    {
        public static string Get_Rtsp_Url(string cameratype, string source, int rtspPort, string login, string password, int channel)
        {
            string url = "";
            try
            {
                switch (cameratype)
                {
                    case "Vantech":
                        url = "rtsp://" + source + "//user=" + login + "_password=" + password + "_channel=1_stream=0.sdp";
                        break;
                    case "Secus":
                    case "Hanse":
                    case "Surveon":
                        url = $"rtsp://{source}/stream{channel}";
                        break;
                    case "Shany":
                        url = "rtsp://" + source + ":8557" + "/PSIA/Streaming/channels/2?videoCodecType=H.264";
                        break;
                    case "CNB":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/media/video" + channel;
                        break;
                    case "Dahua":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/cam/realmonitor?channel=1&subtype=" + channel;
                        break;
                    case "HIK":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/PSIA/streaming/channels/" + (channel >= 1 ? channel : 1) + "01";
                        break;
                    case "Enster":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/11";
                        break;
                    case "Afidus":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/media/media.amp?streamprofile=Profile1";
                        break;
                    case "ITX":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/live/main";
                        break;
                    case "Samsung":
                        url = "rtsp://" + source + "/profile1/media.smp";
                        break;
                    case "Tiandy":
                        url = "rtsp://" + login + ":" + password + "@" + source + "/" + channel + "/1";
                        break;
                    case "Camtron":
                        url = $"rtsp://{login}:{password}@{source}/H264?ch=1&subtype=1";
                        break;
                    case "HIVIZ":
                        url = "rtsp://" + login + ":" + password + "@" + source + ":" + rtspPort + "/profile1";
                        break;
                    case "DMAX":
                        url = "rtsp://" + login + ":" + password + "@" + source + ":" + rtspPort + "/1/stream1/Profile1";
                        break;
                    case "Bosch":
                        url = $"rtsp://{login}:{password}@{source}:{rtspPort}/rtsp_tunnel?h26x=4&line=1&inst=1";
                        break;
                    default:
                        url = $"rtsp://{login}:{password}@{source}:{rtspPort}/rtsp_tunnel?h26x=4&line=1&inst=1";
                        break;
                }
            }
            catch (Exception ex)
            {
                url = "";
            }
            return url;
        }

        public static string GetRTSP(string cameratype, string source, int rtspPort, string login, string password, int channel) 
        {
            string url = String.Empty;
            switch (cameratype)
            {
                case "Vantech":
                    url = "rtsp://" + source + "//user=" + login + "_password=" + password + "_channel=1_stream=0.sdp";
                    break;
                case "Secus":
                case "Hanse":
                case "Surveon":
                    url = $"rtsp://{source}/stream{channel}";
                    break;
                case "Shany":
                    url = "rtsp://" + source + ":8557" + "/PSIA/Streaming/channels/2?videoCodecType=H.264";
                    break;
                case "CNB":
                    url = "rtsp://" + source + "/media/video" + channel;
                    break;
                case "Dahua":
                    url = "rtsp://" + source + "/cam/realmonitor?channel=1&subtype=" + channel;
                    break;
                case "HIK":
                    url = "rtsp://" + source + "/streaming/channels/" + channel;
                    break;
                case "Enster":
                    url = $"rtsp://{source}/{channel}1";
                    break;
                case "Afidus":
                    url = "rtsp://" + source + "/media/media.amp?streamprofile=Profile1";
                    break;
                case "ITX":
                    url = "rtsp://" + source + "/live/main";
                    break;
                case "Samsung":
                    url = "rtsp://" + source + "/onvif/profile2/media.smp";
                    break;
                case "Tiandy":
                    url = "rtsp://" + source + "/" + channel + "/1";
                    break;
                case "Camtron":
                    url = $"rtsp://{source}/H264?ch=1&subtype={channel}";
                    break;
                case "HIVIZ":
                    url = "rtsp://" + source + ":" + rtspPort + "/profile1";
                    break;
                case "DMAX":
                    url = "rtsp://" + source + ":" + rtspPort + "/1/stream1/Profile1";
                    break;
                case "Bosch":
                    url = $"rtsp://{login}:{password}@{source}:{rtspPort}/rtsp_tunnel?h26x=4&line=1&inst=1";
                    break;
                default:
                    break;
            }
            return url;
        }

    }
}
