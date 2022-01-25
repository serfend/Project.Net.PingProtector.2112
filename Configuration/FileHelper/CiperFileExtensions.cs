using PingProtector.DAL.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.FileHelper
{
	/// <summary>
	/// 配置文件加载
	/// </summary>
	public interface IConfigContent
	{
		/// <summary>
		/// 读取配置
		/// </summary>
		/// <returns></returns>
		public string Load();

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public T Load<T>() where T : IBaseEntity, new();

		/// <summary>
		/// 存储配置
		/// </summary>
		/// <param name="Content"></param>
		public void Save(string Content);

		/// <summary>
		/// 当加载失败时返回默认值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T DefaultValue<T>() where T : IBaseEntity, new()
		{
			return (T)new T().DefaultValue();
		}
	}

	public class CiperFile
	{
		public event EventHandler<UnhandledExceptionEventArgs>? ErrorOccured;

		/// <summary>
		/// 路径
		/// </summary>
		public string? Path { get; set; }

		/// <summary>
		/// 密钥
		/// </summary>
		public byte[] Key { get; set; } = new byte[] { 0xde, 0xad, 0xbe, 0xef, 0x00, 0x01, 0x02, 0x03, 0xbe, 0xef, 0x00, 0x01, 0x01, 0x02, 0x03, 0xbe };

		public byte[] IV { get; set; } = new byte[] { 0xde, 0xad, 0xbe, 0xef, 0x00, 0x01, 0x02, 0x03, 0xbe, 0xef, 0x00, 0x01, 0x01, 0x02, 0x03, 0xbe };
		public string? Content { set; get; }

		public string? Load()
		{
			if (!File.Exists(Path)) return null;
			try
			{
				using (Aes aesAlg = Aes.Create())
				{
					aesAlg.Key = Key;
					aesAlg.IV = IV;

					ICryptoTransform encryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
					using var fs = new FileStream(Path, FileMode.Open);
					using var cs = new CryptoStream(fs, encryptor, CryptoStreamMode.Read);
					using var sr = new StreamReader(cs);
					Content = sr.ReadToEnd();
					return Content;
				}
			}
			catch (Exception ex)
			{
				ErrorOccured?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
			}
			return null;
		}

		public void Save()
		{
			if (Path == null) return;

			File.WriteAllText(Path, String.Empty);
			try
			{
				using (Aes aesAlg = Aes.Create())
				{
					aesAlg.Key = Key;
					aesAlg.IV = IV;
					ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
					using var fs = new FileStream(Path, FileMode.Open);
					using var cs = new CryptoStream(fs, encryptor, CryptoStreamMode.Write);
					using var sw = new StreamWriter(cs);
					sw.Write(Content);
				}
			}
			catch (Exception ex)
			{
				ErrorOccured?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
			}
		}
	}
}