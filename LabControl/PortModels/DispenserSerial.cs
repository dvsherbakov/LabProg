using System;
using System.Collections.Generic;
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
        //private readonly Action<string> f_AddLogBoxMessage;
        //private readonly Action<byte[]> f_DispatchData;

        public delegate void DispatchDelegate(byte[] data);
        public event DispatchDelegate DispathRecieveData;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        private DispenserSineWaveData f_SineWaveData;
        private DispenserPulseWaveData f_PulseWaveData;

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
            //this.f_AddLogBoxMessage = addLogBoxMessage;
            //this.f_DispatchData = dispatchData;
        }

        public void OpenPort()
        {
            _mPort.Open();
            Init();
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
            //System.Threading.Thread.Sleep(30);
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
            //f_DispatchData(TrimReceivedData(mRxData));
            DispathRecieveData?.Invoke(mRxData);
        }

        public void GetVersion()
        {
            if (!_mPort.IsOpen)
            {
                SetLogMessage("Порт диспенсера закрыт");
                return;
            }
            var cmd = new byte[4] { 0x53, 0x02, 0xF0, 0xF2 };
            _mPort.Write(cmd, 0, 4);
            System.Threading.Thread.Sleep(50);
        }

        public void SoftReset()
        {
            if (!_mPort.IsOpen)
            {
                SetLogMessage("Порт диспенсера закрыт");
                return;
            }
            var cmd = new byte[4] { 0x53, 0x02, 0x01, 0x03 };
            _mPort.Write(cmd, 0, 4);
            System.Threading.Thread.Sleep(50);
        }

        public void GetNumberOfChannels()
        {
            if (!_mPort.IsOpen)
            {
                SetLogMessage("Порт диспенсера закрыт");
                return;
            }
            var cmd = new byte[4] { 0x53, 0x02, 0x0D, 0x0F };
            _mPort.Write(cmd, 0, 4);
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
            var t1 = BytesUtility.DivideData((short)data.TimeT1);
            var t2 = BytesUtility.DivideData((short)data.TimeT2);
            var v0 = BytesUtility.DivideData((short)(data.V0 * 10));
            var v1 = BytesUtility.DivideData((short)(data.V1 * 10));
            var v2 = BytesUtility.DivideData((short)(data.V2 * 10));
            var tr1 = BytesUtility.DivideData((short)data.TimeRise1);
            var tf = BytesUtility.DivideData((short)data.TimeFall);
            var tr2 = BytesUtility.DivideData((short)data.TimeRise2);

            var cmd = new byte[] {
                0x53,
                0x15, 0x06, //len, command
                0xFF, 0xFF, //unused, 
                t1.Item1, t1.Item2, //0x00, 0xC8, t1, 200
                0xFF,//unused 
                t2.Item1, t2.Item2, //0x01,0x90,//t2, 400
                v0.Item1, v0.Item2, //0x00, 0x00, //v0, 0
                v1.Item1, v1.Item2, //0x00,0x0A, //v1, 10
                v2.Item1, v2.Item2, //0xFF,0xF6, //v2, -10
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

        public void Start()
        {
            if (!_mPort.IsOpen)
            {
                SetLogMessage("Порт диспенсера закрыт");
                return;
            }
            SetInternalSource();
            SetDropsPerTrigger(1);
            SetDiscreteMode();
            TriggerAll(true);
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
            System.Threading.Thread.Sleep(50);
        }

        private void SetInternalSource()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x08, 0x00, 0x0B };
            _mPort.Write(cmd, 0, 5);
            System.Threading.Thread.Sleep(50);
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
            System.Threading.Thread.Sleep(50);
        }

        private void SetDiscreteMode() {
            var cmd = new byte[] { 0x53, 0x03, 0x04, 0x00, 0x07 };
            _mPort.Write(cmd, 0, 5);
            System.Threading.Thread.Sleep(50);
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
            System.Threading.Thread.Sleep(50);
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
            System.Threading.Thread.Sleep(50);
        }

        public void Dump()
        {
            var cmd = new byte[] { 0x53, 0x02, 0x60, 0x62 };
            _mPort.Write(cmd, 0, 4);
            System.Threading.Thread.Sleep(50);
        }

        public void Dump(byte channel)
        {
            var cmd = new byte[5] { 0x53, 0x03, 0x60, channel, 0x0 };
            cmd[4] = (byte)(cmd[1] + cmd[2] + cmd[3]);
            _mPort.Write(cmd, 0, 5);
            System.Threading.Thread.Sleep(50);
        }

        public void Init()
        {
            _mPort.Write("Q");
            System.Threading.Thread.Sleep(50);
            _mPort.Write("X2000");
            System.Threading.Thread.Sleep(50);
            SoftReset();
            
            GetVersion();
           
        }

        public void SetSineWaveData(DispenserSineWaveData data)
        {
            f_SineWaveData = data;
        }

        public void SetPulseWaveData(DispenserPulseWaveData data)
        {
            f_PulseWaveData = data;
        }

    }
}
