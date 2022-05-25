using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using LabControl.ClassHelpers;

namespace LabControl.PortModels
{
    public class DispenserSerial
    {
        private readonly SerialPort _port;
        private readonly List<DispenserCommandData> _commandList;

        public delegate void DispatchDelegate(byte[] data);
        public event DispatchDelegate DispatchReceivedData;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        private DispenserSineWaveData _sineWaveData;
        private DispenserPulseWaveData _pulseWaveData;

        private int _signalType;
        private byte _channel;
        public int Frequency { private get; set; }

        private string _portName;
        public string PortName
        {
            get => _portName;
            set
            {
                _portName = value;
                if (_port.IsOpen)
                {
                    _port.Close();
                    _port.PortName = _portName;
                    _port.Open();
                }
                _port.PortName = _portName;
            }
        }

        public DispenserSerial(string pName)
        {
            _portName = string.IsNullOrEmpty(pName) ? "COM6" : pName;

            _port = new SerialPort(_portName)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _port.DataReceived += DataReceivedHandler;

            _commandList = new List<DispenserCommandData>();

        }

        private void StartNext()
        {
            if (!(_commandList.Count > 0) || !_port.IsOpen)
            {
                return;
            }
            var cmd = _commandList.FirstOrDefault();
            if (cmd == null) return;
            _port.Write(cmd.CommandString, 0, cmd.CommandString.Length);
            _commandList.RemoveAt(0);
        }

        public void OpenPort()
        {
            _port.Open();
            Init();
            StartNext();
        }

        public void ClosePort()
        {
            _port.Close();
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
            var cnt = _port.ReadBufferSize;
            var mRxData = new byte[cnt + 1];
            try
            {
                _port.Read(mRxData, 0, cnt);
                mRxData = TrimReceivedData(mRxData);
            }
            catch (Exception ex)
            {
                SetLogMessage?.Invoke(ex.Message);
            }

            StartNext();
            DispatchReceivedData?.Invoke(mRxData);
        }

        private void GetVersion()
        {

            var cmd = new byte[] { 0x53, 0x02, 0xF0, 0xF2 };
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SoftReset()
        {
            var cmd = new byte[] { 0x53, 0x02, 0x01, 0x03 };
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void GetNumberOfChannels()
        {
            var cmd = new byte[] { 0x53, 0x02, 0x0D, 0x0F };
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
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

        private void SetSineWaveForm(DispenserSineWaveData data)
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
            //_port.Write(cmd, 0, 10);
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });

        }

