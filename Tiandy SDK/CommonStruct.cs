using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TiandyNetClient
{
    [StructLayout(LayoutKind.Sequential)]
    struct DOWNLOAD_FILE
    {
        public Int32 m_iSize;			//结构体大小
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
        public char[] m_cRemoteFilename;   //前端录像文件名
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
        public char[] m_cLocalFilename;	  //本地录像文件名
        public Int32 m_iPosition;		//文件定位时,按百分比0～100;断点续传时，请求的文件指针偏移量
        public Int32 m_iSpeed;			//1，2，4，8，控制文件播放速度, 0-暂停
        public Int32 m_iIFrame;			//只发I帧 1,只播放I帧;0,全部播放					
        public Int32 m_iReqMode;			//需求数据的模式 1,帧模式;0,流模式					
        public Int32 m_iRemoteFileLen;	//	如果本地文件名不为空，此参数置为空
    };
    [StructLayout(LayoutKind.Sequential)]
    struct _MAIN_NOTIFY_DATA
    {
        public Int32 m_iLogonID;
        public Int32 m_wParam;
        public Int32 m_lParam;
        public Int32 m_iUserData;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct DOWNLOAD_CONTROL
    {
        public Int32 m_iSize;			//结构体大小
        public Int32 m_iPosition;		//0～100，定位文件播放位置；-1，不进行定位
        public Int32 m_iSpeed;			//1，2，4，8，控制文件播放速度, 0-暂停
        public Int32 m_iIFrame;			//只发I帧 1,只播放I帧;0,全部播放
        public Int32 m_iReqMode;			//需求数据的模式 1,帧模式;0,流模式
    };

    [StructLayout(LayoutKind.Sequential)]
    struct S_header
    {
        public UInt16 FrameRate;
        public UInt16 Width;
        public UInt16 Height;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct NVS_FILE_QUERY
    {
        public int m_iType;          /* Record type 1-Manual record, 2-Schedule record, 3-Alarm record*/
        public int m_iChannel;       /* Record channel 0~channel defined channel number*/
        public NVS_FILE_TIME m_struStartTime;  /* File start time */
        public NVS_FILE_TIME m_struStoptime;   /* File end time */
        public int m_iPageSize;      /* Record number returned by each research*/
        public int m_iPageNo;        /* From which page to research */
        public int m_iFiletype;      /* File type, 0-All, 1-AVstream, 2-picture*/
        public int m_iDevType;       /* 设备类型，0-摄像 1-网络视频服务器 2-网络摄像机 0xff-全部*/
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct NVS_FILE_TIME
    {
        public UInt16 m_iYear;   /* Year */
        public UInt16 m_iMonth;  /* Month */
        public UInt16 m_iDay;    /* Day */
        public UInt16 m_iHour;   /* Hour */
        public UInt16 m_iMinute; /* Minute */
        public UInt16 m_iSecond; /* Second */
    }
    [StructLayout(LayoutKind.Sequential)]
    struct NVS_FILE_DATA
    {
        public int m_iType;          /* Record type 1-Manual record, 2-Schedule record, 3-Alarm record*/
        public int m_iChannel;       /* Record channel 0~channel defined channel number*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 250)]
        public Char[] m_cFileName; /* File name */
        public NVS_FILE_TIME m_struStartTime;  /* File start time */
        public NVS_FILE_TIME m_struStoptime;   /* File end time */
        public int m_iFileSize;      /* File size */
    }

    /// <summary>
    /// /////////////////////////////
    /// </summary>

    [StructLayout(LayoutKind.Sequential)]
    struct CONNECT_STATE
    {
        public int m_iLogonID;
        public int m_iChannelNO;
        public int m_iStreamNO;
        public UInt32 m_uiConID;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct CLIENTINFO
    {
        public int m_iServerID;        //NVS ID,NetClient_Logon 返回值
        public int m_iChannelNo;	    //Remote host to be connected video channel number (Begin from 0)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
        public Char[] m_cNetFile;    //Play the file on net, not used temporarily
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public Char[] m_cRemoteIP;	//IP address of remote host
        public int m_iNetMode;		    //Select net mode 1--TCP  2--UDP  3--Multicast
        public int m_iTimeout;		    //Timeout length for data receipt
        public int m_iTTL;			    //TTL value when Multicast
        public int m_iBufferCount;     //Buffer number
        public int m_iDelayNum;        //Start to call play progress after which buffer is filled
        public int m_iDelayTime;       //Delay time(second), reserve
        public int m_iStreamNO;        //Stream type
        public int m_iFlag;			//0，首次请求该录像文件；1，操作录像文件
        public int m_iPosition;		//0～100，定位文件播放位置；-1，不进行定位
        public int m_iSpeed;			//1，2，4，8，控制文件播放速度        
    };

    [StructLayout(LayoutKind.Sequential)]
    struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct NVS_IPAndID
    {
        public string m_pIP;
        public string m_pID;
        public UInt32 m_puiLogonID;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct NVS_SCHEDTIME
    {
        public UInt16 m_ustStartHour;
        public UInt16 m_usStartMin;
        public UInt16 m_ustStopHour;
        public UInt16 m_ustStopMin;
        public UInt16 m_ustRecordMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct STR_VideoParam
    {
        public UInt16 m_ustBrightness;             //亮度
        public UInt16 m_usHue;                     //色度
        public UInt16 m_ustContrast;               //对比度
        public UInt16 m_ustSaturation;             //饱和度
        [MarshalAs(UnmanagedType.Struct)]
        public NVS_SCHEDTIME m_strctTempletTime;   //时间模板        
    }

    //Ctrl param
    [StructLayout(LayoutKind.Sequential)]
    struct CONTROL_PARAM
    {
        public Int32 m_iAddress;   //device address
        public Int32 m_iPreset;	   //preset pos
        [MarshalAs(UnmanagedType.Struct)]
        public POINT m_ptMove;     //move pos
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] m_btBuf;     //Ctrl-Code(OUT)
        public Int32 m_iCount;     //Ctrl-Code count(OUT)
    };

    [StructLayout(LayoutKind.Sequential)]
    struct POINT
    {
        public Int32 x;
        public Int32 y;
    };

    [StructLayout(LayoutKind.Sequential)]
    class Reserve
    {
        public Int32 m_iReserved1;
        public UInt32 m_ustReserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] m_btReserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] m_btReserved2;
        public Reserve()
        {
            m_iReserved1 = new Int32();
            m_ustReserved2 = new UInt32();
            m_btReserved1 = new byte[32];
            m_btReserved2 = new byte[64];
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    class NvsSingle
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] m_btNvsIP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] m_btNvsName;
        public Int32 m_iNvsType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] m_btFactoryID;
        [MarshalAs(UnmanagedType.Struct)]
        public Reserve m_stReserve;
        public NvsSingle()
        {
            m_btNvsIP = new byte[32];
            m_btNvsName = new byte[32];
            m_btFactoryID = new byte[32];
            m_stReserve = new Reserve();
        }

    };

    [StructLayout(LayoutKind.Sequential)]
    class DNSRegInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] m_btUserName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] m_btPwd;
        [MarshalAs(UnmanagedType.Struct)]
        public NvsSingle m_stNvs;
        public Int32 m_iPort;
        public Int32 m_iChannel;
        [MarshalAs(UnmanagedType.Struct)]
        public Reserve m_stReserve;
        public DNSRegInfo()
        {
            m_btUserName = new byte[32];
            m_btPwd = new byte[32];
            m_stNvs = new NvsSingle();
            m_stReserve = new Reserve();
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    class REG_DNS
    {
        [MarshalAs(UnmanagedType.Struct)]
        public DNSRegInfo m_stDNSInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] m_btRegTime;
        [MarshalAs(UnmanagedType.Struct)]
        public Reserve m_stReserve;
        public REG_DNS()
        {
            m_stDNSInfo = new DNSRegInfo();
            m_btRegTime = new byte[32];
            m_stReserve = new Reserve();
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    class REG_NVS
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] m_btPrimaryDS;
        [MarshalAs(UnmanagedType.Struct)]
        public NvsSingle m_stNvs;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] m_btRegTime;
        public UInt32 m_uiClientConnNum;
        public Boolean m_blRegister;
        [MarshalAs(UnmanagedType.Struct)]
        public Reserve m_stReserve;
        public REG_NVS()
        {
            m_btPrimaryDS = new byte[32];
            m_stNvs = new NvsSingle();
            m_btRegTime = new byte[32];
            m_stReserve = new Reserve();
        }
    };

    struct FRAME_INFO
    {
        public UInt32 nWidth;    //Video width, audio data is 0；
        public UInt32 nHeight;   //Video height, audio data is 0；
        public UInt32 nStamp;    //Time stamp(ms)。
        public UInt32 nType;     //Audio type，T_AUDIO8,T_YUV420，。
        public UInt32 nFrameRate;//Frame rate。
        public UInt32 nReserved; //reserve
    };

    struct LogonInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] cDSIP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] cUserName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] cUserPwd;
        [MarshalAs(UnmanagedType.Struct)]
        public NvsSingle stNvs;
        [MarshalAs(UnmanagedType.Struct)]
        public Reserve m_stReserve;
    };

    struct ProxyInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] cProxyIP;
        public int iProxyPort;
        [MarshalAs(UnmanagedType.Struct)]
        public Reserve m_stReserve;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct FTP_SNAPSHOT
    {
        public Int32 iChannel;			//	通道 
        public Int32 iEnable;			//	模式 0:不使能,1:使能(定时),2:(报警联动抓拍),3:报警联动多次抓拍注释,默认为2
        public Int32 iQValue;			//	图片质量 取值范围0-100
        public Int32 iInterval;			//	抓拍时间间隔 取值范围1-3600(秒)
        public Int32 iPictureSize;		//  抓拍图片大小	0x7fff：代表自动，其余对应分辨率大小
    };

    [StructLayout(LayoutKind.Sequential)]
    struct FTP_LINKSEND
    {
        public Int32 iChannel;			//	通道
        public Int32 iEnable;			//	使能
        public Int32 iMethod;			//	方式
    };

    [StructLayout(LayoutKind.Sequential)]
    struct STR_Para
    {

    };

    struct VCA_TAlarmInfo
    {
        public Int32 iID; // alarm message ID, used to obtain specific information
        public Int32 iChannel; // channel number
        public Int32 iState; // alarm status
        public Int32 iEventType; // Event type 0: single trip line 1: double trip line 2: perimeter detection 3: wandering 4: parking 5: running
        // 6: Personnel density in the area 7: Stolen objects 8: Discards 9: Face recognition 10: Video diagnosis
        // 11: intelligent tracking 12: traffic statistics 13: crowd gathering 14: leave the job detection 15: audio diagnosis
        public Int32 iRuleID; // Rule ID
        public UInt32 uiTargetID; // Destination ID
        public Int32 iTargetType; // target type
        public RECT rctTarget; // Destination location
        public Int32 iTargetSpeed; // target speed
        public Int32 iTargetDirection; // target direction
        public Int32 iPresetNo; // Preset ID
        public UInt32 iWaterLevelNUm; // surface scale readings
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] cWaterPicName;//  Save the picture path
        public Int32 iPicType; // 0: Key 1: Ordinary
        public Int32 iDataType; // 0: real-time 1: offline
    };

    [StructLayout(LayoutKind.Sequential)]
    struct USER_AUTHORITY
    {
        public Int32 iNeedSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 27)]
        public AUTHORITY_INFO[] structAutInfo;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct AUTHORITY_INFO
    {
        public Int32 iLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Int32[] uiList;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct GROUP_AUTHORITY
    {
        public Int32 iSize;
        public Int32 iGroupNO;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Int32[] uiList;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] btList;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct AUTHORITY_INFO_V1
    {
        public Int32 iSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] tbUsername;                 // User name
        public Int32 iLevel;                      // //Permission number
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public Int32[] iList;                     // Channel list array
    };

    [StructLayout(LayoutKind.Sequential)]
    struct UserReservePhone
    {
        public Int32 iSize;
        public Int32 iFindMode;                   //IN 1-phone 2mail
        public Int32 iSynFlag;                    //OUT 1-Need to synchronize data 0-no need synchronize data
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] btModeInfo;                 // OUT phone num or mail addr
    };

    [StructLayout(LayoutKind.Sequential)]
    struct VCASuspendResult
    {
	    public Int32		iBufSize;				//structure size
	    public Int32		iStatus;				//Status(0:Suspend intelligent analysis 1:Open intelligent analysis)
	    public Int32		iResult;				//Result(1:success[indicate that parameters can be set] 2:fail[indicate that parameter is being set and can not set])
    };

    [StructLayout(LayoutKind.Sequential)]
    struct DOMEPTZ
    {
        public Int32 iSize;
        public Int32 iType;     // type
        public Int32 iAutoEnable; // 0 - not enabled, 1 - enabled
        public Int32 iParam1;   // Parameters
                                // 1 - Enable Preset Frozen: Not used
                                // 2 - Enable Auto Flip: Not used
                                // 3 - Preset speed level: 0 - low, 1 - medium, 2 - high
                                // 4 - manual speed level: 0 - low, 1 - in, 2 - high
                                // 5 - Enable standby action: Specific values: 30,60,300,600,1800 (unit: seconds)
                                // 6 - infrared light mode: 0 - manual, 1 - automatic
        public Int32 iParam2;   // Parameters
                                // 1 - Enable Preset Frozen: Not used
                                // 2 - Enable Auto Flip: Not used
                                // 3 - Preset speed level: Not used
                                // 4 - Manual Speed ??Level: Not used
                                // 5 - Enable standby action: 0 - preset, 1 - scan, 2 - cruise, 3 - mode path
                                // 6 - infrared light mode: "0 - manual, on behalf of infrared light brightness, the specific values: [0,10];
                                // 1 - Automatic, on behalf of infrared Ling \ sensitivity, specific values: 0 - minimum, 1 - low, 2 - standard,
                                // 3 - higher, 4 - highest;
        public Int32 iParam3; // Reserved
        public Int32 iParam4; // Reserved
    };

    [StructLayout(LayoutKind.Sequential)]
    // intelligent analysis configuration parameters
    struct vca_TConfig
    {
	    public int iVideoSize; // Video size
	    public int iDisplayTarget; // Whether to superimpose the target frame
	    public int iDisplayTrace; // Whether the trajectory is superimposed
	    public int iTargetColor; // Target box color
	    public int iTargetAlarmColor; // Color of the target frame when alarming
	    public int iTraceLength; // Track length
    }; 	 

    [StructLayout(LayoutKind.Sequential)]
    // Advanced analysis of intelligent analysis
    struct vca_TAdvParam
    {
	    public int iEnable; // whether to enable advanced parameters
	    public int iTargetMinSize; // target the minimum number of pixels
	    public int iTargetMaxSize; // target the maximum number of pixels
	    public int iTargetMinWidth; // Maximum width
	    public int iTargetMaxWidth; // minimum width
	    public int iTargetMinHeight; // maximum height
	    public int iTargetMaxHeight; // minimum height
	    public int iTargetMinSpeed; // target minimum pixel speed (-1 is not restricted)
	    public int iTargetMaxSpeed; // target maximum pixel speed (-1 is not restricted)
	    public int iTargetMinWHRatio; // Minimum aspect ratio
	    public int iTargetMaxWHRatio; // Maximum aspect ratio
	    public int iSensitivity; // sensitivity level
    };	

    [StructLayout(LayoutKind.Sequential)]
    struct vca_TDisplayParam
    {
	    public Int32					iDisplayRule;			//display Rule,0-display 1-Not display	
	    public Int32					iDisplayStat;			//alarm count display,0-display 1-Not display
	    public Int32					iColor;					//zone color,1-red 2-green 3-yellow 4-blue 5-Purple 6-cyan-blue 7-black 8-White,(defalit green)
	    public Int32					iAlarmColor;			//alarm zone color
    };	

    [StructLayout(LayoutKind.Sequential)]
    struct vca_TAlarmTimeSegment
    {
	    public int		iStartHour;					
	    public int		iStartMinute;
	    public int		iStopHour;
	    public int		iStopMinute;
	    public int		iEnable;
    } ; // Alarm time period

    [StructLayout(LayoutKind.Sequential)]
    struct vca_TAlarmSchedule
    {
        public int iWeekday;					//	Sunday to Saturday (0-6)
	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        public vca_TAlarmTimeSegment[,] timeSeg;				//	time period
    } ; // Alarm arming template

    [StructLayout(LayoutKind.Sequential)]
    struct vca_TLinkPTZ
    {
	    public UInt16 	usType;								//0 Do not associate the channel, 1 preset, 2 track, 3 path
	    public UInt16 	usPresetNO;							//	Preset position number
	    public UInt16 	usTrackNO;							//	track number
	    public UInt16 	usPathNO;							//	path number
	    public UInt16 	usSceneNO;							//	Scene number
    } ;

    // Alarm linkage
    [StructLayout(LayoutKind.Sequential)]
    struct vca_TAlarmLink
    {
	    public int						iLinkType;					//	0, linkage voice prompts; 1, linkage screen display; 2, linkage output port; 3, linkage recording; 4, linkage PTZ;
	    public int						iLinkChannel;
    	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public int[]                    iLinkSet;				//	0,1,2,3,5
	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public vca_TLinkPTZ[]			ptz;		//	4
    } ;	

    [StructLayout(LayoutKind.Sequential)]
    struct vca_TScheduleEnable
    {
        public int iEnable;					//	Whether to enable	
        public int iParam;						//	Parameters, reserved
    } ;

    [StructLayout(LayoutKind.Sequential)]
    struct vca_TPoint
    {
        public int iX;
        public int iY;
    } ;

    [StructLayout(LayoutKind.Sequential)]
    struct vca_TLine
    {
	    public vca_TPoint 	stStart;
	    public vca_TPoint 	stEnd;
    } ;						//	sizeof = 2*8 = 16

    // Single trip line parameter
    [StructLayout(LayoutKind.Sequential)]
    struct vca_TTripwire
    {
        public int iValid;					//	is valid, the original rule iEventID to determine which events can be effective,
    // But if all rules are not effective iEventID will always point to an event, the upper can not determine whether it is really effective, can only add an event effective field
        public int iTargetTypeCheck;		//	target type limit
        public int iMinDistance;			//	minimum distance, the target movement distance must be greater than this threshold
        public int iMinTime;				//	minimum time, the target moving distance must be greater than this threshold	
        public int iType;					//	that through the type
        public int iDirection;				//	single trip line to prohibit the direction of the angle
        public vca_TLine stLine;					//	trip line coordinates
    } ; 
     

    // Rule Sets the parameter
    [StructLayout(LayoutKind.Sequential)]
    struct vca_TRuleParam
    {
	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public byte[]				cRuleName;	// Name of the rule
	    public Int32					iValid;								// Whether the rule takes effect

	    public vca_TDisplayParam	stDisplayParam;						//	Display the parameter settings

	    public Int32					iAlarmCount;						//	he number of alarms
	    public vca_TAlarmSchedule	alarmSchedule;						//	alarm arming template parameters
	    public vca_TAlarmLink		alarmLink;							//	Alarm linkage setting parameters
	    public vca_TScheduleEnable	scheduleEnable;						//	Arming Enable

        public Int32    iEventID;							// Behavior analysis event type

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
        public byte[]               cBuf;
        //public vca_TTripwire		tripwire;								// Behavior analysis event parameters
    };

    // intelligent analysis of channel parameters
    [StructLayout(LayoutKind.Sequential)]
    struct vca_TVCAChannelParam
    {
	    public Int32	    iEnable;					//	Whether to enable this channel intelligent analysis
	    public vca_TConfig			config;  					//intelligent analysis of configuration parameters
	    public vca_TAdvParam		advParam;					//	intelligent analysis of advanced parameters
	    public Int32					iRuleID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public vca_TRuleParam[]		rule; 		//	Rules Sets the parameters
    } ;


    // VCA configuration parameter set,
    [StructLayout(LayoutKind.Sequential)]
    struct vca_TVCAParam
    {
	    public Int32 iEnable; // whether to enable intelligent analysis
	    public Int32 iChannelBit; // Bit set of the intelligent analysis channel
	    public Int32 iChannelID; // VCA channel
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public vca_TVCAChannelParam[]	chnParam;				//	parameter settings for each channel
    };

    [StructLayout(LayoutKind.Sequential)]
    struct AnyScene
    {
        public int iBufSize;			//Scene application timed cruise template structure size
        public int iSceneID;			//Scene number(0~15)
	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] cSceneName;	//Scene name
        public int iArithmetic;		//enable the algorithm type( bit0: 1-action analysis algorithm enable；0-not enable
										    //bit1：1-track algorithm enable；0-not enable
										    //bit2：1-face detection algorithm enable；0-not enable
										    //bit3：1-people amount statistics algorithm enable；0-not enable
										    //bit4：1-video diagnose algorithm enable；0-not enable
	                                        //bit5：1-license plate recognition algorithm enable；0-not enable
	                                        //bit6：1-audio and video exception algorithm enable；0-not enable 
										    //bit7：1-off post algorithm enable；0-not enable      
										    //bit8：1-people gathering algorithm enable；0-not enable
										    //bit11:1-witness protection algorithm enable；0-not enable
										    //bit20:1-Structured algorithm enable；0-not enable
        public int iDevType;
    };

     [StructLayout(LayoutKind.Sequential)]
    struct vca_TPolygonEx
    {
	    public Int32 		iPointNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	    public vca_TPoint[] 	stPoints;   
    } ;

    [StructLayout(LayoutKind.Sequential)]
    struct VCARule
    {
        public Int32 iSceneID;				//scene ID(0~15)
        public Int32 iRuleID;				//rule ID(0~7)
        public Int32 iValid;					//event valid, 0:invalid,1:valid
    };

    [StructLayout(LayoutKind.Sequential)]
    struct VCA_TRuleParam_Tripwire
    {
        public Int32 iBufSize;
	    public VCARule				stRule;
        public Int32 iTargetTypeCheck;
	    public vca_TDisplayParam	stDisplayParam;
        public Int32 iTripwireType;			//0: unidirectional, 1: bidirectional
        public Int32 iTripwireDirection;		//(0～359°)
        public Int32 iMinSize;				//[0, 100] init 5
        public Int32 iMaxSize;				//[0, 100] init 30
	    public vca_TPolygonEx		stRegion1;				//Line1 Point Num and Points
        public Int32 iDisplayTarget;			//display target box, 0-not dispaly, 1-dispaly
    };


    delegate void RECVDATA_NOTIFY(uint _ulID, string _strData, int _iLen);
    delegate void DNSList_NOTIFY(Int32 _iCount, IntPtr _pDns);
    delegate void NVSList_NOTIFY(Int32 _iCount, IntPtr _pNvs);
    delegate void COMRECV_NOTIFY(Int32 _iLogonID, IntPtr _pBuf, Int32 _iLen, Int32 _iComNO);
    delegate void DECYUV_NOTIFY(UInt32 _ulID, IntPtr _pData, Int32 _iLen, ref FRAME_INFO _pFrameInfo, IntPtr _pContext);

    //  SDK4.0回调接口委托V4接口为—__CDECL调用
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void MAIN_NOTIFY_V4(UInt32 _ulLogonID, IntPtr _iWparam, IntPtr _iLParam, Int32 _iUser);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void ALARM_NOTIFY_V4(Int32 _ulLogonID, Int32 _iChan, Int32 _iAlarmState, Int32 _iAlarmType, Int32 _iUser);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void PARACHANGE_NOTIFY_V4(Int32 _ulLogonID, Int32 _iChan, Int32 _iParaType, ref STR_Para _strPara, Int32 _iUser);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void COMRECV_NOTIFY_V4(Int32 _ulLogonID, IntPtr _cData, Int32 _iLen, Int32 _iComNo, Int32 _iUser);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void PROXY_NOTIFY(Int32 _ulLogonID, Int32 _iCmdKey, IntPtr _cData, Int32 _iLen, Int32 _iUser);


    public class SDKConstMsg
    {
        public const int WM_USER = 0x0400; //
        public const int WM_MAIN_MESSAGE = WM_USER + 1001;
        public const int MSG_PARACHG = WM_USER + 90910;
        public const int MSG_ALARM = WM_USER + 90911;
        public const int WCM_ERR_ORDER = 2;
        public const int WCM_ERR_DATANET = 3;
        public const int WCM_LOGON_NOTIFY = 7;
        public const int WCM_VIDEO_HEAD = 8;
        public const int WCM_VIDEO_DISCONNECT = 9;
        public const int WCM_RECORD_ERR = 13;

        public const int LOGON_SUCCESS = 0;
        public const int LOGON_ING = 1;
        public const int LOGON_RETRY = 2;
        public const int LOGON_DSMING = 3;
        public const int LOGON_FAILED = 4;
        public const int LOGON_TIMEOUT = 5;
        public const int NOT_LOGON = 6;
        public const int LOGON_DSMFAILED = 7;
        public const int LOGON_DSMTIMEOUT = 8;
        public const int PLAYER_PLAYING = 0x02;
        public const int USER_ERROR = 0x10000000;

        public const int WCM_QUERYFILE_FINISHED = 18;  
        public const int WCM_DWONLOAD_FINISHED = 19;
        public const int WCM_DWONLOAD_FAULT = 20;

        // 对讲消息
        public const int WCM_TALK = 23;
        public const int WCM_VCA_SUSPEND = 103;// Suspends the intelligent analysis notification message
    }

    public class AlarmConstMsgType
    {
        public const int ALARM_VDO_MOTION	=	0;  //移动侦测
        public const int ALARM_VDO_REC		=	1;
        public const int ALARM_VDO_LOST		=	2;    
        public const int ALARM_VDO_INPORT	=	3;  //报警输入
        public const int ALARM_VDO_OUTPORT	=	4;  //报警输出
        public const int ALARM_VDO_COVER 	=	5;  //视频遮挡
        public const int ALARM_VCA_INFO		=	6;  //智能分析报警
        public const int ALARM_AUDIO_LOST	=	7;  //音频丢失
        public const int ALARM_EXCEPTION    =   8;  
                                           
    }


    public class ActionControlMsg
    {
        public const int MOVE = 60;
        public const int MOVE_STOP = 61;
        public const int MOVE_UP = 1;
        public const int MOVE_DOWN = 2;
        public const int MOVE_LEFT = 3;
        public const int MOVE_RIGHT = 4;
        public const int MOVE_UP_LEFT = 6;
        public const int MOVE_UP_RIGHT = 5;
        public const int MOVE_DOWN_LEFT = 8;
        public const int MOVE_DOWN_RIGHT = 7;
        public const int ZOOM_BIG = 10;
        public const int ZOOM_SMALL = 11;
        public const int FOCUS_NEAR = 13;
        public const int FOCUS_FAR = 14;
        public const int IRIS_OPEN = 17;
        public const int IRIS_CLOSE = 18;
        public const int RAIN_ON = 19;
        public const int RAIN_OFF = 20;
        public const int LIGHT_ON = 21;
        public const int LIGHT_OFF = 22;
        public const int HOR_AUTO = 23;
        public const int HOR_AUTO_STOP = 24;
        public const int CALL_VIEW = 25;
        public const int SET_VIEW = 28;
        public const int POWER_ON = 29;
        public const int POWER_OFF = 30;
        public const int ZOOM_BIG_STOP = 32;
        public const int ZOOM_SMALL_STOP = 34;
        public const int FOCUS_FAR_STOP = 36;
        public const int FOCUS_NEAR_STOP = 38;
        public const int IRIS_OPEN_STOP = 40;
        public const int IRIS_CLOSE_STOP = 42;
    }

    public class NetSDKCmd
    {
        public const int NET_CLIENT_MIN             = 0;
        public const int NET_CLIENT_ANYSCENE = (NET_CLIENT_MIN + 21); // Analyze the scene
        public const int NET_CLIENT_VCA_SUSPEND = NET_CLIENT_MIN + 32; // Suspend intelligence Analysis
        public const int NET_CLIENT_MODIFYAUTHORITY = (NET_CLIENT_MIN+84);	//modify authority
        public const int COMMAND_ID_USER_RESERVEPHONE = 72;

        public const int VCA_CMD_SET_MIN             = 1;// VCA sets the lower limit of the command
        public const int VCA_CMD_SET_CHANNEL_ENABLE = (VCA_CMD_SET_MIN + 20); // set VCA channel enable (0: do not enable intelligent analysis 1: enable local channel intelligent analysis 2: enable front-end channel intelligent analysis)
    
        public const int VCA_CMD_GET_MIN						= 16; // VCA read command lower limit, in order to be compatible with VIDEODETECT before the macro
        public const int VCA_CMD_GET_CHANNEL    = (VCA_CMD_GET_MIN + 0); // read VCA channel number and enable
        public const int VCA_CMD_GET_CHANNEL_ENABLE = (VCA_CMD_GET_MIN + 20);// read VCA channel enable (0: do not enable intelligent analysis 1: enable local channel intelligence analysis 2: enable front-end channel intelligent analysis)
           
        public const int VCA_CMD_MIN							= 100;
        public const int VCA_CMD_TARGET_PARAM				= VCA_CMD_MIN + 0 ;   // Set the VCA overlay and color settings
        public const int VCA_CMD_ADVANCE_PARAM				= VCA_CMD_MIN + 1 ;	// Set VCA advanced parameters
        public const int VCA_CMD_RULE_PARAM					= VCA_CMD_MIN + 2 ;	// Set VCA Rule General Parameters
        public const int VCA_CMD_ALARM_STATISTIC				= VCA_CMD_MIN + 3 ;	// Set the VCA alarm count to zero
        public const int VCA_CMD_TRIPWIRE					= VCA_CMD_MIN + 4 ;	// Set VCA rule (single trip line)
        public const int VCA_CMD_PERIMETER					= VCA_CMD_MIN + 5 ;	// Set VCA rule (perimeter)
        public const int VCA_CMD_LINGER						= VCA_CMD_MIN + 6 ;	// Set VCA rule (wandering)
        public const int VCA_CMD_PARKING						= VCA_CMD_MIN + 7 ;	// Set VCA rule (stop)
        public const int VCA_CMD_RUNNING						= VCA_CMD_MIN + 8 ;	// Set VCA rule (run)
        public const int VCA_CMD_CROWD						= VCA_CMD_MIN + 9 ;	// Set VCA rule (crowd)
        public const int VCA_CMD_DERELICT					= VCA_CMD_MIN + 10 ; // Set VCA rule (discard)
        public const int VCA_CMD_STOLEN						= VCA_CMD_MIN + 11 ; // Set the VCA rule (stolen)
        public const int VCA_CMD_MULITTRIP					= VCA_CMD_MIN + 12 ; // Set the VCA rule (double trip line)
        public const int VCA_CMD_VIDEODIAGNOSE				= VCA_CMD_MIN + 13 ; // set VCA rule (video diagnose for 300W)
        public const int VCA_CMD_AUDIODIAGNOSE				= VCA_CMD_MIN + 14 ; // set VCA rule (audio diagnose for 300W)
        public const int VCA_CMD_TRIPWIRE_EX					= VCA_CMD_MIN + 15 ; // set VCA rule (Tripwire for 300W)
        public const int VCA_CMD_RULE14_LEAVE_DETECT			= VCA_CMD_MIN + 16 ; // set VCA rule (leave detect for 300w)
        public const int VCA_CMD_CHANNEL_ENABLE				= VCA_CMD_MIN + 17 ; // Channel Enable
        public const int VCA_CMD_FACEREC						= VCA_CMD_MIN + 18; // Set VCA Rule (Face Recognition ;
        public const int VCA_CMD_TRACK						= VCA_CMD_MIN + 19 ; // Set the VCA rule (track)
        public const int VCA_CMD_VIDEODETECTION				= VCA_CMD_MIN + 20 ; // Set VCA Rule (Video Diagnostics)
        public const int VCA_CMD_VIDEOSIZE					= VCA_CMD_MIN + 21 ;                  //set the intelligent analysis of video input size
        public const int VCA_CMD_RIVERCLEAN					= VCA_CMD_MIN + 22 ; // Set the VCA rule = river detection)
        public const int VCA_CMD_DREDGE						= VCA_CMD_MIN + 23 ; // set the VCA rule
        public const int VCA_CMD_RIVERADV					= VCA_CMD_MIN + 24 ; // Set VCA channel detection advanced parameters
        public const int VCA_CMD_SCENEREV					= VCA_CMD_MIN + 25 ; //intelligent analysis scene recovery time
        public const int VCA_CMD_FIGHT						= VCA_CMD_MIN + 26 ; // Fight Arithmetic
        public const int VCA_CMD_PROTECT						= VCA_CMD_MIN + 27 ; // Set the alert rule parameters

    }

    public class SDKDefine
    {
        public const int USER_FIND_PWD_MODE_PHONE	= 1;
        public const int USER_FIND_PWD_MODE_MAIL    = 2;
    }

    public class SDKCOMMON_ID
    {
        public const int CI_COMMON_ID_FORBIDCHN = 0x12012;	//Forbidden channel number
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SetPtz
    {
        public Int32 iSize;   
        public Int32 iType; // control type 0- control speed 1- control position 2- relative control position
        public Int32 iPan;  //Itype is 0 at the level of horizontal direction (0-100)
							//Itype is 1 for moving target level 0-36000, corresponding to 0-360 degrees, accurate to 0.01.
							//Itype is a relative movement angle 0-36000, representing a -180~180 degree at 2.  
        public Int32 iTilt; //The vertical direction velocity (0-100) when itype is 0
							//Itype represents the moving target vertical position 1000~19000 at 1 and 2, corresponding to the -90-90 degree, to 0.01
        public Int32 iZoom; //When itype is 0, the speed of double times (0-100)
							//When itype is 1, the target variable position of the moving target is 0-1000 times that of 0-1000, accurate to 0.01.
							//Itype is a multiple 0~100000 of relative movement at 2, corresponding to -500~500 times, accurate to 0.01         
    };

    [StructLayout(LayoutKind.Sequential)]
    struct GetPtz
    {
        public Int32 iSize;   
        public Int32 iPosP; 
        public Int32 iPosT;  		
        public Int32 iPosZ; 
    };

    public class COMMAND_ID
    {
        public const int COMMAND_ID_MIN							= 0;
        public const int COMMAND_ID_ITS_FOCUS					= (COMMAND_ID_MIN + 1);		//Five-point focused camera control protocol
        public const int COMMAND_ID_MODIFY_CGF_FILE				= (COMMAND_ID_MIN + 2);		//Set the value of any field in the configuration file 
        public const int COMMAND_ID_AUTO_FOCUS					= (COMMAND_ID_MIN + 3);		//auto focus
        public const int COMMAND_ID_SAVECFG						= (COMMAND_ID_MIN + 4);		//save param
        public const int COMMAND_ID_DEFAULT_PARA					= (COMMAND_ID_MIN + 5);		//default para
        public const int COMMAND_ID_DIGITAL_CHANNEL				= (COMMAND_ID_MIN + 6);		//digital channel
        public const int COMMAND_ID_DELETE_FOG					= (COMMAND_ID_MIN + 7);		//delete fog
        public const int COMMAND_ID_QUERY_REPORT					= (COMMAND_ID_MIN + 8);		//query report form
        public const int COMMAND_ID_DEV_LASTERROR				= (COMMAND_ID_MIN + 9);		//device last error
        public const int COMMAND_ID_TEST_HTTP					= (COMMAND_ID_MIN + 10);		//test http 
        public const int COMMAND_ID_HOTBACKUP_REQUESTSYNC		= (COMMAND_ID_MIN + 11);		//hot banckup device request sync
        public const int COMMAND_ID_HOTBACKUP_RECORDFILE			= (COMMAND_ID_MIN + 12);		//hot banckup device record file info
        public const int COMMAND_ID_HOTBACKUP_SYNCFINISH			= (COMMAND_ID_MIN + 13);		//hot banckup device sync file finish
        public const int COMMAND_ID_BATTERYINFO					= (COMMAND_ID_MIN + 14);		//Battery Info
        public const int COMMAND_ID_PROOF_ADJUST					= (COMMAND_ID_MIN + 15);		//Proof Adjust
        public const int COMMAND_ID_PLAY_AUDIO_SAMPLE			= (COMMAND_ID_MIN + 16);
        public const int COMMAND_ID_CANCEL_EMAIL					= (COMMAND_ID_MIN + 17);
        public const int COMMAND_ID_BIGPACK_UPGRADE				= (COMMAND_ID_MIN + 18);
        public const int COMMAND_ID_SEARCH_EXDEV					= (COMMAND_ID_MIN + 19);		//search exter device
        public const int COMMAND_ID_GET_SEARCHEXDEV_COUNT		= (COMMAND_ID_MIN + 20);		//search exter device count
        public const int COMMAND_ID_GET_EXDEVLIST				= (COMMAND_ID_MIN + 21);		//get exter device list
        public const int COMMAND_ID_GET_EXDEVLIST_COUNT			= (COMMAND_ID_MIN + 22);		//get exter device list count
        public const int COMMAND_ID_GET_RECORDINGAUDIOLIST		= (COMMAND_ID_MIN + 23);		//get recording audio list
        public const int COMMAND_ID_GET_RECORDINGAUDIOLIST_COUNT	= (COMMAND_ID_MIN + 24);		//get recording audio count
        public const int COMMAND_ID_SET_TRACKING_RATE			= (COMMAND_ID_MIN + 25);		//set tracking rate
        public const int COMMAND_ID_TRACKING_RATE               = COMMAND_ID_SET_TRACKING_RATE;//tracking rate
        public const int COMMAND_ID_PWD_VERIFY					= (COMMAND_ID_MIN + 26);		//user and password verify
        public const int COMMAND_ID_SEEK_NVSS					= (COMMAND_ID_MIN + 27);		//seek nvss
        public const int COMMAND_ID_XCHG_IP						= (COMMAND_ID_MIN + 28);		//chang nvss ip
        public const int COMMAND_ID_FILE_MAP						= (COMMAND_ID_MIN + 29);		//file map
        public const int COMMAND_ID_GET_AUDIO					= (COMMAND_ID_MIN + 30);		//Get Audio Value
        public const int COMMAND_ID_PLATE_LIST_QUERY				= (COMMAND_ID_MIN + 31);		//plate list query
        public const int COMMAND_ID_PLATE_LIST_MODIFY			= (COMMAND_ID_MIN + 32);		//plate list modify
        public const int COMMAND_ID_DISK_PART					= (COMMAND_ID_MIN + 33);		//disk part
        public const int COMMAND_ID_BADBCLOCK_SIZE				= (COMMAND_ID_MIN + 34);		//disk bad block size
        public const int COMMAND_ID_BADBCLOCK_TEST				= (COMMAND_ID_MIN + 35);		//disk bad block test
        public const int COMMAND_ID_BADBCLOCK_TEST_STATE			= (COMMAND_ID_MIN + 36);		//disk bad block test state
        public const int COMMAND_ID_BADBCLOCK_BLOCK_STATE		= (COMMAND_ID_MIN + 37);		//disk bad block block state
        public const int COMMAND_ID_BADBCLOCK_ACTION				= (COMMAND_ID_MIN + 38);		//disk bad block action
        public const int COMMAND_ID_FILE_INPUT					= (COMMAND_ID_MIN + 39);		//file input
        public const int COMMAND_ID_FILE_OUTPUT					= (COMMAND_ID_MIN + 40);		//file output
        public const int COMMAND_ID_FILE_CONVERT					= (COMMAND_ID_MIN + 41);		//file convert
        public const int COMMAND_ID_FILE_TRANSPORT				= (COMMAND_ID_MIN + 42);		//file transport
        public const int COMMAND_ID_GET_PTZ						= (COMMAND_ID_MIN + 43);	
        public const int COMMAND_ID_ASENSOR_CORRECT				= (COMMAND_ID_MIN + 44);	
        public const int COMMAND_ID_REPORT_QUERY					= (COMMAND_ID_MIN + 45);	
        public const int COMMAND_ID_HEATMAP_GET					= (COMMAND_ID_MIN + 46);	
        public const int COMMAND_ID_PORT_MAP						= (COMMAND_ID_MIN + 47);	
        public const int COMMAND_ID_NTP_TEST						= (COMMAND_ID_MIN + 48);	
        public const int COMMAND_ID_QUERY_AUTOTEST_RESULT		= (COMMAND_ID_MIN + 49);	
        public const int COMMAND_ID_FTP_DOWNLOAD_VIDEO			= (COMMAND_ID_MIN + 50);	
        public const int COMMAND_ID_UPGRADE_ERROR_INFO			= (COMMAND_ID_MIN + 51);	
        public const int COMMAND_ID_GET_DEV_MAC					= (COMMAND_ID_MIN + 52);	
        public const int COMMAND_ID_UPGRADE_FINISH_INFO			= (COMMAND_ID_MIN + 53);	
        public const int COMMAND_ID_CALIBRATE_STATUS				= (COMMAND_ID_MIN + 54);	
        public const int COMMAND_ID_SET_CALIBRATE				= (COMMAND_ID_MIN + 55);	
        public const int COMMAND_ID_CALIBRATE_VCA_PARA			= (COMMAND_ID_MIN + 56);	
        public const int COMMAND_ID_3D_POSITION					= (COMMAND_ID_MIN + 57);
        public const int COMMAND_ID_SYSTEM_TEMPERATURE			= (COMMAND_ID_MIN + 58);
        public const int COMMAND_ID_SYSTEM_FAN_NUM				= (COMMAND_ID_MIN + 59);
        public const int COMMAND_ID_SYSTEM_FAN_SPEED				= (COMMAND_ID_MIN + 60);
        public const int COMMAND_ID_VCAFPGA_QUERYINFO			= (COMMAND_ID_MIN + 61); 
        public const int COMMAND_ID_CCM_CALIBRATE				= (COMMAND_ID_MIN + 62);
        public const int COMMAND_ID_DDNSTEST						= (COMMAND_ID_MIN + 64);
        public const int COMMAND_ID_VCA_BIND						= (COMMAND_ID_MIN + 65);
        public const int COMMAND_ID_CLOUD_DETECT					= (COMMAND_ID_MIN + 66);
        public const int COMMAND_ID_CLOUD_UPGRADE				= (COMMAND_ID_MIN + 67);
        public const int COMMAND_ID_CLOUD_DOWNLOAD				= (COMMAND_ID_MIN + 68);
        public const int COMMAND_ID_CLOUD_UPGRADEPRO				= (COMMAND_ID_MIN + 69);  
        public const int COMMAND_ID_USER_FINDPSW					= (COMMAND_ID_MIN + 70);  
        public const int COMMAND_ID_USER_SECURITYCODE			= (COMMAND_ID_MIN + 71);  
        public const int COMMAND_ID_USER_RESERVEPHONE			= (COMMAND_ID_MIN + 72);  
        public const int COMMAND_ID_SET_PTZ						= (COMMAND_ID_MIN + 73);	
        public const int COMMAND_ID_VENCPARAM_REFRESH			= (COMMAND_ID_MIN + 74);
        public const int COMMAND_ID_PLATFORM_GAUGE				= (COMMAND_ID_MIN + 75);
        public const int COMMAND_ID_DETECT_AREA                  = (COMMAND_ID_MIN + 76);
        public const int COMMAND_ID_SELFTEST						= (COMMAND_ID_MIN + 77);
        public const int COMMAND_ID_MAX							= (COMMAND_ID_MIN + 78);
    }

    public class VCA_DEFINE
    {
        public const int VCA_SUSPEND	= 0;
        public const int VCA_RESUME     = 1;
        public const int VCA_SUSPEND_RESULT_SUCCESS	= 1;
        public const int VCA_SUSPEND_RESULT_CONFIGING    = 2;
    }

    public class DOME_PTZ
    {
        public const int DOME_PTZ_TYPE_MIN               = 1;
        public const int DOME_PTZ_TYPE_PRESET_FREEZE_UP  = (DOME_PTZ_TYPE_MIN + 0);
        public const int DOME_PTZ_TYPE_AUTO_FLIP         = (DOME_PTZ_TYPE_MIN + 1);
        public const int DOME_PTZ_TYPE_PRESET_SPEED_LEVE = (DOME_PTZ_TYPE_MIN + 2);
        public const int DOME_PTZ_TYPE_MANUL_SEPPD_LEVEL = (DOME_PTZ_TYPE_MIN + 3);
        public const int DOME_PTZ_TYPE_WAIT_ACT          = (DOME_PTZ_TYPE_MIN + 4);
        public const int DOME_PTZ_TYPE_INFRARED_MODE     = (DOME_PTZ_TYPE_MIN + 5);
        public const int DOME_PTZ_TYPE_SCALE_ZOOM        = (DOME_PTZ_TYPE_MIN + 6);
        public const int DOME_PTZ_TYPE_LINK_DISJUNCTOR   = (DOME_PTZ_TYPE_MIN + 7);
        public const int DOME_PTZ_TYPE_ANTI_SHAKE        = (DOME_PTZ_TYPE_MIN + 8);
        public const int DOME_PTZ_TYPE_MAX               = (DOME_PTZ_TYPE_MIN + 9);
    }

    public class PICTYPE_DEF
    {
        public const int PIC_TYPE_BMP = 1;
        public const int PIC_TYPE_JPG = 2;
    }

}
