using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabProg
{
    public class Mb
    {

        public byte ReadHolding = 0x03;
        public const byte WriteSingle = 0x06;
        public const byte WriteMultiple = 0x10;
        public byte ReadDiscreteInputs = 0x02;
        public byte WriteSingleBit = 0x05;

        private const ushort MyPolinom = 0xA001;
        private readonly List<byte> _buff;

        public Mb()
        {
            _buff = new List<byte>();
        }

        public byte[] GetMb(byte pumpAdr, byte functionCode, byte[] registerAddr, byte[] operationData)
        {
            _buff.Clear();
            _buff.Add(pumpAdr);
            _buff.Add(functionCode);
            foreach (var bOd in registerAddr.Reverse()) _buff.Add(bOd);
            if (operationData.Length > 2)
            {
                _buff.Add(0x00);
                _buff.Add(0x02);
                _buff.Add((byte)operationData.Length);
                foreach (var bDt in operationData.Reverse()) _buff.Add(bDt);
            }
            else foreach (var bDt in operationData) _buff.Add(bDt);

            var crcMdb = GetCrc(_buff.ToArray());
            _buff.Add((byte)(crcMdb & 0xFF));
            _buff.Add((byte)((crcMdb >> 8) & 0xFF));

            return _buff.ToArray();
        }

        private static ushort GetCrc(IEnumerable<byte> mdbBuf)
        {
            ushort crc = 0xFFFF;
            foreach (var t in mdbBuf)
            {
                var tmpB = t;
                for (var j = 0; j < 8; j++)
                {
                    crc = (ushort)(crc ^ (tmpB & 1));
                    if ((crc & 1) != 0)
                        crc = (ushort)((crc >> 1) ^ MyPolinom);
                    else
                        crc = (ushort)(crc >> 1);
                    tmpB = (byte)(tmpB >> 1);
                }
            }
            return crc;
        }
    }


    public class ModBus
    {
        readonly List<byte> _buff;
        private const ushort MyPolinom = 0xA001;

        public ModBus() => _buff = new List<byte>();

        public byte[] GetFirmCommand(byte com, byte[] data, byte adr)
        {
            _buff.Clear();
            _buff.Add(adr);
            _buff.Add(0x02);
            _buff.Add(com);
            if (data != null)
                foreach (var t in data)
                    _buff.Add(t);
            var crcMdb = GetCrc(_buff.ToArray());
            _buff.Add((byte)(crcMdb & 0xFF));
            _buff.Add((byte)((crcMdb >> 8) & 0xFF));
            //BitConverter.ToString(_buff.ToArray());
            return _buff.ToArray();
        }

        public byte[] GetMaxVolts(int dest, int maxVolts)
        {
            _buff.Clear();
            _buff.Add(0x01);
            _buff.Add(0x10);
            var d = BitConverter.GetBytes((short)dest).Reverse().ToArray();
            _buff.Add(d[0]);
            _buff.Add(d[1]);
            _buff.Add(0x0);
            _buff.Add(0x1);
            _buff.Add(0x2);
            var b = BitConverter.GetBytes((short)maxVolts).Reverse().ToArray();
            _buff.Add(b[0]);
            _buff.Add(b[1]);
            var crcMdb = GetCrc(_buff.ToArray());
            _buff.Add((byte)(crcMdb & 0xFF));
            _buff.Add((byte)((crcMdb >> 8) & 0xFF));
            return _buff.ToArray();
        }

        public byte[] GetBias(int dest, int bias)
        {
            _buff.Clear();
            _buff.Add(0x01);
            _buff.Add(0x10);
            var d = BitConverter.GetBytes((short)dest).Reverse().ToArray();
            _buff.Add(d[0]);
            _buff.Add(d[1]);
            _buff.Add(0x0);
            _buff.Add(0x1);
            _buff.Add(0x2);
            var b = BitConverter.GetBytes((short)bias).Reverse().ToArray();
            _buff.Add(b[0]);
            _buff.Add(b[1]);
            var crcMdb = GetCrc(_buff.ToArray());
            _buff.Add((byte)(crcMdb & 0xFF));
            _buff.Add((byte)((crcMdb >> 8) & 0xFF));
            return _buff.ToArray();
        }

        private static ushort GetCrc(IEnumerable<byte> mdbBuf)
        {
            ushort crc = 0xFFFF;
            foreach (var t in mdbBuf)
            {
                var tmpB = t;
                for (var j = 0; j < 8; j++)
                {
                    crc = (ushort)(crc ^ (tmpB & 1));
                    if ((crc & 1) != 0)
                        crc = (ushort)((crc >> 1) ^ MyPolinom);
                    else
                        crc = (ushort)(crc >> 1);
                    tmpB = (byte)(tmpB >> 1);
                }
            }
            return crc;
        }
    }
}
