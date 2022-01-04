using Configuration.FileHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Net.PingProtector._2006.I18n
{
    public class I18nReader : IConfigContent
    {
        public CiperFile File { get; set; } = new CiperFile() { Path = "../conf/i18n.dat" };
        public string Load()
        {
            return File.Load() ?? "{}";
        }

        public void Save(string Content)
        {
            File.Content = Content;
            File.Save();
        }
    }
}
