using LabControl.ClassHelpers;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace LabControl.PortModels
{
    internal class PwrSerial
    {
        public int CurChannel { get; set; }
        private static SerialPort _port;
        private byte[] RxData { get; set; }
        private static byte ChCommand { get; set; }
        private static readonly List<string> ErrList = new List<string>();
        //readonly Timer _aTimer = new Timer();
        private static ModBus _modBus;

        public delegate void RecievedData(byte[] data);
        public event RecievedData OnRecieve;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public PwrSerial(string port)
        {
            CurChannel = 99;
            _modBus = new ModBus();
            SetLogMessage?.Invoke($"Try connected Power Supply on port {port}");
            if (port == "") port = "COM9";
            _port = new SerialPort(port)
            {
                BaudRate = 115200,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _port.DataReceived += DataReceivedHandler;
        }

        public static void OpenPort()
        {
            _port.Open();
        }

        public static void ClosePort()
        {
            _port.Close();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            try
            {
                var cnt = sp.ReadBufferSize;
                RxData = new byte[cnt + 1];
                var rc = sp.Read(RxData, 0, cnt);
                if (cnt > 3 && RxData[0] == 1 && RxData[1] == 3 && RxData[2] == 18)
                {
                    // EventArgs ea = new EventArgs();
                    OnRecieve?.Invoke(RxData);
                }
            }
            catch (Exception ex)
            {
                SetLogMessage?.Invoke(ex.Message);
                //ErrList.Add(ex.Message);
            }
        }

        private static void Write(byte[] buf)
        {
            try
            {
                _port.Write(buf, 0, buf.Length);
            }
            catch (Exception ex)
            {
                ErrList.Add(ex.Message);
            }
        }

        public void GetChanelData(byte channel)
        {
            CurChannel = channel;
            var dt = _modBus.GetQueryChannel(channel);
            //{ 0x1, 0x3, 0x7, 0xD0, 0x0, 0x9, 0x85, 0x41 };
            Write(dt);
            ChCommand = channel;
        }

        public void SetChannelOn(int channel)
        {
            if (!_port.IsOpen) return;
            var dt = channel switch
            {
                0 => new byte[] { 0x1, 0x10, 0x7, 0xD0, 0x0, 0x1, 0x2, 0x0, 0x1, 0x2, 0xC0 },
                1 => new byte[] { 0x1, 0x10, 0x7, 0xDA, 0x0, 0x1, 0x2, 0x0, 0x1, 0x2, 0x6A },
                2 => new byte[] { 0x1, 0x10, 0x7, 0xE4, 0x0, 0x1, 0x2, 0x0, 0x1, 0x6, 0xB4 },
                3 => new byte[] { 0x1, 0x10, 0x7, 0xEE, 0x0, 0x1, 0x2, 0x0, 0x1, 0x6, 0x1E },
                4 => new byte[] { 0x1, 0x10, 0x7, 0xF8, 0x0, 0x1, 0x2, 0x0, 0x1, 0x4, 0xE8 },
                5 => new byte[] { 0x1, 0x10, 0x8, 0x2, 0x0, 0x1, 0x2, 0x0, 0x1, 0xEF, 0xB2 },
                _ => null
            };
            if (dt is null)
            {
                return;
            }
            Write(dt);
        }

        public void SetChannelOff(int channel)
        {
            if (!_port.IsOpen) return;
            byte[] dt = channel switch
            {
                0 => new byte[] { 0x1, 0x10, 0x7, 0xD0, 0x0, 0x1, 0x2, 0x0, 0x0, 0xC3, 0x0 },
                1 => new byte[] { 0x1, 0x10, 0x7, 0xDA, 0x0, 0x1, 0x2, 0x0, 0x0, 0xC3, 0xAA },
                2 => new byte[] { 0x1, 0x10, 0x7, 0xE4, 0x0, 0x1, 0x2, 0x0, 0x0, 0xC7, 0x74 },
                3 => new byte[] { 0x1, 0x10, 0x7, 0xEE, 0x0, 0x1, 0x2, 0x0, 0x0, 0xC7, 0xDE },
                4 => new byte[] { 0x1, 0x10, 0x7, 0xF8, 0x0, 0x1, 0x2, 0x0, 0x0, 0xC5, 0x28 },
                5 => new byte[] { 0x1, 0x10, 0x8, 0x2, 0x0, 0x1, 0x2, 0x0, 0x0, 0x2E, 0x72 },
                _ => null
            };
            if (dt is null)
            {
                return;
            }
            Write(dt);
        }

        public void SetMode(int chanel, int mode)
        {
            var dt = _modBus.GetMaxVolts(PwrParams.Modes[chanel], mode);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetAmplitude(int chanel, int amplitude)
        {
            var dt = _modBus.GetMaxVolts(PwrParams.Amplitudes[chanel], amplitude);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetBias(int chanel, int bias)
        {
            var dt = _modBus.GetMaxVolts(PwrParams.Biases[chanel], bias);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetFreq(int chanel, int freq)
        {
            var dt = _modBus.GetMaxVolts(PwrParams.Freqs[chanel], freq);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetDuty(int chanel, int duty)
        {
            var dt = _modBus.GetMaxVolts(PwrParams.Dutys[chanel], duty);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetPhase(int chanel, int phase)
        {
            var dt = _modBus.GetMaxVolts(PwrParams.Phases[chanel], phase);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetMaxVolts(int chanel, int maxVolts)
        {
            var dt = _modBus.GetMaxVolts(PwrParams.MaxVolts[chanel], maxVolts);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetMaxAmps(int chanel, int amps)
        {
            var dt = _modBus.GetMaxVolts(PwrParams.MaxAmps[chanel], amps);
            Write(dt);
            Thread.Sleep(1000);
        }
    }
}
