using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using PCOConvertDll;
using PCOConvertStructures;
using Image = System.Drawing.Image;


namespace LabProg
{

    public partial class MainWindow
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        private IntPtr _cameraHandle = IntPtr.Zero;
        private IntPtr _convertHandle = IntPtr.Zero;
        private IntPtr _convertDialog = IntPtr.Zero;
        private IntPtr _camDialog = IntPtr.Zero;
        private const int WM_APPp100 = 0x8000 + 100;
        private const int WM_APPp101 = 0x8000 + 101;
        private const int WM_APPp102 = 0x8000 + 102;
        private int _countSaved = 0;
        private int _maxCameraFrame;
        private int _bufwidth = 0, _bufheight = 0;
        private byte[] _imagedata;
        private short _bufnr = -1;

        private PCO_Description _pcoDescr;
        private PCO_Storage _pcoStorage;
        private PCO_Image _pcoImage;
        private PcoCameraType _pcoCameraType;


        private void ConnectToCamera(object sender, RoutedEventArgs e)
        {
            const ushort boardNum = 1;

            _cameraHandle = IntPtr.Zero;
            _convertHandle = IntPtr.Zero;
            _convertDialog = IntPtr.Zero;
            _camDialog = IntPtr.Zero;
            _bufwidth = 0;
            _bufheight = 0;

            // Verify board number validity
            // Open a handle to the camera
            var err = PCO_SDK_LibWrapper.PCO_OpenCamera(ref _cameraHandle, boardNum);
            if (err == 0)
            {
                ushort wrecstate = 0;
                CbStartCamera.IsEnabled = false;
                CbStopCamera.IsEnabled = false;
                CbGetDescription.IsEnabled = true;
                CbOpenCamera.IsEnabled = false;
                CbCloseCamera.IsEnabled = true;

                PCO_SDK_LibWrapper.PCO_GetRecordingState(_cameraHandle, ref wrecstate);
                if (wrecstate != 0)
                    PCO_SDK_LibWrapper.PCO_SetRecordingState(_cameraHandle, 0);

                // buttonOpenCamDialog.Enabled = true;
            }
            else
            {
                PCO_SDK_LibWrapper.PCO_ResetLib();
            }
        }

        private void OnGetDescription(object sender, EventArgs e)
        {
            _pcoCameraType = new PcoCameraType();
            _pcoDescr = new PCO_Description();
            _pcoStorage = new PCO_Storage();
            _pcoImage = new PCO_Image();

            _pcoDescr.wSize = (ushort)Marshal.SizeOf(_pcoDescr);
            _pcoStorage.wSize = (ushort)Marshal.SizeOf(_pcoStorage);
            _pcoImage.wSize = (ushort)Marshal.SizeOf(_pcoImage);

            int err = 0;

            err = PCO_SDK_LibWrapper.PCO_GetCameraDescription(_cameraHandle, ref _pcoDescr);

            err = PCO_SDK_LibWrapper.PCO_GetStorageStruct(_cameraHandle, ref _pcoStorage);

            _pcoImage.strSegment = new PCO_Segment[4];

            _pcoImage.strSegment[0].wSize = (ushort)Marshal.SizeOf(typeof(PCO_Segment));
            _pcoImage.strSegment[1].wSize = (ushort)Marshal.SizeOf(typeof(PCO_Segment));
            _pcoImage.strSegment[2].wSize = (ushort)Marshal.SizeOf(typeof(PCO_Segment));
            _pcoImage.strSegment[3].wSize = (ushort)Marshal.SizeOf(typeof(PCO_Segment));

            err = PCO_SDK_LibWrapper.PCO_GetImageStruct(_cameraHandle, ref _pcoImage);

            ushort usfwsize = (ushort)Marshal.SizeOf(typeof(PcoSc2FirmwareDesc));

            usfwsize = (ushort)Marshal.SizeOf(typeof(PcoFwVers));
            _pcoCameraType.strHardwareVersion.Board = new PcoSc2HardwareDesc[10];
            _pcoCameraType.strFirmwareVersion.Device = new PcoSc2FirmwareDesc[10];
            for (int i = 0; i < 10; i++)
            {
                _pcoCameraType.strHardwareVersion.Board[i].szName = "123456789012345";
                _pcoCameraType.strFirmwareVersion.Device[i].szName = "123456789012345";
            }
            _pcoCameraType.wSize = (ushort)Marshal.SizeOf(_pcoCameraType);

            err = PCO_SDK_LibWrapper.PCO_GetCameraType(_cameraHandle, ref _pcoCameraType);

            byte[] szCameraName;
            szCameraName = new byte[30];
            string cameraname;

            err = PCO_SDK_LibWrapper.PCO_GetCameraName(_cameraHandle, szCameraName, 30);
            cameraname = System.Text.Encoding.Default.GetString(szCameraName);

            Setupconvert();

           CbStartCamera.IsEnabled = true;
            CbStopCamera.IsEnabled = true;
        }

