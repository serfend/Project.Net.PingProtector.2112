using Configuration.FileHelper;
using Newtonsoft.Json;
using PingProtector.DAL.Entity;

namespace Project.Net.PingProtector._2006.I18n
{
	public class I18nReader : IConfigContent
	{
		public I18nReader() : this("./conf/i18n.dat")
		{
		}

		public I18nReader(string path)
		{
			File = new CiperFile() { Path = path };
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