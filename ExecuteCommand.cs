using System;
using System.IO;
using System.Threading;
using System.Net;

namespace Futech.Video
{
	/// <summary>
	/// PTZSource - PTZ downloader
	/// </summary>
	public class ExecuteCommand
	{
		private string	source;
		private string	login = null;
		private string	password = null;
		private bool	useSeparateConnectionGroup = false;
		private bool	preventCaching = true;
        // timeout value for web request 
        private int requestTimeout = 10000;

        private string userAgent = "Futech.Video";

		private Thread	thread = null;
		private ManualResetEvent stopEvent = null;

        // VideoSource property
        public string Source
        {
            get { return source; }
            set { source = value; }
        }
		
		// Login property
		public string Login
		{
			get { return login; }
			set { login = value; }
		}
		// Password property
		public string Password
		{
			get { return password; }
			set { password = value; }
		}

        // SeparateConnectioGroup property
        // indicates to open WebRequest in separate connection group
        public bool SeparateConnectionGroup
        {
            get { return useSeparateConnectionGroup; }
            set { useSeparateConnectionGroup = value; }
        }
        // PreventCaching property
        // If the property is set to true, we are trying to prevent caching
        // appneding fake parameter to URL. It's needed is client is behind
        // proxy server.
        public bool PreventCaching
        {
            get { return preventCaching; }
            set { preventCaching = value; }
        }

        // RequestTimeout property
        public int RequestTimeout
        {
            get { return requestTimeout; }
            set { requestTimeout = value; }
        }

        /// <summary>
        /// User agent to specify in HTTP request header.
        /// </summary>
        /// 
        /// <remarks><para>Some IP cameras check what is the requesting user agent and depending
        /// on it they provide video in different formats or do not provide it at all. The property
        /// sets the value of user agent string, which is sent to camera in request header.
        /// </para>
        /// 
        /// <para>Default value is set to "Mozilla/5.0". If the value is set to <see langword="null"/>,
        /// the user agent string is not sent in request header.</para>
        /// </remarks>
        /// 
        public string HttpUserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
        }

		// Get state of the video source thread
		public bool Running
		{
			get
			{
				if (thread != null)
				{
					if (thread.Join(0) == false)
						return true;

					// the thread is not running, so free resources
					Free();
				}
				return false;
			}
		}

        // Constructor
        public ExecuteCommand()
        {

        }

        // Constructor
        public ExecuteCommand(string source)
        {
            this.source = source;
        }

		// Constructor
		public ExecuteCommand(string source, string login, string password, string useragent)
		{
            Source = source;
            Login = login;
            Password = password;
            userAgent = useragent;
            Start();
		}

		// Start work
		public void Start()
		{
			if (thread == null)
			{
				// create events
				stopEvent	= new ManualResetEvent(false);
				
				// create and start new thread
				thread = new Thread(new ThreadStart(WorkerThread));
                thread.IsBackground = true;
				thread.Name = source;
				thread.Start();
			}
		}

		// Signal thread to stop work
		public void SignalToStop()
		{
			// stop thread
			if (thread != null)
			{
				// signal to stop
				stopEvent.Set();
			}
		}

		// Wait for thread stop
		public void WaitForStop()
		{
			if (thread != null)
			{
				// wait for thread stop
				thread.Join();

				Free();
			}
		}

		// Abort thread
		public void Stop()
		{
			if (this.Running)
			{
				thread.Abort();
				WaitForStop();
			}
		}

		// Free resources
		private void Free()
		{
			thread = null;

			// release events
			stopEvent.Close();
			stopEvent = null;
		}

		// Thread entry point
        public void WorkerThread()
        {
            HttpWebRequest req = null;
            WebResponse resp = null;
            Random rnd = new Random((int)DateTime.Now.Ticks);

            try
            {
                // create request
                if (!preventCaching)
                {
                    req = (HttpWebRequest)WebRequest.Create(source);
                }
                else
                {
                    req = (HttpWebRequest)WebRequest.Create(source + ((source.IndexOf('?') == -1) ? '?' : '&') + "fake=" + rnd.Next().ToString());
                }

                // set user agent
                if (userAgent != null)
                {
                    req.UserAgent = userAgent;
                }
                
                // set timeout value for the request 
                req.Timeout = requestTimeout;
                
                // set login and password
                if ((login != null) && (password != null) && (login != ""))
                    req.Credentials = new NetworkCredential(login, password);
                // set connection group name
                if (useSeparateConnectionGroup)
                    req.ConnectionGroupName = GetHashCode().ToString();
                // get response
                resp = req.GetResponse();
            }
            catch (WebException)
            {
                // wait for a while before the next try
                Thread.Sleep(250);
            }
            catch (Exception)
            {
                
            }
            finally
            {
                // abort request
                if (req != null)
                {
                    req.Abort();
                    req = null;
                }
                // close response
                if (resp != null)
                {
                    resp.Close();
                    resp = null;
                }
            }
        }
	}
}
