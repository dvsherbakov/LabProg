using LabControl.ClassHelpers;
using LabControl.PortModels;
using System;
using System.Diagnostics;

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
            var st = data.Length - 2;
            switch (data[1])
            {
                case 0x01:
                    SetLogMessage?.Invoke($"Успешный сброс параметров диспенсера, Status byte:{data[st]}");
                    break;
                case 0x03:
                    SetLogMessage?.Invoke($"Число капель на импульс установлено, Status byte:{data[st]}");
                    break;
                case 0x04:
                    SetLogMessage?.Invoke($"Дискретный режим установлен, Status byte:{data[st]}");
                    break;
                case 0x06:
                    SetLogMessage?.Invoke($"Параметры сигнала установлены, Status byte:{data[st]}");
                    break;
                case 0x08:
                    SetLogMessage?.Invoke($"Внутренний источник импульсов установлен, Status byte:{data[st]}");
                    break;
                case 0x0A:
                    var tmpStr = data[3] == 38 ?  "Запущен" : "Остановлен";
                    SetLogMessage?.Invoke($"{tmpStr} процесс, Status byte:{data[st]}");
                    break;
                case 0x19:
                    SetLogMessage?.Invoke($"Частота установлена, Status byte:{data[st]}");
                    break;
                case 0xF0:
                    SetLogMessage?.Invoke($"Успешный запрос версии: {data[4]}, Status byte:{data[st]}");
                    break;
                case 0x0C:
                    SetLogMessage?.Invoke($"Канал установлен, Status byte:{data[st]}");
                    break;
                case 0x0D:
                    SetLogMessage?.Invoke($"Доступно каналов: {data[4]}, Status byte:{data[st]}");
                    break;
                case 0x60:
                    SetLogMessage?.Invoke($"Port was sent {data.Length}, x60: {BitConverter.ToString(data)}");
                    SetDispOptions(data);
                    break;
                default:
                    SetLogMessage?.Invoke($"NoHandler: {BitConverter.ToString(data)}");
                    break;

            }
        }

        private void SetDispOptions(byte[] data)
        {
            var v0 = BitConverter.ToInt16(data, 1);
            Debug.Write($"v0:{v0}, ");
            var rt1 = BitConverter.ToInt16(data, 2);
            Debug.Write($"rt1:{rt1}, ");
            var v1 = BitConverter.ToInt16(data, 3);
            Debug.Write($"v1:{v1}, ");
            var t1 = BitConverter.ToInt16(data, 4);
            Debug.Write($"t1:{t1}, ");
            var ft = BitConverter.ToInt16(data, 5);
            Debug.Write($"ft:{ft}, ");
            var v2 = BitConverter.ToInt16(data, 6);
            Debug.Write($"v2:{v2}, ");
            var t2 = BitConverter.ToInt16(data, 7);
            Debug.Write($"t2:{t2}, ");
            var rt2 = BitConverter.ToInt16(data, 8);
            Debug.Write($"rt2:{rt2}, ");
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
