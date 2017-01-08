using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AB__Log_Viewer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            Instance = this; //Unsafe, dirty and whatever you want, but meh
        }

        public static frmMain Instance;

        private Dictionary<int,string> lineNumberTimestamp = new Dictionary<int, string>();
        private string[] LastLines = new string[0];
        private int ReadAfterLine = 0;
        private string LogPath
        {
            get { return Config.Inst.LogPath;  }
            set { Config.Inst.LogPath = value; }
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
            
            ColumnHeader timeStampHeader = new ColumnHeader();
            timeStampHeader.Text = "";
            timeStampHeader.Name = "col0";

            ColumnHeader header = new ColumnHeader();
            header.Text = "";
            header.Name = "col1";

            lvLog.HeaderStyle = ColumnHeaderStyle.None;
            lvLog.Columns.Add(timeStampHeader);
            lvLog.Columns.Add(header);

            txtLogPath.Text = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "My Games\\Binding of Isaac Afterbirth+");

            LoadChanges();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            LastLines = new string[0];
            ReadAfterLine = 0;
            ClearAll();
            LoadChanges();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            UpdateColumsWidths();
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

        private void chkInfo_CheckedChanged(object sender, EventArgs e)
        {
            Reload();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new frmCustomFilters().Show();
        }

        private void btnClearLogs_Click(object sender, EventArgs e)
        {
               // As we can't clear the log file. Keep track of where we have currently read upto. And then read after that number
               ReadAfterLine = LastLines.Length;
               lvLog.Items.Clear();
        }

        private void checkBoxTimstamp_CheckedChanged(object sender, EventArgs e)
        {
               UpdateColumsWidths();
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
             
               lvLog.BeginUpdate();
                       
                var dif = lines.Except(LastLines);
                for (int i = LastLines.Length; i < lines.Length; i++)
                {
                    string CurTimestamp = string.Format("[{0}]", DateTime.Now.TimeOfDay.ToString());

                    if(ReadAfterLine > i)
                         continue;

                    if(!lineNumberTimestamp.ContainsKey(i))
                         lineNumberTimestamp.Add(i,CurTimestamp);

                    string item = lines[i];
                    ListViewItem itm = new ListViewItem(lineNumberTimestamp[i]);
                    itm.UseItemStyleForSubItems = false;
                    
                    bool error = IsErrorLine(item);
                    bool debug = IsDebugLine(item);
                    bool info = IsInfoLine(item);
                    bool visible = ShouldSee(item);

                    if (!visible)
                         continue;
                    
                    lvLog.Items.Add(itm);
                    itm.SubItems.Add(item);

                    itm.SubItems[1].ForeColor = error ? Color.Red : debug ? Color.DarkOrange : Color.Black;

                    if (info)
                    {
                        foreach (var filter in Config.Inst.CustomFilters)
                        {
                              if (filter.Enabled && Regex.IsMatch(item, filter.Regex))
                              {
                                   if(!filter.Visible)
                                        itm.Remove();

                                   if (filter.BackEnable)
                                   {
                                        itm.SubItems[1].BackColor = filter.BackColor;
                                   }
                                   if (filter.ForeEnable)
                                   {
                                        itm.SubItems[1].ForeColor = filter.ForeColor;
                                   }
                              }
                        }
                    }

                }
                
                lvLog.EndUpdate();

                UpdateColumsWidths();

                LastLines = lines;
                if (lvLog.Items.Count > 0)
                    lvLog.Items[lvLog.Items.Count - 1].EnsureVisible();
            }
        }

        public void Reload()
        {
            LastLines = new string[ReadAfterLine];
            ClearAll();
            LoadChanges();
        }

        public void ClearAll()
        {
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
            return !((IsErrorLine(line) && !chkError.Checked) || (IsDebugLine(line) && !chkDebug.Checked) || (IsInfoLine(line) && !chkInfo.Checked));
        }

        private void UpdateColumsWidths()
        {
               lvLog.BeginUpdate();
               lvLog.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

               if (!checkBoxTimstamp.Checked)
                    lvLog.Columns[0].Width = 0;
               lvLog.EndUpdate();
        }
        #endregion
        
     }
}
