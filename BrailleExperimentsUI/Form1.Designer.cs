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
            this.ListBoxLeft = new System.Windows.Forms.ListBox();
            this.ListBoxRight = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ListBoxLeft
            // 
            this.ListBoxLeft.AccessibleName = " ";
            this.ListBoxLeft.FormattingEnabled = true;
            this.ListBoxLeft.Location = new System.Drawing.Point(28, 22);
            this.ListBoxLeft.Name = "ListBoxLeft";
            this.ListBoxLeft.Size = new System.Drawing.Size(400, 212);
            this.ListBoxLeft.TabIndex = 0;
            this.ListBoxLeft.SelectedIndexChanged += new System.EventHandler(this.ListBoxLeft_SelectedIndexChanged);
            // 
            // ListBoxRight
            // 
            this.ListBoxRight.AccessibleName = "LR";
            this.ListBoxRight.FormattingEnabled = true;
            this.ListBoxRight.Location = new System.Drawing.Point(469, 23);
            this.ListBoxRight.Name = "ListBoxRight";
            this.ListBoxRight.Size = new System.Drawing.Size(525, 212);
            this.ListBoxRight.TabIndex = 1;
            this.ListBoxRight.SelectedIndexChanged += new System.EventHandler(this.ListBoxRight_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(28, 240);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "a";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(469, 240);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "b";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 278);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ListBoxRight);
            this.Controls.Add(this.ListBoxLeft);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ListBoxLeft;
        private System.Windows.Forms.ListBox ListBoxRight;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

