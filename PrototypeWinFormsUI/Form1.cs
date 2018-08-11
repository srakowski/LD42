using LD42;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrototypeWinFormsUI
{
    public partial class Form1 : Form
    {
        private LocationControl[] locationControls;

        private MiningLogisticsGame game;
        private IEnumerator play;

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
                control.HasWarehouse = true;
                var wh = loc.Occupant is Mine m ? m.Storage :
                    loc.Occupant is Warehouse w ? w :
                    null;

                foreach (var cb in control.resourceChecboxes)
                {
                    cb.BackColor = Color.Transparent;
                }

                int i = 0;
                foreach (var rs in wh.UnitsOfResources)
                {
                    control.resourceChecboxes[i].Checked = true;
                    control.resourceChecboxes[i].BackColor = Color.Green;
                    i++;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Next();
        }
    }
}
