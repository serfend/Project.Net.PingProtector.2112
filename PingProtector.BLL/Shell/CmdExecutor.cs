using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingProtector.BLL.Shell
{
	public static class CmdExecutor
	{
		public static Task<Tuple<string, string>> CmdRunAsync(string title, string str) => Task.FromResult(CmdRun(title, str));

		public static Tuple<string, string> CmdRun(string title, string str)
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo.FileName = "cmd.exe";
			p.StartInfo.UseShellExecute = false;    // 是否使用操作系统shell启动
			p.StartInfo.RedirectStandardInput = true; // 接受来自调用程序的输入信息
			p.StartInfo.RedirectStandardOutput = true; // 由调用程序获取输出信息
			p.StartInfo.RedirectStandardError = true; // 重定向标准错误输出
			p.StartInfo.CreateNoWindow = false; // 不显示程序窗口
			var errors = new StringBuilder();
			p.ErrorDataReceived += (sender, args) =>
			{
				Debug.WriteLine(args.Data);
				errors.AppendLine(args.Data);
			};
			var outputs = new StringBuilder();
			p.OutputDataReceived += (sender, args) =>
			{
				Debug.WriteLine(args.Data);
				outputs.AppendLine(args.Data);
			};
			p.Start();//启动程序
			p.BeginErrorReadLine();
			p.BeginOutputReadLine();
			p.StandardInput.AutoFlush = true;
			var lines = str.Split('\n').Select(l => l.Replace("\r", ""));
			//向cmd窗口发送输入信息
			foreach (var line in lines)
			{
				Debug.WriteLine($"{title}:{line}");
				p.StandardInput.WriteLine(line);
			}

			p.StandardInput.WriteLine("exit");
			//向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
			//同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

			p.WaitForExit(30000);//等待程序执行完退出进程
			p.Close();
			p.Dispose();
			//Debug.WriteLine(outputs);
			return new Tuple<string, string>(title, outputs.ToString());
			//Logger.SysLog(output, "Cmd");
		}
	}
}