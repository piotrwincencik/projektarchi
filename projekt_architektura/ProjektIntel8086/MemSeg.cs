using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektSymulatora
{
    class MemSeg
    {
        byte[] memory = new byte[65536];

        public MemSeg()
        {
            for (int i = 0; i < 65536; i++)
            {
                memory[i] = 0x00;
            }
        }

        public void setByte(ushort add, byte b)
        {
            memory[add] = b;
        }

        public byte getByte(ushort add)
        {
            return memory[add];
        }

        public void setBytes(ushort add, ushort val)
        {
                memory[add] = (byte)(val >> 8);
                memory[add + 1] = (byte)(val & 255);
        }

        public ushort getBytes(ushort add)
        {
                byte[] bytes = new byte[] { memory[add + 1], memory[add] };
                return BitConverter.ToUInt16(bytes);
        }


    }
}
