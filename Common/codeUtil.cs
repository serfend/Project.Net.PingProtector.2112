using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNet4.Utilities
{
	namespace UtilCode
	{
		internal class Base64
		{
			public static Image Base64ToImage(string code)
			{
				byte[] bytes = Convert.FromBase64String(code);
				MemoryStream memStream = new MemoryStream(bytes);
				return new Bitmap(memStream);
			}

			public static string ImageToBase64(Image img)
			{
				byte[] bytes = ImageToByte(img);
				return Convert.ToBase64String(bytes);
			}

			public static byte[] ImageToByte(Image img)
			{
				BinaryFormatter binFormatter = new BinaryFormatter();
				MemoryStream memStream = new MemoryStream();
				binFormatter.Serialize(memStream, img);
				return memStream.GetBuffer();
			}
		}

		//class Crypt
		//{
		//	#region des实现

		//	/// <summary>
		//	/// Des默认密钥向量
		//	/// </summary>
		//	public static byte[] DesIv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
		//	/// <summary>
		//	/// Des加解密钥必须8位
		//	/// </summary>
		//	public const string DesKey = "ffddg666";
		//	/// <summary>
		//	/// 获取Des8位密钥
		//	/// </summary>
		//	/// <param name="key">Des密钥字符串</param>
		//	/// <returns>Des8位密钥</returns>
		//	static byte[] GetDesKey(string key)
		//	{
		//		if (string.IsNullOrEmpty(key))
		//		{
		//			throw new ArgumentNullException("key", "Des密钥不能为空");
		//		}
		//		if (key.Length > 8)
		//		{
		//			key = key.Substring(0, 8);
		//		}
		//		if (key.Length < 8)
		//		{
		//			// 不足8补全
		//			key = key.PadRight(8, '0');
		//		}
		//		return Encoding.UTF8.GetBytes(key);
		//	}
		//	/// <summary>
		//	/// Des加密
		//	/// </summary>
		//	/// <param name="source">源字符串</param>
		//	/// <param name="key">des密钥，长度必须8位</param>
		//	/// <param name="iv">密钥向量</param>
		//	/// <returns>加密后的字符串</returns>
		//	public static string EncryptDes(string source, string key, byte[] iv)
		//	{
		//		using (DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider())
		//		{
		//			byte[] rgbKeys = GetDesKey(key),
		//				rgbIvs = iv,
		//				inputByteArray = Encoding.UTF8.GetBytes(source);
		//			using (MemoryStream memoryStream = new MemoryStream())
		//			{
		//				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, desProvider.CreateEncryptor(rgbKeys, rgbIvs), CryptoStreamMode.Write))
		//				{
		//					cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
		//					cryptoStream.FlushFinalBlock();
		//					return Convert.ToBase64String(memoryStream.ToArray());;
		//				}
		//			}
		//		}
		//	}
		//	public static string EncryptDes(string source, string key) { return EncryptDes(source, key, DesIv); }
		//	public static string EncryptDes(string source) { return EncryptDes(source, DesKey); }
		//	/// <summary>
		//	/// Des解密
		//	/// </summary>
		//	/// <param name="source">源字符串</param>
		//	/// <param name="key">des密钥，长度必须8位</param>
		//	/// <param name="iv">密钥向量</param>
		//	/// <returns>解密后的字符串</returns>
		//	public static string DecryptDes(string source, string key, byte[] iv)
		//	{
		//		using (DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider())
		//		{
		//			byte[] rgbKeys = GetDesKey(key),
		//				rgbIvs = iv,
		//				inputByteArray = Convert.FromBase64String(source);
		//			using (MemoryStream memoryStream = new MemoryStream())
		//			{
		//				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, desProvider.CreateDecryptor(rgbKeys, rgbIvs), CryptoStreamMode.Write))
		//				{
		//					cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
		//					cryptoStream.FlushFinalBlock();
		//					return Encoding.UTF8.GetString(memoryStream.ToArray());
		//				}
		//			}
		//		}
		//	}
		//	public static string DecryptDes(string source,string key) { return DecryptDes(source, key, DesIv); }
		//	public static string DecryptDes(string source) { return DecryptDes(source, DesKey); }
		//	#endregion

		//	#region aes实现

		//	/// <summary>
		//	/// Aes加解密钥必须32位
		//	/// </summary>
		//	public static string AesKey = "ffddg666ffddg666ffddg666ffddg666";
		//	/// <summary>
		//	/// 获取Aes32位密钥
		//	/// </summary>
		//	/// <param name="key">Aes密钥字符串</param>
		//	/// <returns>Aes32位密钥</returns>
		//	static byte[] GetAesKey(string key)
		//	{
		//		if (string.IsNullOrEmpty(key))
		//		{
		//			throw new ArgumentNullException("key", "Aes密钥不能为空");
		//		}
		//		if (key.Length < 32)
		//		{
		//			// 不足32补全
		//			key = key.PadRight(32, '0');
		//		}
		//		if (key.Length > 32)
		//		{
		//			key = key.Substring(0, 32);
		//		}
		//		return Encoding.UTF8.GetBytes(key);
		//	}
		//	/// <summary>
		//	/// Aes加密
		//	/// </summary>
		//	/// <param name="source">源字符串</param>
		//	/// <param name="key">aes密钥，长度必须32位</param>
		//	/// <returns>加密后的字符串</returns>
		//	public static string EncryptAes(string source, string key)
		//	{
		//		using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
		//		{
		//			aesProvider.Key = GetAesKey(key);
		//			aesProvider.Mode = CipherMode.ECB;
		//			aesProvider.Padding = PaddingMode.PKCS7;
		//			using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor())
		//			{
		//				byte[] inputBuffers = Encoding.UTF8.GetBytes(source);
		//				byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
		//				aesProvider.Clear();
		//				aesProvider.Dispose();
		//				return Convert.ToBase64String(results, 0, results.Length);
		//			}
		//		}
		//	}
		//	public static string EncryptAes(string source) { return EncryptAes(source, AesKey); }
		//	/// <summary>
		//	/// Aes解密
		//	/// </summary>
		//	/// <param name="source">源字符串</param>
		//	/// <param name="key">aes密钥，长度必须32位</param>
		//	/// <returns>解密后的字符串</returns>
		//	public static string DecryptAes(string source, string key)
		//	{
		//		using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
		//		{
		//			aesProvider.Key = GetAesKey(key);
		//			aesProvider.Mode = CipherMode.ECB;
		//			aesProvider.Padding = PaddingMode.PKCS7;
		//			using (ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor())
		//			{
		//				byte[] inputBuffers = Convert.FromBase64String(source);
		//				byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
		//				aesProvider.Clear();
		//				return Encoding.UTF8.GetString(results);
		//			}
		//		}
		//	}
		//	public static string DecryptAes(string source) { return DecryptAes(source, AesKey); }
		//	#endregion
		//}
		public class ZipHelper
		{
			/// <summary>
			/// 解压
			/// </summary>
			/// <param name="Value"></param>
			/// <returns></returns>
			public static DataSet GetDatasetByString(string Value)
			{
				DataSet ds = new DataSet();
				string CC = GZipDecompressString(Value);
				System.IO.StringReader Sr = new StringReader(CC);
				ds.ReadXml(Sr);
				return ds;
			}

			/// <summary>
			/// 根据DATASET压缩字符串
			/// </summary>
			/// <param name="ds"></param>
			/// <returns></returns>
			public static string GetStringByDataset(string ds)
			{
				return GZipCompressString(ds);
			}

			/// <summary>
			/// 将传入字符串以GZip算法压缩后，返回Base64编码字符
			/// </summary>
			/// <param name="rawString">需要压缩的字符串</param>
			/// <returns>压缩后的Base64编码的字符串</returns>
			public static string GZipCompressString(string rawString)
			{
				if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
				{
					return "";
				}
				else
				{
					byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
					byte[] zippedData = Compress(rawData);
					return (string)(Convert.ToBase64String(zippedData));
				}
			}

			/// <summary>
			/// GZip压缩
			/// </summary>
			/// <param name="rawData"></param>
			/// <returns></returns>
			public static byte[] Compress(byte[] rawData)
			{
				MemoryStream ms = new MemoryStream();
				GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
				compressedzipStream.Write(rawData, 0, rawData.Length);
				compressedzipStream.Close();
				return ms.ToArray();
			}

			/// <summary>
			/// 将传入的二进制字符串资料以GZip算法解压缩
			/// </summary>
			/// <param name="zippedString">经GZip压缩后的二进制字符串</param>
			/// <returns>原始未压缩字符串</returns>
			public static string GZipDecompressString(string zippedString)
			{
				if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
				{
					return "";
				}
				else
				{
					byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
					return (string)(System.Text.Encoding.UTF8.GetString(Decompress(zippedData)));
				}
			}

			/// <summary>
			/// ZIP解压
			/// </summary>
			/// <param name="zippedData"></param>
			/// <returns></returns>
			public static byte[] Decompress(byte[] zippedData)
			{
				MemoryStream ms = new MemoryStream(zippedData);
				GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
				MemoryStream outBuffer = new MemoryStream();
				byte[] block = new byte[1024];
				while (true)
				{
					int bytesRead = compressedzipStream.Read(block, 0, block.Length);
					if (bytesRead <= 0)
						break;
					else
						outBuffer.Write(block, 0, bytesRead);
				}
				compressedzipStream.Close();
				return outBuffer.ToArray();
			}
		}

		public class HttpUtil
		{
			public static string GetDomainName(string url)
			{
				if (url == null)
				{
					throw new Exception("输入的url为空");
				}
				Regex reg = new Regex(@"(?<=://)([\w-]+\.)+[\w-]+(?<=/?)");
				return reg.Match(url, 0).Value.Replace("/", string.Empty);
			}

			public static long TimeStamp
			{
				get
				{
					TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
					return Convert.ToInt64(ts.TotalMilliseconds);
				}
			}

			/// <summary>
			/// 随机生成16位的UUID
			/// </summary>
			public static string UUID
			{
				get
				{
					var tmp = new StringBuilder(16);
					var rnd = new Random();
					for (int i = 0; i < 16; i++)
					{
						tmp.Append(rnd.Next(0, 9));
					}
					return tmp.ToString();
				}
			}

			public static string UfUID
			{
				get
				{
					var tmp = new byte[8];
					var rnd = new Random();
					rnd.NextBytes(tmp);
					var cst = new StringBuilder();
					foreach (var c in tmp)
					{
						cst.Append(string.Format("{0:x}", Convert.ToInt32(c)));
					}
					return cst.ToString();
				}
			}

			/// <summary>
			/// 通过MD5CryptoServiceProvider类中的ComputeHash方法直接传入一个FileStream类实现计算MD5
			/// 操作简单，代码少，调用即可
			/// </summary>
			/// <param name="path">文件地址</param>
			/// <returns>MD5Hash</returns>
			public static string GetMD5ByMD5CryptoService(string path)
			{
				if (!File.Exists(path))
					return "0";
				FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
				MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
				byte[] buffer = md5Provider.ComputeHash(fs);
				string result = BitConverter.ToString(buffer);
				result = result.Replace("-", "");
				md5Provider.Clear();
				fs.Close();
				return result;
			}

			/// <summary>
			/// 格式化输出md5
			/// </summary>
			/// <param name="raw">原始数据</param>
			/// <param name="formatModel">格式默认为16进制小写</param>
			/// <returns></returns>
			public static string ToMD5(string raw, string formatModel = "x2")
			{
				MD5 md5 = MD5.Create();
				byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
				var result = new StringBuilder();
				for (int i = 0; i < s.Length; i++)
				{
					result.Append(s[i].ToString(formatModel));
				}
				return result.ToString();
			}

			/// <summary>
			/// Unicode编码
			/// </summary>
			/// <param name="str"></param>
			/// <returns></returns>
			public static string EncodeUnicode(string str)
			{
				StringBuilder strResult = new StringBuilder();
				if (!string.IsNullOrEmpty(str))
				{
					for (int i = 0; i < str.Length; i++)
					{
						strResult.Append("\\u");
						strResult.Append(((int)str[i]).ToString("x"));
					}
				}
				return strResult.ToString();
			}

			private static Regex unicodeRegex = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");

			/// <summary>
			/// Unicode解码
			/// </summary>
			/// <param name="str"></param>
			/// <returns></returns>
			public static string DecodeUnicode(string str)
			{
				//最直接的方法Regex.Unescape(str);

				return unicodeRegex.Replace(str, delegate (Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
			}

			/// <summary>
			/// 将16进制转换成10进制
			/// </summary>
			/// <param name="str">16进制字符串</param>
			/// <returns></returns>
			public static int ConvertToIntBy16(string str)
			{
				try
				{
					return Convert.ToInt32(str, 16);
				}
				catch (Exception)
				{
				}
				return 0;
			}

			public static string GetCurrentPath(string url)
			{
				int index = url.LastIndexOf("/");

				if (index != -1)
				{
					return url.Substring(0, index) + "/";
				}
				else
				{
					return "";
				}
			}


			public static string RemoveElementItem(string iniContent, string item, int beginPos = 0)
			{
				return RemoveElement(iniContent, string.Format("<{0}>", item), string.Format("</{0}>", item), beginPos);
			}

			public static string RemoveElement(string iniContent, string beginContent, string endContent, int beginPos = 0)
			{
				TextPoint point = new TextPoint(iniContent, beginContent, endContent, beginPos);
				if (point.BeginIndex > 0)
					return iniContent.Remove(point.BeginIndex, point.EndIndex - point.BeginIndex);
				else return iniContent;
			}

			public static string GetElement(string iniContent, string beginContent, string endContent, int beginPos = 0)
			{
				TextPoint point = new TextPoint(iniContent, beginContent, endContent, beginPos);
				return point.Info;
			}

			public static string GetElementInItem(string iniContent, string item, int beginPos = 0)
			{
				return GetElement(iniContent, string.Format("<{0}>", item), string.Format("</{0}>", item), beginPos);
			}

			public static IList<string> GetAllElements(string iniContext, string beginContent, string endContent, int maxNum = 0, int beginPos = 0)
			{
				IList<string> result = new List<string>();
				do
				{
					TextPoint p = new TextPoint(iniContext, beginContent, endContent, beginPos);
					if (!p.Init) break;
					result.Add(p.Info);
					beginPos = p.EndIndex;
				} while (true);
				return result;
			}

			public static IList<string> GetLines(string iniContent, string beginLineContain, string endLineContain, int maxNum = 0, int beginPos = 0)
			{
				IList<string> result = new List<string>();

				var lines = iniContent.Split(new string[] { "\r\n" }, StringSplitOptions.None);
				bool findBegin = false;
				int beginLineIndex = 0;
				for (int i = 0; i < lines.Length; i++)
				{
					var line = lines[i];
					if (!findBegin)
					{
						findBegin = line.Contains(beginLineContain);
						if (findBegin) beginLineIndex = i;
					}
					else
					{
						bool getEnd = false;
						if (endLineContain == null)
						{
							getEnd = line.Length <= 1;
						}
						else
						{
							getEnd = line.Contains(endLineContain);
						}
						if (getEnd)
						{
							var cstr = new StringBuilder();
							for (int j = beginLineIndex; j < i; j++)
							{
								cstr.Append(lines[j]).Append('\r').Append('\n');
							}
							cstr.Append(lines[i]);
							result.Add(cstr.ToString());
							findBegin = false;
						}
					}
				}
				return result;
			}

			public static string GetElementLeft(string content, string left)
			{
				return GetElement("#theEnd#" + content, "#theEnd#", left);
			}

			public static string GetElementRight(string content, string right)
			{
				return GetElement(content + "#theEnd#", right, "#theEnd#");
			}

			private class TextPoint
			{
				private int beginIndex, endIndex;
				private bool init;
				private string info;

				public TextPoint(string content, string cutBegin, string cutEnd, int countBegin)
				{
					if (content == null)
						return;
					if (content.Length < cutBegin.Length) return;
					beginIndex = content.IndexOf(cutBegin, countBegin);
					if (beginIndex == -1)
						return;
					else
						beginIndex += cutBegin.Length;
					endIndex = content.IndexOf(cutEnd, beginIndex);
					if (EndIndex > 0)
					{
						Init = true;
						Info = content.Substring(beginIndex, endIndex - beginIndex);
					}
				}

				public TextPoint(string content, string cutBegin, string cutEnd) : this(content, cutBegin, cutEnd, 0)
				{
				}

				public int BeginIndex { get => beginIndex; set => beginIndex = value; }
				public int EndIndex { get => endIndex; set => endIndex = value; }
				public bool Init { get => init; set => init = value; }
				public string Info { get => info; set => info = value; }
			}
		}

		public class WebForm
		{
			private string formName;
			private string formAction;

			/// <summary>
			/// 输入整页,输出目标表单字段
			/// </summary>
			/// <param name="formInfo">原始数据</param>
			/// <param name="targetFormName">表单名称</param>
			/// <param name="targetMatchName">匹配表单方式 reg.Match=> name="[formName]"</param>
			public WebForm(string formInfo, string targetFormName, string targetMatchName = "name")
			{
				IList<string> fromInfos;
				bool findTarget = false;
				string targetAction = "";
				var pageForms = HttpUtil.GetAllElements(formInfo, "<form", "</form>");
				var tarString = targetMatchName + "=\"" + targetFormName + "\"";
				foreach (var form in pageForms)
				{
					if (form.Contains(tarString)) { findTarget = true; tarString = form; targetAction = HttpUtil.GetElement(form, "action=\"", "\""); break; }
				}
				if (!findTarget) tarString = pageForms.Count > 0 ? pageForms[0] : "";
				fromInfos = HttpUtil.GetAllElements(tarString, "<input", ">");
				Build(fromInfos, targetFormName, targetAction);
			}

			private Dictionary<string, string> input;

			public string FormName { get => formName; set => formName = value; }
			public string FormAction { get => formAction; set => formAction = value; }

			public WebForm(IList<string> formInfo, string formName = null, string formAction = null)
			{
				Build(formInfo, formName, formAction);
			}

			private void Build(IList<string> formInfo, string formName = null, string formAction = null)
			{
				input = new Dictionary<string, string>(formInfo.Count);
				foreach (var info in formInfo)
				{
					var key = HttpUtil.GetElement(info, "name=\"", "\"");
					var value = HttpUtil.GetElement(info, "value=\"", "\"");
					if (key != null) input[key] = value;
				}
			}

			public string this[string key]
			{
				get => input[key];
				set => input[key] = value;
			}

			public override string ToString()
			{
				var cstr = new StringBuilder();
				foreach (var info in input)
				{
					cstr.Append(info.Key).Append("=").Append(info.Value).Append("&");
				}
				return cstr.ToString(0, cstr.Length - 1);
			}
		}

		/// <summary>
		/// 加密工具类
		/// </summary>
		public class EncryptHelper
		{
			//默认密钥
			private static string aESKey = "[48/*Serfend.e;]";

			private static string dESKey = "[&Ser75]";
			public static string AESKey { get => aESKey; set => aESKey = value; }
			public static string DESKey { get => dESKey; set => dESKey = value; }

			/// 对字符串进行SHA1加密
			/// </summary>
			/// <param name="Source_String">需要加密的字符串</param>
			/// <returns>密文</returns>
			public static string SHA1_Encrypt(string Source_String)
			{
				byte[] StrRes = Encoding.Default.GetBytes(Source_String);
				HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
				StrRes = iSHA.ComputeHash(StrRes);
				StringBuilder EnText = new StringBuilder();
				foreach (byte iByte in StrRes)
				{
					EnText.AppendFormat("{0:x2}", iByte);
				}
				return EnText.ToString().ToUpper();
			}

			/// <summary>
			/// AES加密
			/// </summary>
			public static string AESEncrypt(string value, string _aeskey = null)
			{
				if (string.IsNullOrEmpty(_aeskey))
				{
					_aeskey = AESKey;
				}

				byte[] keyArray = Encoding.UTF8.GetBytes(_aeskey);
				byte[] toEncryptArray = Encoding.UTF8.GetBytes(value);

				RijndaelManaged rDel = new RijndaelManaged
				{
					Key = keyArray,
					Mode = CipherMode.ECB,
					Padding = PaddingMode.PKCS7
				};

				ICryptoTransform cTransform = rDel.CreateEncryptor();
				byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

				return Convert.ToBase64String(resultArray, 0, resultArray.Length);
			}

			/// <summary>
			/// AES解密
			/// </summary>
			public static string AESDecrypt(string value, string _aeskey = null)
			{
				try
				{
					if (string.IsNullOrEmpty(_aeskey))
					{
						_aeskey = AESKey;
					}
					byte[] keyArray = Encoding.UTF8.GetBytes(_aeskey);
					byte[] toEncryptArray = Convert.FromBase64String(value);

					RijndaelManaged rDel = new RijndaelManaged
					{
						Key = keyArray,
						Mode = CipherMode.ECB,
						Padding = PaddingMode.PKCS7
					};
					ICryptoTransform cTransform = rDel.CreateDecryptor();
					byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

					return Encoding.UTF8.GetString(resultArray);
				}
				catch
				{
					return string.Empty;
				}
			}

			/// <summary>
			/// DES加密
			/// </summary>
			public static string DESEncrypt(string value, string _deskey = null)
			{
				if (string.IsNullOrEmpty(_deskey))
				{
					_deskey = DESKey;
				}

				byte[] keyArray = Encoding.UTF8.GetBytes(_deskey);
				byte[] toEncryptArray = Encoding.UTF8.GetBytes(value);

				DESCryptoServiceProvider rDel = new DESCryptoServiceProvider
				{
					Key = keyArray,
					Mode = CipherMode.ECB,
					Padding = PaddingMode.PKCS7
				};

				ICryptoTransform cTransform = rDel.CreateEncryptor();
				byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

				return Convert.ToBase64String(resultArray, 0, resultArray.Length);
			}

			/// <summary>
			/// DES解密
			/// </summary>
			public static string DESDecrypt(string value, string _deskey = null)
			{
				try
				{
					if (string.IsNullOrEmpty(_deskey))
					{
						_deskey = DESKey;
					}
					byte[] keyArray = Encoding.UTF8.GetBytes(_deskey);
					byte[] toEncryptArray = Convert.FromBase64String(value);

					DESCryptoServiceProvider rDel = new DESCryptoServiceProvider
					{
						Key = keyArray,
						Mode = CipherMode.ECB,
						Padding = PaddingMode.PKCS7
					};

					ICryptoTransform cTransform = rDel.CreateDecryptor();
					byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

					return Encoding.UTF8.GetString(resultArray);
				}
				catch
				{
					return string.Empty;
				}
			}

			public static string MD5(string value)
			{
				byte[] result = Encoding.UTF8.GetBytes(value);
				MD5 md5 = new MD5CryptoServiceProvider();
				byte[] output = md5.ComputeHash(result);
				return BitConverter.ToString(output).Replace("-", "");
			}

			public static string HMACMD5(string value, string hmacKey)
			{
				HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(hmacKey));
				byte[] result = System.Text.Encoding.UTF8.GetBytes(value);
				byte[] output = hmacsha1.ComputeHash(result);

				return BitConverter.ToString(output).Replace("-", "");
			}

			/// <summary>
			/// base64编码
			/// </summary>
			/// <returns></returns>
			public static string Base64Encode(string value)
			{
				string result = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
				return result;
			}

			/// <summary>
			/// base64解码
			/// </summary>
			/// <returns></returns>
			public static string Base64Decode(string value)
			{
				string result = Encoding.UTF8.GetString(Convert.FromBase64String(value));
				return result;
			}
		}
	}
}