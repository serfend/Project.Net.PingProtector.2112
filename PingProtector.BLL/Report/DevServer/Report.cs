namespace DevServer
{
	public class Report : ReportRaw
	{
		public string Device { get; set; }
	}

	public class ReportRaw
	{
		/// <summary>
		/// 使用路径
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 内容
		/// </summary>
		public string Message { get; set; }

		public int Operation { get; set; } = 64;

		public ActionRank Rank { get; set; } = ActionRank.Debug;
	}

	public enum ActionRank
	{
		Debug = 32,
		Infomation = 16,
		Warning = 8,
		Danger = 4,
		Disaster = 0
	}
}