        private void Setupconvert()
        {
            _pcoDescr = new PCO_Description();
            _pcoDescr.wSize = (ushort)Marshal.SizeOf(_pcoDescr);

            PCO_SDK_LibWrapper.PCO_GetCameraDescription(_cameraHandle, ref _pcoDescr);
            var strsensorinf = new PCO_ConvertStructures.PCO_SensorInfo();
            var strDisplay = new PCO_ConvertStructures.PCO_Display();
            strsensorinf.wSize = (ushort)Marshal.SizeOf(strsensorinf);
            strDisplay.wSize = (ushort)Marshal.SizeOf(strDisplay);
            strsensorinf.wDummy = 0;
            strsensorinf.iConversionFactor = 0;
            strsensorinf.iDataBits = _pcoDescr.wDynResDESC;
            strsensorinf.iSensorInfoBits = 1;
            strsensorinf.iDarkOffset = 100;
            strsensorinf.dwzzDummy0 = 0;
            strsensorinf.strColorCoeff.da11 = 1.0;
            strsensorinf.strColorCoeff.da12 = 0.0;
            strsensorinf.strColorCoeff.da13 = 0.0;
            strsensorinf.strColorCoeff.da21 = 0.0;
            strsensorinf.strColorCoeff.da22 = 1.0;
            strsensorinf.strColorCoeff.da23 = 0.0;
            strsensorinf.strColorCoeff.da31 = 0.0;
            strsensorinf.strColorCoeff.da32 = 0.0;
            strsensorinf.strColorCoeff.da33 = 1.0;
            strsensorinf.iCamNum = 0;
            strsensorinf.hCamera = _cameraHandle;

            /* We created a pointer to a convert object here */
            PCO_Convert_LibWrapper.PCO_ConvertCreate(ref _convertHandle, ref strsensorinf, PCO_Convert_LibWrapper.PCO_COLOR_CONVERT);

            var pcoConv = new PCO_ConvertStructures.PCO_Convert(); ;

            pcoConv.wSize = (ushort)Marshal.SizeOf(pcoConv);
            PCO_Convert_LibWrapper.PCO_ConvertGet(_convertHandle, ref pcoConv);
            pcoConv.wSize = (ushort)Marshal.SizeOf(pcoConv);

            var debugIntPtr = _convertHandle;
            var pcoConvertlocal = (PCO_ConvertStructures.PCO_Convert)Marshal.PtrToStructure(debugIntPtr, typeof(PCO_ConvertStructures.PCO_Convert));
        }

        private void OnStartRecord(object sender, EventArgs e)
        {
            uint dwWarn = 0, dwError = 0, dwStatus = 0;
            ushort width = 0;
            ushort height = 0;
            ushort widthmax = 0;
            ushort heightmax = 0;

            // It is recommended to call this function in order to get information about the camera internal state
            PCO_SDK_LibWrapper.PCO_GetCameraHealthStatus(_cameraHandle, ref dwWarn, ref dwError, ref dwStatus);
            PCO_SDK_LibWrapper.PCO_SetTriggerMode(_cameraHandle, 0);
            PCO_SDK_LibWrapper.PCO_ArmCamera(_cameraHandle);
            PCO_SDK_LibWrapper.PCO_GetSizes(_cameraHandle, ref width, ref height, ref widthmax, ref heightmax);
            PCO_SDK_LibWrapper.PCO_CamLinkSetImageParameters(_cameraHandle, (ushort)width, (ushort)height);

            PCO_SDK_LibWrapper.PCO_SetRecordingState(_cameraHandle, 1);
            CbGrabCamera.IsEnabled = true;
            if (_camDialog != IntPtr.Zero)
            {
                PCO_SDK_LibWrapper.PCO_EnableDialogCam(_camDialog, false);
            }
            _cameraTimer.Start();
        }

