using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingProtector.DAL.Entity.System
{
	public abstract class BaseEntity<T> : ISoftRemove
	{
		public T Key { get; set; }
		public bool IsRemoved { get; set; }
		public DateTime RemoveDate { get; set; }

		public void Remove()
		{
			IsRemoved = true;
			RemoveDate = DateTime.Now;
		}
	}

	public interface ISoftRemove
	{
		void Remove();

		bool IsRemoved { get; set; }
		DateTime RemoveDate { get; set; }
	}
}