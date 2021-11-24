using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilReg;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace DevServer
{
	public class Reporter : IDisposable
	{
		private readonly HttpClient http;

		public static string Host { get; set; } = "https://serfend.top";
		public static string LogPath { get; set; } = "/log/report";
		public static string UserName { get; set; } = "PC";
		private string Uid { get; set; } = new Reg().In("Setting").GetInfo("uid", HttpUtil.UfUID);

		public Reporter()
		{
			http = new HttpClient();
		}

		public HttpResponseMessage Report(string host = null, string logPath = null, Report report = null, string method = "Post")
		{
			if (host == null) host = Host;
			if (!host.StartsWith("http")) host = $"http://{host}";
			if (logPath == null) logPath = LogPath;
			report = report ?? new DevServer.Report();
			if (report.Device == null || report.Device.Length == 0) report.Device = Uid;
			if (report.UserName == null || report.UserName.Length == 0) report.UserName = UserName;
			var str = JsonConvert.SerializeObject(report);

			try
			{
				using (var http = new HttpClient())
				{
					HttpContent content = new StringContent(str);
					content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
					content.Headers.Add("Device", report.Device);
					var msg = new HttpRequestMessage()
					{
						Content = content,
						Method = new HttpMethod(method),
						RequestUri = new Uri($"{host}/{logPath}"),
					};
					var res = http.SendAsync(msg).Result;
					Debug.WriteLine($"report result:{JsonConvert.SerializeObject(res)}");
					return res;
				}
			}
			catch (Exception ex)
			{
				return new HttpResponseMessage(System.Net.HttpStatusCode.BadGateway) { Content = new StringContent($"请求发生异常:{ex.Message}") };
			}
		}

		public void Dispose()
		{
			((IDisposable)http).Dispose();
		}
	}
}