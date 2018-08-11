using LD42;
using System.Windows.Forms;

namespace PrototypeWinFormsUI
{
    public partial class LocationControl : UserControl
    {
        private Warehouse _warehouse;

        public readonly CheckBox[] resourceChecboxes;

        public Warehouse Warehouse
        {
            get => _warehouse;
            set
            {
                _warehouse = value;
                Visible = value != null;
            }
        }

        public bool HasWarehouse
        {
            get => Warehouse != null;
        }
        public Location BoardLocation { get; internal set; }

        public LocationControl()
        {
            InitializeComponent();
            Visible = false;

            resourceChecboxes = new CheckBox[]
            {
                checkBox1,
                checkBox2,
                checkBox3,
                checkBox4,
                checkBox5,
                checkBox6,
                checkBox7,
                checkBox8,
                checkBox9,
                checkBox10,
                checkBox11,
                checkBox12,
                checkBox13,
                checkBox14,
                checkBox15,
                checkBox16,
            };

            foreach (var button in resourceChecboxes)
            {
                button.Enabled = false;
                button.Checked = false;
            }
        }
    }
}
