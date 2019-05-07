using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Text;

namespace LabProg
{
    internal class PwrSerial
    {
        private static SerialPort Port;
        private byte[] _rxdata;
        private static readonly List<string> ErrList = new List<string>();
        //readonly Timer _aTimer = new Timer();
        private static ModBus modBus;

        public PwrSerial(string port)
        {
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
                _rxdata = new byte[cnt + 1];
                var rc = sp.Read(_rxdata, 0, cnt);
                var ascii = Encoding.ASCII;
                var answrs = ascii.GetString(_rxdata).Split('\r');
            }
            catch (Exception ex)
            {
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

        public static void GetChanellData(int channel)
        {
            byte[] dt = { 0x1, 0x3, 0x7, 0xD0, 0x0, 0x9, 0x85, 0x41 };
            Write(dt);
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

        public void SetMaxVolts(int chanell, int maxVolts)
        {
            byte[] dt={};
            if (chanell == 0) { dt = modBus.GetMaxVolts(PwrParams.REG_P0_MAX_VOLTS, maxVolts); }
            if (chanell == 1) { dt = modBus.GetMaxVolts(PwrParams.REG_P1_MAX_VOLTS, maxVolts); }
            if (chanell == 2) { dt = modBus.GetMaxVolts(PwrParams.REG_P2_MAX_VOLTS, maxVolts); }
            if (chanell == 3) { dt = modBus.GetMaxVolts(PwrParams.REG_P3_MAX_VOLTS, maxVolts); }
            if (chanell == 4) { dt = modBus.GetMaxVolts(PwrParams.REG_P4_MAX_VOLTS, maxVolts); }
            if (chanell == 5) { dt = modBus.GetMaxVolts(PwrParams.REG_P5_MAX_VOLTS, maxVolts); }
            Write(dt);
            Thread.Sleep(1000);
            
        }

        public void SetBias(int chanell, int bias)
        {
            byte[] dt = { };
            if (chanell == 0) { dt = modBus.GetMaxVolts(PwrParams.REG_P0_BIAS, bias); }
            if (chanell == 1) { dt = modBus.GetMaxVolts(PwrParams.REG_P1_BIAS, bias); }
            if (chanell == 2) { dt = modBus.GetMaxVolts(PwrParams.REG_P2_BIAS, bias); }
            if (chanell == 3) { dt = modBus.GetMaxVolts(PwrParams.REG_P3_BIAS, bias); }
            if (chanell == 4) { dt = modBus.GetMaxVolts(PwrParams.REG_P4_BIAS, bias); }
            if (chanell == 5) { dt = modBus.GetMaxVolts(PwrParams.REG_P5_BIAS, bias); }
            Write(dt);
            Thread.Sleep(1000);
        }
    }
}

