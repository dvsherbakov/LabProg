using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace LabProg
{
    internal class LaserSerial
    {
        private readonly SerialPort _mPort;
        private readonly LaserCommand lCommand;
        private static List<string> _errList;
        private static List<string> _msgList;

        public LaserSerial(string portStr)
        {
            _errList = new List<string>();
            _msgList = new List<string>();
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
            SendCommand(1);
            SendCommand(7);
            SendCommand(16);
            SendCommand(21);
            SendCommand(13);

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
                var answrs = ascii.GetString(mRxdata).Split('\r');
                foreach (var s in answrs)
                {
                    if ((s.Length > 3)&&(s.Length<50))
                    _msgList.Add(s);
                }
            }
            catch (Exception ex)
            {
                //Dispatcher.Invoke(() => windowLogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Приложение запущено" }));
                _errList.Add(ex.Message);
            }
        }

       

        private  void SendCommand(int cmd)
        {
            _mPort.Write(lCommand.GetCmdById(cmd).SCommand);
        }
    }
}
