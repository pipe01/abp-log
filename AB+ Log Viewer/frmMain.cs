using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AB__Log_Viewer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private string[] LastLines = new string[0];

        private string LogPath
        {
            get
            {
                return Config.Inst.LogPath;
            }
            set
            {
                Config.Inst.LogPath = value;
            }
        }

        #region Forms
        private void txtLogPath_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(Path.Combine(txtLogPath.Text, "log.txt")))
            {
                LogPath = txtLogPath.Text;

                ClearAll();

                LoadChanges();
                txtLogPath.ForeColor = Color.Black;
            }
            else
            {
                if (txtLogPath.Text.EndsWith("log.txt"))
                {
                    txtLogPath.Text = Path.GetDirectoryName(txtLogPath.Text);
                }
                else
                {
                    txtLogPath.ForeColor = Color.Red;
                }
            }
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            Config.Load();
            
            ColumnHeader header = new ColumnHeader();
            header.Text = "";
            header.Name = "col1";
            lvLog.HeaderStyle = ColumnHeaderStyle.None;
            lvLog.Columns.Add(header);
            lvLog.AutoResizeColumns(ColumnHeaderAutoResizeStyle.Head‌​erSize);

            txtLogPath.Text = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "My Games\\Binding of Isaac Afterbirth+");

            LoadChanges();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            LoadChanges();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            lvLog.AutoResizeColumns(ColumnHeaderAutoResizeStyle.Head‌​erSize);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            LoadChanges();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (ofdLog.ShowDialog() == DialogResult.OK)
            {
                txtLogPath.Text = ofdLog.FileName;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Config.Save();
        }
        #endregion

        #region Logic
        private void LoadChanges()
        {
            if (LogPath == "")
                return;
            string[] lines = new string[0];
            try
            {
                //string p = "C:/Users/pipe_/Documents/My Games/Binding of Isaac Afterbirth+/log.txt";
                string p = Path.Combine(LogPath, "log.txt");
                using (var stream = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var l = new List<string>();
                        while (!reader.EndOfStream)
                        {
                            l.Add(reader.ReadLine());
                        }
                        lines = l.ToArray();
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Can't access the specified path: Access denied", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!lines.SequenceEqual(LastLines))
            {
                var dif = lines.Except(LastLines);
                for (int i = LastLines.Length; i < lines.Length; i++)
                {
                    string item = lines[i];
                    ListViewItem itm = new ListViewItem(item);
                    bool error = IsErrorLine(item);
                    bool debug = IsDebugLine(item);
                    bool info = IsInfoLine(item);

                    itm.ForeColor = error ? Color.Red : debug ? Color.DarkOrange : Color.Black;

                    if (!((error && !chkError.Checked) || (debug && !chkDebug.Checked) || (info && !chkInfo.Checked)))
                    {
                        lvLog.Items.Add(itm);
                    }
                }

                LastLines = lines;

                if (lvLog.Items.Count > 0)
                    lvLog.Items[lvLog.Items.Count - 1].EnsureVisible();
            }
        }

        public void ClearAll()
        {
            LastLines = new string[0];
            lvLog.Items.Clear();
        }

        public bool IsErrorLine(string line)
        {
            return line.Contains("Error") || line.Contains("error") || line.Contains("ERR");
        }

        public bool IsDebugLine(string line)
        {
            return line.StartsWith("[INFO] - Lua Debug:");
        }

        public bool IsInfoLine(string line)
        {
            return (!IsErrorLine(line)) && (!IsDebugLine(line));
        }

        public bool ShouldSee(string line)
        {
            //return !((IsErrorLine(line) && !chkError.Checked) || (IsDebugLine(line) && !chkDebug.Checked) || (IsInfoLine(line) && !chkInfo.Checked));
            return true;
        }

        private void chkInfo_CheckedChanged(object sender, EventArgs e)
        {
            ClearAll();
            LoadChanges();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            new frmCustomFilters().Show();
        }
    }
}
