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
    public partial class PlayerActionsControl : UserControl
    {
        public PlayerActionsControl(Action onShip)
        {
            InitializeComponent();
            shipResourcesButton.Click += (s, e) => onShip();
        }
    }
}
