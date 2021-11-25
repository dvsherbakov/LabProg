using LabControl.PortModels;

namespace LabControl.LogicModels
{
    internal class LightingDriver
    {
        private YodnSerial _serial;
        private int _uvChannelValue;
        private int _blueChannelValue;
        private int _greenRedChannelValue;

        public bool IsDynamicManageChannels { get; set; }
        public string PortStr { get; set; }

        public LightingDriver()
        {
            _uvChannelValue = 0;
            _blueChannelValue = 0;
            _greenRedChannelValue = 0;
        }

        public void ConnectToPort()
        {
            _serial = new YodnSerial();
            _serial.Init(PortStr);
        }

        public void ClosePort()
        {
            _serial?.Close();
        }

        public void On()
        {
            _serial?.On();
        }

        public void Off()
        {
            _serial?.Off();
        }

        public void SetUvChannel(int value)
        {
            _serial?.SetUvChannel(value);
            _uvChannelValue = value;
        }

        public void SetBlueChannel(int value)
        {
            _serial?.SetBlueChannel(value);
            _blueChannelValue = value;
        }

        public void SetGreenRedChannel(int value)
        {
            _serial?.SetGreenRedChannel(value);
            _greenRedChannelValue = value;
        }

        public void SetGeneralChannel(int value)
        {
            if (IsDynamicManageChannels)
            {
                if (_uvChannelValue > 0 && (_uvChannelValue + value) < 99) _serial?.SetUvChannel(_uvChannelValue + value);
                if (_blueChannelValue > 0 && (_blueChannelValue + value) < 99) _serial?.SetBlueChannel(_blueChannelValue + value);
                if (_greenRedChannelValue > 0 && (_greenRedChannelValue + value) < 99) _serial?.SetGreenRedChannel(_greenRedChannelValue + value);
            }
            else
            {
                SetUvChannel(value);
                SetBlueChannel(value);
                SetGreenRedChannel(value);
            }
        }
    }
}
