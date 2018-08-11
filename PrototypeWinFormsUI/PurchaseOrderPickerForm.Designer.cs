namespace PrototypeWinFormsUI
{
    partial class PurchaseOrderPickerForm
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
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.purchaseOrderControl5 = new PrototypeWinFormsUI.PurchaseOrderControl();
            this.purchaseOrderControl4 = new PrototypeWinFormsUI.PurchaseOrderControl();
            this.purchaseOrderControl3 = new PrototypeWinFormsUI.PurchaseOrderControl();
            this.purchaseOrderControl2 = new PrototypeWinFormsUI.PurchaseOrderControl();
            this.purchaseOrderControl1 = new PrototypeWinFormsUI.PurchaseOrderControl();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(27, 110);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(27, 314);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(15, 14);
            this.checkBox2.TabIndex = 6;
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(27, 524);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(15, 14);
            this.checkBox3.TabIndex = 7;
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(470, 204);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(15, 14);
            this.checkBox4.TabIndex = 8;
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(470, 399);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(15, 14);
            this.checkBox5.TabIndex = 9;
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.checkBox5_CheckedChanged);
            // 
            // purchaseOrderControl5
            // 
            this.purchaseOrderControl5.Location = new System.Drawing.Point(491, 314);
            this.purchaseOrderControl5.Name = "purchaseOrderControl5";
            this.purchaseOrderControl5.Size = new System.Drawing.Size(391, 189);
            this.purchaseOrderControl5.TabIndex = 4;
            // 
            // purchaseOrderControl4
            // 
            this.purchaseOrderControl4.Location = new System.Drawing.Point(491, 110);
            this.purchaseOrderControl4.Name = "purchaseOrderControl4";
            this.purchaseOrderControl4.Size = new System.Drawing.Size(391, 189);
            this.purchaseOrderControl4.TabIndex = 3;
            // 
            // purchaseOrderControl3
            // 
            this.purchaseOrderControl3.Location = new System.Drawing.Point(48, 428);
            this.purchaseOrderControl3.Name = "purchaseOrderControl3";
            this.purchaseOrderControl3.Size = new System.Drawing.Size(391, 189);
            this.purchaseOrderControl3.TabIndex = 2;
            // 
            // purchaseOrderControl2
            // 
            this.purchaseOrderControl2.Location = new System.Drawing.Point(48, 224);
            this.purchaseOrderControl2.Name = "purchaseOrderControl2";
            this.purchaseOrderControl2.Size = new System.Drawing.Size(391, 189);
            this.purchaseOrderControl2.TabIndex = 1;
            // 
            // purchaseOrderControl1
            // 
            this.purchaseOrderControl1.Location = new System.Drawing.Point(48, 29);
            this.purchaseOrderControl1.Name = "purchaseOrderControl1";
            this.purchaseOrderControl1.Size = new System.Drawing.Size(391, 189);
            this.purchaseOrderControl1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(856, 614);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PurchaseOrderPickerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 649);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox5);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.purchaseOrderControl5);
            this.Controls.Add(this.purchaseOrderControl4);
            this.Controls.Add(this.purchaseOrderControl3);
            this.Controls.Add(this.purchaseOrderControl2);
            this.Controls.Add(this.purchaseOrderControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PurchaseOrderPickerForm";
            this.Text = "PurchaseOrderPickerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PurchaseOrderControl purchaseOrderControl1;
        private PurchaseOrderControl purchaseOrderControl2;
        private PurchaseOrderControl purchaseOrderControl3;
        private PurchaseOrderControl purchaseOrderControl4;
        private PurchaseOrderControl purchaseOrderControl5;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.Button button1;
    }
}