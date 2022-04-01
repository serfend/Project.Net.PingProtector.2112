using Common.Extensions;
using Configuration.FileHelper;
using Newtonsoft.Json;
using PingProtector.DAL.Entity;
using Project.Net.PingProtector._2006.Services;

namespace Project.Net.PingProtector._2006.I18n
{
	public class I18nReader : IConfigContent
	{
		public I18nReader() : this("./conf/i18n.dat")
		{
		}

		public I18nReader(string path)
		{
			File = new CiperFile(path) ;
			File.ErrorOccured += File_ErrorOccured;
		}

		private void File_ErrorOccured(object? sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject as Exception ?? new Exception("无效异常");
			LogServices.ShowMessageBox(IntPtr.Zero, $"{ex.ToSummary()}", "数据加载失败", WinAPI.WTSapi32.DialogStyle.MB_ICONWARNING);
		}

		public CiperFile File { get; set; }

		public string Load()
		{
			return File.Load() ?? "{}";
		}

		public T Load<T>() where T : IBaseEntity, new()
		{
			var content = File.Load() ?? "null";
			return JsonConvert.DeserializeObject<T>(content) ?? DefaultValue<T>();
		}

		public T DefaultValue<T>() where T : IBaseEntity, new()
		{
			return (T)new T().DefaultValue();
		}

		public void Save(string Content)
		{
			File.Content = Content;
			File.Save();
		}
	}
}