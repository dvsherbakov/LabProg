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
        private List<byte> _buff { get; set; }

        public DispSerial(string portName, Action<string> addLogBoxMessage)
        {
            if (portName == "")
            {
                portName = "COM6";
            }

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
            _buff = new List<byte>();

        }

        public void OpenPort()
        {
            _mPort.Open();
        }

        public void ClosePort()
        {
            _mPort.Close();
        }

        private byte[] trimRecievedData(byte[] src)
        {
            var res = new List<byte>(src);
            while (res.LastOrDefault() == 0){
                res.RemoveAt(res.Count - 1);
            }
            return res.ToArray();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(30);
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
            f_AddLogBoxMessage(BitConverter.ToString(trimRecievedData(mRxData)));
        }

        public void GetVersion()
        {
            _buff.Clear();
            _buff.Add((byte)'S');
            _buff.Add(0x02); //Number bytes
            _buff.Add(0xF0); //Command
            _buff.Add(0xF2); //Checksum
            _mPort.Write(_buff.ToArray(), 0, _buff.Count);
        }

        public void init()
        {
            _mPort.Write("Q");
            System.Threading.Thread.Sleep(50);
            _mPort.Write("X2000");
        }

    }
}
