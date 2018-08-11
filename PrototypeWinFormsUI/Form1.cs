using LD42;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PrototypeWinFormsUI
{
    public partial class Form1 : Form
    {
        private LocationControl[] locationControls;
        private PurchaseOrderControl[] purchaseOrderControls;

        private MiningLogisticsGame game;
        private IEnumerator play;

        private PlayerActionRequest playerActionRequest;

        private Control MessageControl
        {
            get => mapAndMessageContainer.Panel2.Controls.Count > 0 ? mapAndMessageContainer.Panel2.Controls[0] : null;
            set
            {
                mapAndMessageContainer.Panel2.Controls.Clear();
                if (value != null)
                    mapAndMessageContainer.Panel2.Controls.Add(value);
            }
        }

        public Form1()
        {
            InitializeComponent();

            locationControls = new[]
            {
                seattleLocation,
                renoLocation,
                lostAngelesLocation,
                saltLakeCityLocation,
                tusconLocation,
                billingsLocation,
                denverLocation,
                duluthLocation,
                kansasCityLocation,
                houstonLocation,
                newOrleansLocation,
                jacksonvilleLocation,
                bostonLocation,
                knoxvilleLocation,
                indianaoplisLocation,
                detroitLocation,
            };

            foreach (var lc in locationControls)
            {
                lc.Click += location_Click;
            }

            purchaseOrderControls = new[]
            {
                purchaseOrder1,
                purchaseOrder2,
                purchaseOrder3,
            };

            game = new MiningLogisticsGame();
            play = game.Play().GetEnumerator();
            
        }

        private void label1_Click(object sender, EventArgs e) {}

        private void Form1_Load(object sender, EventArgs e) { }

        private void Next()
        {
            play.MoveNext();
            var mineLocations = game.GameBoard.Locations.Where(l => !(l.Occupant is EmptyLocation));
            foreach (var loc in mineLocations)
            {
                var control = locationControls.Where(l => l.Tag == loc.Name).FirstOrDefault();
                var wh = loc.Occupant is Mine m ? m.Storage :
                    loc.Occupant is Warehouse w ? w :
                    null;
                control.BoardLocation = loc;
                control.Warehouse = wh;

                foreach (var cb in control.resourceChecboxes)
                {
                    cb.BackColor = Color.Transparent;
                }

                int i = 0;
                foreach (var rs in wh.UnitsOfResources)
                {
                    control.resourceChecboxes[i].Checked = true;
                    var color = rs is Resource.Copper ? Color.Orange :
                        rs is Resource.Iron ? Color.DarkRed :
                        rs is Resource.Silver ? Color.Blue :
                        rs is Resource.Zinc ? Color.LightGray :
                        Color.Black;
                    control.resourceChecboxes[i].BackColor = color;
                    control.resourceChecboxes[i].ForeColor = color;
                    i++;
                }
            }

            this.MessageControl = null;

            if (play.Current is ShowCard sc)
            {
                var cc = new CardControl();
                cc.CardType = sc.Card.GetType().Name;
                cc.Description = sc.Card.Description;
                if (play.Current is ShowAcceptedCard)
                {
                    cc.BackColor = Color.Green;
                }
                else if (play.Current is ShowRejectedCard)
                {
                    cc.BackColor = Color.Red;
                }
                MessageControl = cc;
            }

            if (play.Current is PickPurchaseOrders ppo)
            {
                var form = new PurchaseOrderPickerForm(ppo);
                form.ShowDialog();
                play.MoveNext();
            }

            if (play.Current is PlayerActionRequest par)
            {
                playerActionRequest = par;
                button1.Visible = false;
                MessageControl = new Label { Text = par.Description };
            }

            for (int i = 0; i < game.GameBoard.ActivePurchaseOrders.Length; i++)
            {
                var po = game.GameBoard.ActivePurchaseOrders[i];
                if (po == null) continue;

                ConfigureCardControl(po.Corporation, purchaseOrderControls[i].CorporationCard);
                ConfigureCardControl(po.Sale, purchaseOrderControls[i].SaleCard);
                ConfigureCardControl(po.ShipToLocation, purchaseOrderControls[i].LocationCard);
            }
        }

        private CardControl ConfigureCardControl(ICard card, CardControl cc)
        {
            cc.Tag = card;
            cc.CardType = card.GetType().Name;
            cc.Description = card.Description;
            return cc;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Next();
        }

        private void location_Click(object sender, EventArgs e)
        {
            if (playerActionRequest == null) return;
            var lc = sender as LocationControl;
            if (!lc.HasWarehouse) return;
            MessageControl = new PlayerActionsControl(
                () =>
                {
                    MessageControl = new DestinationPickerControl(lc.BoardLocation, lc.Warehouse);
                });
        }
    }
}
