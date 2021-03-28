using System;
using System.Collections.Generic;
using System.Text;

namespace laba2
{
    class Icmp
    {
        private const int DATA_SIZE = 28;

        public byte type = 8;
        public byte code = 0;
        public ushort checkSum = 0xFFF7;
        public byte[] data = new byte[DATA_SIZE];

        public byte[] GetPackage()
        {
            byte[] package = new byte[data.Length + 4];

            package[0] = type;
            package[1] = code;
            Buffer.BlockCopy(BitConverter.GetBytes(checkSum), 0, package, 2, 2);
            Buffer.BlockCopy(data, 0, package, 4, data.Length);

            return package;
        }

        public void GetType(byte[] buffer)
        {
            type = buffer[20];
        }
    }
}
