using Configuration.FileHelper;
using System.Text.Json;

namespace CipperGenerator
{
    public partial class FrmMain : Form
    {
        public FrmConfig? Config { get; set; }
        public CiperFile File { get; set; }
        public FrmMain()
        {
            InitializeComponent();
            File = new CiperFile() { Path = Config?.TxtFile };
        }
        private Guid? lastUpdateTxtInput;
        private void TxtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            new Task(() =>
              {
                  var now = Guid.NewGuid();
                  lastUpdateTxtInput = now;
                  Thread.Sleep(1000);
                  if (now != lastUpdateTxtInput) return;
                  File.Content = (sender as TextBox)?.Text;
                  File.Save();
              }).Start();
        }
        private CiperFile frmSettingFile = new CiperFile() { Path = "app.setting.dat" };
        private void FrmMain_Load(object sender, EventArgs e)
        {
            frmSettingFile.Load();
            var c = frmSettingFile.Content;
            Config = JsonSerializer.Deserialize<FrmConfig>((((c?.Length ?? 0) == 0) ? "{}" :c) ?? "{}");
            TxtFile.Text = Config?.TxtFile;
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            BtnSelect.Left = TxtFile.Left + TxtFile.Width + 5;
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            var f = new OpenFileDialog() { };
            if (f.ShowDialog() != DialogResult.OK)
                return;
            TxtFile.Text = f.FileName;
        }
        private Guid? lastFrmSettingUpdate = null;
        private void TxtFile_TextChanged(object sender, EventArgs e)
        {
            if (Config == null) Config = new FrmConfig();
            Config.TxtFile = (sender as TextBox)?.Text;
            File.Path = Config?.TxtFile;
            File.Load();
            TxtInput.Text = File.Content;
            new Task(() =>
            {
                var now = Guid.NewGuid();
                lastFrmSettingUpdate = now;
                Thread.Sleep(1000);
                if (now != lastFrmSettingUpdate) return;
                frmSettingFile.Content = JsonSerializer.Serialize(Config);
                frmSettingFile.Save();
            }).Start();
        }
    }

    public class FrmConfig
    {
        public string? TxtFile { get; set; }
    }
}