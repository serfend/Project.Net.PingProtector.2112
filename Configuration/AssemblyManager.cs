using System.Reflection;

namespace NETworkManager.Settings
{
	public static class AssemblyManager
	{
		public static AssemblyInfomation Current { get; set; }

		static AssemblyManager()
		{
			var assembly = Assembly.GetEntryAssembly();

			var name = assembly.GetName();
			Current = new AssemblyInfomation
			{
				Version = name.Version,
				Location = assembly.Location.Replace(".dll", ".exe"),
				Name = name.Name.Replace(".dll", ".exe")
			};
		}
	}
}