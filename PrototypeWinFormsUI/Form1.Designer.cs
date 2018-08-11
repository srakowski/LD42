namespace PrototypeWinFormsUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.kansasCityLocation = new PrototypeWinFormsUI.LocationControl();
            this.knoxvilleLocation = new PrototypeWinFormsUI.LocationControl();
            this.indianaoplisLocation = new PrototypeWinFormsUI.LocationControl();
            this.detroitLocation = new PrototypeWinFormsUI.LocationControl();
            this.bostonLocation = new PrototypeWinFormsUI.LocationControl();
            this.jacksonvilleLocation = new PrototypeWinFormsUI.LocationControl();
            this.newOrleansLocation = new PrototypeWinFormsUI.LocationControl();
            this.duluthLocation = new PrototypeWinFormsUI.LocationControl();
            this.houstonLocation = new PrototypeWinFormsUI.LocationControl();
            this.denverLocation = new PrototypeWinFormsUI.LocationControl();
            this.renoLocation = new PrototypeWinFormsUI.LocationControl();
            this.saltLakeCityLocation = new PrototypeWinFormsUI.LocationControl();
            this.billingsLocation = new PrototypeWinFormsUI.LocationControl();
            this.seattleLocation = new PrototypeWinFormsUI.LocationControl();
            this.lostAngelesLocation = new PrototypeWinFormsUI.LocationControl();
            this.tusconLocation = new PrototypeWinFormsUI.LocationControl();
            this.purchaseOrder3 = new PrototypeWinFormsUI.PurchaseOrderControl();
            this.purchaseOrder2 = new PrototypeWinFormsUI.PurchaseOrderControl();
            this.purchaseOrder1 = new PrototypeWinFormsUI.PurchaseOrderControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Panel2.Controls.Add(this.purchaseOrder3);
            this.splitContainer1.Panel2.Controls.Add(this.purchaseOrder2);
            this.splitContainer1.Panel2.Controls.Add(this.purchaseOrder1);
            this.splitContainer1.Size = new System.Drawing.Size(1904, 1041);
            this.splitContainer1.SplitterDistance = 1497;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackgroundImage = global::PrototypeWinFormsUI.Properties.Resources.USMAP;
            this.splitContainer2.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.splitContainer2.Panel1.Controls.Add(this.label4);
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.kansasCityLocation);
            this.splitContainer2.Panel1.Controls.Add(this.knoxvilleLocation);
            this.splitContainer2.Panel1.Controls.Add(this.indianaoplisLocation);
            this.splitContainer2.Panel1.Controls.Add(this.detroitLocation);
            this.splitContainer2.Panel1.Controls.Add(this.bostonLocation);
            this.splitContainer2.Panel1.Controls.Add(this.jacksonvilleLocation);
            this.splitContainer2.Panel1.Controls.Add(this.newOrleansLocation);
            this.splitContainer2.Panel1.Controls.Add(this.duluthLocation);
            this.splitContainer2.Panel1.Controls.Add(this.houstonLocation);
            this.splitContainer2.Panel1.Controls.Add(this.denverLocation);
            this.splitContainer2.Panel1.Controls.Add(this.renoLocation);
            this.splitContainer2.Panel1.Controls.Add(this.saltLakeCityLocation);
            this.splitContainer2.Panel1.Controls.Add(this.billingsLocation);
            this.splitContainer2.Panel1.Controls.Add(this.seattleLocation);
            this.splitContainer2.Panel1.Controls.Add(this.lostAngelesLocation);
            this.splitContainer2.Panel1.Controls.Add(this.tusconLocation);
            this.splitContainer2.Size = new System.Drawing.Size(1497, 1041);
            this.splitContainer2.SplitterDistance = 804;
            this.splitContainer2.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(413, 617);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Tuscon Copper Mine";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(234, 297);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Reno Silver Mine";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1065, 518);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Knoxville Zinc Mine";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(867, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Duluth Iron Mine";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(319, 1006);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Next";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // kansasCityLocation
            // 
            this.kansasCityLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.kansasCityLocation.HasWarehouse = true;
            this.kansasCityLocation.Location = new System.Drawing.Point(802, 361);
            this.kansasCityLocation.Name = "kansasCityLocation";
            this.kansasCityLocation.Size = new System.Drawing.Size(64, 64);
            this.kansasCityLocation.TabIndex = 15;
            this.kansasCityLocation.Tag = "Kansas City";
            this.kansasCityLocation.Visible = false;
            // 
            // knoxvilleLocation
            // 
            this.knoxvilleLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.knoxvilleLocation.HasWarehouse = true;
            this.knoxvilleLocation.Location = new System.Drawing.Point(1046, 447);
            this.knoxvilleLocation.Name = "knoxvilleLocation";
            this.knoxvilleLocation.Size = new System.Drawing.Size(64, 64);
            this.knoxvilleLocation.TabIndex = 14;
            this.knoxvilleLocation.Tag = "Knoxville";
            this.knoxvilleLocation.Visible = false;
            // 
            // indianaoplisLocation
            // 
            this.indianaoplisLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.indianaoplisLocation.HasWarehouse = true;
            this.indianaoplisLocation.Location = new System.Drawing.Point(983, 336);
            this.indianaoplisLocation.Name = "indianaoplisLocation";
            this.indianaoplisLocation.Size = new System.Drawing.Size(64, 64);
            this.indianaoplisLocation.TabIndex = 13;
            this.indianaoplisLocation.Tag = "Indianapolis";
            this.indianaoplisLocation.Visible = false;
            // 
            // detroitLocation
            // 
            this.detroitLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.detroitLocation.HasWarehouse = true;
            this.detroitLocation.Location = new System.Drawing.Point(1065, 255);
            this.detroitLocation.Name = "detroitLocation";
            this.detroitLocation.Size = new System.Drawing.Size(64, 64);
            this.detroitLocation.TabIndex = 12;
            this.detroitLocation.Tag = "Detroit";
            this.detroitLocation.Visible = false;
            // 
            // bostonLocation
            // 
            this.bostonLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bostonLocation.HasWarehouse = true;
            this.bostonLocation.Location = new System.Drawing.Point(1342, 268);
            this.bostonLocation.Name = "bostonLocation";
            this.bostonLocation.Size = new System.Drawing.Size(64, 64);
            this.bostonLocation.TabIndex = 11;
            this.bostonLocation.Tag = "Boston";
            this.bostonLocation.Visible = false;
            // 
            // jacksonvilleLocation
            // 
            this.jacksonvilleLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.jacksonvilleLocation.HasWarehouse = true;
            this.jacksonvilleLocation.Location = new System.Drawing.Point(1100, 595);
            this.jacksonvilleLocation.Name = "jacksonvilleLocation";
            this.jacksonvilleLocation.Size = new System.Drawing.Size(64, 64);
            this.jacksonvilleLocation.TabIndex = 10;
            this.jacksonvilleLocation.Tag = "Jacksonville";
            this.jacksonvilleLocation.Visible = false;
            // 
            // newOrleansLocation
            // 
            this.newOrleansLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.newOrleansLocation.HasWarehouse = true;
            this.newOrleansLocation.Location = new System.Drawing.Point(910, 604);
            this.newOrleansLocation.Name = "newOrleansLocation";
            this.newOrleansLocation.Size = new System.Drawing.Size(64, 64);
            this.newOrleansLocation.TabIndex = 9;
            this.newOrleansLocation.Tag = "New Orleans";
            this.newOrleansLocation.Visible = false;
            // 
            // duluthLocation
            // 
            this.duluthLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.duluthLocation.HasWarehouse = true;
            this.duluthLocation.Location = new System.Drawing.Point(856, 124);
            this.duluthLocation.Name = "duluthLocation";
            this.duluthLocation.Size = new System.Drawing.Size(64, 64);
            this.duluthLocation.TabIndex = 8;
            this.duluthLocation.Tag = "Duluth";
            this.duluthLocation.Visible = false;
            // 
            // houstonLocation
            // 
            this.houstonLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.houstonLocation.HasWarehouse = true;
            this.houstonLocation.Location = new System.Drawing.Point(792, 617);
            this.houstonLocation.Name = "houstonLocation";
            this.houstonLocation.Size = new System.Drawing.Size(64, 64);
            this.houstonLocation.TabIndex = 7;
            this.houstonLocation.Tag = "Houston";
            this.houstonLocation.Visible = false;
            // 
            // denverLocation
            // 
            this.denverLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.denverLocation.HasWarehouse = true;
            this.denverLocation.Location = new System.Drawing.Point(568, 347);
            this.denverLocation.Name = "denverLocation";
            this.denverLocation.Size = new System.Drawing.Size(64, 64);
            this.denverLocation.TabIndex = 6;
            this.denverLocation.Tag = "Denver";
            this.denverLocation.Visible = false;
            // 
            // renoLocation
            // 
            this.renoLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.renoLocation.HasWarehouse = true;
            this.renoLocation.Location = new System.Drawing.Point(234, 316);
            this.renoLocation.Name = "renoLocation";
            this.renoLocation.Size = new System.Drawing.Size(64, 64);
            this.renoLocation.TabIndex = 5;
            this.renoLocation.Tag = "Reno";
            this.renoLocation.Visible = false;
            // 
            // saltLakeCityLocation
            // 
            this.saltLakeCityLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.saltLakeCityLocation.HasWarehouse = true;
            this.saltLakeCityLocation.Location = new System.Drawing.Point(399, 303);
            this.saltLakeCityLocation.Name = "saltLakeCityLocation";
            this.saltLakeCityLocation.Size = new System.Drawing.Size(64, 64);
            this.saltLakeCityLocation.TabIndex = 4;
            this.saltLakeCityLocation.Tag = "Salt Lake City";
            this.saltLakeCityLocation.Visible = false;
            // 
            // billingsLocation
            // 
            this.billingsLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.billingsLocation.HasWarehouse = true;
            this.billingsLocation.Location = new System.Drawing.Point(470, 138);
            this.billingsLocation.Name = "billingsLocation";
            this.billingsLocation.Size = new System.Drawing.Size(64, 64);
            this.billingsLocation.TabIndex = 3;
            this.billingsLocation.Tag = "Billings";
            this.billingsLocation.Visible = false;
            // 
            // seattleLocation
            // 
            this.seattleLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.seattleLocation.HasWarehouse = true;
            this.seattleLocation.Location = new System.Drawing.Point(163, 84);
            this.seattleLocation.Name = "seattleLocation";
            this.seattleLocation.Size = new System.Drawing.Size(64, 64);
            this.seattleLocation.TabIndex = 2;
            this.seattleLocation.Tag = "Seattle";
            this.seattleLocation.Visible = false;
            // 
            // lostAngelesLocation
            // 
            this.lostAngelesLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lostAngelesLocation.HasWarehouse = true;
            this.lostAngelesLocation.Location = new System.Drawing.Point(251, 509);
            this.lostAngelesLocation.Name = "lostAngelesLocation";
            this.lostAngelesLocation.Size = new System.Drawing.Size(64, 64);
            this.lostAngelesLocation.TabIndex = 1;
            this.lostAngelesLocation.Tag = "Los Angeles";
            this.lostAngelesLocation.Visible = false;
            // 
            // tusconLocation
            // 
            this.tusconLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tusconLocation.HasWarehouse = true;
            this.tusconLocation.Location = new System.Drawing.Point(413, 548);
            this.tusconLocation.Name = "tusconLocation";
            this.tusconLocation.Size = new System.Drawing.Size(64, 64);
            this.tusconLocation.TabIndex = 0;
            this.tusconLocation.Tag = "Tuscon";
            this.tusconLocation.Visible = false;
            // 
            // purchaseOrder3
            // 
            this.purchaseOrder3.Location = new System.Drawing.Point(3, 548);
            this.purchaseOrder3.Name = "purchaseOrder3";
            this.purchaseOrder3.Size = new System.Drawing.Size(391, 271);
            this.purchaseOrder3.TabIndex = 2;
            // 
            // purchaseOrder2
            // 
            this.purchaseOrder2.Location = new System.Drawing.Point(3, 277);
            this.purchaseOrder2.Name = "purchaseOrder2";
            this.purchaseOrder2.Size = new System.Drawing.Size(391, 271);
            this.purchaseOrder2.TabIndex = 1;
            // 
            // purchaseOrder1
            // 
            this.purchaseOrder1.Location = new System.Drawing.Point(3, 12);
            this.purchaseOrder1.Name = "purchaseOrder1";
            this.purchaseOrder1.Size = new System.Drawing.Size(391, 271);
            this.purchaseOrder1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Prototype Game UI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private LocationControl knoxvilleLocation;
        private LocationControl indianaoplisLocation;
        private LocationControl detroitLocation;
        private LocationControl bostonLocation;
        private LocationControl jacksonvilleLocation;
        private LocationControl newOrleansLocation;
        private LocationControl duluthLocation;
        private LocationControl houstonLocation;
        private LocationControl denverLocation;
        private LocationControl renoLocation;
        private LocationControl saltLakeCityLocation;
        private LocationControl billingsLocation;
        private LocationControl seattleLocation;
        private LocationControl lostAngelesLocation;
        private LocationControl tusconLocation;
        private LocationControl kansasCityLocation;
        private PurchaseOrderControl purchaseOrder3;
        private PurchaseOrderControl purchaseOrder2;
        private PurchaseOrderControl purchaseOrder1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}

