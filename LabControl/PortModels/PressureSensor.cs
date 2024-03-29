﻿using System;
using Modbus.Device;
using System.IO.Ports;

namespace LabControl.PortModels
{
    internal class PressureSensor
    {
        public ModbusSerialMaster ModBus;
        private SerialPort _port;
        public byte AddrId;
        public string PortStr { get; set; }

        public PressureSensor(string portName = "")
        {
            PortStr = portName;
            AddrId = 16;
        }

        public void Open()
        {
            _port = new SerialPort
            {
                PortName = PortStr,
                BaudRate = 9600,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };
            _port.Open();
            ModBus = ModbusSerialMaster.CreateRtu(_port);
        }

        public void Close()
        {
            ModBus.Dispose();
            _port.Close();
        }

        private static float ConvertToFloat(ushort[] buf)
        {
            var bytes = new byte[4];
            bytes[0] = (byte)(buf[1] & 0xFF);
            bytes[1] = (byte)(buf[1] >> 8);
            bytes[2] = (byte)(buf[0] & 0xFF);
            bytes[3] = (byte)(buf[0] >> 8);
            return BitConverter.ToSingle(bytes, 0);
        }

        public float GetPressure()
        {
            return ConvertToFloat(ModBus.ReadHoldingRegisters(AddrId, 2200, 2));
        }

        public float GetTemperature()
        {

            return ConvertToFloat(ModBus.ReadHoldingRegisters(AddrId, 2250, 2));
        }

        public string GetId()
        {
            var source = ModBus.ReadHoldingRegisters(AddrId, 1000, 5);
            var target = new byte[source.Length * 2];
            Buffer.BlockCopy(source, 0, target, 0, source.Length * 2);

            try
            {
                for (var d = 0; d < target.Length / 2; d++)
                {
                    var i = d * 2;
                    var tmp = target[i];
                    target[i] = target[i + 1];
                    target[i + 1] = tmp;
                }
                return System.Text.Encoding.Default.GetString(target);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public bool IsOpen => _port.IsOpen;
    }
}
