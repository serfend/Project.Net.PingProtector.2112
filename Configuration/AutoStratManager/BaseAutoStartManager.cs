using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.AutoStratManager
{
	public abstract class BaseAutoStartManager : IAutoStartManager
	{
		public virtual async Task DisableAsync() => await Task.Run(Disable);

		public virtual async Task EnableAsync() => await Task.Run(Enable);

		public virtual void Disable()
		{
		}

		public virtual void Enable()
		{
		}

		public virtual bool IsEnabled()
		{
			return false;
		}
	}
}