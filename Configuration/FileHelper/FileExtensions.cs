using NETworkManager.Settings;

namespace Common.FileHelper
{
    public static class FileExtensions
    {
        public static string MD5(this string filepath)
        {
            string result = string.Empty;
            var currentPath = ConfigurationManager.Current.ExecutionPath;
            var checkFile = $"{currentPath}\\{new Random().Next(10000, 99999)}";
            File.Copy(filepath, checkFile);
            using (var file = new FileStream(checkFile, FileMode.Open))
            {
                var md5 = System.Security.Cryptography.MD5.Create().ComputeHash(file).Select(i => i.ToString("x2"));
                result = string.Join("", md5);
            }
            File.Delete(checkFile);
            return result;
        }
    }
}
