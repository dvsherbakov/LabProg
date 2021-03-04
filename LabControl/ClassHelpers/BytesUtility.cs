using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.ClassHelpers
{
    public static class BytesUtility
    {
        public static short JoinByte(byte hiB, byte lowB)
        {
            return (short)((hiB << 8) | lowB & 0x00FF);
        }

        public static Tuple<byte, byte> DivideData(short number)
        {
            return new Tuple<byte, byte>((byte)(number >> 8), (byte)(number & 0xff));
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct FooUnion
    {
        [FieldOffset(0)]
        public byte byte0;
        [FieldOffset(1)]
        public byte byte1;
        [FieldOffset(2)]
        public byte byte2;
        [FieldOffset(3)]
        public byte byte3;

        [FieldOffset(0)]
        public uint integer;

    }
}
