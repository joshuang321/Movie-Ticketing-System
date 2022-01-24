using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace Movie_Ticketing_System
{
    class UnmanagedConsole
    {
        // We would have to get a little hacky if we have to!

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll")]
        public static extern int GetConsoleMode(IntPtr hConsoleMode, IntPtr dwMode);
        [DllImport("kernel32.dll")]
        public static extern int SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

        const int STD_INPUT_HANDLE = -10;
        const int STD_OUTPUT_HANDLE = -11;
        static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
    }
}
