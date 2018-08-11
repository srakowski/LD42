using LD42;
using System.Linq;
using System.Windows.Forms;

namespace PrototypeWinFormsUI
{
    public partial class PurchaseOrderPickerForm : Form
    {
        private PickPurchaseOrders ppo;

        public PurchaseOrderPickerForm(PickPurchaseOrders ppo)
        {
            InitializeComponent();
            this.ppo = ppo;

            var po = ppo.PurchaseOrderOptions.ElementAt(0);
            ConfigureCardControl(po.Corporation, purchaseOrderControl1.CorporationCard);
            ConfigureCardControl(po.Sale, purchaseOrderControl1.SaleCard);
            ConfigureCardControl(po.ShipToLocation, purchaseOrderControl1.LocationCard);

            po = ppo.PurchaseOrderOptions.ElementAt(1);
            ConfigureCardControl(po.Corporation, purchaseOrderControl2.CorporationCard);
            ConfigureCardControl(po.Sale, purchaseOrderControl2.SaleCard);
            ConfigureCardControl(po.ShipToLocation, purchaseOrderControl2.LocationCard);

            po = ppo.PurchaseOrderOptions.ElementAt(2);
            ConfigureCardControl(po.Corporation, purchaseOrderControl3.CorporationCard);
            ConfigureCardControl(po.Sale, purchaseOrderControl3.SaleCard);
            ConfigureCardControl(po.ShipToLocation, purchaseOrderControl3.LocationCard);

            po = ppo.PurchaseOrderOptions.ElementAt(3);
            ConfigureCardControl(po.Corporation, purchaseOrderControl4.CorporationCard);
            ConfigureCardControl(po.Sale, purchaseOrderControl4.SaleCard);
            ConfigureCardControl(po.ShipToLocation, purchaseOrderControl4.LocationCard);

            po = ppo.PurchaseOrderOptions.ElementAt(4);
            ConfigureCardControl(po.Corporation, purchaseOrderControl5.CorporationCard);
            ConfigureCardControl(po.Sale, purchaseOrderControl5.SaleCard);
            ConfigureCardControl(po.ShipToLocation, purchaseOrderControl5.LocationCard);
        }

        private CardControl ConfigureCardControl(ICard card, CardControl cc)
        {
            cc.Tag = card;
            cc.CardType = card.GetType().Name;
            cc.Description = card.Description;
            return cc;
        }

        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkBox1.Checked) ppo.AddSelection(ppo.PurchaseOrderOptions.ElementAt(0));
            else ppo.RemoveSelection(ppo.PurchaseOrderOptions.ElementAt(0));
        }

        private void checkBox2_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkBox2.Checked) ppo.AddSelection(ppo.PurchaseOrderOptions.ElementAt(1));
            else ppo.RemoveSelection(ppo.PurchaseOrderOptions.ElementAt(1));
        }

        private void checkBox3_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkBox3.Checked) ppo.AddSelection(ppo.PurchaseOrderOptions.ElementAt(2));
            else ppo.RemoveSelection(ppo.PurchaseOrderOptions.ElementAt(2));
        }

        private void checkBox4_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkBox4.Checked) ppo.AddSelection(ppo.PurchaseOrderOptions.ElementAt(3));
            else ppo.RemoveSelection(ppo.PurchaseOrderOptions.ElementAt(3));
        }

        private void checkBox5_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkBox5.Checked) ppo.AddSelection(ppo.PurchaseOrderOptions.ElementAt(4));
            else ppo.RemoveSelection(ppo.PurchaseOrderOptions.ElementAt(4));
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