        private void OnStopRecord(object sender, EventArgs e)
        {
            PCO_SDK_LibWrapper.PCO_SetRecordingState(_cameraHandle, 0);
            CbGrabCamera.IsEnabled = false;
            if (_camDialog != IntPtr.Zero)
            {
                PCO_SDK_LibWrapper.PCO_EnableDialogCam(_camDialog, true);
            }
            PCO_SDK_LibWrapper.PCO_CancelImages(_cameraHandle);
            _cameraTimer.Stop();
        }

        private void OnGrabImage(object sender, EventArgs evt)
        {
            var err = 0;

            ushort width = 0;
            ushort height = 0;
            ushort widthmax = 0;
            ushort heightmax = 0;
            var ishift = 16 - _pcoDescr.wDynResDESC;
            var ipadd = width / 4;
            var iconvertcol = _pcoDescr.wColorPatternDESC / 0x1000;
            int max;
            int min;
            ipadd *= 4;
            ipadd = width - ipadd;

            PCO_SDK_LibWrapper.PCO_GetSizes(_cameraHandle, ref width, ref height, ref widthmax, ref heightmax);
            var size = width * height * 2;

            var buf = UIntPtr.Zero;
            var evhandle = IntPtr.Zero;
            if ((_bufwidth != width) || (_bufheight != height))
            {
                if (_bufnr != -1)
                {
                    PCO_SDK_LibWrapper.PCO_FreeBuffer(_cameraHandle, _bufnr);
                }
                _bufnr = -1;
                _imagedata = new byte[(width + ipadd) * height * 3];

                err = PCO_SDK_LibWrapper.PCO_AllocateBuffer(_cameraHandle, ref _bufnr, size, ref buf, ref evhandle);
                if (err == 0)
                {
                    _bufwidth = width;
                    _bufheight = height;
                }
            }
            else
                err = PCO_SDK_LibWrapper.PCO_GetBuffer(_cameraHandle, _bufnr, ref buf, ref evhandle);

            if ((_convertDialog == IntPtr.Zero) && (_convertHandle != IntPtr.Zero))
            {
                var hdl = new WindowInteropHelper(this).Handle;
                PCO_Convert_LibWrapper.PCO_OpenConvertDialog(ref _convertDialog, hdl, "Convert Dialog", WM_APPp100, _convertHandle, 500, 100);
            }

            //Mandatory for Cameralink and GigE. Don't care for all other interfaces, so leave it intact here.

            err = PCO_SDK_LibWrapper.PCO_AddBufferEx(_cameraHandle, 0, 0, _bufnr, (UInt16)width, (UInt16)height, (UInt16)_pcoDescr.wDynResDESC);

            var bImageIsOk = false;
            var res = WaitForSingleObject(evhandle, 3000);
            if (res == 0)
            {
                bImageIsOk = true;
            }
            if (!bImageIsOk)
                return;
            // End Event Block

            unsafe
            {
                var bufi = (short*)buf.ToPointer();
                max = 0;
                min = 65535;
                for (var i = 20 * width; i < height * width; i++)
                {
                    bufi[i] >>= ishift;
                    if (bufi[i] > max)
                        max = bufi[i];
                    if (bufi[i] < min)
                        min = bufi[i];
                }
                if (max <= min)
                    max = min + 1;
            }
            PCO_Convert_LibWrapper.PCO_Convert16TOCOL(_convertHandle, 0, iconvertcol, width, height,
                buf, _imagedata);

            if ((_convertDialog != IntPtr.Zero) && (_convertHandle != IntPtr.Zero))
            {
                PCO_Convert_LibWrapper.PCO_SetDataToDialog(_convertDialog, width, height, buf, _imagedata);
            }

            {
                var strDisplay = new PCO_ConvertStructures.PCO_Display(1)
                {
                    wSize = (ushort)Marshal.SizeOf(typeof(PCO_ConvertStructures.PCO_Display))
                };

                PCO_Convert_LibWrapper.PCO_ConvertGetDisplay(_convertHandle, ref strDisplay);
                strDisplay.iScale_min = min;
                strDisplay.iScale_max = max;

                PCO_Convert_LibWrapper.PCO_ConvertSetDisplay(_convertHandle, ref strDisplay);
                PCO_Convert_LibWrapper.PCO_SetConvertDialog(_convertDialog, _convertHandle);
            }

            var imagebmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            var dimension = new Rectangle(0, 0, imagebmp.Width, imagebmp.Height);
            var picData = imagebmp.LockBits(dimension, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);//Format24bppRgb
            var pixelStartAddress = picData.Scan0;

            //Copy the pixel data into the bitmap structure
            Marshal.Copy(_imagedata, 0, pixelStartAddress, _imagedata.Length);

            imagebmp.UnlockBits(picData);
            var isOnSave = false;
            if (Dispatcher == null) return;
            Dispatcher.Invoke(() => CbSaveCameraImage.IsChecked != null && (isOnSave = CbSaveCameraImage.IsChecked.Value));
            if (isOnSave)
            {
                var propPath = "";
                var prefix = "00";
                Dispatcher.Invoke(() =>
                {
                    propPath = TbSaveCamPath.Text;
                    prefix = TbSaveCamPrefix.Text;
                });
                if (propPath.Length <= 0)
                {
                    propPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }

                var dt = DateTime.Now;
                var fName =
                    $"{propPath}\\{prefix}-{dt.Month}-{dt.Day}-{dt.Hour}-{dt.Minute}-{dt.Second}_{dt.Millisecond}.jpg";
                if (!Properties.Settings.Default.CameraMaxFrameOn)
                {
                    imagebmp.Save(fName, ImageFormat.Jpeg);
                }
                else if (_countSaved <= _maxCameraFrame)
                {
                    imagebmp.Save(fName, ImageFormat.Jpeg);
                    Dispatcher.Invoke(() => tbCurrentFrame.Text = _countSaved.ToString());
                    _countSaved++;
                }
            }

            Dispatcher.Invoke(() =>
            {
                var bmpImage = BitmapToImageSource(imagebmp);
                PictureBox1.Source = null;
                PictureBox1.Source = bmpImage;
                LogBox.Items.Insert(0,
                    new LogBoxItem {Dt = DateTime.Now, LogText = @"Получено изображение с камеры"});
            });
        }

