using Configuration.FileHelper;
using Microsoft.Extensions.Configuration;
using Project.Net.PingProtector._2006.I18n.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.Net.PingProtector._2006
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
        /// 存储配置
        /// </summary>
        /// <param name="Content"></param>
        public void Save(string Content);
    }
    public class ProjectI18n
    {
        public static ProjectI18n? Default { get; set; }
        public IConfigContent Config { get; set; }
        public I18nSettings I18NSettings { get; set; }
        public ProjectI18n(IConfigContent config)
        {
            Config = config;
            I18NSettings = new I18nSettings(Config.Load() ?? "{}");
            Current = I18NSettings.GetI18N(Guid.NewGuid().ToString());
        }
        public Config GetI18N(string lang) => I18NSettings.GetI18N(lang);
        public Config Current { get; set; }
    }
    public class I18nSettings
    {
        public Dictionary<string, Config> Config { get; set; }

        public I18nSettings(string content)
        {
            Dictionary<string, Config>? result = null;
            try
            {
                if (content != null)
                    result = JsonSerializer.Deserialize<Dictionary<string, Config>>(content);
            }
            catch (Exception)
            {

            }
            Config = result ?? new Dictionary<string, Config>();
        }
        public Config GetI18N(string key)
        {
            if (Config.ContainsKey(key)) return Config[key];
            return Config.FirstOrDefault().Value;
        }
    }
}
