using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrototypeWinFormsUI
{
    public partial class CardControl : UserControl
    {        
        public CardControl()
        {
            InitializeComponent();
        }

        public string CardType
        {
            get => groupBox1.Text;
            set => groupBox1.Text = value;
        }

        public string Description
        {
            get => textBox1.Text;
            set => textBox1.Text = value;
        }
    }
}
