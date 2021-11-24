using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingProtector.BLL.Shell
{
	public static class ResManager
	{
		public static List<string> ShellFileName { get; set; }

		public static void SetCurrentResPath(string path)
		{
			ShellFileName = new List<string>();
			ShellFileName = Directory.GetFiles(path).Where(f => f.EndsWith(".bat")).ToList();
		}

		public static async Task<IEnumerable<Tuple<string, string>>> RunAll()
		{
			var tasks = ShellFileName
				.Select(f => new Tuple<string, string>(f, File.ReadAllText(f, Encoding.UTF8)))
				.Select(cmd => CmdExecutor.CmdRunAsync(cmd.Item1, cmd.Item2));
			var results = new List<Tuple<string, string>>();
			foreach (var t in tasks) results.Add(await t.ConfigureAwait(true));
			return results;
		}
	}
}