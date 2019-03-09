using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace LabProg
{
    internal class LaserSerial
    {
        private readonly SerialPort _mPort;
        private readonly LaserCommand _cmdList;
        private static List<string> _errList;
        private static List<string> _msgList;

        public LaserSerial(string portStr)
        {
            _errList = new List<string>();
            _msgList = new List<string>();
            _cmdList = new LaserCommand();  
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
            _mPort.DataReceived += DataReceivedHandler;
        }

        public void OpenPort()
        {
            _mPort.Open();
            InitLaserCommands();
        }

        public void ClosePort()
        {
            _mPort.Close();
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
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
                _msgList.Add(ascii.GetString(mRxdata));
            }
            catch (Exception ex)
            {
                //Dispatcher.Invoke(() => LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Приложение запущено" }));
                _errList.Add(ex.Message);
            }
        }

        private  void InitLaserCommands()
        {
            SendCommand(_cmdList.GetCommand(1));
            SendCommand(_cmdList.GetCommand(7));
            SendCommand(_cmdList.GetCommand(16));
            SendCommand(_cmdList.GetCommand(21));
            SendCommand(_cmdList.GetCommand(13));
        }

        private  void SendCommand(LaserCommand.LCommand cmd)
        {
            _mPort.Write(cmd.SCommand);
        }
    }
}
