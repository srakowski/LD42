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
    public partial class DestinationPickerControl : UserControl
    {
        private Location location;
        private Warehouse warehouse;

        public DestinationPickerControl(Location location, Warehouse wh)
        {
            InitializeComponent();
            this.location = location;
            this.warehouse = wh;

            var destinations = location.Destinations;
            foreach (var d in destinations)
                this.locationBindingSource.Add(d);
        }
    }
}
