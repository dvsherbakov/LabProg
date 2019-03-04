using System.IO.Ports;

namespace LabProg
{
    class LaserSerial
    {
        private readonly SerialPort _mPort;
        private bool active = false;

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

        }

    }
}
