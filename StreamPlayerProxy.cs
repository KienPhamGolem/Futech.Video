﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
//using WebEye.Controls.WinForms.StreamPlayerControl.Properties;
//using WebEye;
namespace WebEye
{
    /// <summary>
    /// Forwards calls to the stream player library.
    /// </summary>
    internal sealed class StreamPlayerProxy : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the StreamPlayerProxy class.
        /// </summary>
        /// <exception cref="Win32Exception">Failed to load the utilities dll.</exception>
        internal StreamPlayerProxy()
        {
            LoadDll();
            BindToDll(_hDll);
        }

        /// <summary>
        /// Initializes the player.
        /// </summary>
        /// <param name="playerParams">The StreamPlayerParams object that contains the information that is used to initialize the player.</param>
        internal void Initialize(Futech.Video.CameraFFMPEG.StreamPlayerParams playerParams)
        {
            if (_initialize(playerParams) != 0)
            {
                throw new StreamPlayerException("Failed to initialize the player.");
            }
        }

        /// <summary>
        /// Asynchronously plays a stream.
        /// </summary>
        /// <param name="url">The url of a stream to play.</param>
        /// <param name="connectionTimeout">The connection timeout.</param>
        /// <exception cref="StreamPlayerException">Failed to play the stream.</exception>
        /// <param name="transport">RTSP transport protocol.</param>
        /// <param name="flags">RTSP flags.</param>
        internal void StartPlay(String url, TimeSpan connectionTimeout,
            RtspTransport transport, RtspFlags flags)
        {
            if (_startPlayDelegate(url,
                Convert.ToInt32(connectionTimeout.TotalMilliseconds),
                Convert.ToInt32(transport),
                Convert.ToInt32(flags)) != 0)
            {
                throw new StreamPlayerException("Failed to play the stream.");
            }
        }

        /// <summary>
        /// Stops a stream.
        /// </summary>
        internal void Stop()
        {
            if (_stop() != 0)
            {
                throw new StreamPlayerException("Failed to stop the stream.");
            }
        }

        /// <summary>
        /// Uninitializes the player.
        /// </summary>
        internal void Uninitialize()
        {
            if (_uninitialize() != 0)
            {
                throw new StreamPlayerException("Failed to uninitialize the player.");
            }
        }

        /// <summary>
        /// The BITMAPINFOHEADER structure contains information about the dimensions and color format of a device independent bitmap.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFOHEADER
        {
            public UInt32 biSize;
            public Int32 biWidth;
            public Int32 biHeight;
            public UInt16 biPlanes;
            public UInt16 biBitCount;
            public UInt32 biCompression;
            public UInt32 biSizeImage;
            public Int32 biXPelsPerMeter;
            public Int32 biYPelsPerMeter;
            public UInt32 biClrUsed;
            public UInt32 biClrImportant;
        }

        /// <summary>
        /// Retrieves the current frame being displayed by the player.
        /// </summary>
        /// <returns>The current frame being displayed by the player.</returns>
        internal Bitmap GetCurrentFrame()
        {
            IntPtr dibPtr;
            if (_getCurrentFrame(out dibPtr) != 0)
            {
                throw new StreamPlayerException("Failed to get the current image.");
            }

            try
            {
                BITMAPINFOHEADER biHeader = (BITMAPINFOHEADER)Marshal.PtrToStructure(dibPtr, typeof(BITMAPINFOHEADER));
                Int32 stride = biHeader.biWidth * (biHeader.biBitCount / 8);

                // The bits in the array are packed together, but each scan line must be
                // padded with zeros to end on a LONG data-type boundary.
                Int32 padding = stride % 4 > 0 ? 4 - stride % 4 : 0;
                stride += padding;

                Bitmap image = new Bitmap(biHeader.biWidth, biHeader.biHeight, stride,
                    PixelFormat.Format24bppRgb, (IntPtr) (dibPtr.ToInt64() + Marshal.SizeOf(biHeader)));
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                return image;
            }
            finally
            {
                if (dibPtr != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(dibPtr);
                }
            }
        }

