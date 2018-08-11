namespace PrototypeWinFormsUI
{
    partial class PlayerActionsControl
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
            this.shipResourcesButton = new System.Windows.Forms.Button();
            this.fulfillPurchaseOrderBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // shipResourcesButton
            // 
            this.shipResourcesButton.Location = new System.Drawing.Point(3, 3);
            this.shipResourcesButton.Name = "shipResourcesButton";
            this.shipResourcesButton.Size = new System.Drawing.Size(144, 23);
            this.shipResourcesButton.TabIndex = 0;
            this.shipResourcesButton.Text = "Ship Resources";
            this.shipResourcesButton.UseVisualStyleBackColor = true;
            // 
            // fulfillPurchaseOrderBtn
            // 
            this.fulfillPurchaseOrderBtn.Location = new System.Drawing.Point(3, 32);
            this.fulfillPurchaseOrderBtn.Name = "fulfillPurchaseOrderBtn";
            this.fulfillPurchaseOrderBtn.Size = new System.Drawing.Size(144, 23);
            this.fulfillPurchaseOrderBtn.TabIndex = 1;
            this.fulfillPurchaseOrderBtn.Text = "Purchase Order Fulfillment";
            this.fulfillPurchaseOrderBtn.UseVisualStyleBackColor = true;
            // 
            // PlayerActionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fulfillPurchaseOrderBtn);
            this.Controls.Add(this.shipResourcesButton);
            this.Name = "PlayerActionsControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button shipResourcesButton;
        private System.Windows.Forms.Button fulfillPurchaseOrderBtn;
    }
}
