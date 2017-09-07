using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NativeCommand
{
    public class NativeIOCommand
    {
        internal enum INPUT_TYPE : uint
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2
        }
        [Flags]
        public enum MOUSEEVENTF
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal MOUSEEVENTF dwFlags;
            internal uint time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBOARDINPUT
        {
            public ushort Vk;
            public ushort Scan;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        };

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        internal struct INPUT
        {
            [FieldOffset(0)]
            public uint type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBOARDINPUT ki;
        };

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        private static NativeIOCommand _instance;
        private NativeIOCommand(){ }

        public static NativeIOCommand Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NativeIOCommand();
                return _instance;
            }
        }

        public void SendLeftClick(int x, int y)
        {
            double fx = 65535.0 / Screen.PrimaryScreen.Bounds.Width * x;
            double fy = 65535.0 / Screen.PrimaryScreen.Bounds.Height * y;
            INPUT[] input = new INPUT[]
            {
                new INPUT
                {
                    type = (int)INPUT_TYPE.INPUT_MOUSE,
                    mi = new MOUSEINPUT()
                    {
                        dx = (int)fx,
                        dy = (int)fy,
                        dwFlags = MOUSEEVENTF.MOVE|MOUSEEVENTF.ABSOLUTE,
                    }
                },
                new INPUT
                {
                    type = (int)INPUT_TYPE.INPUT_MOUSE,
                    mi = new MOUSEINPUT()
                    {
                        dwFlags = MOUSEEVENTF.LEFTDOWN,
                    }
                },
                new INPUT
                {
                    type = (int)INPUT_TYPE.INPUT_MOUSE,
                    mi = new MOUSEINPUT()
                    {
                        dwFlags = MOUSEEVENTF.LEFTUP,
                    }
                }
            };
            SendInput(3, input, Marshal.SizeOf(typeof(uint)) + Marshal.SizeOf(typeof(MOUSEINPUT)));
        }

        public void SendRightClick(int x, int y)
        {
           /* mouse_event((int)MouseEventFlags.RIGHTDOWN, x, y, 0, 0);
            mouse_event((int)MouseEventFlags.RIGHTUP, x, y, 0, 0);*/
        }

        public void SendKeyboardSequence(List<int> keyCodes)
        {
            List<INPUT> input = new List<INPUT>();
            for (int i = 0; i < keyCodes.Count; i++) {
                var inp = new INPUT
                {
                    type = (int)INPUT_TYPE.INPUT_KEYBOARD
                };
                inp.ki = new KEYBOARDINPUT
                {
                    Vk = (ushort)keyCodes[i]
                };
                input.Add(inp);
            }
            for (int i = keyCodes.Count - 1; i >= 0; i--)
            {
                var inp = new INPUT
                {
                    type = (int)INPUT_TYPE.INPUT_KEYBOARD
                };
                inp.ki = new KEYBOARDINPUT
                {
                    Vk = (ushort)keyCodes[i],
                    Flags = 2
                };
                input.Add(inp);
            }
            SendInput((uint)keyCodes.Count * 2, input.ToArray(), Marshal.SizeOf(typeof(uint)) + Marshal.SizeOf(typeof(MOUSEINPUT)));
        }

        public void SendKeyboard(int keyCode)
        {
            INPUT[] input = new INPUT[]
            {
                new INPUT
                {
                    type = (int)INPUT_TYPE.INPUT_KEYBOARD,
                    ki = new KEYBOARDINPUT
                    {
                        Vk = (ushort)keyCode
                    }
                },
                new INPUT
                {
                    type = (int)INPUT_TYPE.INPUT_KEYBOARD,
                    ki = new KEYBOARDINPUT
                    {
                        Vk = (ushort)keyCode,
                        Flags = 2
                    }
                }
            };
            SendInput(2, input.ToArray(), Marshal.SizeOf(typeof(uint)) + Marshal.SizeOf(typeof(MOUSEINPUT)));
        }
    }
}
