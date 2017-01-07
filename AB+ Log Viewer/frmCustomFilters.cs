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
            var sel = Config.Inst.CustomFilters[lbFilters.SelectedIndex];

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
                panel1.Enabled = false;

            }
            else
            {
                panel1.Enabled = true;
            }
        }

        private void txtRegex_TextChanged(object sender, EventArgs e)
        {
            var sel = Config.Inst.CustomFilters[lbFilters.SelectedIndex];
            sel.Regex = txtRegex.Text;
        }

        private void chkVisible_CheckedChanged(object sender, EventArgs e)
        {
            var sel = Config.Inst.CustomFilters[lbFilters.SelectedIndex];
            sel.Visible = chkVisible.Checked;
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            colorChooser.Color = btnColor.BackColor;
        }
    }
}