        private void SetPulseWaveForm(DispenserPulseWaveData data)
        {
            var t1 = BytesUtility.DivideData((short)(data.TimeT1 * 10));
            var t2 = BytesUtility.DivideData((short)(data.TimeT2 * 10));
            var v0 = BytesUtility.DivideData((short)(data.V0));
            var v1 = BytesUtility.DivideData((short)(data.V1));
            var v2 = BytesUtility.DivideData((short)(data.V2));
            var tr1 = BytesUtility.DivideData((short)(data.TimeRise1 * 10));
            var tf = BytesUtility.DivideData((short)(data.TimeFall * 10));
            var tr2 = BytesUtility.DivideData((short)(data.TimeRise2 * 10));

            var cmd = new byte[] {
                0x53,
                0x15, 0x06, //len, command, 1-2
                0xFF, 0xFF, //unused, 3-4
                t1.Item1, t1.Item2, //0x00, 0xC8, t1, 200, 5-6
                0xFF,//unused, 7
                t2.Item1, t2.Item2, //0x01,0x90,//t2, 400 ,8-9
                v0.Item1, v0.Item2, //0x00, 0x00, //v0, 0, 10-11
                v1.Item1, v1.Item2, //0x00,0x0A, //v1, 10, 12-13
                v2.Item1, v2.Item2, //0xFF,0xF6, //v2, -10, 14-15
                tr1.Item1, tr1.Item2, //0x00, 0x1E, //tr1, 30
                tf.Item1, tf.Item2, //0x00, 0x1E, //tf, 30
                tr2.Item1, tr2.Item2,//0x00, 0x1E, //tr2,30
                0xCA //checksumm
            };
            cmd[22] = CheckSum(cmd);
            
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SetFrequency()
        {
            var (item1, item2) = BytesUtility.DivideData((short)Frequency);
            var cmd = new byte[]
            {
                0x53,
                0x04,
                0x12,
                item1,
                item2,
                0xFF
            };
            cmd[5] = CheckSum(cmd);
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SetPeriod(uint period)
        {
            var v = new FooUnion { integer = 0, byte0 = 0x0, byte1 = 0x0, byte2 = 0, byte3 = 0 };
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
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void Init()
        {
            SoftReset();
            GetVersion();
            GetNumberOfChannels();
        }

        public void Reset()
        {
            SoftReset();
        }

        public void Start()
        {

            SetChannel();
            if (_signalType == 0) { SetPulseWaveForm(_pulseWaveData); } else { SetSineWaveForm(_sineWaveData); }
            SetDiscreteMode();
            SetDropsPerTrigger(50);
            SetPeriod(10000);
            SetFrequency();
            SetEnableStrobe();
            GroupTriggerSource(1);
            StartTrigger(1);
            Dump(0);
            Dump(1);
            StartNext();
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
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SoftTrigger()
        {
            _commandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x02, 0x09, 0x0B }, StartData = DateTime.Now });
        }

        private void StartTrigger(byte src)
        {
            var cmd = new byte[] { 0x53, 0x03, 0x08, src, 0x0C };
            cmd[4] = CheckSum(cmd);
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SetDropsPerTrigger(int drops)
        {
            var (item1, item2) = BytesUtility.DivideData((short)drops);
            var cmd = new byte[] {
                0x53,
                0x04,
                0x03,
                item1,
                item2,
                0x08 };
            cmd[5] = CheckSum(cmd);
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SetDiscreteMode()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x04, 0x01, 0x08 };
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void GroupTriggerSource(byte src)
        {
            var cmd = new byte[] { 0x53, 0x03, 0x0E, src, 0xFF };
            cmd[4] = CheckSum(cmd);
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        public void Stop()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x0A, 0x00, 0x03 + 0x0A + 0x00 };
            _port.Write(cmd, 0, 5);
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        public void SetChannel(byte ch)
        {
            _channel = ch;
        }

        private void SetChannel()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x0C, _channel, 0x62 };
            cmd[4] = (byte)(cmd[1] + cmd[2] + cmd[3]);
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void Dump(byte channel)
        {
            var cmd = new byte[] { 0x53, 0x03, 0x60, channel, 0x0 };
            cmd[4] = (byte)(cmd[1] + cmd[2] + cmd[3]);
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        //private void SetStrobeDivider()
        //{
        //    _commandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x03, 0x07, 0x01, 0x0B }, StartData = DateTime.Now });
        //}

        private void SetEnableStrobe()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x10, 0x00, 0x13 };
            cmd[4] = CheckSum(cmd);
            _commandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        //private void SetStrobeDelay()
        //{
        //    _commandList.Add(new DispenserCommandData { CommandString = new byte[] { 0x53, 0x05, 0x13, 0x01, 0x00, 0x00, 0x19 }, StartData = DateTime.Now });
        //}

        public void SetSineWaveData(DispenserSineWaveData data)
        {
            _sineWaveData = data;
        }

        public void SetPulseWaveData(DispenserPulseWaveData data)
        {
            _pulseWaveData = data;
        }

        public void SetSignalType(int signalType)
        {
            _signalType = signalType;
        }

        private static void PortSleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
    }

    internal class DispenserCommandData
    {
        public byte[] CommandString { get; set; }
        public DateTime StartData { get; set; }
    }
}
