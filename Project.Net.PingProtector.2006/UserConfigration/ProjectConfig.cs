using Configuration.FileHelper;
using PingProtector.DAL.Entity;

namespace Project.Net.PingProtector._2006.UserConfigration
{
	public class ProjectConfig<T> where T : IBaseEntity, new()
	{
		/// <summary>
		///
		/// </summary>
		public T Data { get; set; }

		public ProjectConfig(IConfigContent config)
		{
			Data = config.Load<T>();
		}
	}
}