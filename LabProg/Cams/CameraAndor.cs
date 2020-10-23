using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using ATMCD64CS;

namespace LabProg.Cams
{
    class CameraAndor
    {
        private readonly AndorSDK Api;

        private uint errorValue = 0;
        public uint ErrValue
        {
            get => errorValue;
            private set
            {
                errorValue = value;
                if (value != 20002)
                {
                    f_AddLogBoxMessage(AndorStatusMessages.GetMessage(value));
                }
            }
        }
        private readonly int f_X = 0;
        private readonly int f_Y = 0;
        public int XResolution { get => f_X; }
        public int YResolution { get => f_Y; }
        private static readonly int gblXPixels = 0;
        private static readonly int gblYPixels = 0;
        private readonly int f_buffSize;
        readonly List<ImgBufferItem> Buffer = new List<ImgBufferItem>();
        private readonly Action<string> f_AddLogBoxMessage;
        public int BufSize { get => f_buffSize; }
        private AndorSDK.AndorCapabilities Capabilities;
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
                SetExposure(f_exposure);
            }
        }
        private readonly System.Timers.Timer f_seriesTimer;

        public CameraAndor(Action<string> addLogBoxMessage)
        {
            Api = new AndorSDK();
            Capabilities = new AndorSDK.AndorCapabilities();
            var CapabilitiesSize = Marshal.SizeOf(Capabilities);
            Capabilities.ulSize = (uint)CapabilitiesSize;
            var curDir = Environment.CurrentDirectory;
            f_AddLogBoxMessage = addLogBoxMessage;
            ErrValue = Api.Initialize(curDir);
            ErrValue = Api.GetCapabilities(ref Capabilities);
            ErrValue = Api.GetHeadModel(ref f_headModel);
            ErrValue = Api.GetDetector(ref f_X, ref f_Y);
            SetImageMode();
            SetFasterSpeed();
            if ((Capabilities.ulSetFunctions & AndorSDK.AC_SETFUNCTION_BASELINECLAMP) == AndorSDK.AC_SETFUNCTION_BASELINECLAMP)
            {
                ErrValue = Api.SetBaselineClamp(1);
            }
            ErrValue = Api.GetAcquisitionTimings(ref f_exposure, ref f_AccumTime, ref f_KineticTime);
            SetExposure(0.01f);
            ErrValue = Api.SetImage(1, 1, 1, f_X, 1, f_Y);
            ErrValue = Api.SetShutter(1, 1, 0, 0);
            f_buffSize = f_X * f_Y;
            f_seriesTimer = new System.Timers.Timer();
        }

        public float GetTempereture()
        {
            float temperature = -99.0f;
            ErrValue = Api.GetTemperatureF(ref temperature);
            return temperature;
        }

        public void SetImageMode()
        {
            ErrValue = Api.SetReadMode(4);
        }

        public void SetFasterSpeed()
        {
            int vsNumber = 0; float speed = 0.0f;
            ErrValue = Api.GetFastestRecommendedVSSpeed(ref vsNumber, ref speed);
            int nAD = 0;
            ErrValue = Api.GetNumberADChannels(ref nAD);
            float sTemp = 0.0f;
            int hNumber = 0, ADnumber = 0;

            for (int iAD = 0; iAD < nAD; iAD++)
            {
                int index = 0;
                Api.GetNumberHSSpeeds(iAD, 0, ref index);
                for (int iSpeed = 0; iSpeed < index; iSpeed++)
                {
                    Api.GetHSSpeed(iAD, 0, iSpeed, ref speed);
                    if (sTemp < speed)
                    {
                        sTemp = speed;
                        hNumber = iSpeed;
                        ADnumber = iAD;
                    }
                }
            }
            ErrValue = Api.SetADChannel(ADnumber);
            ErrValue = Api.SetHSSpeed(0, hNumber);
        }
        private void SetExposure(float exposure) => Api.SetExposureTime(exposure);

        public void SetInternalTriggerMode() => Api.SetTriggerMode(0);
        public void SetSoftwareTriggerMode() => Api.SetTriggerMode(10);

        public ImgBufferItem GetOnceBuffer()
        {
            ErrValue = Api.SetAcquisitionMode(1); //Тип серии 1 - одиночных снимок, 5 - снимать до остановки
            ErrValue = Api.StartAcquisition();
            ErrValue = Api.WaitForAcquisition();
            uint size = (uint)f_buffSize;
            int[] imageArray = new int[size];
            ErrValue = Api.GetMostRecentImage(imageArray, size);
            return new ImgBufferItem(imageArray);
        }

        static public byte[] NormalizeData(int[] src)
        {
            var dest = new byte[src.Length];
            var min = src.Min();
            var max = src.Max();
            var cScale = (256.0f / (float)(max - min));
            for (var i = 0; i < src.Length; i++)
                dest[i] = (byte)((src[i] - min) * cScale);
            return dest;
        }

        static Bitmap Convert2Bitmap(byte[] DATA, int width, int height)
        {
            Bitmap Bm = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            var b = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ColorPalette ncp = b.Palette;
            for (int i = 0; i < 256; i++)
                ncp.Entries[i] = Color.FromArgb(255, i, i, i);
            b.Palette = ncp;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int Value = DATA[x + (y * width)];
                    Color C = ncp.Entries[Value];
                    Bm.SetPixel(x, y, C);
                }
            }
            return Bm;
        }

        void CompexPrepareFile(int[] src)
        {
            var dest = new byte[src.Length];
            var min = src.Min();
            var max = src.Max();
            var cScale = (256.0f / (max - min));
            for (var i = 0; i < src.Length; i++)
                dest[i] = (byte)((src[i] - min) * cScale);

            Bitmap Bm = new Bitmap(gblXPixels, gblYPixels, PixelFormat.Format24bppRgb);
            var b = new Bitmap(gblXPixels, gblYPixels, PixelFormat.Format8bppIndexed);
            ColorPalette ncp = b.Palette;
            for (int i = 0; i < 256; i++)
                ncp.Entries[i] = Color.FromArgb(255, i, i, i);
            b.Palette = ncp;
            for (int y = 0; y < gblYPixels; y++)
            {
                for (int x = 0; x < gblXPixels; x++)
                {
                    int Value = dest[x + (y * gblXPixels)];
                    Color C = ncp.Entries[Value];
                    Bm.SetPixel(x, y, C);
                }
            }
            var fName = DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString();
            fName = fName.Replace(":", string.Empty).Replace(" ", string.Empty).Replace(".", String.Empty) +
                DateTime.Now.Millisecond.ToString() + ".bmp";
            Bm.Save(fName);
            f_AddLogBoxMessage($"Bimap saved by name {fName}");
        }

        async void SaveToBitmap(int[] Data) => await Task.Run(() => CompexPrepareFile(Data));

        public void SaveOnceBitnap()
        {
            ErrValue = Api.SetAcquisitionMode(1); //Тип серии 1 - одиночных снимок, 5 - снимать до остановки
            ErrValue = Api.StartAcquisition();
            ErrValue = Api.WaitForAcquisition();
            uint size = (uint)(gblXPixels * gblYPixels);
            int[] imageArray = new int[size];
            ErrValue = Api.GetMostRecentImage(imageArray, size);

            SaveToBitmap(imageArray);
        }

        public Bitmap GetOnceBitmap()
        {
            int imagesCount = 0;
            ErrValue = Api.GetTotalNumberImagesAcquired(ref imagesCount);
            //Console.WriteLine($"В памяти изображений: {imagesCount}");
            ErrValue = Api.SetAcquisitionMode(1); //Тип серии 1 - одиночных снимок, 5 - снимать до остановки
            errorValue = Api.StartAcquisition();
            errorValue = Api.WaitForAcquisition();
            uint size = (uint)(gblXPixels * gblYPixels);
            int[] imageArray = new int[size];
            errorValue = Api.GetMostRecentImage(imageArray, size);
            var bm = Convert2Bitmap(NormalizeData(imageArray), 1024, 1024);
            return bm;
        }

        public void SustainedRun(float exTime, float acTyme, float kiTime)
        {

            f_AddLogBoxMessage($"Начата серия {DateTime.Now}");
            ErrValue = Api.SetReadMode(4);
            ErrValue = Api.SetTriggerMode(0);
            ErrValue = Api.SetAcquisitionMode(3);
            ErrValue = Api.SetExposureTime(exTime);
            ErrValue = Api.SetKineticCycleTime(kiTime);
            ErrValue = Api.SetNumberAccumulations(1);
            ErrValue = Api.SetAccumulationCycleTime(acTyme);
            ErrValue = Api.SetNumberKinetics(300);
            ErrValue = Api.PrepareAcquisition();

            ErrValue = Api.StartAcquisition();

            SetTimer(08);

        }

        private void GetOldestData()
        {
            try
            {
                int fImage = 0, lImage = 0;
                ErrValue = Api.GetNumberNewImages(ref fImage, ref lImage);
                //Console.WriteLine($"{fImage} : {lImage}");
                if ((lImage - fImage) > 0)
                {
                    //Console.WriteLine(imagesCount);
                    var size = gblXPixels * gblYPixels;
                    var imagesData = new int[size];
                    errorValue = Api.GetOldestImage(imagesData, (uint)size);
                    Buffer.Add(new ImgBufferItem(imagesData));
                    //Task.Run(() => CompexPrepareFile(imagesData));
                }
                else { f_AddLogBoxMessage("Нет записей"); }
                if (fImage >= 300)
                {
                    f_AddLogBoxMessage("Буффер заполнен");
                    StopTimer();
                    Parallel.ForEach(Buffer, new ParallelOptions { MaxDegreeOfParallelism = 3 }, buf => buf.Save());
                }
            }
            catch (Exception ex) { f_AddLogBoxMessage(ex.Message); }
        }

        public async void StoreBufferedData()
        {
            int fImage = 0, lImage = 0;
            do
            {
                await Task.Run(() => GetOldestData());
                ErrValue = Api.GetNumberNewImages(ref fImage, ref lImage);
                //Console.WriteLine($"Количество доступных изображений: {fImage} : {lImage}");
            }
            while (lImage - fImage > 0);
        }

        public Task StoreOldestImage => Task.Run(() => GetOldestData());

        void SetTimer(int interval)
        {
            f_seriesTimer.Interval = interval;
            f_seriesTimer.Start();
        }

        void StopTimer() => f_seriesTimer.Stop();
    }
}
