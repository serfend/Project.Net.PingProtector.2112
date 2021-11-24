using Microsoft.EntityFrameworkCore;
using Project.Core.Protector.DAL.Entity.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingProtector.DAL.Data
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<Record> UserActionLogRecord { set; get; }
	}
}