        /// <summary>
        /// Retrieves the frame size.
        /// </summary>
        /// <returns>The frame size.</returns>
        internal Size GetFrameSize()
        {
            Int32 width, height;
            if (_getFrameSize(out width, out height) != 0)
            {
                throw new StreamPlayerException("Failed to get the frame size.");
            }

            return new Size(width, height);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean FreeLibrary(IntPtr hModule);
        public void Dispose()
        {
            if (_hDll != IntPtr.Zero)
            {
                FreeLibrary(_hDll);
                _hDll = IntPtr.Zero;
            }

            if (File.Exists(_dllFile))
            {
                 File.Delete(_dllFile);
            }
        }

        private Boolean IsX86Platform
        {
            get { return IntPtr.Size == 4; }
        }
        
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(String lpFileName);

        /// <summary>
        /// Extracts the FFmpeg facade dll from resources to a temp file and loads it.
        /// </summary>
        /// <exception cref="Win32Exception">Failed to load the FFmpeg facade dll.</exception>
        private void LoadDll()
        {
            _dllFile = Path.GetTempFileName();
            using (var stream = new FileStream(_dllFile, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(IsX86Platform ?
                        Futech.Video.Properties.Resources.StreamPlayer : Futech.Video.Properties.Resources.StreamPlayer64);
                }
            }

            _hDll = LoadLibrary(_dllFile);
            if (_hDll == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, String procName);

        /// <summary>
        /// Binds the class instance methods to the stream player library functions.
        /// </summary>
        /// <param name="hDll">The library to bind to.</param>
        private void BindToDll(IntPtr hDll)
        {
            IntPtr procPtr = GetProcAddress(hDll, "Initialize");
            _initialize =
                (InitializeDelegate)Marshal.GetDelegateForFunctionPointer(procPtr, typeof(InitializeDelegate));

            procPtr = GetProcAddress(hDll, "StartPlay");
            _startPlayDelegate =
                (StartPlayDelegate)Marshal.GetDelegateForFunctionPointer(procPtr, typeof(StartPlayDelegate));

            procPtr = GetProcAddress(hDll, "GetCurrentFrame");
            _getCurrentFrame =
                (GetCurrentFrameDelegate)Marshal.GetDelegateForFunctionPointer(procPtr, typeof(GetCurrentFrameDelegate));

            procPtr = GetProcAddress(hDll, "GetFrameSize");
            _getFrameSize =
                (GetFrameSizeDelegate)Marshal.GetDelegateForFunctionPointer(procPtr, typeof(GetFrameSizeDelegate));

            procPtr = GetProcAddress(hDll, "Stop");
            _stop =
                (StopDelegate)Marshal.GetDelegateForFunctionPointer(procPtr, typeof(StopDelegate));

            procPtr = GetProcAddress(hDll, "Uninitialize");
            _uninitialize = (UninitializeDelegate)Marshal.GetDelegateForFunctionPointer(procPtr, typeof(UninitializeDelegate));
        }

        private delegate Int32 InitializeDelegate(Futech.Video.CameraFFMPEG.StreamPlayerParams playerParams);
        private InitializeDelegate _initialize;

        private delegate Int32 StartPlayDelegate([MarshalAs(UnmanagedType.LPStr)]String url,
            Int32 connectionTimeout, Int32 transport, Int32 flags);
        private StartPlayDelegate _startPlayDelegate;

        private delegate Int32 GetCurrentFrameDelegate([Out] out IntPtr dibPtr);
        private GetCurrentFrameDelegate _getCurrentFrame;

        private delegate Int32 GetFrameSizeDelegate(out Int32 width, out Int32 height);
        private GetFrameSizeDelegate _getFrameSize;

        private delegate Int32 StopDelegate();
        private StopDelegate _stop;

        private delegate Int32 UninitializeDelegate();
        private UninitializeDelegate _uninitialize;

        private String _dllFile = string.Empty;
        private IntPtr _hDll = IntPtr.Zero;
    }
}

