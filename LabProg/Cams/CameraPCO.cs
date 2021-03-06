﻿using System;
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


namespace LabProg
{

    public partial class MainWindow
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        private IntPtr f_CameraHandle = IntPtr.Zero;
        private IntPtr f_ConvertHandle = IntPtr.Zero;
        IntPtr convertDialog = IntPtr.Zero;
        IntPtr camDialog = IntPtr.Zero;
        private const int WM_APPp100 = 0x8000 + 100;
        private const int WM_APPp101 = 0x8000 + 101;
        private const int WM_APPp102 = 0x8000 + 102;
        private int countSaved = 0;
        private int MaxCameraFrame;
        int bufwidth = 0, bufheight = 0;
        byte[] imagedata;
        short bufnr = -1;

        PCO_Description pcoDescr;
        PCO_Storage pcoStorage;
        PCO_Image pcoImage;
        PCO_CameraType pcoCameraType;


        private void ConnectToCamera(object sender, RoutedEventArgs e)
        {
            const ushort boardNum = 1;

            f_CameraHandle = IntPtr.Zero;
            f_ConvertHandle = IntPtr.Zero;
            convertDialog = IntPtr.Zero;
            camDialog = IntPtr.Zero;
            bufwidth = 0;
            bufheight = 0;

            // Verify board number validity
            // Open a handle to the camera
            try
            {
                var err = PCO_SDK_LibWrapper.PCO_OpenCamera(ref f_CameraHandle, boardNum);
                if (err == 0)
                {
                    ushort wrecState = 0;
                    cbStartCamera.IsEnabled = false;
                    cbStopCamera.IsEnabled = false;
                    cbGetDescription.IsEnabled = true;
                    cbOpenCamera.IsEnabled = false;
                    cbCloseCamera.IsEnabled = true;

                    PCO_SDK_LibWrapper.PCO_GetRecordingState(f_CameraHandle, ref wrecState);
                    if (wrecState != 0)
                        PCO_SDK_LibWrapper.PCO_SetRecordingState(f_CameraHandle, 0);

                    // buttonOpenCamDialog.Enabled = true;
                }
                else
                {
                    err = PCO_SDK_LibWrapper.PCO_ResetLib();
                }
            }
            catch (Exception ex)
            {
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = $"Ошибка соединения с камерой {ex}" });
            }
        }

        private void OnGetDescription(object sender, System.EventArgs e)
        {
            pcoCameraType = new PCO_CameraType();
            pcoDescr = new PCO_Description();
            pcoStorage = new PCO_Storage();
            pcoImage = new PCO_Image();

            pcoDescr.wSize = (ushort)Marshal.SizeOf(pcoDescr);
            pcoStorage.wSize = (ushort)Marshal.SizeOf(pcoStorage);
            pcoImage.wSize = (ushort)Marshal.SizeOf(pcoImage);
            var err = PCO_SDK_LibWrapper.PCO_GetCameraDescription(f_CameraHandle, ref pcoDescr);
            err = PCO_SDK_LibWrapper.PCO_GetStorageStruct(f_CameraHandle, ref pcoStorage);

            pcoImage.strSegment = new PCO_Segment[4];

            pcoImage.strSegment[0].wSize = (ushort)Marshal.SizeOf(typeof(PCO_Segment));
            pcoImage.strSegment[1].wSize = (ushort)Marshal.SizeOf(typeof(PCO_Segment));
            pcoImage.strSegment[2].wSize = (ushort)Marshal.SizeOf(typeof(PCO_Segment));
            pcoImage.strSegment[3].wSize = (ushort)Marshal.SizeOf(typeof(PCO_Segment));

            err = PCO_SDK_LibWrapper.PCO_GetImageStruct(f_CameraHandle, ref pcoImage);

            var usfwsize = (ushort)Marshal.SizeOf(typeof(PCO_SC2_Firmware_DESC));

            usfwsize = (ushort)Marshal.SizeOf(typeof(PCO_FW_Vers));
            pcoCameraType.strHardwareVersion.Board = new PCO_SC2_Hardware_DESC[10];
            pcoCameraType.strFirmwareVersion.Device = new PCO_SC2_Firmware_DESC[10];
            for (var i = 0; i < 10; i++)
            {
                pcoCameraType.strHardwareVersion.Board[i].szName = "123456789012345";
                pcoCameraType.strFirmwareVersion.Device[i].szName = "123456789012345";
            }
            pcoCameraType.wSize = (ushort)Marshal.SizeOf(pcoCameraType);

            err = PCO_SDK_LibWrapper.PCO_GetCameraType(f_CameraHandle, ref pcoCameraType);

            var szCameraName = new byte[30];

            err = PCO_SDK_LibWrapper.PCO_GetCameraName(f_CameraHandle, szCameraName, 30);

            SetupConvert();

           cbStartCamera.IsEnabled = true;
            cbStopCamera.IsEnabled = true;
        }

        private void SetupConvert()
        {
            pcoDescr = new PCO_Description();
            pcoDescr.wSize = (ushort)Marshal.SizeOf(pcoDescr);
            int err = 0;

            err = PCO_SDK_LibWrapper.PCO_GetCameraDescription(f_CameraHandle, ref pcoDescr);
            PCO_ConvertStructures.PCO_SensorInfo strsensorinf = new PCO_ConvertStructures.PCO_SensorInfo();
            PCO_ConvertStructures.PCO_Display strDisplay = new PCO_ConvertStructures.PCO_Display();
            strsensorinf.wSize = (ushort)Marshal.SizeOf(strsensorinf);
            strDisplay.wSize = (ushort)Marshal.SizeOf(strDisplay);
            strsensorinf.wDummy = 0;
            strsensorinf.iConversionFactor = 0;
            strsensorinf.iDataBits = pcoDescr.wDynResDESC;
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
            strsensorinf.hCamera = f_CameraHandle;

            int errorCode;
            /* We created a pointer to a convert object here */
            errorCode = PCO_Convert_LibWrapper.PCO_ConvertCreate(ref f_ConvertHandle, ref strsensorinf, PCO_Convert_LibWrapper.PCO_COLOR_CONVERT);

            PCO_ConvertStructures.PCO_Convert pcoConv = new PCO_ConvertStructures.PCO_Convert(); ;

            pcoConv.wSize = (ushort)Marshal.SizeOf(pcoConv);
            errorCode = PCOConvertDll.PCO_Convert_LibWrapper.PCO_ConvertGet(f_ConvertHandle, ref pcoConv);
            pcoConv.wSize = (ushort)Marshal.SizeOf(pcoConv);

            IntPtr debugIntPtr = f_ConvertHandle;
            PCO_ConvertStructures.PCO_Convert pcoConvertlocal = (PCO_ConvertStructures.PCO_Convert)Marshal.PtrToStructure(debugIntPtr, typeof(PCO_ConvertStructures.PCO_Convert));
        }

        private void OnStartRecord(object sender, EventArgs e)
        {
            int err;
            uint dwWarn = 0, dwError = 0, dwStatus = 0;
            ushort width = 0;
            ushort height = 0;
            ushort widthmax = 0;
            ushort heightmax = 0;

            // It is recommended to call this function in order to get information about the camera internal state
            err = PCO_SDK_LibWrapper.PCO_GetCameraHealthStatus(f_CameraHandle, ref dwWarn, ref dwError, ref dwStatus);
            PCO_SDK_LibWrapper.PCO_SetTriggerMode(f_CameraHandle, 0);
            err = PCO_SDK_LibWrapper.PCO_ArmCamera(f_CameraHandle);
            err = PCO_SDK_LibWrapper.PCO_GetSizes(f_CameraHandle, ref width, ref height, ref widthmax, ref heightmax);
            err = PCO_SDK_LibWrapper.PCO_CamLinkSetImageParameters(f_CameraHandle, (UInt16)width, (UInt16)height);

            err = PCO_SDK_LibWrapper.PCO_SetRecordingState(f_CameraHandle, 1);
            cbGrabCamera.IsEnabled = true;
            if (camDialog != IntPtr.Zero)
            {
                PCO_SDK_LibWrapper.PCO_EnableDialogCam(camDialog, false);
            }
            f_CameraTimer.Start();
        }

        private void OnStopRecord(object sender, EventArgs e)
        {
            int err;
            err = PCO_SDK_LibWrapper.PCO_SetRecordingState(f_CameraHandle, 0);
            cbGrabCamera.IsEnabled = false;
            if (camDialog != IntPtr.Zero)
            {
                PCO_SDK_LibWrapper.PCO_EnableDialogCam(camDialog, true);
            }
            err = PCO_SDK_LibWrapper.PCO_CancelImages(f_CameraHandle);
            f_CameraTimer.Stop();
        }

        private void OnGrabImage(object sender, EventArgs evt)
        {
            int err = 0;
            int size;
            System.IntPtr evhandle;
            Bitmap imagebmp;
            UIntPtr buf;
            bool bauto = true;              // set this to true to get auto min max

            UInt16 width = 0;
            UInt16 height = 0;
            UInt16 widthmax = 0;
            UInt16 heightmax = 0;
            int ishift = 16 - pcoDescr.wDynResDESC;
            int ipadd = width / 4;
            int iconvertcol = pcoDescr.wColorPatternDESC / 0x1000;
            int max;
            int min;
            ipadd *= 4;
            ipadd = width - ipadd;

            err = PCO_SDK_LibWrapper.PCO_GetSizes(f_CameraHandle, ref width, ref height, ref widthmax, ref heightmax);
            size = width * height * 2;

            buf = UIntPtr.Zero;
            evhandle = IntPtr.Zero;
            if ((bufwidth != width) || (bufheight != height))
            {
                if (bufnr != -1)
                {
                    PCO_SDK_LibWrapper.PCO_FreeBuffer(f_CameraHandle, bufnr);
                }
                bufnr = -1;
                imagedata = new byte[(width + ipadd) * height * 3];

                err = PCO_SDK_LibWrapper.PCO_AllocateBuffer(f_CameraHandle, ref bufnr, size, ref buf, ref evhandle);
                if (err == 0)
                {
                    bufwidth = width;
                    bufheight = height;
                }
            }
            else
                err = PCO_SDK_LibWrapper.PCO_GetBuffer(f_CameraHandle, bufnr, ref buf, ref evhandle);

            if ((convertDialog == IntPtr.Zero) && (f_ConvertHandle != IntPtr.Zero))
            {
                var hdl = new WindowInteropHelper(this).Handle;
                PCO_Convert_LibWrapper.PCO_OpenConvertDialog(ref convertDialog, hdl, "Convert Dialog", WM_APPp100, f_ConvertHandle, 500, 100);
            }

            //Mandatory for Cameralink and GigE. Don't care for all other interfaces, so leave it intact here.

            err = PCO_SDK_LibWrapper.PCO_AddBufferEx(f_CameraHandle, 0, 0, bufnr, (UInt16)width, (UInt16)height, (UInt16)pcoDescr.wDynResDESC);

            bool bImageIsOk = false;
            uint res = WaitForSingleObject(evhandle, 3000);
            if (res == 0)
            {
                bImageIsOk = true;
            }
            if (!bImageIsOk)
                return;
            // End Event Block

            unsafe
            {
                short* bufi = (short*)buf.ToPointer();
                max = 0;
                min = 65535;
                for (int i = 20 * width; i < height * width; i++)
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
            PCO_Convert_LibWrapper.PCO_Convert16TOCOL(f_ConvertHandle, 0, iconvertcol, width, height,
                buf, imagedata);

            if ((convertDialog != IntPtr.Zero) && (f_ConvertHandle != IntPtr.Zero))
            {
                PCO_Convert_LibWrapper.PCO_SetDataToDialog(convertDialog, width, height, buf, imagedata);
            }

            if (bauto)
            {
                PCO_ConvertStructures.PCO_Display strDisplay = new PCO_ConvertStructures.PCO_Display(1)
                {
                    wSize = (ushort)Marshal.SizeOf(typeof(PCO_ConvertStructures.PCO_Display))
                };

                err = PCO_Convert_LibWrapper.PCO_ConvertGetDisplay(f_ConvertHandle, ref strDisplay);
                strDisplay.iScale_min = min;
                strDisplay.iScale_max = max;

                err = PCO_Convert_LibWrapper.PCO_ConvertSetDisplay(f_ConvertHandle, ref strDisplay);
                err = PCO_Convert_LibWrapper.PCO_SetConvertDialog(convertDialog, f_ConvertHandle);
            }
            Dispatcher.Invoke(() => Debug.WriteLine(CbPixelFormatConv.SelectedValue));
            imagebmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);//Format24bppRgb
            Rectangle dimension = new Rectangle(0, 0, imagebmp.Width, imagebmp.Height);
            BitmapData picData = imagebmp.LockBits(dimension, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);//Format24bppRgb
            IntPtr pixelStartAddress = picData.Scan0;

            //Copy the pixel data into the bitmap structure
            Marshal.Copy(imagedata, 0, pixelStartAddress, imagedata.Length);

            imagebmp.UnlockBits(picData);
            bool isOnSave = false;
            Dispatcher.Invoke(() => isOnSave = cbSaveCameraImage.IsChecked.Value);
            if (isOnSave)
            {
                string propPath = "";
                string prefix = "00";
                Dispatcher.Invoke(() =>
                {
                    propPath = tbSaveCamPath.Text;
                    prefix = tbSaveCamPrefix.Text; 
                });
                if (propPath.Length<=0)
                {
                    propPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                var dt = DateTime.Now;
                var fName = $"{propPath}\\{prefix}-{dt.Month}-{dt.Day}-{dt.Hour}-{dt.Minute}-{dt.Second}_{dt.Millisecond}.jpg";
                if (!Properties.Settings.Default.CameraMaxFrameOn)
                {
                    imagebmp.Save(fName, ImageFormat.Jpeg);
                } else if (countSaved<=MaxCameraFrame)
                {
                    imagebmp.Save(fName, ImageFormat.Jpeg);
                    Dispatcher.Invoke(() => tbCurrentFrame.Text = countSaved.ToString());
                    countSaved++;
                }
            }
           
            Dispatcher.Invoke(() =>
            {
                BitmapImage bmpImage = BitmapToImageSource(imagebmp);
                PictureBox1.Source = null;
                PictureBox1.Source = bmpImage;
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Получено изображение с камеры" });
            });
            
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        private void OnChangeCameraPath(object sender, EventArgs e)
        {
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Меняем путь сохранения" });
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                    ((TextBox)sender).Text = dialog.SelectedPath;
            }
        }

        private void OnChangeCameraInterval(object sender, EventArgs e)
        {
            f_CameraTimer.Interval = GetCameraTimerInterval();
        }

        private void OnTimerTeak(object sender, EventArgs e)
        {
            var isReady = false;
            Dispatcher.Invoke(() => isReady = cbGrabCamera.IsChecked.Value);
            if (isReady)
            {
                OnGrabImage(sender, e);
            }
        }

        private void OnChangeFrameMaxCount(object sender, EventArgs e)
        {
            var str = tbFrameCount.Text;
            var res = int.TryParse(str, out var fc);
            MaxCameraFrame = res ? fc : 100;
        }

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