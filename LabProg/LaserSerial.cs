using System;
using System.IO.Ports;
using System.Text;
using System.Collections.Generic;

namespace LabProg
{
    class LaserSerial
    {
        private readonly SerialPort _mPort;
        private bool active = false;
        private LaserCommand lCommand;

        public LaserSerial(string portStr)
        {
           
            if (portStr == "") portStr = "COM2";
            _mPort = new SerialPort(portStr)
            {
                BaudRate = 500000,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            lCommand = new LaserCommand();
            _mPort.DataReceived += DataReceivedHandler;
        }

        public void OpenPort()
        {
            _mPort.Open();
            SendCmd(1);
            SendCmd(4);
            SendCmd(10);
            SendCmd(11);
            SendCmd(12);
            SendCmd(13);

        }

        public void ClosePort()
        {
            _mPort.Close();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var cnt = _mPort.ReadBufferSize;
            var mRxdata = new byte[cnt + 1];
            try
            {
                _mPort.Read(mRxdata, 0, cnt);
            }
            catch (Exception)
            {
                // ignored
            }

            var ascii = Encoding.ASCII;
            var answ = ascii.GetString(mRxdata);
        }

        private void SendCmd(int cmdId)
        {

            _mPort.Write(lCommand.getCmdById(cmdId).SCommand.ToString());
        }
    }
}
