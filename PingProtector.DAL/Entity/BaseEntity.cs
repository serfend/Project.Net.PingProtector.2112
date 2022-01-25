using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingProtector.DAL.Entity
{
	public interface IBaseEntity
	{
		/// <summary>
		/// 当本身实体未能被加载时，从程序集初始化默认值
		/// </summary>
		/// <returns></returns>
		object DefaultValue();
	}

	public class BaseEntity : IBaseEntity
	{
		public virtual object DefaultValue() => new();
	}
}