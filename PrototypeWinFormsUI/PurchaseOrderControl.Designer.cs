namespace PrototypeWinFormsUI
{
    partial class PurchaseOrderControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.card1 = new PrototypeWinFormsUI.CardControl();
            this.card2 = new PrototypeWinFormsUI.CardControl();
            this.card3 = new PrototypeWinFormsUI.CardControl();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.card3);
            this.groupBox1.Controls.Add(this.card2);
            this.groupBox1.Controls.Add(this.card1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(384, 253);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Purchase Order";
            // 
            // card1
            // 
            this.card1.CardType = "groupBox1";
            this.card1.Description = "";
            this.card1.Location = new System.Drawing.Point(6, 19);
            this.card1.Name = "card1";
            this.card1.Size = new System.Drawing.Size(120, 150);
            this.card1.TabIndex = 0;
            // 
            // card2
            // 
            this.card2.CardType = "groupBox1";
            this.card2.Description = "";
            this.card2.Location = new System.Drawing.Point(132, 19);
            this.card2.Name = "card2";
            this.card2.Size = new System.Drawing.Size(120, 150);
            this.card2.TabIndex = 1;
            // 
            // card3
            // 
            this.card3.CardType = "groupBox1";
            this.card3.Description = "";
            this.card3.Location = new System.Drawing.Point(258, 19);
            this.card3.Name = "card3";
            this.card3.Size = new System.Drawing.Size(120, 150);
            this.card3.TabIndex = 2;
            // 
            // PurchaseOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "PurchaseOrder";
            this.Size = new System.Drawing.Size(391, 271);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private CardControl card1;
        private CardControl card3;
        private CardControl card2;
    }
}
