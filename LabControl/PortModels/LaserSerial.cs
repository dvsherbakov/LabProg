﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using LabControl.ClassHelpers;

namespace LabControl.PortModels
{
    internal class LaserSerial
    {
        private readonly SerialPort f_MPort;
        private readonly LaserCommand f_LCommand;
        private static List<string> _errList;
        private static List<string> _msgList;
        private int f_DPower;
        private int f_DMaxPower;
        private float TempAmb;
        private int f_LPwr;
        private int f_LaserType;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public LaserSerial(string portStr)
        {
            _errList = new List<string>();
            _msgList = new List<string>();
            if (portStr.Length == 0) portStr = "COM6";
            f_MPort = new SerialPort(portStr)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            f_LCommand = new LaserCommand();
            f_MPort.DataReceived += DataReceivedHandler;
            f_DPower = 0;
            f_DMaxPower = 0;
        }

        public void OpenPort()
        {
            try
            {
                f_MPort.Open();
                SendCommand(1);
                SendCommand(7);
                SendCommand(18);
                SendCommand(16);
                SendCommand(21);
                SendCommand(13);
                SendCommand(30);
                SendCommand(23);
            } catch (Exception ex)
            {
                SetLogMessage?.Invoke(ex.Message);
            }
        }

        public void ClosePort()
        {
            if (f_MPort.IsOpen) f_MPort.Close();
        }

        public int GetDPower()
        {
            return f_DPower;
        }

        public int GetMaxPower()
        {
            return f_DMaxPower;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            try
            {
                var cnt = sp.ReadBufferSize;
                var mXdata = new byte[cnt + 1];
                try
                {
                    sp.Read(mXdata, 0, cnt);
                }
                catch (Exception ex)
                {
                    _errList.Add(ex.Message);
                    SetLogMessage?.Invoke(ex.Message);
                }

                var ascii = Encoding.ASCII;
                var answers = ascii.GetString(mXdata).Split('\r');
                foreach (var s in answers)
                {
                    if ((s.Length > 3) && (s.Length < 50))
                        _msgList.Add(s);
                    SetResMatt(s);
                }

            }
            catch (Exception ex)
            {
                SetLogMessage?.Invoke(ex.Message);
                _errList.Add(ex.Message);
            }
        }

        private void SetResMatt(string res)
        {
            var cmd = f_LCommand.GetCommandByMem(res);
            switch (cmd)
            {
                case 7:
                    var mPwr = Regex.Match(res, @"([-+]?[0-9]*\.?[0-9]+)");
                    if (mPwr.Success)
                        f_DMaxPower = Convert.ToInt32(mPwr.Groups[1].Value);
                    break;
                case 9:
                    var dPwr = Regex.Match(res, @"([-+]?[0-9]*\.?[0-9]+)");
                    if (dPwr.Success)
                        f_DPower = Convert.ToInt32(dPwr.Groups[1].Value);
                    break;
                case 11:
                    var tMpa = Regex.Match(res, @"([-+]?[0-9]*\.?[0-9]+)");
                    if (tMpa.Success)
                        TempAmb = Convert.ToInt32(tMpa.Groups[1].Value);
                    break;
                case 16:
                    var lPwr = res.Substring(4);
                    var b = Convert.ToInt32(lPwr, 16);
                    f_LPwr = b * 800 / 4095;
                    break;
                default:
                    break;
            }
        }

        private void SendCommand(int cmd)
        {
            if (f_MPort.IsOpen)
                f_MPort.Write(f_LCommand.GetCmdById(cmd).SCommand);
        }

        public void SetPower(int pwr)
        {
            if (this.f_LaserType == 0)
            {
                var cmd = f_LCommand.SetPowerLvl(pwr);
                if (f_MPort.IsOpen)
                {
                    f_MPort.Write(cmd.SCommand);
                }
            }
            else SetPowerLevel(pwr);
        }

        public int GetLasePower()
        {
            return f_LPwr;
        }

        public void SetOn()
        {
            if (f_LaserType == 0)
            {
                SendCommand(35);
                SendCommand(37);
            }
            else Start();
        }

        public void SetOff()
        {
            if (f_LaserType == 0)
            {
                SendCommand(36);
                SendCommand(38);
            }
            else Stop();
        }

        private void Start()
        {
            if (f_LaserType == 1) SetPowerStart();
            else
            {
                var cmd = new byte[8] { 0x53, 0x08, 0x06, 0x01, 0x00, 0x01, 0x63, 0x0D };
                f_MPort.Write(cmd, 0, 8);
            }
        }

        private void Stop()
        {
            if (!f_MPort.IsOpen) return;
            var cmd = new byte[8] { 0x53, 0x08, 0x06, 0x01, 0x00, 0x02, 0x64, 0x0D };
            f_MPort.Write(cmd, 0, 8);
        }

        private void SetPowerLevel(int level)
        {
            var command = new List<byte> { 0x53, 0x08, 0x04, 0x01 };
            var bts = BitConverter.GetBytes(level);
            command.Add(bts[1]);
            command.Add(bts[0]);
            short sm = 0;
            foreach (var x in command)
                sm += x;
            var cb = BitConverter.GetBytes(sm);
            command.Add(cb[0]);
            command.Add(0x0D);

            var cmd = command.ToArray();
            f_MPort.Write(cmd, 0, 8);
        }

        private void SetPowerStart()
        {
            var command = new List<byte> { 0x53, 0x08, 0x06, 0x01, 0x00, 0x01 };

            short sm = 0;
            foreach (var x in command)
                sm += x;
            var cb = BitConverter.GetBytes(sm);
            command.Add(cb[0]);
            command.Add(0x0D);

            var cmd = command.ToArray();
            f_MPort.Write(cmd, 0, 8);
        }

        public void SetLaserType(int tp)
        {
            f_LaserType = tp;
        }
    }
}
