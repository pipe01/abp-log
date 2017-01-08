using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AB__Log_Viewer
{
    public partial class frmCustomFilters : Form
    {
        public frmCustomFilters()
        {
            InitializeComponent();
        }
        
        
        private CustomFilter Selected
        {
            get
            {
                return Config.Inst.CustomFilters[lbFilters.SelectedIndex];
            }
        }

        public void LoadFilters()
        {
            lbFilters.Items.Clear();
            foreach (var item in Config.Inst.CustomFilters)
            {
                lbFilters.Items.Add(item.Name);
            }
        }

        public void LoadSelected()
        {
            var sel = Selected;

            txtRegex.Text = sel.Regex;

            btnColor.BackColor = sel.ForeColor;
            btnBColor.BackColor = sel.BackColor;

            chkColor.Checked = sel.ForeEnable;
            chkBColor.Checked = sel.BackEnable;

            chkVisible.Checked = sel.Visible;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            List<CustomFilter> l = Config.Inst.CustomFilters.ToList();

            var cf = new CustomFilter();
            var ret = Util.ShowInputDialog(ref cf.Name);
            
            if (ret == DialogResult.OK)
            {
                l.Add(cf);
                Config.Inst.CustomFilters = l.ToArray();

                LoadFilters();

                lbFilters.SelectedIndex = l.Count - 1;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            List<CustomFilter> l = Config.Inst.CustomFilters.ToList();
            l.RemoveAt(lbFilters.SelectedIndex);
            Config.Inst.CustomFilters = l.ToArray();

            LoadFilters();
        }

        private void frmCustomFilters_Load(object sender, EventArgs e)
        {
            LoadFilters();
        }

        private void lbFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbFilters.SelectedItems.Count == 1)
            {
                panel1.Enabled = true;
                LoadSelected();

            }
            else
            {
                panel1.Enabled = false;
            }
        }

        private void txtRegex_TextChanged(object sender, EventArgs e)
        {
            Selected.Regex = txtRegex.Text;
        }

        private void chkVisible_CheckedChanged(object sender, EventArgs e)
        {
            Selected.Visible = chkVisible.Checked;
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            colorChooser.Color = btnColor.ForeColor;
            if (colorChooser.ShowDialog() == DialogResult.OK)
            {
                Selected.ForeColor = colorChooser.Color;
                btnColor.BackColor = colorChooser.Color;
            }
        }

        private void btnBColor_Click(object sender, EventArgs e)
        {
            colorChooser.Color = btnColor.BackColor;
            if (colorChooser.ShowDialog() == DialogResult.OK)
            {
                Selected.BackColor = colorChooser.Color;
                btnBColor.BackColor = colorChooser.Color;
            }
        }

        private void chkColor_CheckedChanged(object sender, EventArgs e)
        {
            Selected.ForeEnable = chkColor.Checked;
        }

        private void chkBColor_CheckedChanged(object sender, EventArgs e)
        {
            Selected.BackEnable = chkBColor.Checked;
        }

        private void chkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Selected.Enabled = chkEnabled.Checked;
        }

        private void frmCustomFilters_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmMain.Instance.Reload();
        }
    }
}
