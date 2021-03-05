﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabControl.ClassHelpers;

namespace LabControl.PortModels
{
    public class DispenserSerial
    {
        private readonly SerialPort _mPort;
        private List<DispenserCommandData> f_CommandList;

        public delegate void DispatchDelegate(byte[] data);
        public event DispatchDelegate DispathRecieveData;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        private DispenserSineWaveData f_SineWaveData;
        private DispenserPulseWaveData f_PulseWaveData;

        private int f_SignalType;
        private byte f_Channel;
        public int Frequency { get; set; }

        private string f_PortName;
        public string PortName
        {
            get => f_PortName;
            set
            {
                f_PortName = value;
                if (_mPort.IsOpen)
                {
                    _mPort.Close();
                    _mPort.PortName = f_PortName;
                    _mPort.Open();
                }
                _mPort.PortName = f_PortName;
            }
        }

        public DispenserSerial(string pName)
        {
            f_PortName = string.IsNullOrEmpty(pName) ? "COM6" : pName;

            _mPort = new SerialPort(f_PortName)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _mPort.DataReceived += DataReceivedHandler;

            f_CommandList = new List<DispenserCommandData>();

        }

        private void StartNext()
        {
            if (!(f_CommandList.Count > 0) || !_mPort.IsOpen)
            {
                return;
            }
            var cmd = f_CommandList.FirstOrDefault();
            _mPort.Write(cmd.CommandString, 0, cmd.CommandString.Length);
            f_CommandList.Remove(cmd);
        }

        public void OpenPort()
        {
            _mPort.Open();
            Init();
            StartNext();
        }

        public void ClosePort()
        {
            _mPort.Close();
        }

        private static byte[] TrimReceivedData(IEnumerable<byte> src)
        {
            var res = new List<byte>(src);
            while (res.LastOrDefault() == 0)
            {
                res.RemoveAt(res.Count - 1);
            }
            return res.ToArray();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            PortSleep(50);
            var cnt = _mPort.ReadBufferSize;
            var mRxData = new byte[cnt + 1];
            try
            {
                _mPort.Read(mRxData, 0, cnt);
                mRxData = TrimReceivedData(mRxData);
            }
            catch (Exception ex)
            {
                SetLogMessage(ex.Message);
            }
            var ascii = Encoding.ASCII;
            StartNext();
            DispathRecieveData?.Invoke(mRxData);
        }

