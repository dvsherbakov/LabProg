using LabControl.ClassHelpers;
using LabControl.PortModels;
using System;

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
            //SetLogMessage?.Invoke($"Answer:{BitConverter.ToString(data)}");
            //SetLogMessage?.Invoke($"Status byte:{data[2]}");
            switch (data[1])
            {
                case 0x01:
                    SetLogMessage?.Invoke($"Успешный сброс параметров диспенсера, Status byte:{data[2]}");
                    break;
                case 0x03:
                    SetLogMessage?.Invoke($"Число капель на импульс установлено, Status byte:{data[2]}");
                    break;
                case 0x04:
                    SetLogMessage?.Invoke($"Дискретный режим установлен, Status byte:{data[2]}");
                    break;
                case 0x06:
                    SetLogMessage?.Invoke($"Параметры сигнала установлены, Status byte:{data[2]}");
                    break;
                case 0x08:
                    SetLogMessage?.Invoke($"Внутренний источник импульсов установлен, Status byte:{data[2]}");
                    break;
                case 0x0A:
                    var tmpStr = data[3] == 38 ?  "Запущен" : "Остановлен";
                    SetLogMessage?.Invoke($"{tmpStr} процесс, Status byte:{data[2]}");
                    break;
                case 0x19:
                    SetLogMessage?.Invoke($"Частота установлена, Status byte:{data[2]}");
                    break;
                case 0xF0:
                    SetLogMessage?.Invoke($"Успешный запрос версии: {data[4]}, Status byte:{data[2]}");
                    break;
                case 0x0C:
                    SetLogMessage?.Invoke($"Канал установлен, Status byte:{data[2]}");
                    break;
                case 0x0D:
                    SetLogMessage?.Invoke($"Доступно каналов: {data[4]}, Status byte:{data[2]}");
                    break;
                case 0x60:
                    SetLogMessage?.Invoke($"Port srnded 0n x60: {BitConverter.ToString(data)}, Status byte:{data[2]}");
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
