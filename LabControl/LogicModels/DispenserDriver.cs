using LabControl.ClassHelpers;
using LabControl.PortModels;
using System;
using System.Collections.Generic;
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

            var st =  2;
            var TxtStatus = GetStatusText(data[st]);
            Debug.WriteLine(string.Join(":", TxtStatus));
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
                    var tmpStr = data[3] == 38 ? "Запущен" : "Остановлен";
                    SetLogMessage?.Invoke($"{tmpStr} процесс, Status byte:{data[st]}, data:  {BitConverter.ToString(data)}");
                    break;
                case 0x0C:
                    SetLogMessage?.Invoke($"Канал установлен, Status byte:{data[st]}");
                    break;
                case 0x0D:
                    SetLogMessage?.Invoke($"Доступно каналов: {data[4]}, Status byte:{data[2]}");
                    break;
                case 0x0E:
                    SetLogMessage?.Invoke($"Группировка каналов: {data[3]}, Status byte:{data[st]}");
                    break;
                case 0x19:
                    SetLogMessage?.Invoke($"Частота установлена, Status byte:{data[st]}");
                    break;

                case 0xF0:
                    SetLogMessage?.Invoke($"Успешный запрос версии: {data[4]}, Status byte:{data[st]}");
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
            if (data.Length <= 27)
            {
                return;
            }
            // 0 - header
            // 1 - command

            // 2 - Voltage V0(high byte) XXh
            // 3 - Voltage V0(low byte) XXh
            Debug.WriteLine($"Voltage V0: {BytesUtility.JoinByte(data[2], data[3])}");
            // 4 - Rise Time trise1(µs; high byte) XXh
            // 5 - Rise Time trise1(µs; low byte) XXh
            Debug.WriteLine($"Rise Time: {BytesUtility.JoinByte(data[4], data[5])}");
            // 6 - Voltage V1(high byte) XXh
            // 7 - Voltage V1(low byte) XXh
            Debug.WriteLine($"Voltage V1: {BytesUtility.JoinByte(data[6], data[7])}");
            // 8 - Time t1(µs; high byte) XXh
            // 9 - Time t1(µs; low byte) XXh
            Debug.WriteLine($"Time t1: {BytesUtility.JoinByte(data[8], data[9])}");
            // 10 - Fall Time tfall(µs; high byte) XXh
            // 11 - Fall Time tfall(µs; low byte) XXh
            Debug.WriteLine($"Fall Time: {BytesUtility.JoinByte(data[10], data[11])}");
            // 12 - Voltage V2(high byte) XXh
            // 13 - Voltage V2(low byte) XXh
            Debug.WriteLine($"Voltage V2: {BytesUtility.JoinByte(data[12], data[13])}");
            // 14 - Time t2(µs; high byte) XXh
            // 15 - Time t2(µs; low byte) XXh
            Debug.WriteLine($"Time t2: {BytesUtility.JoinByte(data[14], data[15])}");
            // 16 - Final Rise trise2(µs; high byte) XXh
            // 17 - Final Rise trise2(µs; low byte) XXh
            Debug.WriteLine($"Final Rise: {BytesUtility.JoinByte(data[16], data[17])}");
            // 18 - Number of Drops(high byte) XXh
            // 19 - Number of Drops(low byte) XXh
            Debug.WriteLine($"Number of Drops: {BytesUtility.JoinByte(data[18], data[19])}");
            // 20 - Strobe Divider XXh
            Debug.WriteLine($"Strobe Divider: {data[20]}");
            // 21 - Pulse Period(µs; high byte) XXh
            // 22 - Pulse Period(µs; middle byte) XXh
            // 23 - Pulse Period(µs; low byte) XXh
            FooUnion fu;
            fu.integer = 0;
            fu.byte0 = data[23];
            fu.byte1 = data[22];
            fu.byte2 = data[21];
            fu.byte3 = 0;
            Debug.WriteLine($"Pulse Period: {fu.integer}");
            // 24 - Strobe Delay(µs; high byte) XXh
            // 25 - Strobe Delay(µs; low byte) XXh
            Debug.WriteLine($"Number of Drops: {BytesUtility.JoinByte(data[24], data[25])}");
            // 26 - Status XXh
            Debug.WriteLine($"Status: {data[26]}");
            // 27 - Version XXh
            Debug.WriteLine($"Version: {data[27]}");
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

        private List<string> GetStatusText(uint status)
        {
            var res = new List<string>();
            var bts = BytesUtility.GetBytes(status);
            res.Add(bts[0] ? "Jetting" : "Jetting complete");
            res.Add(bts[1] ? "Continuous jetting" : "Finite number of drops are being jetted");
            res.Add(bts[2] ? "External trigger input is used" : "Internal trigger is used");
            res.Add(bts[3] ? "Strobe is enabled" : "Strobe is disabled");
            res.Add(bts[4] ? "One of the input parameters is invalid, inconsistent or out of range" : "All input parameters are valid");
            res.Add(bts[5] ? "The active channel is part of the Group Trigger" : "The active channel is not part of the Group Trigger");

            return res;
        }
    }
}
