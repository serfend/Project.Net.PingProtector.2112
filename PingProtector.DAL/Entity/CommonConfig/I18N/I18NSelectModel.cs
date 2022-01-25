using Project.Net.PingProtector._2006.I18n.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingProtector.DAL.Entity.CommonConfig.I18N
{
	public class I18NDictionary<T> : Dictionary<string, T>, IBaseEntity where T : IBaseEntity, new()
	{
		public object DefaultValue()
		{
			return new I18NDictionary<T>()
			{
				{"zh-cn",(T)new T().DefaultValue() }
			};
		}
	}
}