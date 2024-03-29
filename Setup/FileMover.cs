﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAPI;
using static WinAPI.FileHandlerExtensions;

namespace Setup
{
	public class FileMigrateEventArgs : EventArgs
	{
		public string Name { get; set; }
		public FileStatus FileStatus { get; set; }
		public FileMigrateEventArgs(string name, FileStatus fileStatus)
		{
			Name = name;
			FileStatus = fileStatus;
		}
	}
	public static class FileMoverExtensions
	{
		/// <summary>
		/// 将文件从源目录更新为目标目录
		/// </summary>
		/// <param name="path"></param>
		/// <param name="src"></param>
		/// <param name="dst"></param>
		/// <returns></returns>
		public static string SrcToDstPath(this string path, string src, string dst)
		{
			return path.Replace(src.TrimEnd('\\'), dst.TrimEnd('\\'));
		}
	}
	public class FileMover
	{
		private ILogger<FileMover> logger;
		public event EventHandler<FileMigrateEventArgs>? OnFileMigrate;

		public string SrcPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
		public string DstPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
		public ConcurrentQueue<string> ToMoveFiles { get; set; } = new ConcurrentQueue<string>();
		public ConcurrentQueue<string> WaitToMoveFiles { get; set; } = new ConcurrentQueue<string>();

		/// <summary>
		/// 初始化文件迁移
		/// </summary>
		/// <param name="packageName">包名称</param>
		/// <param name="srcPath">源目录</param>
		/// <param name="dstPath">目标目录(默认为ProgramFiles/包名称)</param>
		public FileMover(ILogger<FileMover> logger)
		{
			this.logger = logger;
		}
		public void Init(string? packageName = null, string? srcPath = null, string? dstPath = null)
		{
			if (!Directory.Exists(DstPath)) Directory.CreateDirectory(DstPath);
			if (srcPath != null) SrcPath = srcPath;
			packageName ??= "UnnamedApp";
			DstPath = Path.Combine(dstPath ?? DstPath, packageName);
		}
		public void MovePath(string path)
		{
			var newFolderPath = path.SrcToDstPath(SrcPath, DstPath);
			if (!Directory.Exists(newFolderPath)) Directory.CreateDirectory(newFolderPath);
			foreach (var p in Directory.GetDirectories(path)) MovePath(p);
			MoveFiles(path);
		}
		public void MoveFiles(string path)
		{
			var files = Directory.GetFiles(path);
			foreach (var file in files)
				ToMoveFiles.Enqueue(file);
		}
		public void MoveFile(string path)
		{
			var newPath = path.SrcToDstPath(SrcPath, DstPath);
			logger.LogInformation($"copy file:{Path.GetFileName(path)} -> {newPath}");
			//new FileInfo(file).Copy(newPath);
			var f = new FileInfo(path);
			var newFile = new FileInfo(newPath);
			var s = newFile.FileCurrentStatus();
			var args = new FileMigrateEventArgs(newPath, s);
			OnFileMigrate?.Invoke(this, args);
			if (s == FileStatus.None || s == FileStatus.NotExist)
				f.CopyTo(newPath, true);
			else
			{
				logger.LogError($"file copy fail[{s}]:{path}");
				ToMoveFiles.Enqueue(path);
			}
		}
		public void CheckProcess()
		{
			var process = Process.GetProcesses();
			var targets = process.Where(i => i.ProcessName == Program.packageName || i.ProcessName == Program.servicesName).ToList();
			foreach (var p in targets)
			{
				logger.LogWarning($"关闭进程[{p.Id}]{p.ProcessName}");
				p.Kill();
			}
		}
		public void Migrate()
		{
			CheckProcess();
			logger.LogWarning($"start migrating:{SrcPath} -> {DstPath}");
			MovePath(SrcPath);
			var tryTimes = 0;
			var maxTryTimes = 10;
			while (ToMoveFiles.Count > 0 && tryTimes++ < maxTryTimes)
			{
				WaitToMoveFiles = ToMoveFiles;
				ToMoveFiles = new ConcurrentQueue<string>();
				while (WaitToMoveFiles.TryDequeue(out var file)) { MoveFile(file); }
				if (ToMoveFiles.Count > 0)
				{
					var desc = $"以上{ToMoveFiles.Count}个文件迁移失败，请检查，第{tryTimes}/{maxTryTimes}次尝试重新迁移...";
					logger.LogError($"------\n{string.Join('\n', ToMoveFiles.ToList())}\n----{desc}");
					for (var i = 0; i < 100; i++) Thread.Sleep(2 * i);
				}
			}
			//new FileInfo(file).Copy(newPath);
			//new DirectoryInfo(SrcPath).Copy(DstPath);
		}
	}
}
