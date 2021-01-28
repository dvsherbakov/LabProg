using LabControl.ClassHelpers;
using LabControl.PortModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.LogicModels
{
    internal class DispenserDriver
    {
        private DispenserSerial f_DispenserSerial;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;
        public string PortStr { get; set; }

        public void ConnectToPort()
        {
            f_DispenserSerial = new DispenserSerial(PortStr);
            f_DispenserSerial.OpenPort();
            f_DispenserSerial.SetLogMessage += TestLog;
            f_DispenserSerial.DispathRecieveData += DispatchData;
        }

        public void Disconnect()
        {
            f_DispenserSerial.ClosePort();
        }
        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }

        private void DispatchData(byte[] data)
        {
            SetLogMessage?.Invoke($"Получено из порта диспенсера: {BitConverter.ToString(data)}");
            if (!(data.Length > 2 & data[0] == 0x06)) return;
            switch (data[1])
            {
                case 0x60:
                    //SetDispOptions(data);
                    break;
            }
        }

        public void SetSineWaveData(DispenserSineWaveData data)
        {
            f_DispenserSerial.SetSineWaveData(data);
        }

        public void SetPulseWaveData(DispenserPulseWaveData data)
        {
            f_DispenserSerial.SetPulseWaveData(data);
        }

        public void SetSignalType(int signalType)
        {
            f_DispenserSerial.SetSignalType(signalType);
        }

        public void Start()
        {
            f_DispenserSerial.Start();
        }

        public void Stop()
        {
            f_DispenserSerial.Stop();
        }
    }
}
