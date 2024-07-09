namespace Hexagons
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
            this.hexagonsField1 = new Hexagons.Logic.HexagonsField();
            this.SuspendLayout();
            // 
            // hexagonsField1
            // 
            this.hexagonsField1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexagonsField1.Location = new System.Drawing.Point(0, 0);
            this.hexagonsField1.Name = "hexagonsField1";
            this.hexagonsField1.Size = new System.Drawing.Size(800, 450);
            this.hexagonsField1.TabIndex = 0;
            this.hexagonsField1.Text = "hexagonsField1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.hexagonsField1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Logic.HexagonsField hexagonsField1;
    }
}

