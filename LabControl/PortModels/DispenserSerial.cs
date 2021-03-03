using System;
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
            if (!(f_CommandList.Count > 0))
            {
                return;
            }
            var cmd = f_CommandList.FirstOrDefault().CommandString;
            _mPort.Write(cmd, 0, cmd.Length);
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
            //_mPort.Write(cmd, 0, 4);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        private void SoftReset()
        {
            if (!_mPort.IsOpen)
            {
                SetLogMessage?.Invoke("Порт диспенсера закрыт");
                return;
            }
            var cmd = new byte[4] { 0x53, 0x02, 0x01, 0x03 };
            //_mPort.Write(cmd, 0, 4);
            //System.Threading.Thread.Sleep(1000);
            f_CommandList.Add(new DispenserCommandData { CommandString = cmd, StartData = DateTime.Now });
        }

        public void GetNumberOfChannels()
        {
            
            var cmd = new byte[4] { 0x53, 0x02, 0x0D, 0x0F };
            //_mPort.Write(cmd, 0, 4);
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
            if (!_mPort.IsOpen)
            {
                SetLogMessage("Порт диспенсера закрыт");
                return;
            }
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
            _mPort.Write(cmd, 0, 10);

        }

        public void SetPulseWaveForm(DispenserPulseWaveData data)
        {
            if (!_mPort.IsOpen)
            {
                SetLogMessage("Порт диспенсера закрыт");
                return;
            }

            if (data == null)
            {
                SetLogMessage("Нет информации о форме сигнала");
                return;
            }
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
            _mPort.Write(cmd, 0, 23);
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
            _mPort.Write(cmd, 0, 6);
        }

        public void SetPeriod()
        {
            var bytes = BitConverter.GetBytes(Frequency);
            //Debug.WriteLine(bytes[0].ToString(), bytes[1].ToString(), bytes[2].ToString());
            var cmd = new byte[]
            {
                0x53,
                0x05,
                0x19,
                0x00,
                0x27,
                0x11,
                0xFF
            };
            cmd[6] = CheckSum(cmd);
            _mPort.Write(cmd, 0, 7);
            PortSleep();
        }

        public void Init()
        {
            SoftReset();
            GetVersion();
            GetNumberOfChannels();
        }

        public void Start()
        {
            if (!_mPort.IsOpen)
            {
                SetLogMessage("Порт диспенсера закрыт");
                return;
            }
            // GetVersion();
            // GetNumberOfChannels();
            SetInternalSource();
            SetDiscreteMode();
            SetDropsPerTrigger(1);

            //SetPeriod();
            // SetFrequency(1);
            // if (f_SignalType == 0) { SetPulseWaveForm(f_PulseWaveData); } else { SetSineWaveForm(f_SineWaveData); }
            // TriggerAll(true);
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
            _mPort.Write(cmd, 0, 5);
            PortSleep();
        }

        private void SetInternalSource()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x08, 0x00, 0x0B };
            _mPort.Write(cmd, 0, 5);
            PortSleep(500);
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
            _mPort.Write(cmd, 0, 5);
            PortSleep();
        }

        private void SetDiscreteMode()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x04, 0x00, 0x07 };
            _mPort.Write(cmd, 0, 5);
            PortSleep(1000);
        }

        public void Stop()
        {
            if (!_mPort.IsOpen)
            {
                SetLogMessage("Порт диспенсера закрыт");
                return;
            }
            var cmd = new byte[] { 0x53, 0x03, 0x0A, 0x00, 0x03 + 0x0A + 0x00 };
            _mPort.Write(cmd, 0, 5);
            PortSleep();
        }

        public void SetChannel(byte channel)
        {
            if (!_mPort.IsOpen)
            {
                SetLogMessage("Порт диспенсера закрыт");
                return;
            }
            var cmd = new byte[] { 0x53, 0x03, 0x0C, channel, 0x62 };
            cmd[4] = (byte)(cmd[1] + cmd[2] + cmd[3]);
            _mPort.Write(cmd, 0, 5);
            PortSleep();
        }

        public void Dump()
        {
            var cmd = new byte[] { 0x53, 0x02, 0x60, 0x62 };
            _mPort.Write(cmd, 0, 4);
            PortSleep();
        }

        public void Dump(byte channel)
        {
            var cmd = new byte[5] { 0x53, 0x03, 0x60, channel, 0x0 };
            cmd[4] = (byte)(cmd[1] + cmd[2] + cmd[3]);
            _mPort.Write(cmd, 0, 5);
            PortSleep();
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

        public void PortSleep()
        {
            System.Threading.Thread.Sleep(100);
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
