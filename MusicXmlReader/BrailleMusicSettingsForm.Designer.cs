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
            this.textBoxDeviceName = new System.Windows.Forms.TextBox();
            this.labelBraillePageLayout = new System.Windows.Forms.Label();
            this.listBoxBraillePageLayout = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.Location = new System.Drawing.Point(138, 83);
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownWidth.TabIndex = 20;
            this.numericUpDownWidth.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.Location = new System.Drawing.Point(138, 109);
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            this.numericUpDownHeight.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownHeight.TabIndex = 30;
            this.numericUpDownHeight.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // textBoxEscapeSequence
            // 
            this.textBoxEscapeSequence.Location = new System.Drawing.Point(138, 135);
            this.textBoxEscapeSequence.Name = "textBoxEscapeSequence";
            this.textBoxEscapeSequence.Size = new System.Drawing.Size(673, 20);
            this.textBoxEscapeSequence.TabIndex = 40;
            this.textBoxEscapeSequence.Text = "DTB0";
            // 
            // listBoxFileFormat
            // 
            this.listBoxFileFormat.FormattingEnabled = true;
            this.listBoxFileFormat.Location = new System.Drawing.Point(139, 12);
            this.listBoxFileFormat.Name = "listBoxFileFormat";
            this.listBoxFileFormat.Size = new System.Drawing.Size(207, 30);
            this.listBoxFileFormat.TabIndex = 10;
            // 
            // textBoxApplicationName
            // 
            this.textBoxApplicationName.Location = new System.Drawing.Point(139, 188);
            this.textBoxApplicationName.Name = "textBoxApplicationName";
            this.textBoxApplicationName.Size = new System.Drawing.Size(673, 20);
            this.textBoxApplicationName.TabIndex = 60;
            this.textBoxApplicationName.Text = "IBPrint";
            // 
            // textBoxApplicationExe
            // 
            this.textBoxApplicationExe.Location = new System.Drawing.Point(138, 214);
            this.textBoxApplicationExe.Name = "textBoxApplicationExe";
            this.textBoxApplicationExe.Size = new System.Drawing.Size(674, 20);
            this.textBoxApplicationExe.TabIndex = 70;
            this.textBoxApplicationExe.Text = "IBPrint.exe";
            // 
            // labelDeviceName
            // 
            this.labelDeviceName.AutoSize = true;
            this.labelDeviceName.Location = new System.Drawing.Point(12, 169);
            this.labelDeviceName.Name = "labelDeviceName";
            this.labelDeviceName.Size = new System.Drawing.Size(70, 13);
            this.labelDeviceName.TabIndex = 7;
            this.labelDeviceName.Text = "Device name";
            // 
            // labelBrailleFileFormat
            // 
            this.labelBrailleFileFormat.AutoSize = true;
            this.labelBrailleFileFormat.Location = new System.Drawing.Point(12, 14);
            this.labelBrailleFileFormat.Name = "labelBrailleFileFormat";
            this.labelBrailleFileFormat.Size = new System.Drawing.Size(83, 13);
            this.labelBrailleFileFormat.TabIndex = 8;
            this.labelBrailleFileFormat.Text = "Braille file format";
            // 
            // labelWidth
            // 
            this.labelWidth.AutoSize = true;
            this.labelWidth.Location = new System.Drawing.Point(15, 90);
            this.labelWidth.Name = "labelWidth";
            this.labelWidth.Size = new System.Drawing.Size(35, 13);
            this.labelWidth.TabIndex = 9;
            this.labelWidth.Text = "Width";
            // 
            // labelHeight
            // 
            this.labelHeight.AutoSize = true;
            this.labelHeight.Location = new System.Drawing.Point(12, 109);
            this.labelHeight.Name = "labelHeight";
            this.labelHeight.Size = new System.Drawing.Size(38, 13);
            this.labelHeight.TabIndex = 10;
            this.labelHeight.Text = "Height";
            // 
            // labelEscapeSequence
            // 
            this.labelEscapeSequence.AutoSize = true;
            this.labelEscapeSequence.Location = new System.Drawing.Point(12, 135);
            this.labelEscapeSequence.Name = "labelEscapeSequence";
            this.labelEscapeSequence.Size = new System.Drawing.Size(93, 13);
            this.labelEscapeSequence.TabIndex = 11;
            this.labelEscapeSequence.Text = "Escape sequence";
            // 
            // labelApplicationName
            // 
            this.labelApplicationName.AutoSize = true;
            this.labelApplicationName.Location = new System.Drawing.Point(12, 188);
            this.labelApplicationName.Name = "labelApplicationName";
            this.labelApplicationName.Size = new System.Drawing.Size(88, 13);
            this.labelApplicationName.TabIndex = 12;
            this.labelApplicationName.Text = "Application name";
            // 
            // labelApplicationLocation
            // 
            this.labelApplicationLocation.AutoSize = true;
            this.labelApplicationLocation.Location = new System.Drawing.Point(12, 217);
            this.labelApplicationLocation.Name = "labelApplicationLocation";
            this.labelApplicationLocation.Size = new System.Drawing.Size(99, 13);
            this.labelApplicationLocation.TabIndex = 13;
            this.labelApplicationLocation.Text = "Application location";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(737, 256);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 90;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(656, 256);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 80;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxDeviceName
            // 
            this.textBoxDeviceName.Location = new System.Drawing.Point(139, 162);
            this.textBoxDeviceName.Name = "textBoxDeviceName";
            this.textBoxDeviceName.Size = new System.Drawing.Size(673, 20);
            this.textBoxDeviceName.TabIndex = 50;
            this.textBoxDeviceName.Text = "Index Braille D2";
            // 
            // labelBraillePageLayout
            // 
            this.labelBraillePageLayout.AutoSize = true;
            this.labelBraillePageLayout.Location = new System.Drawing.Point(12, 49);
            this.labelBraillePageLayout.Name = "labelBraillePageLayout";
            this.labelBraillePageLayout.Size = new System.Drawing.Size(90, 13);
            this.labelBraillePageLayout.TabIndex = 91;
            this.labelBraillePageLayout.Text = "Braille pagelayout";
            // 
            // listBoxBraillePageLayout
            // 
            this.listBoxBraillePageLayout.FormattingEnabled = true;
            this.listBoxBraillePageLayout.Location = new System.Drawing.Point(139, 45);
            this.listBoxBraillePageLayout.Name = "listBoxBraillePageLayout";
            this.listBoxBraillePageLayout.Size = new System.Drawing.Size(207, 30);
            this.listBoxBraillePageLayout.TabIndex = 92;
            // 
            // BrailleMusicSettingsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(824, 291);
            this.Controls.Add(this.listBoxBraillePageLayout);
            this.Controls.Add(this.labelBraillePageLayout);
            this.Controls.Add(this.textBoxDeviceName);
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
            this.Controls.Add(this.numericUpDownHeight);
            this.Controls.Add(this.numericUpDownWidth);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BrailleMusicSettingsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "IBOS MusicXmlReader Generic Braille device  settings";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownWidth;
        private System.Windows.Forms.NumericUpDown numericUpDownHeight;
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
        private System.Windows.Forms.TextBox textBoxDeviceName;
        private System.Windows.Forms.Label labelBraillePageLayout;
        private System.Windows.Forms.ListBox listBoxBraillePageLayout;
    }
}