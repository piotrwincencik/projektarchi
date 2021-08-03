using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektSymulatora
{
    class Register
    {
        SRegister H;
        SRegister L;

        public Register(ushort value = 0)
        {
            H = new();
            L = new();
            this.setValue(value);
        }

        public Register(SRegister h, SRegister l)
        {
            H = h;
            L = l;
        }

        public void setValue(ushort val)
        {
            H.Value = (byte)(val >> 8);
            L.Value = (byte)(val & 255);
        }

        public ushort getValue()
        {
            byte[] bytes = new byte[] { L.Value, H.Value };
            return BitConverter.ToUInt16(bytes);
        }

    }
}
