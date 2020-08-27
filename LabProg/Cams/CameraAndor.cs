using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
//using ATMCD64CS;

namespace LabProg.Cams
{
    public class CameraAndor
    {
        //private readonly AndorSDK Api;
        private uint f_error =0;
        private readonly List<string> ErrorList = new List<string>();
        private uint ErrorValue { get => f_error;
        set {
                f_error = value;
                if (f_error != 20002)
                {
                    ErrorList.Add(AndorStatusMessages.Messages[f_error]);

                }
            }
        }
        private readonly int f_X = 0;
        private readonly int f_Y = 0;
        public int XResolution { get => f_X; }
        public int YResolution { get => f_Y; }
        private readonly int f_buffSize;
        public int BufSize { get => f_buffSize; }
        //private AndorSDK.AndorCapabilities Capabilities;
        private readonly string f_headModel = "";
        public string HeadModel { get => f_headModel; }
        private readonly float f_AccumTime = 0.0f;
        private readonly float f_KineticTime = 0.0f;
        private float f_exposure = 0.01f;
        public float Exposure
        {
            get => f_exposure;
            set
            {
                f_exposure = value;
                //SetExposure(f_exposure);
            }
        }
        private readonly System.Timers.Timer f_seriesTimer;

        public CameraAndor()
        {
            //Api = new AndorSDK();
            //Capabilities = new AndorSDK.AndorCapabilities();
            //var CapabilitiesSize = Marshal.SizeOf(Capabilities);
            //Capabilities.ulSize = (uint)CapabilitiesSize;
            var curDir = Environment.CurrentDirectory;

            //ErrorValue = Api.Initialize(curDir);
            //ErrorValue = Api.GetCapabilities(ref Capabilities);
            //ErrorValue = Api.GetHeadModel(ref f_headModel);
            //ErrorValue = Api.GetDetector(ref f_X, ref f_Y);
            //SetImageMode();
            //SetFasterSpeed();
            //if ((Capabilities.ulSetFunctions & AndorSDK.AC_SETFUNCTION_BASELINECLAMP) == AndorSDK.AC_SETFUNCTION_BASELINECLAMP)
            //{
            //    ErrorValue = Api.SetBaselineClamp(1);
            //}
            //ErrorValue = Api.GetAcquisitionTimings(ref f_exposure, ref f_AccumTime, ref f_KineticTime);
            //SetExposure(0.01f);
            //ErrorValue = Api.SetImage(1, 1, 1, f_X, 1, f_Y);
            //ErrorValue = Api.SetShutter(1, 1, 0, 0);
            //f_buffSize = f_X * f_Y;
        }

        public float Tempereture
        {
            get
            {
                float temperature = -99.0f;
                //ErrorValue = Api.GetTemperatureF(ref temperature);
                return temperature;
            }
        }

        public void SetImageMode()
        {
           // ErrorValue = Api.SetReadMode(4);
        }

        public void SetFasterSpeed()
        {
            int vsNumber = 0; float speed = 0.0f;
            //ErrorValue = Api.GetFastestRecommendedVSSpeed(ref vsNumber, ref speed);
            int nAD = 0;
            //ErrorValue = Api.GetNumberADChannels(ref nAD);
            float sTemp = 0.0f;
            int hNumber = 0, ADnumber = 0;

            for (int iAD = 0; iAD < nAD; iAD++)
            {
                int index = 0;
               // Api.GetNumberHSSpeeds(iAD, 0, ref index);
                for (int iSpeed = 0; iSpeed < index; iSpeed++)
                {
                    //Api.GetHSSpeed(iAD, 0, iSpeed, ref speed);
                    if (sTemp < speed)
                    {
                        sTemp = speed;
                        hNumber = iSpeed;
                        ADnumber = iAD;
                    }
                }
            }
            //ErrorValue = Api.SetADChannel(ADnumber);
            //ErrorValue = Api.SetHSSpeed(0, hNumber);
        }
        //private void SetExposure(float exposure) => Api.SetExposureTime(exposure);

        //public void SetInternalTriggerMode() => Api.SetTriggerMode(0);
        //public void SetSoftwareTriggerMode() => Api.SetTriggerMode(10);

        public ImgBufferItem OnceBuffer
        {
            get
            {
                //ErrorValue = Api.SetAcquisitionMode(1); //Тип серии 1 - одиночных снимок, 5 - снимать до остановки
                //ErrorValue = Api.StartAcquisition();
                //ErrorValue = Api.WaitForAcquisition();
                uint size = (uint)f_buffSize;
                int[] imageArray = new int[size];
               // ErrorValue = Api.GetMostRecentImage(imageArray, size);
                return new ImgBufferItem(imageArray);
            }
        }
    }
}
