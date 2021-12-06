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
            var newPath = Path.Combine(DstPath, Path.GetFileName(path));
            Log.Information($"copy file:{newPath}");
            //new FileInfo(file).Copy(newPath);
            var f = new FileInfo(path);
            var newFile = new FileInfo(newPath);
            var s = newFile.FileCurrentStatus();
            var args = new FileMigrateEventArgs(newPath, s);
            OnFileMigrate?.Invoke(this, args);
            if (s == FileStatus.None || s == FileStatus.NotExist)
            {
                f.CopyTo(newPath, true);
            }
            else
            {
                Log.Error($"file copy fail[{s.ToString()}]:{path}");
            }
        }
        public void CheckProcess()
        {
            var process = Process.GetProcesses();
            var targets = process.Where(i => i.ProcessName == Program.packageName).ToList();
            foreach (var p in targets)
            {
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
