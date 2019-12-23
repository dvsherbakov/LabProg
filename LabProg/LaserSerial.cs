using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace LabProg
{
    internal class LaserSerial
    {
        private readonly SerialPort _mPort;
        private readonly LaserCommand _lCommand;
        private static List<string> _errList;
        private static List<string> _msgList;
        private int _dPower;
        private int DMaxPower;
        private float TempAmb;
        private int lPwr;

        public LaserSerial(string portStr)
        {
            _errList = new List<string>();
            _msgList = new List<string>();
            if (portStr == "") portStr = "COM6";
            _mPort = new SerialPort(portStr)
            {
                BaudRate = 500000,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _lCommand = new LaserCommand();
            _mPort.DataReceived += DataReceivedHandler;
            _dPower = 0;
            DMaxPower = 0;
            TempAmb = 0f;
        }

        public void OpenPort()
        {
            _mPort.Open();
            SendCommand(1);
            SendCommand(7);
            SendCommand(18);
            SendCommand(16);
            SendCommand(21);
            SendCommand(13);
            SendCommand(30);
            SendCommand(23);
        }

        public void ClosePort()
        {
            _mPort.Close();
        }

        public int GetDPower()
        {
            return _dPower;
        }

        public int GetMaxPower()
        {
            return DMaxPower;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            try
            {
                var cnt = sp.ReadBufferSize;
                var mRxdata = new byte[cnt + 1];
                try
                {
                    sp.Read(mRxdata, 0, cnt);
                }
                catch (Exception ex)
                {
                    _errList.Add(ex.Message);
                }

                var ascii = Encoding.ASCII;
                var answrs = ascii.GetString(mRxdata).Split('\r');
                foreach (var s in answrs)
                {
                    if ((s.Length > 3)&&(s.Length<50))
                    _msgList.Add(s);
                    SetResMatt(s);
                }

            }
            catch (Exception ex)
            {
                //Dispatcher.Invoke(() => windowLogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Приложение запущено" }));
                _errList.Add(ex.Message);
            }
        }

       private void SetResMatt(string res)
       {
           var cmd = _lCommand.GetCommandByMem(res);
            switch (cmd)
            {
                case 7:
                    Match mpwr = Regex.Match(res, @"([-+]?[0-9]*\.?[0-9]+)");
                    if (mpwr.Success)
                        DMaxPower = Convert.ToInt32(mpwr.Groups[1].Value);
                    break;
                case 9:
                    var dpwr = Regex.Match(res, @"([-+]?[0-9]*\.?[0-9]+)");
                    if (dpwr.Success)
                        _dPower = Convert.ToInt32(dpwr.Groups[1].Value);
                    break;
                case 11:
                    var tmpa = Regex.Match(res, @"([-+]?[0-9]*\.?[0-9]+)");
                    if (tmpa.Success)
                       TempAmb = Convert.ToInt32(tmpa.Groups[1].Value);
                    break;
                case 16:
                    var lpwr = res.Substring(4);
                    int b = Convert.ToInt32(lpwr, 16);
                    lPwr = (int)(b * 800 / 4095);
                    break;
                default:
                    break;
            }
       }

        private  void SendCommand(int cmd)
        {
            if (_mPort.IsOpen)
                _mPort.Write(_lCommand.GetCmdById(cmd).SCommand);
        }

        public void SetPower(int pwr)
        {
            var cmd = _lCommand.SetPowerLvl(pwr);
            if (_mPort.IsOpen)
            {
                _mPort.Write(cmd.SCommand);
            }
        }

        public int GetLasePower()
        {
            return lPwr;
        }

        public void SetOn()
        {
            SendCommand(35);
            SendCommand(37);
        }

        public void SetOff()
        {
            SendCommand(36);
            SendCommand(38);
        }

        static byte[] Start()
        {
            return new byte[8] { 0X53, 0X08, 0X06, 0X01, 0X00, 0X01, 0X63, 0X0D };
        }

        static byte[] Stop()
        {
            return new byte[8] { 0X53, 0X08, 0X06, 0X01, 0X00, 0X02, 0X64, 0X0D };
        }

        static byte[] SetPowerLevel(int level)
        {
            List<byte> command = new List<byte> { 0x53, 0x08, 0x04, 0x01 };
            var bts = BitConverter.GetBytes(level);
            command.Add(bts[1]);
            command.Add(bts[0]);
            Int16 sm = 0;
            foreach (byte x in command)
                sm += x;
            var cb = BitConverter.GetBytes(sm);
            command.Add(cb[0]);
            command.Add(0x0D);

            return command.ToArray();
        }
    }
}
