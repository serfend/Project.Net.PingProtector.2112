using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.AutoStratManager
{
	public interface IAutoStartManager
	{
		bool IsEnabled();

		void Enable();

		void Disable();
	}
}