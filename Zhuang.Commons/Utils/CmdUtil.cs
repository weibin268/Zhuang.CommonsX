using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Zhuang.Commons.Utils
{
    public class CmdUtil
    {
        public static string Exec(string cmd)
        {
            StringBuilder sbResult=new StringBuilder();

            Process proc = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            proc.StandardInput.AutoFlush = true;
            proc.StandardInput.WriteLine(cmd + " &exit");

            sbResult.Append(proc.StandardOutput.ReadToEnd());

            sbResult.Append(proc.StandardError.ReadToEnd());

            while (!proc.HasExited)
            {
                proc.WaitForExit(1000);
            }
            
            return sbResult.ToString();
        }
    }
}
