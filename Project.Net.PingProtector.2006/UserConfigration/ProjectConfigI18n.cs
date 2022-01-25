using Configuration.FileHelper;
using Microsoft.Extensions.Configuration;
using PingProtector.DAL.Entity;
using PingProtector.DAL.Entity.CommonConfig.I18N;
using Project.Net.PingProtector._2006.I18n.Model;
using Project.Net.PingProtector._2006.UserConfigration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.Net.PingProtector._2006
{
	public class ProjectConfigI18n<T> : ProjectConfig<I18NDictionary<T>> where T : IBaseEntity, new()
	{
		public ProjectConfigI18n(IConfigContent config) : base(config)
		{
			Current = GetI18N(Guid.NewGuid().ToString()); //将会初始化为首个配置
		}

		public T Current { get; set; }

		public T GetI18N(string lang) => Data.ContainsKey(lang) ? Data[lang] : (T)new T().DefaultValue();
	}
}