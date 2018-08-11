using System.Windows.Forms;

namespace PrototypeWinFormsUI
{
    public partial class LocationControl : UserControl
    {
        public readonly CheckBox[] resourceChecboxes;

        public bool HasWarehouse
        {
            get => Visible;
            set => Visible = value;
        }

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
