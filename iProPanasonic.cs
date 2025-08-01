using System;
using System.Collections.Generic;
using System.Text;


using System.Windows.Forms;
using System.Threading;


using System.IO;
using System.Net;
using System.Xml;

namespace Futech.Video
{
    internal class iProPanasonic:IVideoInput
    {
        public iProPanasonic()
        {
            Cgi = "/cgi-bin/get_io?mode=monitor";
        }
        public event VideoInputEventHandler VideoInput;
        private Thread thread = null;
        private ManualResetEvent stopEvent = null;

        StreamReader readStream;

        public string HttpUrl { get; set; }
        public string Cgi { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public bool Start()
        {
            
            return true;
        }
        public void Stop()
        {
            PollingStop();
        }
       

        // Start
        public void PollingStart()
        {
            if (thread == null)
            {

                //this.InputCommand = command;
                // create events
                stopEvent = new ManualResetEvent(false);

                // start thread
                thread = new Thread(new ThreadStart(WorkerThread));
                thread.Start();
            }
        }

        // is Running
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
                thread.Join(100);

                Free();
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

        // Stop
        public void PollingStop()
        {
            if (this.Running)
            {
                SignalToStop();
                while (thread.IsAlive)
                {
                    if (WaitHandle.WaitAll(
                        (new ManualResetEvent[] { stopEvent }),
                        100,
                        true))
                    {
                        WaitForStop();
                        break;
                    }

                    Application.DoEvents();
                }
            }
        }

        int count = 0;
        bool laststate = false;
        // Worker thread
        public void WorkerThread()
        {
            try
            {
               
                CreateRequest(HttpUrl, Cgi, Username, Password);
              
                while (!stopEvent.WaitOne(0, true))
                {
                    try
                    {
                        if (readStream == null)
                        {
                            count++;
                            continue;
                        }
                        string temp = "";
                        try
                        {
                            temp = readStream.ReadLine();
                        }
                        catch
                        {
                            count++;
                            if (count > 2)
                            {
                                CreateRequest(HttpUrl, Cgi, Username, Password);
                                count = 0;
                            }

                        }
                       
                        if(temp.Contains("terminal1"))
                        {
                            count = 0;
                            VideoInputEventArgs ie = new VideoInputEventArgs();
                            ie.Input1 = temp;
                            temp = readStream.ReadLine();
                            if (temp.Contains("terminal2"))
                                ie.Input2 = temp;
                            temp = readStream.ReadLine();
                            if (temp.Contains("terminal3"))
                                ie.Input3 = temp;
                            temp = readStream.ReadLine();
                            //if (temp.Contains("motion"))
                                ie.InputType = temp;
                          
                            if (VideoInput != null)
                            {
                                if (ie.InputType.Contains("True"))
                                    ie.MotionDetected = true;
                                else if (ie.InputType.Contains("False"))
                                    ie.MotionDetected = false;
                                if (ie.Input1.Contains("High"))
                                    ie.Input1 = "High";
                                else if (ie.Input1.Contains("Low"))
                                    ie.Input1 = "Low";

                                if (ie.Input2.Contains("High"))
                                    ie.Input2 = "High";
                                else if (ie.Input2.Contains("Low"))
                                    ie.Input2 = "Low";

                                if (ie.Input3.Contains("High"))
                                    ie.Input3 = "High";
                                else if (ie.Input3.Contains("Low"))
                                    ie.Input3 = "Low";

                                laststate = ie.MotionDetected;

                                VideoInput(this, ie);
                            }
                        }
                        else //if (temp == "")
                        {
                            if (VideoInput != null)
                            {
                                VideoInputEventArgs ie = new VideoInputEventArgs();
                                ie.MotionDetected = laststate;
                                if (ie.MotionDetected)
                                {
                                    VideoInput(this, ie);
                                    Thread.Sleep(50);
                                }
                            }
                            
                        }

                        
                     

                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                       
                    }
                }
            }
            catch
            { }
        }


        void CreateRequest(string source, string cgi, string username, string password)
        {
            try
            {
                string url = "";//"http://192.168.1.16/cgi-bin/get_io?mode=monitor";
                url = "http://" + source + cgi;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                // Set credentials to use for this request.
                request.Credentials = new NetworkCredential(username, password);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Get the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                readStream = new StreamReader(receiveStream, Encoding.UTF8);
                //Console.WriteLine("Response stream received.");
            }
            catch
            { 
            }

        }


    }
}