        public void GetVersion()
        {

            var cmd = new byte[4] { 0x53, 0x02, 0xF0, 0xF2 };
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SoftReset()
        {
            var cmd = new byte[4] { 0x53, 0x02, 0x01, 0x03 };
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        public void GetNumberOfChannels()
        {
            var cmd = new byte[4] { 0x53, 0x02, 0x0D, 0x0F };
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private static byte CheckSum(IReadOnlyList<byte> data)
        {
            var chSum = 0;
            for (var i = 1; i < data.Count - 1; i++)
            {
                chSum += data[i];
            }
            return (byte)(chSum & 0xFF);
        }

        public void SetSineWaveForm(DispenserSineWaveData data)
        {
            var v0 = BytesUtility.DivideData((short)data.V0);
            var vp = BytesUtility.DivideData((short)data.VPeak);
            var t = BytesUtility.DivideData((short)data.TimeToverall);
            var cmd = new byte[] {
                0x53,//Header ‘S’
                0x08, 0x17, //Number of Bytes 08h //Command 17h 
                v0.Item1, v0.Item2, //Voltage V0 * 10(high byte) XXh Voltage V0 * 10(low byte) XXh 
                t.Item1, t.Item1, //Time toverall * 10(high byte) XXh Time toverall * 10(low byte) XXh 
                vp.Item1, vp.Item2, //Voltage Vpeak * 10(high byte) XXh Voltage Vpeak * 10(low byte) XXh 
                0x0A//Check Sum XXh
            };
            cmd[9] = CheckSum(cmd);
            //_mPort.Write(cmd, 0, 10);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });

        }

        public void SetPulseWaveForm(DispenserPulseWaveData data)
        {
            var t1 = BytesUtility.DivideData((short)(data.TimeT1 * 10));
            var t2 = BytesUtility.DivideData((short)(data.TimeT2 * 10));
            var v0 = BytesUtility.DivideData((short)(data.V0));
            var v1 = BytesUtility.DivideData((short)(data.V1));
            var v2 = BytesUtility.DivideData((short)(data.V2));
            var tr1 = BytesUtility.DivideData((short)(data.TimeRise1 * 10));
            var tf = BytesUtility.DivideData((short)(data.TimeFall * 10));
            var tr2 = BytesUtility.DivideData((short)(data.TimeRise2 * 10));

            //var cmd = new byte[] {
            //    0x53,
            //    0x15, 0x06, //len, command, 1-2
            //    0xFF, 0xFF, //unused, 3-4
            //    t1.Item1, t1.Item2, //0x00, 0xC8, t1, 200, 5-6
            //    0xFF,//unused, 7
            //    t2.Item1, t2.Item2, //0x01,0x90,//t2, 400 ,8-9
            //    v0.Item1, v0.Item2, //0x00, 0x00, //v0, 0, 10-11
            //    v1.Item1, v1.Item2, //0x00,0x0A, //v1, 10, 12-13
            //    v2.Item1, v2.Item2, //0xFF,0xF6, //v2, -10, 14-15
            //    tr1.Item1, tr1.Item2, //0x00, 0x1E, //tr1, 30
            //    tf.Item1, tf.Item2, //0x00, 0x1E, //tf, 30
            //    tr2.Item1, tr2.Item2,//0x00, 0x1E, //tr2,30
            //    0xCA //checksumm
            //};
            //cmd[22] = CheckSum(cmd);
            var cmd = new byte[] { 0x53, 0x15, 0x06, 0xFF, 0xFF, 0x01, 0x4A, 0xFF, 0x02, 0x8A, 0x00, 0x00, 0x00, 0x23, 0xFF, 0xDD, 0x00, 0x28, 0x00, 0x28, 0x00, 0x14, 0x52 };
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        public void SetFrequency(int freq)
        {
            var cFreq = BytesUtility.DivideData((short)freq);
            var cmd = new byte[]
            {
                0x53,
                0x04,
                0x12,
                cFreq.Item1,
                cFreq.Item1,
                0xFF
            };
            cmd[5] = CheckSum(cmd);
            //_mPort.Write(cmd, 0, 6);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        public void SetPeriod(uint period)
        {
            FooUnion v = new FooUnion { integer = 0, byte0 = 0x0, byte1 = 0x0, byte2 = 0, byte3 = 0 };
            v.integer = period+1;
            var cmd = new byte[]
            {
                0x53,
                0x05,
                0x19,
                v.byte2,
                v.byte1,
                v.byte0,
                0xBB
            };
            cmd[6] = CheckSum(cmd);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        public void Init()
        {
            SoftReset();
            GetVersion();
            GetNumberOfChannels();
        }

        public void Start()
        {
            //f_Channel = 0;
            //SetChannel();
            //if (f_SignalType == 0) { SetPulseWaveForm(f_PulseWaveData); } else { SetSineWaveForm(f_SineWaveData); }
            //SetDiscreteMode();
            //SetDropsPerTrigger(65534);
            //SetPeriod(10000);
            //SetFrequency(10000);
            //SetStrobleDivider();
            //SetEnableStrobe();
            //SetStrobeDelay();

            f_Channel = 1;
            SetChannel();
            if (f_SignalType == 0) { SetPulseWaveForm(f_PulseWaveData); } else { SetSineWaveForm(f_SineWaveData); }
            SetDiscreteMode();
            SetDropsPerTrigger(65534);
            SetPeriod(10000);
            SetFrequency(10000);
            SetStrobleDivider();
            SetEnableStrobe();
            SetStrobeDelay();
            
            //GenerateFromLog();
            StartTrigger(0);
            Dump(0);
            Dump(1);
            StartNext();
        }

        private void GenerateFromLog()
        {
            //Channel0
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x0C, 0x00, 0x0F }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x08, 0x00, 0x0B }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x15, 0x06, 0xFF, 0xFF, 0x00, 0xC8, 0xFF, 0x01, 0x90, 0x00, 0x00, 0x00, 0x0A, 0xFF, 0xF6, 0x00, 0x1E, 0x00, 0x1E, 0x00, 0x1E, 0xCA }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x04, 0x00, 0x07 }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x04, 0x03, 0x00, 0x01, 0x08 }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x05, 0x19, 0x00, 0x27, 0x11, 0x56 }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x10, 0x01, 0x14 }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x05, 0x13, 0x01, 0x00, 0x00, 0x19 }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x07, 0x01, 0x0B }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x05, 0x13, 0x01, 0x00, 0x00, 0x19 }, StartData = DateTime.Now });
            //Channel1
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x0C, 0x01, 0x10 }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x08, 0x00, 0x0B }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x15, 0x06, 0xFF, 0xFF, 0x00, 0xC8, 0xFF, 0x01, 0x90, 0x00, 0x00, 0x00, 0x0A, 0xFF, 0xF6, 0x00, 0x1E, 0x00, 0x1E, 0x00, 0x1E, 0xCA }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x04, 0x00, 0x07 }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x04, 0x03, 0x00, 0x01, 0x08 }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x05, 0x19, 0x00, 0x27, 0x11, 0x56 }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x10, 0x01, 0x14 }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x05, 0x13, 0x01, 0x00, 0x00, 0x19 }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x07, 0x01, 0x0B }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x05, 0x13, 0x01, 0x00, 0x00, 0x19 }, StartData = DateTime.Now });
            //to ch0
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x0C, 0x00, 0x0F }, StartData = DateTime.Now });
            //to ch1
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x0C, 0x01, 0x10 }, StartData = DateTime.Now });
            //grouped
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x08, 0x01, 0x0C }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x0A, 0x00, 0x0D }, StartData = DateTime.Now });
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x0A, 0x01, 0x0E }, StartData = DateTime.Now });
        }

        private void TriggerAll(bool start)
        {
            var onOff = start ? (byte)0x01 : (byte)0x00;
            var cmd = new byte[] {
                0x53,
                0x03,
                0x0A,
                onOff,
                0xFF
            };
            cmd[4] = CheckSum(cmd);
            //_mPort.Write(cmd, 0, 5);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SoftTrigger()
        {
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x02, 0x09, 0x0B }, StartData = DateTime.Now });
        }

        private void StartTrigger(byte src)
        {
            var cmd = new byte[] { 0x53, 0x03, 0x08, src, 0x0C };
            cmd[4] = CheckSum(cmd);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SetDropsPerTrigger(int drops)
        {
            var cDrops = BytesUtility.DivideData((short)drops);
            var cmd = new byte[] {
                0x53,
                0x04,
                0x03,
                cDrops.Item1,
                cDrops.Item2,
                0x08 };
            cmd[5] = CheckSum(cmd);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SetDiscreteMode()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x04, 0x01, 0x08 };
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void GroupTriggerSource(byte src)
        {
            var cmd = new byte[] { 0x53, 0x03, 0x0E, src, 0xFF };
            cmd[4] = CheckSum(cmd);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        public void Stop()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x0A, 0x00, 0x03 + 0x0A + 0x00 };
            _mPort.Write(cmd, 0, 5);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        public void SetChannel(byte ch)
        {
            f_Channel = ch;
        }

        public void SetChannel()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x0C, f_Channel, 0x62 };
            cmd[4] = (byte)(cmd[1] + cmd[2] + cmd[3]);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        public void Dump()
        {
            var cmd = new byte[] { 0x53, 0x02, 0x60, 0x62 };
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        public void Dump(byte channel)
        {
            var cmd = new byte[5] { 0x53, 0x03, 0x60, channel, 0x0 };
            cmd[4] = (byte)(cmd[1] + cmd[2] + cmd[3]);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SetStrobleDivider()
        {
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x07, 0x01, 0x0B }, StartData = DateTime.Now });
        }

        private void SetEnableStrobe()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x10, 0x00, 0x13 };
            cmd[4] = CheckSum(cmd);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SetStrobeDelay()
        {
            f_CommandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x05, 0x13, 0x01, 0x00, 0x00, 0x19 }, StartData = DateTime.Now });
        }

        public void SetSineWaveData(DispenserSineWaveData data)
        {
            f_SineWaveData = data;
        }

        public void SetPulseWaveData(DispenserPulseWaveData data)
        {
            f_PulseWaveData = data;
        }

        public void SetSignalType(int signalType)
        {
            f_SignalType = signalType;
        }

        public void PortSleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }

    class DispenserCommandData
    {
        public byte[] CommandString { get; set; }
        public DateTime StartData { get; set; }
    }
}
