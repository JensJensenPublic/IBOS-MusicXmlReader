namespace KeyboardTest
{
    partial class KeyboardTestForm
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
            this.listBoxForKeyDown = new System.Windows.Forms.ListBox();
            this.listBoxForKeyPress = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBoxForKeyDown
            // 
            this.listBoxForKeyDown.FormattingEnabled = true;
            this.listBoxForKeyDown.Location = new System.Drawing.Point(12, 9);
            this.listBoxForKeyDown.Name = "listBoxForKeyDown";
            this.listBoxForKeyDown.Size = new System.Drawing.Size(783, 199);
            this.listBoxForKeyDown.TabIndex = 0;
            this.listBoxForKeyDown.Click += new System.EventHandler(this.listBox_Click);
            this.listBoxForKeyDown.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            this.listBoxForKeyDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyDown);
            this.listBoxForKeyDown.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listBox_KeyPress);
            this.listBoxForKeyDown.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyUp);
            // 
            // listBoxForKeyPress
            // 
            this.listBoxForKeyPress.FormattingEnabled = true;
            this.listBoxForKeyPress.Location = new System.Drawing.Point(12, 232);
            this.listBoxForKeyPress.Name = "listBoxForKeyPress";
            this.listBoxForKeyPress.Size = new System.Drawing.Size(783, 186);
            this.listBoxForKeyPress.TabIndex = 1;
            this.listBoxForKeyPress.Click += new System.EventHandler(this.listBoxForKeyPress_Click);
            this.listBoxForKeyPress.SelectedIndexChanged += new System.EventHandler(this.listBoxForKeyPress_SelectedIndexChanged);
            this.listBoxForKeyPress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxForKeyPress_KeyDown);
            this.listBoxForKeyPress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listBoxForKeyPress_KeyPress);
            this.listBoxForKeyPress.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listBoxForKeyPress_KeyUp);
            // 
            // KeyboardTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 427);
            this.Controls.Add(this.listBoxForKeyPress);
            this.Controls.Add(this.listBoxForKeyDown);
            this.Name = "KeyboardTestForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxForKeyDown;
        private System.Windows.Forms.ListBox listBoxForKeyPress;
    }
}