        private static BitmapImage BitmapToImageSource(Image bitmap)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        private void OnChangeCameraPath(object sender, EventArgs e)
        {
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = @"Меняем путь сохранения" });
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                    ((TextBox)sender).Text = dialog.SelectedPath;
            }
        }

        private void OnChangeCameraInterval(object sender, EventArgs e)
        {
            _cameraTimer.Interval = GetCameraTimerInterval();
        }

        private void OnTimerTeak(object sender, EventArgs e)
        {
            var isReady = false;
            Dispatcher?.Invoke((Action) (() =>
            {
                Debug.Assert(CbGrabCamera.IsChecked != null, "cbGrabCamera.IsChecked != null");
                isReady = CbGrabCamera.IsChecked.Value;
            }));
            if (isReady)
            {
                OnGrabImage(sender, e);
            }
        }

        private void OnChangeFrameMaxCount()
        {
            var str = tbFrameCount.Text;
            var res = int.TryParse(str, out var fc);
            _maxCameraFrame = res ? fc : 100;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PcoSc2HardwareDesc
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string szName;               // string with board name
        private readonly ushort wBatchNo;             // production batch no
        private readonly ushort wRevision;            // use range 0 to 99
        private readonly ushort wVariant;             // variant    // 22
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        private readonly ushort[] ZZwDummy;             //            // 62
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PcoSc2FirmwareDesc
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
    public struct PcoHwVers
    {
        private readonly ushort BoardNum;       // number of devices
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10)]
        public PcoSc2HardwareDesc[] Board;// 622
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PcoFwVers
    {
        private readonly ushort DeviceNum;       // number of devices
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10)]
        public PcoSc2FirmwareDesc[] Device;// 642
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PcoCameraType
    {
        public ushort wSize;                   // Sizeof this struct
        public ushort wCamType;                // Camera type
        public ushort wCamSubType;             // Camera sub type
        public ushort ZZwAlignDummy1;
        public UInt32 dwSerialNumber;          // Serial number of camera // 12
        public UInt32 dwHWVersion;             // Hardware version number
        public UInt32 dwFWVersion;             // Firmware version number
        public ushort wInterfaceType;          // Interface type          // 22
        public PcoHwVers strHardwareVersion;      // Hardware versions of all boards // 644
        public PcoFwVers strFirmwareVersion;      // Firmware versions of all devices // 1286
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