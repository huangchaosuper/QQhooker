using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace hooker
{
    static class Program
    {
        static KeyboardHook kbh;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 frm = new Form1();
            using (kbh = new KeyboardHook(frm))
            {
                Application.Run(frm);
            }
        }
    }
}
