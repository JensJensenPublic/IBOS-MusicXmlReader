namespace MusicXmlReader
{
    partial class BrailleMusicSettingsForm
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
            this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
            this.comboBoxDeviceName = new System.Windows.Forms.ComboBox();
            this.textBoxEscapeSequence = new System.Windows.Forms.TextBox();
            this.listBoxFileFormat = new System.Windows.Forms.ListBox();
            this.textBoxApplicationName = new System.Windows.Forms.TextBox();
            this.textBoxApplicationExe = new System.Windows.Forms.TextBox();
            this.labelDeviceName = new System.Windows.Forms.Label();
            this.labelBrailleFileFormat = new System.Windows.Forms.Label();
            this.labelWidth = new System.Windows.Forms.Label();
            this.labelHeight = new System.Windows.Forms.Label();
            this.labelEscapeSequence = new System.Windows.Forms.Label();
            this.labelApplicationName = new System.Windows.Forms.Label();
            this.labelApplicationLocation = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.Location = new System.Drawing.Point(139, 85);
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownWidth.TabIndex = 0;
            this.numericUpDownWidth.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.Location = new System.Drawing.Point(139, 111);
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            this.numericUpDownHeight.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownHeight.TabIndex = 1;
            this.numericUpDownHeight.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // comboBoxDeviceName
            // 
            this.comboBoxDeviceName.FormattingEnabled = true;
            this.comboBoxDeviceName.Location = new System.Drawing.Point(138, 12);
            this.comboBoxDeviceName.Name = "comboBoxDeviceName";
            this.comboBoxDeviceName.Size = new System.Drawing.Size(121, 21);
            this.comboBoxDeviceName.TabIndex = 2;
            // 
            // textBoxEscapeSequence
            // 
            this.textBoxEscapeSequence.Location = new System.Drawing.Point(139, 137);
            this.textBoxEscapeSequence.Name = "textBoxEscapeSequence";
            this.textBoxEscapeSequence.Size = new System.Drawing.Size(100, 20);
            this.textBoxEscapeSequence.TabIndex = 3;
            this.textBoxEscapeSequence.Text = "DTB0";
            // 
            // listBoxFileFormat
            // 
            this.listBoxFileFormat.FormattingEnabled = true;
            this.listBoxFileFormat.Location = new System.Drawing.Point(139, 39);
            this.listBoxFileFormat.Name = "listBoxFileFormat";
            this.listBoxFileFormat.Size = new System.Drawing.Size(120, 30);
            this.listBoxFileFormat.TabIndex = 4;
            // 
            // textBoxApplicationName
            // 
            this.textBoxApplicationName.Location = new System.Drawing.Point(139, 163);
            this.textBoxApplicationName.Name = "textBoxApplicationName";
            this.textBoxApplicationName.Size = new System.Drawing.Size(100, 20);
            this.textBoxApplicationName.TabIndex = 5;
            this.textBoxApplicationName.Text = "IBPrint";
            this.textBoxApplicationName.TextChanged += new System.EventHandler(this.textBoxApplicationName_TextChanged);
            // 
            // textBoxApplicationExe
            // 
            this.textBoxApplicationExe.Location = new System.Drawing.Point(138, 189);
            this.textBoxApplicationExe.Name = "textBoxApplicationExe";
            this.textBoxApplicationExe.Size = new System.Drawing.Size(100, 20);
            this.textBoxApplicationExe.TabIndex = 6;
            this.textBoxApplicationExe.Text = "IBPrint.exe";
            // 
            // labelDeviceName
            // 
            this.labelDeviceName.AutoSize = true;
            this.labelDeviceName.Location = new System.Drawing.Point(12, 20);
            this.labelDeviceName.Name = "labelDeviceName";
            this.labelDeviceName.Size = new System.Drawing.Size(70, 13);
            this.labelDeviceName.TabIndex = 7;
            this.labelDeviceName.Text = "Device name";
            // 
            // labelBrailleFileFormat
            // 
            this.labelBrailleFileFormat.AutoSize = true;
            this.labelBrailleFileFormat.Location = new System.Drawing.Point(12, 56);
            this.labelBrailleFileFormat.Name = "labelBrailleFileFormat";
            this.labelBrailleFileFormat.Size = new System.Drawing.Size(83, 13);
            this.labelBrailleFileFormat.TabIndex = 8;
            this.labelBrailleFileFormat.Text = "Braille file format";
            // 
            // labelWidth
            // 
            this.labelWidth.AutoSize = true;
            this.labelWidth.Location = new System.Drawing.Point(15, 92);
            this.labelWidth.Name = "labelWidth";
            this.labelWidth.Size = new System.Drawing.Size(35, 13);
            this.labelWidth.TabIndex = 9;
            this.labelWidth.Text = "Width";
            // 
            // labelHeight
            // 
            this.labelHeight.AutoSize = true;
            this.labelHeight.Location = new System.Drawing.Point(15, 118);
            this.labelHeight.Name = "labelHeight";
            this.labelHeight.Size = new System.Drawing.Size(38, 13);
            this.labelHeight.TabIndex = 10;
            this.labelHeight.Text = "Height";
            // 
            // labelEscapeSequence
            // 
            this.labelEscapeSequence.AutoSize = true;
            this.labelEscapeSequence.Location = new System.Drawing.Point(15, 144);
            this.labelEscapeSequence.Name = "labelEscapeSequence";
            this.labelEscapeSequence.Size = new System.Drawing.Size(93, 13);
            this.labelEscapeSequence.TabIndex = 11;
            this.labelEscapeSequence.Text = "Escape sequence";
            // 
            // labelApplicationName
            // 
            this.labelApplicationName.AutoSize = true;
            this.labelApplicationName.Location = new System.Drawing.Point(12, 170);
            this.labelApplicationName.Name = "labelApplicationName";
            this.labelApplicationName.Size = new System.Drawing.Size(88, 13);
            this.labelApplicationName.TabIndex = 12;
            this.labelApplicationName.Text = "Application name";
            // 
            // labelApplicationLocation
            // 
            this.labelApplicationLocation.AutoSize = true;
            this.labelApplicationLocation.Location = new System.Drawing.Point(15, 196);
            this.labelApplicationLocation.Name = "labelApplicationLocation";
            this.labelApplicationLocation.Size = new System.Drawing.Size(99, 13);
            this.labelApplicationLocation.TabIndex = 13;
            this.labelApplicationLocation.Text = "Application location";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(197, 226);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 14;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(116, 226);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 15;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // BrailleMusicSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelApplicationLocation);
            this.Controls.Add(this.labelApplicationName);
            this.Controls.Add(this.labelEscapeSequence);
            this.Controls.Add(this.labelHeight);
            this.Controls.Add(this.labelWidth);
            this.Controls.Add(this.labelBrailleFileFormat);
            this.Controls.Add(this.labelDeviceName);
            this.Controls.Add(this.textBoxApplicationExe);
            this.Controls.Add(this.textBoxApplicationName);
            this.Controls.Add(this.listBoxFileFormat);
            this.Controls.Add(this.textBoxEscapeSequence);
            this.Controls.Add(this.comboBoxDeviceName);
            this.Controls.Add(this.numericUpDownHeight);
            this.Controls.Add(this.numericUpDownWidth);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BrailleMusicSettingsForm";
            this.Text = "IBOS MusicXmlReader Device settings";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownWidth;
        private System.Windows.Forms.NumericUpDown numericUpDownHeight;
        private System.Windows.Forms.ComboBox comboBoxDeviceName;
        private System.Windows.Forms.TextBox textBoxEscapeSequence;
        private System.Windows.Forms.ListBox listBoxFileFormat;
        private System.Windows.Forms.TextBox textBoxApplicationName;
        private System.Windows.Forms.TextBox textBoxApplicationExe;
        private System.Windows.Forms.Label labelDeviceName;
        private System.Windows.Forms.Label labelBrailleFileFormat;
        private System.Windows.Forms.Label labelWidth;
        private System.Windows.Forms.Label labelHeight;
        private System.Windows.Forms.Label labelEscapeSequence;
        private System.Windows.Forms.Label labelApplicationName;
        private System.Windows.Forms.Label labelApplicationLocation;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}