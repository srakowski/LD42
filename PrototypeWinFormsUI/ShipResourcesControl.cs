using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LD42;

namespace PrototypeWinFormsUI
{
    public partial class ShipResourcesControl : UserControl
    {
        private Warehouse warehouse;

        public ShipResourcesControl(Warehouse warehouse)
        {
            InitializeComponent();
        }

        private void truckTabPage_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
