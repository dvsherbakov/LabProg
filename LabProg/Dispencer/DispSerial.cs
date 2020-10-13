using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabProg.Dispencer
{
    class DispSerial
    {
        private readonly SerialPort _mPort;
        private readonly Action<string> f_AddLogBoxMessage;
        private readonly Action<byte[]> f_DispatchData;
        private string portName;
        public string PortName {
            get => portName;
            set { 
                portName = value; 
                if (_mPort.IsOpen)
                {
                    _mPort.Close();
                    _mPort.PortName = portName;
                    _mPort.Open();
                }
                _mPort.PortName = portName;
            }
        }

        public DispSerial(string pName, Action<string> addLogBoxMessage, Action<byte[]> dispathData)
        {
            portName = pName == "" ? "COM6" : pName;
         
            _mPort = new SerialPort(portName)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _mPort.DataReceived += DataReceivedHandler;
            this.f_AddLogBoxMessage = addLogBoxMessage;
            this.f_DispatchData = dispathData;
        }

        public void OpenPort()
        {
            _mPort.Open();
        }

        public void ClosePort()
        {
            _mPort.Close();
        }

        private byte[] TrimRecievedData(byte[] src)
        {
            var res = new List<byte>(src);
            while (res.LastOrDefault() == 0){
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
            }
            catch (Exception ex)
            {
                f_AddLogBoxMessage(ex.Message);
            }
            var ascii = Encoding.ASCII;
            f_DispatchData(TrimRecievedData(mRxData));
        }

        public void GetVersion()
        {
            var cmd = new byte[4] { 0x53, 0x02, 0xF0, 0xF2 };
            _mPort.Write(cmd, 0, 4);
        }

        public void SoftReset()
        {
            var cmd = new byte[4] { 0x53, 0x02, 0x01, 0x03 };
            _mPort.Write(cmd, 0, 4);
        }

        public void GetNumberOfChannels()
        {
            var cmd = new byte[4] { 0x53, 0x02, 0x0D, 0x0F };
            _mPort.Write(cmd, 0, 4);
        }

        public void SetPulseVaveForm()
        {
            
            var cmd = new byte[23] { 0x53, //S
                0x15, //Number of bytes
                0x06, //command
                0x00, //not used
                0x00, //not used
                0x00, //t1 hi
                0x1E, //t1 low
                0x00, //not used
                0x00, //t2 hi
                0x5A, //t2 low
                0x00, //V0 hi
                0x32, //V0 low 
                0x00, //V1 hi
                0x14, //V1 low
                0x00, //V2 hi
                0x50, //V2 low
                0x00, //tr1 hi
                0x0A, //tr1 low
                0x00, //tf hi 
                0x0A, //tf low
                0x00, //tr2 hi
                0x0A, //tr2 low
                0x48 //checkSumm
            };
            var chSum = 0;
            for (int i = 3; i < 23; i++)
            {
                chSum += cmd[i];
            }
            if (chSum > 255) chSum -= 255;
            cmd[23] = (byte)chSum;
            _mPort.Write(cmd, 0, 23);
        }

        public void Start()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x0A, 0x01, 0x03+0x0A+0x01 };
            _mPort.Write(cmd, 0, 5);
        }

        public void Stop()
        {
            var cmd = new byte[] { 0x53, 0x03, 0x0A, 0x00, 0x03+0x0A+0x00 };
            _mPort.Write(cmd, 0, 5);
        }
        
        public void Dump()
        {
            var cmd = new byte[] { 0x53, 0x02, 0x60, 0x62 };
            _mPort.Write(cmd, 0, 4);
        }

        public void Init()
        {
            _mPort.Write("Q");
            System.Threading.Thread.Sleep(50);
            _mPort.Write("X2000");
            System.Threading.Thread.Sleep(50);
            SoftReset();
            System.Threading.Thread.Sleep(50);
            GetVersion();
            System.Threading.Thread.Sleep(50);
            SetPulseVaveForm();
            System.Threading.Thread.Sleep(50);

        }

    }
}
/*
 * 06-60- command
 * 00-00- voltage V0
 * 00-01- Rise tima t1
 * 00-00- voltage V1
 * 00-0A- time t1
 * 00-01- fall time
 * 00-00- voltage v2
 * 00-19- time t2
 * 00-01- final rise time
 * 00-01- number of drops
 * 01- strobble divider
 * 00-01-F5- pulse period
 * 00-00- strobble delay
 * 0E-
 * 40
 * 
 */