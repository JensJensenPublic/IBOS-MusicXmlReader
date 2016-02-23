namespace MusicXmlReaderUI
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
            this.listBoxRaw = new System.Windows.Forms.ListBox();
            this.listBoxFiltered = new System.Windows.Forms.ListBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.Play = new System.Windows.Forms.Button();
            this.Stop = new System.Windows.Forms.Button();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.numericUpDownPlaySpeed = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPlaySpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxRaw
            // 
            this.listBoxRaw.FormattingEnabled = true;
            this.listBoxRaw.Location = new System.Drawing.Point(321, 276);
            this.listBoxRaw.Name = "listBoxRaw";
            this.listBoxRaw.Size = new System.Drawing.Size(275, 186);
            this.listBoxRaw.TabIndex = 1;
            // 
            // listBoxFiltered
            // 
            this.listBoxFiltered.FormattingEnabled = true;
            this.listBoxFiltered.Location = new System.Drawing.Point(12, 57);
            this.listBoxFiltered.Name = "listBoxFiltered";
            this.listBoxFiltered.Size = new System.Drawing.Size(303, 407);
            this.listBoxFiltered.TabIndex = 2;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(13, 12);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(67, 23);
            this.buttonStart.TabIndex = 3;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStartUsingDOM_Click);
            // 
            // Play
            // 
            this.Play.Location = new System.Drawing.Point(323, 11);
            this.Play.Name = "Play";
            this.Play.Size = new System.Drawing.Size(75, 23);
            this.Play.TabIndex = 4;
            this.Play.Text = "Spil";
            this.Play.UseVisualStyleBackColor = true;
            this.Play.Click += new System.EventHandler(this.Play_Click);
            // 
            // Stop
            // 
            this.Stop.Location = new System.Drawing.Point(420, 11);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(75, 23);
            this.Stop.TabIndex = 5;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(101, 12);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(214, 20);
            this.textBoxMessage.TabIndex = 6;
            // 
            // numericUpDownPlaySpeed
            // 
            this.numericUpDownPlaySpeed.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPlaySpeed.Location = new System.Drawing.Point(328, 58);
            this.numericUpDownPlaySpeed.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDownPlaySpeed.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPlaySpeed.Name = "numericUpDownPlaySpeed";
            this.numericUpDownPlaySpeed.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownPlaySpeed.TabIndex = 7;
            this.numericUpDownPlaySpeed.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownPlaySpeed.ValueChanged += new System.EventHandler(this.numericUpDownPlaySpeed_ValueChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 496);
            this.Controls.Add(this.numericUpDownPlaySpeed);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.Play);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.listBoxFiltered);
            this.Controls.Add(this.listBoxRaw);
            this.Name = "Form1";
            this.Text = "MusikLæser";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPlaySpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox listBoxRaw;
        private System.Windows.Forms.ListBox listBoxFiltered;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button Play;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.NumericUpDown numericUpDownPlaySpeed;
    }
}

