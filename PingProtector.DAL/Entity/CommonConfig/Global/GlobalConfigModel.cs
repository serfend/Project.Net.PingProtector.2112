using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingProtector.DAL.Entity.CommonConfig.Global
{
	public class GlobalConfigModel : BaseEntity
	{
		public string Env { get; set; } = "csw";

		public override object DefaultValue()
		{
			return new GlobalConfigModel()
			{
				Env = "csw"
			};
		}
	}
}