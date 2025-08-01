using System;
using System.Collections.Generic;
using System.Text;

namespace Futech.Video
{
    public interface IVideoInput
    {
        event VideoInputEventHandler VideoInput;
        string HttpUrl { get; set; }
        string Cgi { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        bool Start();
        void Stop();
        void PollingStart();
        void PollingStop();
    }
}
