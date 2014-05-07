using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace hooker
{
    public class KeyboardHook : IDisposable
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookExW(
            int idHook,
            HookHandlerDelegate lpfn,
            IntPtr hmod,
            uint dwThreadID);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(
            IntPtr idHook,
            int nCode,
            IntPtr wparam,
            ref KBDLLHOOKSTRUCT lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(String modulename);

        public const int WM_KEYDOWN = 0x0100;
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WH_MOUSE_LL = 14;


        private HookHandlerDelegate proc;
        private IntPtr hookID = IntPtr.Zero;


        private Form1 m_F;
        public KeyboardHook(Form1 frm)
        {
            m_F = frm;
            proc = new HookHandlerDelegate(HookCallback);
            using (Process curPro = Process.GetCurrentProcess())
            using (ProcessModule curMod = curPro.MainModule)
            {
                hookID = SetWindowsHookExW(WH_KEYBOARD_LL,
                    proc,
                    GetModuleHandle(curMod.ModuleName),
                    0);
            }
        }

        private IntPtr HookCallback(
            int nCode,
            IntPtr wparam,
            ref KBDLLHOOKSTRUCT lparam)
        {
            if (
               nCode >= 0
               &&
               (wparam == (IntPtr)WM_KEYDOWN
               ||
               wparam == (IntPtr)WM_SYSKEYDOWN)
               )
            {

                //  m_F.SetText((char)lparam.vkCode);     

                string outstr = zhuanhuan(lparam.vkCode).ToString();
                m_F.SetText(outstr);

                return CallNextHookEx(hookID, nCode, wparam, ref lparam);
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(hookID);
        }
        [DllImport("user32.dll", EntryPoint = "GetKeyboardState")]
        public static extern int GetKeyboardState(byte[] pbKeyState);
        public char zhuanhuan(int vKey)
        {
            char ch = '0';

            int iShift = 16;
            int iCapital = 17;
            int iNumLock = 144;
            bool bShift = false;
            bool bCapital = false;

            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                bShift = true;
                //MessageBox.Show("shiftanxia");
            }
            byte[] bs = new byte[256];
            GetKeyboardState(bs);
            if (bs[0x14] == 1)
            {
                bCapital = true;
            }



            bool bNumLock = (iNumLock == 144);

            //if(vKey==32)   //TAB 
            //{ 
            //ch   =   '32'; 
            //return   ch; 
            //} 

            if (vKey == 100)   //回车键 
            {
                ch = '回';
                return ch;
            }
            if (vKey == 32)   //回车键 
            {
                ch = '空';
                return ch;
            }

            if (vKey >= 48 && vKey <= 57)   //数字键0-9 
            {
                if (!bShift)
                {
                    ch = (char)vKey;
                    return ch;
                }
                else
                {
                    switch (vKey)
                    {
                        case 49:
                            ch = '!';
                            break;
                        case 50:
                            ch = '@';
                            break;
                        case 51:
                            ch = '#';
                            break;
                        case 52:
                            ch = '$';
                            break;
                        case 53:
                            ch = '%';
                            break;
                        case 54:
                            ch = '^';
                            break;
                        case 55:
                            ch = '&';
                            break;
                        case 56:
                            ch = '*';
                            break;
                        case 57:
                            ch = '(';
                            break;
                        case 48:
                            ch = ')';
                            break;
                    }
                }
                return ch;
            }
            if (vKey >= 65 && vKey <= 90)   //A-Z   a-z 
            {
                if (!bCapital)
                {
                    if (bShift)
                        ch = (char)vKey;
                    else
                        ch = (char)(vKey + 32);
                }
                else if (bShift)
                    ch = (char)(vKey + 32);
                else
                    ch = (char)vKey;
                return ch;
            }

            if (vKey >= 96 && vKey <= 105)   //小键盘0-9 
            {
                ch = (char)(vKey - 96 + 48);
                return ch;
            }

            if (vKey == 106)
            {
                ch = '*';
                return ch;
            }
            if (vKey == 107)
            {
                ch = '+';
                return ch;
            }
            if (vKey == 111)
            {
                ch = '/';
                return ch;
            }
            if (vKey == 109)
            {
                ch = '-';
                return ch;
            }
            if (vKey == 110)
            {
                if (bNumLock)
                {
                    ch = '.';
                }
                return ch;
            }

            if (vKey >= 186 && vKey <= 222)   //其它键 
            {
                switch (vKey)
                {
                    case 186:
                        if (!bShift) ch = ';';
                        else ch = ':';
                        break;
                    case 187:
                        if (!bShift) ch = '=';
                        else ch = '+';
                        break;
                    case 188:
                        if (!bShift) ch = ',';
                        else ch = '<';
                        break;
                    case 189:
                        if (!bShift) ch = '-';
                        else ch = '_';
                        break;
                    case 190:
                        if (!bShift) ch = '.';
                        else ch = '>';
                        break;
                    case 191:
                        if (!bShift) ch = '/';
                        else ch = '?';
                        break;
                    case 192:
                        if (!bShift) ch = '`';
                        else ch = '~';
                        break;
                    case 219:
                        if (!bShift) ch = '[';
                        else ch = '{';
                        break;
                    case 220:
                        if (!bShift) ch = '\\';
                        else ch = '|';
                        break;
                    case 221:
                        if (!bShift) ch = ']';
                        else ch = '}';
                        break;
                    case 222:
                        if (!bShift) ch = '\'';
                        else ch = '\"';
                        break;
                }
                return ch;
            }
            return ' ';
        }

    }

    public struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }

    public delegate IntPtr HookHandlerDelegate(
        int nCode,
        IntPtr wparam,
        ref KBDLLHOOKSTRUCT lparam);
}

