namespace MusicXmlReader
{
    partial class HelpForm
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
            this.listBoxHelp = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBoxHelp
            // 
            this.listBoxHelp.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxHelp.FormattingEnabled = true;
            this.listBoxHelp.ItemHeight = 14;
            this.listBoxHelp.Location = new System.Drawing.Point(13, 13);
            this.listBoxHelp.Name = "listBoxHelp";
            this.listBoxHelp.Size = new System.Drawing.Size(800, 788);
            this.listBoxHelp.TabIndex = 0;
            this.listBoxHelp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxHelp_KeyDown);
            // 
            // HelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(510, 261);
            this.Controls.Add(this.listBoxHelp);
            this.Name = "HelpForm";
            this.Text = "HelpForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HelpForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxHelp;
    }
}