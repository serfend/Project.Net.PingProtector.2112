using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingProtector.BLL.Updater.FileUpdater
{
	public interface IFileServerUpdater
	{
		/// <summary>
		/// 开始监听等待服务器的更新
		/// </summary>
		void StartMonitor(bool isStart = true);
		/// <summary>
		/// 是否有更新可用
		/// </summary>
		bool UpdateAvailable { get; set; }
		/// <summary>
		/// 可用的版本
		/// </summary>
		Version AvailableVersion { get; set; }
		/// <summary>
		/// 更新程序启动
		/// </summary>
		void DoUpdate();
	}
}
