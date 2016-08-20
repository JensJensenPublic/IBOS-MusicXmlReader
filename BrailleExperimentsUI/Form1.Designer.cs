namespace BrailleExperimentsUI
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
            this.ListBoxRight = new System.Windows.Forms.ListBox();
            this.textBoxBraille = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ListBoxRight
            // 
            this.ListBoxRight.AccessibleName = "LR";
            this.ListBoxRight.FormattingEnabled = true;
            this.ListBoxRight.Location = new System.Drawing.Point(28, 37);
            this.ListBoxRight.Name = "ListBoxRight";
            this.ListBoxRight.Size = new System.Drawing.Size(966, 212);
            this.ListBoxRight.TabIndex = 1;
            this.ListBoxRight.SelectedIndexChanged += new System.EventHandler(this.ListBoxRight_SelectedIndexChanged);
            this.ListBoxRight.Leave += new System.EventHandler(this.ListBoxRight_Leave);
            // 
            // textBoxBraille
            // 
            this.textBoxBraille.BackColor = System.Drawing.SystemColors.WindowText;
            this.textBoxBraille.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxBraille.ForeColor = System.Drawing.SystemColors.Window;
            this.textBoxBraille.Location = new System.Drawing.Point(28, 287);
            this.textBoxBraille.Name = "textBoxBraille";
            this.textBoxBraille.Size = new System.Drawing.Size(966, 62);
            this.textBoxBraille.TabIndex = 5;
            this.textBoxBraille.TextChanged += new System.EventHandler(this.textBoxBraille_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 378);
            this.Controls.Add(this.textBoxBraille);
            this.Controls.Add(this.ListBoxRight);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox ListBoxRight;
        private System.Windows.Forms.TextBox textBoxBraille;
    }
}

