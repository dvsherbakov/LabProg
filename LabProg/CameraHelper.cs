using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace LabProg
{

    public partial class MainWindow
    {
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PCO_SC2_Hardware_DESC
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public String szName;               // string with board name
        public ushort wBatchNo;             // production batch no
        public ushort wRevision;            // use range 0 to 99
        public ushort wVariant;             // variant    // 22
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public ushort[] ZZwDummy;             //            // 62
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PCO_SC2_Firmware_DESC
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public String szName;                // string with device name
        public byte bMinorRev;              // use range 0 to 99
        public byte bMajorRev;              // use range 0 to 255
        public ushort wVariant;             // variant    // 20
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public ushort[] ZZwDummy;             //            // 64
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PCO_HW_Vers
    {
        public ushort BoardNum;       // number of devices
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10)]
        public PCO_SC2_Hardware_DESC[] Board;// 622
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PCO_FW_Vers
    {
        public ushort DeviceNum;       // number of devices
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10)]
        public PCO_SC2_Firmware_DESC[] Device;// 642
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PCO_CameraType
    {
        public ushort wSize;                   // Sizeof this struct
        public ushort wCamType;                // Camera type
        public ushort wCamSubType;             // Camera sub type
        public ushort ZZwAlignDummy1;
        public UInt32 dwSerialNumber;          // Serial number of camera // 12
        public UInt32 dwHWVersion;             // Hardware version number
        public UInt32 dwFWVersion;             // Firmware version number
        public ushort wInterfaceType;          // Interface type          // 22
        public PCO_HW_Vers strHardwareVersion;      // Hardware versions of all boards // 644
        public PCO_FW_Vers strFirmwareVersion;      // Firmware versions of all devices // 1286
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 39)]
        public ushort[] ZZwDummy;                                          // 1364
    };


    [StructLayout(LayoutKind.Sequential)]
    public struct PCO_Description
    {
        public ushort wSize;                   // Sizeof this struct
        public ushort wSensorTypeDESC;         // Sensor type
        public ushort wSensorSubTypeDESC;      // Sensor subtype
        public ushort wMaxHorzResStdDESC;      // Maxmimum horz. resolution in std.mode
        public ushort wMaxVertResStdDESC;      // Maxmimum vert. resolution in std.mode
        public ushort wMaxHorzResExtDESC;      // Maxmimum horz. resolution in ext.mode
        public ushort wMaxVertResExtDESC;      // Maxmimum vert. resolution in ext.mode
        public ushort wDynResDESC;             // Dynamic resolution of ADC in bit
        public ushort wMaxBinHorzDESC;         // Maxmimum horz. binning
        public ushort wBinHorzSteppingDESC;    // Horz. bin. stepping (0:bin, 1:lin)
        public ushort wMaxBinVertDESC;         // Maxmimum vert. binning
        public ushort wBinVertSteppingDESC;    // Vert. bin. stepping (0:bin, 1:lin)
        public ushort wRoiHorStepsDESC;        // Minimum granularity of ROI in pixels
        public ushort wRoiVertStepsDESC;       // Minimum granularity of ROI in pixels
        public ushort wNumADCsDESC;            // Number of ADCs in system
        public ushort ZZwAlignDummy1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] dwPixelRateDESC;       // Possible pixelrate in Hz
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public uint[] ZZdwDummypr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] wConvFactDESC;       // Possible conversion factor in e/cnt
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public ushort[] ZZdwDummycv;
        public ushort wIRDESC;                 // IR enhancment possibility
        public ushort ZZwAlignDummy2;
        public uint dwMinDelayDESC;          // Minimum delay time in ns
        public uint dwMaxDelayDESC;          // Maximum delay time in ms
        public uint dwMinDelayStepDESC;      // Minimum stepping of delay time in ns
        public uint dwMinExposureDESC;       // Minimum exposure time in ns
        public uint dwMaxExposureDESC;       // Maximum exposure time in ms
        public uint dwMinExposureStepDESC;   // Minimum stepping of exposure time in ns
        public uint dwMinDelayIRDESC;        // Minimum delay time in ns
        public uint dwMaxDelayIRDESC;        // Maximum delay time in ms
        public uint dwMinExposureIRDESC;     // Minimum exposure time in ns
        public uint dwMaxExposureIRDESC;     // Maximum exposure time in ms
        public ushort wTimeTableDESC;          // Timetable for exp/del possibility
        public ushort wDoubleImageDESC;        // Double image mode possibility
        public short sMinCoolSetDESC;         // Minimum value for cooling
        public short sMaxCoolSetDESC;         // Maximum value for cooling
        public short sDefaultCoolSetDESC;     // Default value for cooling
        public ushort wPowerDownModeDESC;      // Power down mode possibility 
        public ushort wOffsetRegulationDESC;   // Offset regulation possibility
        public ushort wColorPatternDESC;       // Color pattern of color chip
                                               // four nibbles (0,1,2,3) in ushort 
                                               //  ----------------- 
                                               //  | 3 | 2 | 1 | 0 |
                                               //  ----------------- 
                                               //   
                                               // describe row,column  2,2 2,1 1,2 1,1
                                               // 
                                               //   column1 column2
                                               //  ----------------- 
                                               //  |       |       |
                                               //  |   0   |   1   |   row1
                                               //  |       |       |
                                               //  -----------------
                                               //  |       |       |
                                               //  |   2   |   3   |   row2
                                               //  |       |       |
                                               //  -----------------
                                               // 
        public ushort wPatternTypeDESC;        // Pattern type of color chip
                                               // 0: Bayer pattern RGB
                                               // 1: Bayer pattern CMY
        public ushort wDSNUCorrectionModeDESC; // DSNU correction mode possibility
        public ushort ZZwAlignDummy3;          //
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public UInt32[] dwReservedDESC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public UInt32[] ZZdwDummy;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PCO_Description2
    {
        public ushort wSize;                   // Sizeof this struct
        public ushort ZZwAlignDummy1;
        public uint dwMinPeriodicalTimeDESC2;// Minimum periodical time tp in (nsec)
        public uint dwMaxPeriodicalTimeDESC2;// Maximum periodical time tp in (msec)        (12)
        public uint dwMinPeriodicalConditionDESC2;// System imanent condition in (nsec)
                                                  // tp - (td + te) must be equal or longer than
                                                  // dwMinPeriodicalCondition
        public uint dwMaxNumberOfExposuresDESC2;// Maximum number of exporures possible        (20)
        public int lMinMonitorSignalOffsetDESC2;// Minimum monitor signal offset tm in (nsec)
                                                // if(td + tstd) > dwMinMon.)
                                                //   tm must not be longer than dwMinMon
                                                // else
                                                //   tm must not be longer than td + tstd
        public uint dwMaxMonitorSignalOffsetDESC2;// Maximum -''- in (nsec)                      
        public uint dwMinPeriodicalStepDESC2;// Minimum step for periodical time in (nsec)  (32)
        public uint dwStartTimeDelayDESC2; // Minimum monitor signal offset tstd in (nsec)
                                           // see condition at dwMinMonitorSignalOffset
        public uint dwMinMonitorStepDESC2; // Minimum step for monitor time in (nsec)     (40)
        public uint dwMinDelayModDESC2;    // Minimum delay time for modulate mode in (nsec)
        public uint dwMaxDelayModDESC2;    // Maximum delay time for modulate mode in (msec)
        public uint dwMinDelayStepModDESC2;// Minimum delay time step for modulate mode in (nsec)(52)
        public uint dwMinExposureModDESC2; // Minimum exposure time for modulate mode in (nsec)
        public uint dwMaxExposureModDESC2; // Maximum exposure time for modulate mode in (msec)(60)
        public uint dwMinExposureStepModDESC2;// Minimum exposure time step for modulate mode in (nsec)
        public uint dwModulateCapsDESC2;   // Modulate capabilities descriptor
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public UInt32[] dwReservedDESC;                                                    //(132)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
        public UInt32[] ZZdwDummy;                                                         // 296};
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PCO_DescriptionEx
    {
        public ushort wSize;                   // Sizeof this struct
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PCO_Storage
    {
        public ushort wSize;                   // Sizeof this struct
        public ushort ZZwAlignDummy1;
        public UInt32 dwRamSize;               // Size of camera ram in pages
        public ushort wPageSize;               // Size of one page in pixel       // 10
        public ushort ZZwAlignDummy4;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt32[] dwRamSegSize;          // Size of ram segment 1-4 in pages // 28
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public UInt32[] ZZdwDummyrs;                                                // 108
        public ushort wActSeg;                 // no. (0 .. 3) of active segment  // 110
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 39)]
        public ushort[] ZZwDummy;                                     // 188
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PCO_Segment
    {
        public ushort wSize;                   // Sizeof this struct
        public ushort wXRes;                   // Res. h. = resulting horz.res.(sensor resolution, ROI, binning)
        public ushort wYRes;                   // Res. v. = resulting vert.res.(sensor resolution, ROI, binning)
        public ushort wBinHorz;                // Horizontal binning
        public ushort wBinVert;                // Vertical binning                // 10
        public ushort wRoiX0;                  // Roi upper left x
        public ushort wRoiY0;                  // Roi upper left y
        public ushort wRoiX1;                  // Roi lower right x
        public ushort wRoiY1;                  // Roi lower right y
        public ushort ZZwAlignDummy1;                                             // 20
        public UInt32 dwValidImageCnt;         // no. of valid images in segment
        public UInt32 dwMaxImageCnt;           // maximum no. of images in segment // 28
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public ushort[] ZZwDummy;                                         // 188
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PCO_Image
    {
        public ushort wSize;      // Sizeof this struct
        public ushort ZZwAlignDummy1;                                    // 4
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
        public PCO_Segment[] strSegment;// Segment info                      // 436
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16)]
        public PCO_Segment[] ZZstrDummySeg;// Segment info dummy            // 2164
        public ushort wBitAlignment;// Bitalignment during readout. 0: MSB, 1: LSB aligned
        public ushort wHotPixelCorrectionMode;   // Correction mode for hotpixel
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 38)]
        public ushort[] ZZwDummy;                                              // 2244
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PCO_OpenStruct
    {
        public ushort wSize;        // Sizeof this struct
        public ushort wInterfaceType;
        public ushort wCameraNumber;
        public ushort wCameraNumAtInterface; // Current number of camera at the interface
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] wOpenFlags;   // [0]: moved to dwnext to position 0xFF00
                                      // [1]: moved to dwnext to position 0xFFFF0000
                                      // [2]: Bit0: PCO_OPENFLAG_GENERIC_IS_CAMLINK
                                      //            Set this bit in case of a generic Cameralink interface
                                      //            This enables the import of the additional three camera-
                                      //            link interface functions.

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
        public UInt32[] dwOpenFlags;// [0]-[4]: moved to strCLOpen.dummy[0]-[4]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
        public IntPtr[] wOpenPtr;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
        public ushort[] zzwDummy;     // 88 - 64bit: 112
    };
}