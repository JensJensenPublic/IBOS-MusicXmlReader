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
            this.listBoxTimes = new System.Windows.Forms.ListBox();
            this.listBoxFiltered = new System.Windows.Forms.ListBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.Play = new System.Windows.Forms.Button();
            this.Stop = new System.Windows.Forms.Button();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.numericUpDownPlaySpeed = new System.Windows.Forms.NumericUpDown();
            this.butonPlayPoly = new System.Windows.Forms.Button();
            this.checkedListBoxPartsToPlay = new System.Windows.Forms.CheckedListBox();
            this.checkedListBoxPartsToRead = new System.Windows.Forms.CheckedListBox();
            this.checkBoxShowStartTime = new System.Windows.Forms.CheckBox();
            this.labelPartsPlayed = new System.Windows.Forms.Label();
            this.labelPartsRead = new System.Windows.Forms.Label();
            this.checkBoxShowHarmonies = new System.Windows.Forms.CheckBox();
            this.checkBoxPlayHarmonies = new System.Windows.Forms.CheckBox();
            this.checkBoxOplæsBecifringskoder = new System.Windows.Forms.CheckBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStripFile = new System.Windows.Forms.MenuStrip();
            this.checkBoxReadMeasureNumbers = new System.Windows.Forms.CheckBox();
            this.checkBoxPlayMeasureNumbers = new System.Windows.Forms.CheckBox();
            this.checkBoxReadEndEvents = new System.Windows.Forms.CheckBox();
            this.checkedListBoxPartsToReadLyrics = new System.Windows.Forms.CheckedListBox();
            this.buttonReadLogFile = new System.Windows.Forms.Button();
            this.checkBoxReadPitch = new System.Windows.Forms.CheckBox();
            this.checkBoxReadOctave = new System.Windows.Forms.CheckBox();
            this.checkBoxReadDuration = new System.Windows.Forms.CheckBox();
            this.buttonReadMusicXmlFile = new System.Windows.Forms.Button();
            this.buttonReadInterpretation = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPlaySpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxTimes
            // 
            this.listBoxTimes.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxTimes.FormattingEnabled = true;
            this.listBoxTimes.Location = new System.Drawing.Point(257, 77);
            this.listBoxTimes.Name = "listBoxTimes";
            this.listBoxTimes.Size = new System.Drawing.Size(540, 407);
            this.listBoxTimes.TabIndex = 1;
            this.listBoxTimes.SelectedIndexChanged += new System.EventHandler(this.listBoxTimes_SelectedIndexChanged);
            // 
            // listBoxFiltered
            // 
            this.listBoxFiltered.FormattingEnabled = true;
            this.listBoxFiltered.Location = new System.Drawing.Point(12, 77);
            this.listBoxFiltered.Name = "listBoxFiltered";
            this.listBoxFiltered.Size = new System.Drawing.Size(239, 407);
            this.listBoxFiltered.TabIndex = 2;
            // 
            // buttonStart
            // 
            this.buttonStart.AccessibleDescription = "Tryk enter for at vælge MusicXml fil.";
            this.buttonStart.AccessibleName = "Startknap";
            this.buttonStart.Location = new System.Drawing.Point(12, 24);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(67, 23);
            this.buttonStart.TabIndex = 3;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStartUsingDOM_Click);
            // 
            // Play
            // 
            this.Play.Location = new System.Drawing.Point(12, 53);
            this.Play.Name = "Play";
            this.Play.Size = new System.Drawing.Size(75, 23);
            this.Play.TabIndex = 4;
            this.Play.Text = "Spil enstemmigt";
            this.Play.UseVisualStyleBackColor = true;
            this.Play.Click += new System.EventHandler(this.Play_Click);
            // 
            // Stop
            // 
            this.Stop.Location = new System.Drawing.Point(157, 53);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(75, 23);
            this.Stop.TabIndex = 5;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(101, 24);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(1001, 20);
            this.textBoxMessage.TabIndex = 6;
            // 
            // numericUpDownPlaySpeed
            // 
            this.numericUpDownPlaySpeed.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPlaySpeed.Location = new System.Drawing.Point(411, 56);
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
            // butonPlayPoly
            // 
            this.butonPlayPoly.Location = new System.Drawing.Point(321, 53);
            this.butonPlayPoly.Name = "butonPlayPoly";
            this.butonPlayPoly.Size = new System.Drawing.Size(75, 23);
            this.butonPlayPoly.TabIndex = 8;
            this.butonPlayPoly.Text = "Spil flerstemmigt";
            this.butonPlayPoly.UseVisualStyleBackColor = true;
            this.butonPlayPoly.Click += new System.EventHandler(this.butonPlayPoly_Click);
            // 
            // checkedListBoxPartsToPlay
            // 
            this.checkedListBoxPartsToPlay.AccessibleDescription = "En checkbox for hver stemme. Hvis checked spilles stemmen";
            this.checkedListBoxPartsToPlay.AccessibleName = "Spil stemmer";
            this.checkedListBoxPartsToPlay.FormattingEnabled = true;
            this.checkedListBoxPartsToPlay.Location = new System.Drawing.Point(856, 72);
            this.checkedListBoxPartsToPlay.Name = "checkedListBoxPartsToPlay";
            this.checkedListBoxPartsToPlay.Size = new System.Drawing.Size(120, 184);
            this.checkedListBoxPartsToPlay.TabIndex = 9;
            this.checkedListBoxPartsToPlay.Tag = "";
            this.checkedListBoxPartsToPlay.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxPartsToPlay_ItemCheck);
            // 
            // checkedListBoxPartsToRead
            // 
            this.checkedListBoxPartsToRead.AccessibleName = "Oplæs stemmer";
            this.checkedListBoxPartsToRead.FormattingEnabled = true;
            this.checkedListBoxPartsToRead.Location = new System.Drawing.Point(982, 72);
            this.checkedListBoxPartsToRead.Name = "checkedListBoxPartsToRead";
            this.checkedListBoxPartsToRead.Size = new System.Drawing.Size(120, 184);
            this.checkedListBoxPartsToRead.TabIndex = 10;
            this.checkedListBoxPartsToRead.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxPartsToRead_ItemCheck);
            // 
            // checkBoxShowStartTime
            // 
            this.checkBoxShowStartTime.AccessibleName = "Oplæs starttid";
            this.checkBoxShowStartTime.AutoSize = true;
            this.checkBoxShowStartTime.Location = new System.Drawing.Point(982, 423);
            this.checkBoxShowStartTime.Name = "checkBoxShowStartTime";
            this.checkBoxShowStartTime.Size = new System.Drawing.Size(91, 17);
            this.checkBoxShowStartTime.TabIndex = 11;
            this.checkBoxShowStartTime.Text = "Oplæs starttid";
            this.checkBoxShowStartTime.UseVisualStyleBackColor = true;
            this.checkBoxShowStartTime.CheckedChanged += new System.EventHandler(this.checkBoxShowStartTime_CheckedChanged);
            // 
            // labelPartsPlayed
            // 
            this.labelPartsPlayed.AutoSize = true;
            this.labelPartsPlayed.Location = new System.Drawing.Point(856, 53);
            this.labelPartsPlayed.Name = "labelPartsPlayed";
            this.labelPartsPlayed.Size = new System.Drawing.Size(66, 13);
            this.labelPartsPlayed.TabIndex = 12;
            this.labelPartsPlayed.Text = "Spil stemmer";
            // 
            // labelPartsRead
            // 
            this.labelPartsRead.AutoSize = true;
            this.labelPartsRead.Location = new System.Drawing.Point(983, 53);
            this.labelPartsRead.Name = "labelPartsRead";
            this.labelPartsRead.Size = new System.Drawing.Size(80, 13);
            this.labelPartsRead.TabIndex = 13;
            this.labelPartsRead.Text = "Oplæs stemmer";
            // 
            // checkBoxShowHarmonies
            // 
            this.checkBoxShowHarmonies.AccessibleName = "Oplæs becifringer";
            this.checkBoxShowHarmonies.AutoSize = true;
            this.checkBoxShowHarmonies.Location = new System.Drawing.Point(982, 263);
            this.checkBoxShowHarmonies.Name = "checkBoxShowHarmonies";
            this.checkBoxShowHarmonies.Size = new System.Drawing.Size(110, 17);
            this.checkBoxShowHarmonies.TabIndex = 14;
            this.checkBoxShowHarmonies.Text = "Oplæs Becifringer";
            this.checkBoxShowHarmonies.UseVisualStyleBackColor = true;
            this.checkBoxShowHarmonies.CheckedChanged += new System.EventHandler(this.checkBoxShowHarmonies_CheckedChanged);
            // 
            // checkBoxPlayHarmonies
            // 
            this.checkBoxPlayHarmonies.AccessibleName = "Spil becifringer";
            this.checkBoxPlayHarmonies.AutoSize = true;
            this.checkBoxPlayHarmonies.Location = new System.Drawing.Point(856, 263);
            this.checkBoxPlayHarmonies.Name = "checkBoxPlayHarmonies";
            this.checkBoxPlayHarmonies.Size = new System.Drawing.Size(96, 17);
            this.checkBoxPlayHarmonies.TabIndex = 15;
            this.checkBoxPlayHarmonies.Text = "Spil Becifringer";
            this.checkBoxPlayHarmonies.UseVisualStyleBackColor = true;
            this.checkBoxPlayHarmonies.CheckedChanged += new System.EventHandler(this.checkBoxPlayHarmonies_CheckedChanged);
            // 
            // checkBoxOplæsBecifringskoder
            // 
            this.checkBoxOplæsBecifringskoder.AutoSize = true;
            this.checkBoxOplæsBecifringskoder.Location = new System.Drawing.Point(982, 446);
            this.checkBoxOplæsBecifringskoder.Name = "checkBoxOplæsBecifringskoder";
            this.checkBoxOplæsBecifringskoder.Size = new System.Drawing.Size(132, 17);
            this.checkBoxOplæsBecifringskoder.TabIndex = 16;
            this.checkBoxOplæsBecifringskoder.Text = "Oplæs becifringskoder";
            this.checkBoxOplæsBecifringskoder.UseVisualStyleBackColor = true;
            this.checkBoxOplæsBecifringskoder.CheckedChanged += new System.EventHandler(this.checkBoxOplæsBecifringskoder_CheckedChanged);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // menuStripFile
            // 
            this.menuStripFile.Location = new System.Drawing.Point(0, 0);
            this.menuStripFile.Name = "menuStripFile";
            this.menuStripFile.Size = new System.Drawing.Size(1276, 24);
            this.menuStripFile.TabIndex = 17;
            this.menuStripFile.Text = "Filer";
            // 
            // checkBoxReadMeasureNumbers
            // 
            this.checkBoxReadMeasureNumbers.AutoSize = true;
            this.checkBoxReadMeasureNumbers.Location = new System.Drawing.Point(982, 286);
            this.checkBoxReadMeasureNumbers.Name = "checkBoxReadMeasureNumbers";
            this.checkBoxReadMeasureNumbers.Size = new System.Drawing.Size(111, 17);
            this.checkBoxReadMeasureNumbers.TabIndex = 18;
            this.checkBoxReadMeasureNumbers.Text = "Oplæs Taktnumre";
            this.checkBoxReadMeasureNumbers.UseVisualStyleBackColor = true;
            this.checkBoxReadMeasureNumbers.CheckedChanged += new System.EventHandler(this.checkBoxReadMeasureNumbers_CheckedChanged);
            // 
            // checkBoxPlayMeasureNumbers
            // 
            this.checkBoxPlayMeasureNumbers.AutoSize = true;
            this.checkBoxPlayMeasureNumbers.Location = new System.Drawing.Point(856, 286);
            this.checkBoxPlayMeasureNumbers.Name = "checkBoxPlayMeasureNumbers";
            this.checkBoxPlayMeasureNumbers.Size = new System.Drawing.Size(87, 17);
            this.checkBoxPlayMeasureNumbers.TabIndex = 19;
            this.checkBoxPlayMeasureNumbers.Text = "Spil Taktslag";
            this.checkBoxPlayMeasureNumbers.UseVisualStyleBackColor = true;
            // 
            // checkBoxReadEndEvents
            // 
            this.checkBoxReadEndEvents.AutoSize = true;
            this.checkBoxReadEndEvents.Location = new System.Drawing.Point(982, 466);
            this.checkBoxReadEndEvents.Name = "checkBoxReadEndEvents";
            this.checkBoxReadEndEvents.Size = new System.Drawing.Size(112, 17);
            this.checkBoxReadEndEvents.TabIndex = 20;
            this.checkBoxReadEndEvents.Text = "Oplæs EndEvents";
            this.checkBoxReadEndEvents.UseVisualStyleBackColor = true;
            this.checkBoxReadEndEvents.CheckedChanged += new System.EventHandler(this.checkBoxReadEndEvents_CheckedChanged);
            // 
            // checkedListBoxPartsToReadLyrics
            // 
            this.checkedListBoxPartsToReadLyrics.AccessibleName = "Oplæs lyrik";
            this.checkedListBoxPartsToReadLyrics.FormattingEnabled = true;
            this.checkedListBoxPartsToReadLyrics.Location = new System.Drawing.Point(1119, 72);
            this.checkedListBoxPartsToReadLyrics.Name = "checkedListBoxPartsToReadLyrics";
            this.checkedListBoxPartsToReadLyrics.Size = new System.Drawing.Size(120, 184);
            this.checkedListBoxPartsToReadLyrics.TabIndex = 21;
            this.checkedListBoxPartsToReadLyrics.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxPartsToReadLyrics_ItemCheck);
            // 
            // buttonReadLogFile
            // 
            this.buttonReadLogFile.Location = new System.Drawing.Point(856, 417);
            this.buttonReadLogFile.Name = "buttonReadLogFile";
            this.buttonReadLogFile.Size = new System.Drawing.Size(120, 23);
            this.buttonReadLogFile.TabIndex = 22;
            this.buttonReadLogFile.Text = "Oplæs logfil";
            this.buttonReadLogFile.UseVisualStyleBackColor = true;
            this.buttonReadLogFile.Click += new System.EventHandler(this.buttonReadLogFile_Click);
            // 
            // checkBoxReadPitch
            // 
            this.checkBoxReadPitch.AutoSize = true;
            this.checkBoxReadPitch.Checked = true;
            this.checkBoxReadPitch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxReadPitch.Location = new System.Drawing.Point(982, 310);
            this.checkBoxReadPitch.Name = "checkBoxReadPitch";
            this.checkBoxReadPitch.Size = new System.Drawing.Size(81, 17);
            this.checkBoxReadPitch.TabIndex = 23;
            this.checkBoxReadPitch.Text = "Oplæs tone";
            this.checkBoxReadPitch.UseVisualStyleBackColor = true;
            this.checkBoxReadPitch.CheckedChanged += new System.EventHandler(this.checkBoxReadPitch_CheckedChanged);
            // 
            // checkBoxReadOctave
            // 
            this.checkBoxReadOctave.AutoSize = true;
            this.checkBoxReadOctave.Checked = true;
            this.checkBoxReadOctave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxReadOctave.Location = new System.Drawing.Point(982, 334);
            this.checkBoxReadOctave.Name = "checkBoxReadOctave";
            this.checkBoxReadOctave.Size = new System.Drawing.Size(87, 17);
            this.checkBoxReadOctave.TabIndex = 24;
            this.checkBoxReadOctave.Text = "Oplæs oktav";
            this.checkBoxReadOctave.UseVisualStyleBackColor = true;
            this.checkBoxReadOctave.CheckedChanged += new System.EventHandler(this.checkBoxReadOctave_CheckedChanged);
            // 
            // checkBoxReadDuration
            // 
            this.checkBoxReadDuration.AutoSize = true;
            this.checkBoxReadDuration.Checked = true;
            this.checkBoxReadDuration.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxReadDuration.Location = new System.Drawing.Point(982, 358);
            this.checkBoxReadDuration.Name = "checkBoxReadDuration";
            this.checkBoxReadDuration.Size = new System.Drawing.Size(101, 17);
            this.checkBoxReadDuration.TabIndex = 25;
            this.checkBoxReadDuration.Text = "Oplæs varighed";
            this.checkBoxReadDuration.UseVisualStyleBackColor = true;
            this.checkBoxReadDuration.CheckedChanged += new System.EventHandler(this.checkBoxReadDuration_CheckedChanged);
            // 
            // buttonReadMusicXmlFile
            // 
            this.buttonReadMusicXmlFile.Location = new System.Drawing.Point(856, 446);
            this.buttonReadMusicXmlFile.Name = "buttonReadMusicXmlFile";
            this.buttonReadMusicXmlFile.Size = new System.Drawing.Size(120, 23);
            this.buttonReadMusicXmlFile.TabIndex = 26;
            this.buttonReadMusicXmlFile.Text = "Oplæs MusicXml fil";
            this.buttonReadMusicXmlFile.UseVisualStyleBackColor = true;
            this.buttonReadMusicXmlFile.Click += new System.EventHandler(this.buttonReadMusicXmlFile_Click);
            // 
            // buttonReadInterpretation
            // 
            this.buttonReadInterpretation.Location = new System.Drawing.Point(856, 476);
            this.buttonReadInterpretation.Name = "buttonReadInterpretation";
            this.buttonReadInterpretation.Size = new System.Drawing.Size(120, 23);
            this.buttonReadInterpretation.TabIndex = 27;
            this.buttonReadInterpretation.Text = "Oplæs fortolkning";
            this.buttonReadInterpretation.UseVisualStyleBackColor = true;
            this.buttonReadInterpretation.Click += new System.EventHandler(this.buttonReadInterpretation_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1276, 496);
            this.Controls.Add(this.buttonReadInterpretation);
            this.Controls.Add(this.buttonReadMusicXmlFile);
            this.Controls.Add(this.checkBoxReadDuration);
            this.Controls.Add(this.checkBoxReadOctave);
            this.Controls.Add(this.checkBoxReadPitch);
            this.Controls.Add(this.buttonReadLogFile);
            this.Controls.Add(this.checkedListBoxPartsToReadLyrics);
            this.Controls.Add(this.checkBoxReadEndEvents);
            this.Controls.Add(this.checkBoxPlayMeasureNumbers);
            this.Controls.Add(this.checkBoxReadMeasureNumbers);
            this.Controls.Add(this.checkBoxOplæsBecifringskoder);
            this.Controls.Add(this.checkBoxPlayHarmonies);
            this.Controls.Add(this.checkBoxShowHarmonies);
            this.Controls.Add(this.labelPartsRead);
            this.Controls.Add(this.labelPartsPlayed);
            this.Controls.Add(this.checkBoxShowStartTime);
            this.Controls.Add(this.checkedListBoxPartsToRead);
            this.Controls.Add(this.checkedListBoxPartsToPlay);
            this.Controls.Add(this.butonPlayPoly);
            this.Controls.Add(this.numericUpDownPlaySpeed);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.Play);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.listBoxFiltered);
            this.Controls.Add(this.listBoxTimes);
            this.Controls.Add(this.menuStripFile);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStripFile;
            this.Name = "Form1";
            this.Text = "MusikLæser";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPlaySpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox listBoxTimes;
        private System.Windows.Forms.ListBox listBoxFiltered;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button Play;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.NumericUpDown numericUpDownPlaySpeed;
        private System.Windows.Forms.Button butonPlayPoly;
        private System.Windows.Forms.CheckedListBox checkedListBoxPartsToPlay;
        private System.Windows.Forms.CheckedListBox checkedListBoxPartsToRead;
        private System.Windows.Forms.CheckBox checkBoxShowStartTime;
        private System.Windows.Forms.Label labelPartsPlayed;
        private System.Windows.Forms.Label labelPartsRead;
        private System.Windows.Forms.CheckBox checkBoxShowHarmonies;
        private System.Windows.Forms.CheckBox checkBoxPlayHarmonies;
        private System.Windows.Forms.CheckBox checkBoxOplæsBecifringskoder;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.MenuStrip menuStripFile;
        private System.Windows.Forms.CheckBox checkBoxReadMeasureNumbers;
        private System.Windows.Forms.CheckBox checkBoxPlayMeasureNumbers;
        private System.Windows.Forms.CheckBox checkBoxReadEndEvents;
        private System.Windows.Forms.CheckedListBox checkedListBoxPartsToReadLyrics;
        private System.Windows.Forms.Button buttonReadLogFile;
        private System.Windows.Forms.CheckBox checkBoxReadPitch;
        private System.Windows.Forms.CheckBox checkBoxReadOctave;
        private System.Windows.Forms.CheckBox checkBoxReadDuration;
        private System.Windows.Forms.Button buttonReadMusicXmlFile;
        private System.Windows.Forms.Button buttonReadInterpretation;
    }
}

