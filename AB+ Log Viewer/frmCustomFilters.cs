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
    }
}
