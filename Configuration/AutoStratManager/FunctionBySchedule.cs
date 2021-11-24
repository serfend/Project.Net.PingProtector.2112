using Microsoft.Win32.TaskScheduler;
using NETworkManager.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Configuration.AutoStratManager

{
	public class FunctionBySchedule : BaseAutoStartManager
	{
		private const string description = "XT2U safeguard team Intelligent Security project";
		private const string name = "safeguard_services";
		private Microsoft.Win32.TaskScheduler.Task task;

		public FunctionBySchedule()
		{
			using (var taskService = new TaskService())
			{
				var tasks = taskService.RootFolder.GetTasks(new Regex(name));
				foreach (var t in tasks)
				{
					if (t.Name == name)
					{
						task = t;
						break;
					}
				}
			}
		}

		public override void Disable()
		{
			if (!IsEnabled()) return;
			using (var taskService = new TaskService())
			{
				taskService.RootFolder.DeleteTask(name);
			}
		}

		public override Task DisableAsync() => Task.Run(Disable);

		public override void Enable()
		{
			if (IsEnabled()) return;
			// Create a new task definition and assign properties
			using (var taskService = new TaskService())
			{
				using (var td = taskService.NewTask())
				{
					td.RegistrationInfo.Description = description;

					// Create a trigger that will fire the task at this time every other day
					td.Triggers.Add(new BootTrigger { });

					// Create an action that will launch Notepad whenever the trigger fires
					td.Actions.Add(new ExecAction(ConfigurationManager.Current.ApplicationFullName));

					// Register the task in the root folder
					taskService.RootFolder.RegisterTaskDefinition(name, td);

					// Remove the task we just created
					// ts.RootFolder.DeleteTask("Test");
				}
			}
		}

		public override Task EnableAsync() => Task.Run(Enable);

		public override bool IsEnabled()
		{
			return task != null;
		}
	}
}