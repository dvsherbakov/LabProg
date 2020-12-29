using LabControl.ClassHelpers;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace LabControl.PortModels
{
    internal class PwrSerial
    {
        public static int CurChannel { get; set; }
        private static SerialPort Port;
        public byte[] Rxdata { get; set; }
        public static byte ChCommand { get; set; }
        private static readonly List<string> ErrList = new List<string>();
        //readonly Timer _aTimer = new Timer();
        private static ModBus modBus;

        public delegate void RecievedData(object sender, EventArgs e);
        public event RecievedData onRecieve;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public PwrSerial(string port)
        {
            CurChannel = 99;
            modBus = new ModBus();
            if (port == "") port = "COM9";
            Port = new SerialPort(port)
            {
                BaudRate = 115200,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            Port.DataReceived += DataReceivedHandler;
        }

        public void OpenPort()
        {
            Port.Open();
        }

        public void ClosePort()
        {
            Port.Close();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            try
            {
                var cnt = sp.ReadBufferSize;
                Rxdata = new byte[cnt + 1];
                var rc = sp.Read(Rxdata, 0, cnt);
                if (cnt > 3 && Rxdata[0] == 1 && Rxdata[1] == 3 && Rxdata[2] == 18)
                {
                    // EventArgs ea = new EventArgs();
                    onRecieve?.Invoke(this, e);
                }
            }
            catch (Exception ex)
            {
                SetLogMessage?.Invoke(ex.Message);
                ErrList.Add(ex.Message);
            }
        }

        private static void Write(byte[] buf)
        {
            try
            {
                Port.Write(buf, 0, buf.Length);
            }
            catch (Exception ex)
            {
                ErrList.Add(ex.Message);
            }
        }

        public static void GetChanellData(byte channel)
        {
            CurChannel = channel;
            byte[] dt = modBus.GetQueryChannel(channel);
            //{ 0x1, 0x3, 0x7, 0xD0, 0x0, 0x9, 0x85, 0x41 };
            Write(dt);
            ChCommand = channel;
        }

        public static void SetChannelOn(int channel)
        {
            if (Port.IsOpen)
            {
                switch (channel)
                {
                    case 0:
                        {
                            byte[] dt = { 0x1, 0x10, 0x7, 0xD0, 0x0, 0x1, 0x2, 0x0, 0x1, 0x2, 0xC0 };
                            Write(dt);
                            break;
                        }
                    case 1:
                        {
                            byte[] dt = { 0x1, 0x10, 0x7, 0xDA, 0x0, 0x1, 0x2, 0x0, 0x1, 0x2, 0x6A };
                            Write(dt);
                            break;
                        }
                    case 2:
                        {
                            byte[] dt = { 0x1, 0x10, 0x7, 0xE4, 0x0, 0x1, 0x2, 0x0, 0x1, 0x6, 0xB4 };
                            Write(dt);
                            break;
                        }
                    case 3:
                        {
                            byte[] dt = { 0x1, 0x10, 0x7, 0xEE, 0x0, 0x1, 0x2, 0x0, 0x1, 0x6, 0x1E };
                            Write(dt);
                            break;
                        }
                    case 4:
                        {
                            byte[] dt = { 0x1, 0x10, 0x7, 0xF8, 0x0, 0x1, 0x2, 0x0, 0x1, 0x4, 0xE8 };
                            Write(dt);
                            break;
                        }
                    case 5:
                        {
                            byte[] dt = { 0x1, 0x10, 0x8, 0x2, 0x0, 0x1, 0x2, 0x0, 0x1, 0xEF, 0xB2 };
                            Write(dt);
                            break;
                        }
                }
            }
        }

        public static void SetChannelOff(int channel)
        {
            if (Port.IsOpen)
            {
                switch (channel)
                {
                    case 0:
                        {
                            byte[] dt = { 0x1, 0x10, 0x7, 0xD0, 0x0, 0x1, 0x2, 0x0, 0x0, 0xC3, 0x0 };
                            Write(dt);
                            break;
                        }
                    case 1:
                        {
                            byte[] dt = { 0x1, 0x10, 0x7, 0xDA, 0x0, 0x1, 0x2, 0x0, 0x0, 0xC3, 0xAA };
                            Write(dt);
                            break;
                        }
                    case 2:
                        {
                            byte[] dt = { 0x1, 0x10, 0x7, 0xE4, 0x0, 0x1, 0x2, 0x0, 0x0, 0xC7, 0x74 };
                            Write(dt);
                            break;
                        }
                    case 3:
                        {
                            byte[] dt = { 0x1, 0x10, 0x7, 0xEE, 0x0, 0x1, 0x2, 0x0, 0x0, 0xC7, 0xDE };
                            Write(dt);
                            break;
                        }
                    case 4:
                        {
                            byte[] dt = { 0x1, 0x10, 0x7, 0xF8, 0x0, 0x1, 0x2, 0x0, 0x0, 0xC5, 0x28 };
                            Write(dt);
                            break;
                        }
                    case 5:
                        {
                            byte[] dt = { 0x1, 0x10, 0x8, 0x2, 0x0, 0x1, 0x2, 0x0, 0x0, 0x2E, 0x72 };
                            Write(dt);
                            break;
                        }
                }
            }
        }

        public void SetMode(int chanell, int mode)
        {
            byte[] dt = modBus.GetMaxVolts(PwrParams.Modes[chanell], mode);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetAmplitude(int chanell, int amplitude)
        {
            byte[] dt = modBus.GetMaxVolts(PwrParams.Amplitudes[chanell], amplitude);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetBias(int chanell, int bias)
        {
            byte[] dt = modBus.GetMaxVolts(PwrParams.Biases[chanell], bias);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetFreq(int chanell, int freq)
        {
            byte[] dt = modBus.GetMaxVolts(PwrParams.Freqs[chanell], freq);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetDuty(int chanell, int duty)
        {
            byte[] dt = modBus.GetMaxVolts(PwrParams.Dutys[chanell], duty);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetPhase(int chanell, int phase)
        {
            byte[] dt = modBus.GetMaxVolts(PwrParams.Phases[chanell], phase);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetMaxVolts(int chanell, int maxVolts)
        {
            byte[] dt = modBus.GetMaxVolts(PwrParams.MaxVolts[chanell], maxVolts);
            Write(dt);
            Thread.Sleep(1000);
        }

        public void SetMaxAmps(int chanell, int amps)
        {
            byte[] dt = modBus.GetMaxVolts(PwrParams.MaxAmps[chanell], amps);
            Write(dt);
            Thread.Sleep(1000);
        }
    }
}
