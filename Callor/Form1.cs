using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace Callor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!IsAdmin())
            {
                RunAsRestart();
                Environment.Exit(0);
            }
            CenterToScreen();
            Init();
        }
        private bool IsAdmin()
        {
            OperatingSystem osInfo = Environment.OSVersion;
            if (osInfo.Platform == PlatformID.Win32Windows)
            {
                return true;
            }
            else
            {
                WindowsIdentity usrId = WindowsIdentity.GetCurrent();
                WindowsPrincipal p = new WindowsPrincipal(usrId);
                return p.IsInRole(@"BUILTIN\Administrators");
            }
        }

        private bool RunAsRestart()
        {
            string[] args = Environment.GetCommandLineArgs();

            foreach (string s in args)
            {
                if (s.Equals("runas"))
                {
                    return false;
                }
            }
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Application.ExecutablePath;
            startInfo.Verb = "runas";
            startInfo.Arguments = "runas";

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                return false;
            }
            return true;
        }
        private void Init()
        {
            this.textBox1.Text = "";
            this.textBox1.Refresh();

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                //-----------------------------------------------------------------------------
                // Initialize
                //-----------------------------------------------------------------------------
                Ols ols = new Ols();

                // Check support library sutatus
                switch (ols.GetStatus())
                {
                    case (uint)Ols.Status.NO_ERROR:
                        break;
                    case (uint)Ols.Status.DLL_NOT_FOUND:
                        MessageBox.Show("Status Error!! DLL_NOT_FOUND");
                        Environment.Exit(0);
                        break;
                    case (uint)Ols.Status.DLL_INCORRECT_VERSION:
                        MessageBox.Show("Status Error!! DLL_INCORRECT_VERSION");
                        break;
                    case (uint)Ols.Status.DLL_INITIALIZE_ERROR:
                        MessageBox.Show("Status Error!! DLL_INITIALIZE_ERROR");
                        break;
                }

                // Check WinRing0 status
                switch (ols.GetDllStatus())
                {
                    case (uint)Ols.OlsDllStatus.OLS_DLL_NO_ERROR:
                        break;
                    case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_LOADED:
                        MessageBox.Show("DLL Status Error!! OLS_DRIVER_NOT_LOADED");
                        Environment.Exit(0);
                        break;
                    case (uint)Ols.OlsDllStatus.OLS_DLL_UNSUPPORTED_PLATFORM:
                        MessageBox.Show("DLL Status Error!! OLS_UNSUPPORTED_PLATFORM");
                        Environment.Exit(0);
                        break;
                    case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_FOUND:
                        MessageBox.Show("DLL Status Error!! OLS_DLL_DRIVER_NOT_FOUND");
                        Environment.Exit(0);
                        break;
                    case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_UNLOADED:
                        MessageBox.Show("DLL Status Error!! OLS_DLL_DRIVER_UNLOADED");
                        Environment.Exit(0);
                        break;
                    case (uint)Ols.OlsDllStatus.OLS_DLL_DRIVER_NOT_LOADED_ON_NETWORK:
                        MessageBox.Show("DLL Status Error!! DRIVER_NOT_LOADED_ON_NETWORK");
                        Environment.Exit(0);
                        break;
                    case (uint)Ols.OlsDllStatus.OLS_DLL_UNKNOWN_ERROR:
                        MessageBox.Show("DLL Status Error!! OLS_DLL_UNKNOWN_ERROR");
                        Environment.Exit(0);
                        break;
                }

                String str = "";
                
                //-----------------------------------------------------------------------------
                // I/O (Beep)
                //-----------------------------------------------------------------------------
                byte b = ols.ReadIoPortByte(0x60);
                System.Threading.Thread.Sleep(1000);
                str += b.ToString();


                this.textBox1.Text = str;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                Environment.Exit(0);
            }

            Cursor.Current = Cursors.Default;
        }
    }
}
