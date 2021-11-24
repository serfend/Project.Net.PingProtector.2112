using PingProtector.DAL.Entity.System;
using System;

namespace Project.Core.Protector.DAL.Entity.Record
{
	public class Record : BaseEntity<int>
	{
		/// <summary>
		/// 记录时间
		/// </summary>
		public DateTime Create { get; set; }

		/// <summary>
		/// 尝试连接的ip
		/// </summary>
		public string TargetIp { get; set; }
	}
}