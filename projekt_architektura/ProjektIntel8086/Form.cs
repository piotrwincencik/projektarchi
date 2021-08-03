using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektSymulatora
{
    public partial class Form1 : Form
    {

        static Dictionary<string, SRegister> sregDict = new()
        {
            { "AH", new SRegister() },
            { "AL", new SRegister() },
            { "BH", new SRegister() },
            { "BL", new SRegister() },
            { "CH", new SRegister() },
            { "CL", new SRegister() },
            { "DH", new SRegister() },
            { "DL", new SRegister() }
        };

        Dictionary<string, Register> regDict = new()
        {
            { "AX", new Register(sregDict["AH"], sregDict["AL"]) },
            { "BX", new Register(sregDict["BH"], sregDict["BL"]) },
            { "CX", new Register(sregDict["CH"], sregDict["CL"]) },
            { "DX", new Register(sregDict["DH"], sregDict["DL"]) },
            { "SI", new Register() },
            { "DI", new Register() },
            { "BP", new Register() },
            { "SP", new Register(0xFFFE) },
            { "DISP", new Register() }
        };

        static MemSeg memory = new();
        static MemSeg stack = new();

        bool DI_ON = false;
        bool SI_ON = false;
        bool BX_ON = false;
        bool BP_ON = false;
        bool DISP_ON = false;

        private void SI_Checked(object sender, EventArgs e)
        {
            if (SI_ON)
            {
                SI_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                SI_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }

        private void DI_Checked(object sender, EventArgs e)
        {
            if (DI_ON)
            {
                DI_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                DI_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }

        private void BX_Checked(object sender, EventArgs e)
        {
            if (BX_ON)
            {
                BX_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                BX_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }
        private void BP_Checked(object sender, EventArgs e)
        {
            if (BP_ON)
            {
                BP_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                BP_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }

        private void DISP_Checked(object sender, EventArgs e)
        {
            if (DISP_ON)
            {
                DISP_ON = false;
                ((RadioButton)sender).Checked = false;
            }
            else
            {
                DISP_ON = true;
                ((RadioButton)sender).Checked = true;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }



        private void MOV(object src, object dst)
        {
            if (src is Register && dst is Register)
            {
                ((Register)dst).setValue(((Register)src).getValue());
            }
            else if (src is SRegister && dst is SRegister)
            {
                ((SRegister)dst).Value = ((SRegister)src).Value;
            }
            else if (src is Register && dst is ushort)
            {
                memory.setBytes((ushort)dst, ((Register)src).getValue());
            }
            else if (src is SRegister && dst is ushort)
            {
                memory.setByte((ushort)dst, ((SRegister)src).Value);
            }
            else if (src is ushort && dst is Register)
            {
                ((Register)dst).setValue(memory.getBytes((ushort)src));
            }
            else if (src is ushort && dst is SRegister)
            {
                ((SRegister)dst).Value = memory.getByte((ushort)src);
            }
        }

        private void XCHG(object src, object dst)
        {
            if (src is Register && dst is Register)
            {
                ushort temp = ((Register)dst).getValue();
                ((Register)dst).setValue(((Register)src).getValue());
                ((Register)src).setValue(temp);
            }
            else if (src is SRegister && dst is SRegister)
            {
                byte temp = ((SRegister)dst).Value;
                ((SRegister)dst).Value = ((SRegister)src).Value;
                ((SRegister)src).Value = temp;
            }
            else if (src is Register && dst is ushort)
            {
                ushort temp = memory.getBytes((ushort)dst);
                memory.setBytes((ushort)dst, ((Register)src).getValue());
                ((Register)src).setValue(temp);
            }
            else if (src is SRegister && dst is ushort)
            {
                byte temp = memory.getByte((ushort)dst);
                memory.setByte((ushort)dst, ((SRegister)src).Value);
                ((SRegister)src).Value = temp;
            }
            else if (src is ushort && dst is Register)
            {
                ushort temp = ((Register)dst).getValue();
                ((Register)dst).setValue(memory.getBytes((ushort)src));
                memory.setBytes((ushort)src, temp);
            }
            else if (src is ushort && dst is SRegister)
            {
                byte temp = ((SRegister)dst).Value;
                ((SRegister)dst).Value = memory.getByte((ushort)src);
                memory.setByte((ushort)src, temp);
            }
        }

        private void PUSH(object src)
        {

            ushort sp = regDict["SP"].getValue();

            if (src is Register) stack.setBytes(sp, ((Register)src).getValue());
            if (src is ushort) stack.setBytes(sp, memory.getBytes((ushort)src));

            regDict["SP"].setValue((ushort)(sp - 2));
        }

        private void POP(object dst)
        {
            ushort sp = ((ushort)(regDict["SP"].getValue() + 2));

            if (dst is Register) ((Register)dst).setValue(stack.getBytes(sp));
            if (dst is ushort) memory.setBytes((ushort)dst, stack.getBytes(sp));

            regDict["SP"].setValue((ushort)(sp));
        }

        private void refresh()
        {
            AX_Value.Text = regDict["AX"].getValue().ToString("X4");
            BX_Value.Text = regDict["BX"].getValue().ToString("X4");
            CX_Value.Text = regDict["CX"].getValue().ToString("X4");
            DX_Value.Text = regDict["DX"].getValue().ToString("X4");

            SI_Value.Text = regDict["SI"].getValue().ToString("X4");
            DI_Value.Text = regDict["DI"].getValue().ToString("X4");
            BP_Value.Text = regDict["BP"].getValue().ToString("X4");
            SP_Value.Text = regDict["SP"].getValue().ToString("X4");
            DISP_Value.Text = regDict["DISP"].getValue().ToString("X4");
        }

        private bool isInputValid(string input)
        {
            return (Regex.IsMatch(input, @"^[a-fA-F0-9]+$") && input.Length <= 4);
        }

        private ushort calculateAddress()
        {
            ushort result = 0;

            if (SI_RadioButton.Checked) result += regDict["SI"].getValue();
            if (DI_RadioButton.Checked) result += regDict["DI"].getValue();
            if (BX_RadioButton.Checked) result += regDict["BX"].getValue();
            if (BP_RadioButton.Checked) result += regDict["BP"].getValue();
            if (DISP_RadioButton.Checked) result += regDict["DISP"].getValue();

            return result;
        }

        private object[] getOperandsFromText()
        {
            object[] operands = new object[] { Src_ComboBox.Text, Dst_ComboBox.Text };

            for (int i = 0; i < 2; i++)
            {
                string operand = (string)operands[i];

                if (operand.StartsWith('[') && operand.EndsWith(']'))
                {
                    operand = operand[1..(operand.Length - 1)];
                    if (isInputValid(operand)) operands[i] = Convert.ToUInt16(operand, 16);

                }

                try
                {
                    operands[i] = regDict[operand];
                }
                catch (Exception) { }

                try
                {
                    operands[i] = sregDict[operand];
                }
                catch (Exception) { }

            }

            return operands;
        }

        private void Insert_Button_Click(object sender, EventArgs e)
        {
            if (isInputValid(AX_Insert.Text))
                regDict["AX"].setValue(Convert.ToUInt16(AX_Insert.Text, 16));
            if (isInputValid(BX_Insert.Text))
                regDict["BX"].setValue(Convert.ToUInt16(BX_Insert.Text, 16));
            if (isInputValid(CX_Insert.Text))
                regDict["CX"].setValue(Convert.ToUInt16(CX_Insert.Text, 16));
            if (isInputValid(DX_Insert.Text))
                regDict["DX"].setValue(Convert.ToUInt16(DX_Insert.Text, 16));
            if (isInputValid(SI_Insert.Text))
                regDict["SI"].setValue(Convert.ToUInt16(SI_Insert.Text, 16));
            if (isInputValid(DI_Insert.Text))
                regDict["DI"].setValue(Convert.ToUInt16(DI_Insert.Text, 16));
            if (isInputValid(BP_Insert.Text))
                regDict["BP"].setValue(Convert.ToUInt16(BP_Insert.Text, 16));
            if (isInputValid(SP_Insert.Text))
                regDict["SP"].setValue(Convert.ToUInt16(SP_Insert.Text, 16));
            if (isInputValid(DISP_Insert.Text))
                regDict["DISP"].setValue(Convert.ToUInt16(DISP_Insert.Text, 16));

            refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void To_SrcButton_Click(object sender, EventArgs e)
        {
            Src_ComboBox.Text = $"[{calculateAddress().ToString("X4")}]";
        }

        private void To_DstButton_Click(object sender, EventArgs e)
        {
            Dst_ComboBox.Text = $"[{calculateAddress().ToString("X4")}]";
        }

        private void MOV_Button_Click(object sender, EventArgs e)
        {
            object[] operands = getOperandsFromText();
            MOV(operands[0], operands[1]);
            refresh();
        }

        private void XCHG_Button_Click(object sender, EventArgs e)
        {
            object[] operands = getOperandsFromText();
            XCHG(operands[0], operands[1]);
            refresh();
        }

        private void PUSH_Button_Click(object sender, EventArgs e)
        {
            object[] operands = getOperandsFromText();
            PUSH(operands[0]);
            refresh();
        }

        private void POP_Button_Click(object sender, EventArgs e)
        {
            object[] operands = getOperandsFromText();
            POP(operands[1]);
            refresh();
        }
    }
}
