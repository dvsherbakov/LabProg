using LabControl.PortModels;

namespace LabControl.LogicModels
{
    internal class LightingDriver
    {
        private YodnSerial _serial;
        public bool IsDynamicManageChannels { get; set; }
        public string PortStr { get; set; }

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
        }

        public void SetBlueChannel(int value)
        {
            _serial?.SetBlueChannel(value);
        }

        public void SetGreenRedChannel(int value)
        {
            _serial?.SetGreenRedChannel(value);
        }


    }
}
