using PingProtector.DAL.Entity.CommonConfig.Global;
using PingProtector.DAL.Entity.CommonConfig.Server;
using Project.Net.PingProtector._2006.I18n;
using Project.Net.PingProtector._2006.I18n.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Net.PingProtector._2006.UserConfigration
{
	public static class UnitOfWork
	{
		public static ProjectConfigI18n<I18NConfigModel> I18N { get; set; } = new(new I18nReader("./conf/i18n.dat"));
		public static ProjectConfig<ServerRecordModel> ServerList { get; set; } = new(new I18nReader("./conf/server-list-config.dat"));
		public static ProjectConfig<GlobalConfigModel> GlobalConfig { get; set; } = new(new I18nReader("./conf/global.dat"));
	}
}