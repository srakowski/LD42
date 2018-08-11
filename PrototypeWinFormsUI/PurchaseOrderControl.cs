using System.Windows.Forms;

namespace PrototypeWinFormsUI
{
    public partial class PurchaseOrderControl : UserControl
    {
        public CardControl CorporationCard
        {
            get => card1;
            set => card1 = value;
        }

        public CardControl SaleCard
        {
            get => card2;
            set => card2 = value;
        }

        public CardControl LocationCard
        {
            get => card3;
            set => card3 = value;
        }

        public PurchaseOrderControl()
        {
            InitializeComponent();
        }
    }
}
