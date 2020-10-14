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
            
            //Set wave form to 3.0/20.0/3.0/40.0/3.0µs, 0.0/10.0/-10.0V
            var cmd = new byte[] { 
                0x53, 
                0x15, 0x06, //len, command
                0xFF, 0xFF, //unused, 
                0x00, 0xC8, //t1, 200
                0xFF,//unused 
                0x01,0x90,//t2, 400
                0x00, 0x00, //v0, 0
                0x00,0x0A, //v1, 10
                0xFF,0xF6, //v2, -10
                0x00, 0x1E, //tr1, 30
                0x00, 0x1E, //tf, 30
                0x00, 0x1E, //tr2,30
                0xCA //checksumm
            };

            var chSum = 0;
            for (int i = 1; i < 22; i++)
            {
                chSum += cmd[i];
            }

            cmd[22] = (byte)(chSum & 0xFF);
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
        
        public void SetChannel(byte channel)
        {
            var cmd = new byte[] { 0x53, 0x03, 0x0C, channel, 0x62 };
            cmd[4] = (byte)(cmd[1] + cmd[2] + cmd[3]);
            _mPort.Write(cmd, 0, 5);
        }

        public void Dump()
        {
            var cmd = new byte[] { 0x53, 0x02, 0x60, 0x62 };
            _mPort.Write(cmd, 0, 4);
        }

        public void Dump(byte channel)
        {
            var cmd = new byte[5] { 0x53, 0x03, 0x60, channel, 0x0 };
            cmd[4] = (byte)(cmd[1] + cmd[2] + cmd[3]);
            _mPort.Write(cmd, 0, 5);
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

        //Вынести в отдельный класс

        private static Int16 JoinByte(byte hiB, byte lowB)
        {
            return (short)((hiB << 8) | lowB & 0x00FF);
        }

        private static Tuple<byte, byte> DivideData(Int16 number)
        {
            return new Tuple<byte, byte>((byte)(number >> 8), (byte)(number & 0xff));
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

/*06 - 60
 * 23-8E
 * 6F-FE mks
 * DF-FE 
 * 6E-FF 
 * 3D - F9
 * 9F - D3
 * DB - 6F
 * DF - 56 
 * CB - 8D 
 * 37 - CC - F7 - C7 - 6E-49 - 08 - 40*/