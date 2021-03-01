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
            f_DispenserSerial?.ClosePort();
        }
        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }

        private void DispatchData(byte[] data)
        {

            if (!(data.Length > 2 & data[0] == 0x06))
            {
                SetLogMessage?.Invoke($"Вероятная ошибка из порта диспенсера: {BitConverter.ToString(data)}");
                return;
            }
            switch (data[1])
            {
                case 0x01:
                    SetLogMessage?.Invoke($"Успешный сброс параметров диспенсера");
                    break;
                case 0x03:
                    SetLogMessage?.Invoke($"Число капель на импульс установлено");
                    break;
                case 0x04:
                    SetLogMessage?.Invoke($"Дискретный режим установлен");
                    break;
                case 0x06:
                    SetLogMessage?.Invoke($"Параметры сигнала установлены");
                    break;
                case 0x08:
                    SetLogMessage?.Invoke($"Внутренний источник импульсов установлен");
                    break;
                case 0x0A:
                    SetLogMessage?.Invoke($"Запущен процесс");
                    break;
                case 0x19:
                    SetLogMessage?.Invoke($"Частота установлена");
                    break;
                case 0xF0:
                    SetLogMessage?.Invoke($"Успешный запрос версии: {data[5]}");
                    break;
                case 0x0C:
                    SetLogMessage?.Invoke($"Канал установлен");
                    break;
                case 0x0D:
                    SetLogMessage?.Invoke($"Доступно каналов: {data[5]}");
                    break;
                case 0x60:
                    //SetDispOptions(data);
                    break;

            }
        }

        public void SetSineWaveData(DispenserSineWaveData data)
        {
            f_DispenserSerial?.SetSineWaveData(data);
        }

        public void SetPulseWaveData(DispenserPulseWaveData data)
        {
            f_DispenserSerial?.SetPulseWaveData(data);
        }

        public void SetSignalType(int signalType)
        {
            f_DispenserSerial?.SetSignalType(signalType);
        }

        public void Start()
        {
            f_DispenserSerial?.Start();
        }

        public void Stop()
        {
            f_DispenserSerial?.Stop();
        }

        public void SetChannel(int channel)
        {
            f_DispenserSerial?.SetChannel((byte)channel);
        }

        public void SetFrequency(int freq)
        {
            if (f_DispenserSerial != null) f_DispenserSerial.Frequency = freq;
        }
    }
}
