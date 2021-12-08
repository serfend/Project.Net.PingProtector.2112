using Serilog;
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
    internal class FileMover
    {
        public event EventHandler<FileMigrateEventArgs> OnFileMigrate;

        public string SrcPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        public string DstPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        public ConcurrentQueue<string> ToMoveFiles { get; set; } = new ConcurrentQueue<string>();
        /// <summary>
        /// 初始化文件迁移
        /// </summary>
        /// <param name="packageName">包名称</param>
        /// <param name="srcPath">源目录</param>
        /// <param name="dstPath">目标目录(默认为ProgramFiles/包名称)</param>
        public FileMover(string? packageName = null, string? srcPath = null, string? dstPath = null)
        {
            if (srcPath != null) SrcPath = srcPath;
            packageName ??= "UnnamedApp";
            DstPath = Path.Combine(dstPath ?? DstPath, packageName);
            Init();
        }
        private void Init()
        {
            if (!Directory.Exists(DstPath)) Directory.CreateDirectory(DstPath);

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
            Log.Information($"copy file:{Path.GetFileName(path)} -> {newPath}");
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
                Log.Error($"file copy fail[{s}]:{path}");
            }
        }
        public void CheckProcess()
        {
            var process = Process.GetProcesses();
            var targets = process.Where(i => i.ProcessName == Program.packageName || i.ProcessName == Program.servicesName).ToList();
            foreach (var p in targets)
            {
                Log.Warning($"关闭进程[{p.Id}]{p.ProcessName}");
                p.Kill();
            }
        }
        public void Migrate()
        {
            CheckProcess();
            Log.Warning($"start migrating:{SrcPath} -> {DstPath}");
            MovePath(SrcPath);
            while (ToMoveFiles.TryDequeue(out var file)) { MoveFile(file); }
            //new FileInfo(file).Copy(newPath);
            //new DirectoryInfo(SrcPath).Copy(DstPath);
        }
    }
}
