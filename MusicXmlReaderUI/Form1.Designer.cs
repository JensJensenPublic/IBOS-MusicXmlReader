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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.listBoxTimes = new System.Windows.Forms.ListBox();
            this.listBoxFiltered = new System.Windows.Forms.ListBox();
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
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.stemmerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stemmerSomSpillesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stemmerSomOplæsesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stemmerMedLyrikoplæsningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.museScoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xmlFilToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fortolketXMLFilToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxReadMeasureNumbers = new System.Windows.Forms.CheckBox();
            this.checkBoxPlayMeasureNumbers = new System.Windows.Forms.CheckBox();
            this.checkBoxReadEndEvents = new System.Windows.Forms.CheckBox();
            this.checkedListBoxPartsToReadLyrics = new System.Windows.Forms.CheckedListBox();
            this.checkBoxReadPitch = new System.Windows.Forms.CheckBox();
            this.checkBoxReadOctave = new System.Windows.Forms.CheckBox();
            this.checkBoxReadDuration = new System.Windows.Forms.CheckBox();
            this.labelReadLyrics = new System.Windows.Forms.Label();
            this.checkedListBoxParts = new System.Windows.Forms.CheckedListBox();
            this.labelSelectParts = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPlaySpeed)).BeginInit();
            this.menuStripFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxTimes
            // 
            this.listBoxTimes.AccessibleDescription = "Stemmer";
            this.listBoxTimes.AccessibleName = "Stemmer";
            this.listBoxTimes.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.listBoxTimes.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxTimes.FormattingEnabled = true;
            this.listBoxTimes.Location = new System.Drawing.Point(257, 77);
            this.listBoxTimes.Name = "listBoxTimes";
            this.listBoxTimes.Size = new System.Drawing.Size(574, 407);
            this.listBoxTimes.TabIndex = 1;
            this.listBoxTimes.SelectedIndexChanged += new System.EventHandler(this.listBoxTimes_SelectedIndexChanged);
            // 
            // listBoxFiltered
            // 
            this.listBoxFiltered.AccessibleDescription = "Metainformation";
            this.listBoxFiltered.AccessibleName = "Metainformation";
            this.listBoxFiltered.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.listBoxFiltered.FormattingEnabled = true;
            this.listBoxFiltered.Location = new System.Drawing.Point(12, 77);
            this.listBoxFiltered.Name = "listBoxFiltered";
            this.listBoxFiltered.Size = new System.Drawing.Size(239, 407);
            this.listBoxFiltered.TabIndex = 4;
            // 
            // Stop
            // 
            this.Stop.Location = new System.Drawing.Point(350, 53);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(75, 23);
            this.Stop.TabIndex = 3;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(12, 27);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(1001, 20);
            this.textBoxMessage.TabIndex = 0;
            this.textBoxMessage.TabStop = false;
            // 
            // numericUpDownPlaySpeed
            // 
            this.numericUpDownPlaySpeed.Enabled = false;
            this.numericUpDownPlaySpeed.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPlaySpeed.Location = new System.Drawing.Point(440, 55);
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
            this.numericUpDownPlaySpeed.TabIndex = 0;
            this.numericUpDownPlaySpeed.TabStop = false;
            this.numericUpDownPlaySpeed.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownPlaySpeed.ValueChanged += new System.EventHandler(this.numericUpDownPlaySpeed_ValueChanged);
            // 
            // butonPlayPoly
            // 
            this.butonPlayPoly.Location = new System.Drawing.Point(257, 52);
            this.butonPlayPoly.Name = "butonPlayPoly";
            this.butonPlayPoly.Size = new System.Drawing.Size(75, 23);
            this.butonPlayPoly.TabIndex = 2;
            this.butonPlayPoly.Text = "Spil flerstemmigt";
            this.butonPlayPoly.UseVisualStyleBackColor = true;
            this.butonPlayPoly.Click += new System.EventHandler(this.butonPlayPoly_Click);
            // 
            // checkedListBoxPartsToPlay
            // 
            this.checkedListBoxPartsToPlay.AccessibleDescription = "En checkbox for hver stemme. Hvis checked spilles stemmen";
            this.checkedListBoxPartsToPlay.AccessibleName = "Spil stemmer";
            this.checkedListBoxPartsToPlay.FormattingEnabled = true;
            this.checkedListBoxPartsToPlay.Location = new System.Drawing.Point(963, 72);
            this.checkedListBoxPartsToPlay.Name = "checkedListBoxPartsToPlay";
            this.checkedListBoxPartsToPlay.Size = new System.Drawing.Size(100, 184);
            this.checkedListBoxPartsToPlay.TabIndex = 4;
            this.checkedListBoxPartsToPlay.TabStop = false;
            this.checkedListBoxPartsToPlay.Tag = "";
            this.checkedListBoxPartsToPlay.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxPartsToPlay_ItemCheck);
            // 
            // checkedListBoxPartsToRead
            // 
            this.checkedListBoxPartsToRead.AccessibleName = "Oplæs stemmer";
            this.checkedListBoxPartsToRead.FormattingEnabled = true;
            this.checkedListBoxPartsToRead.Location = new System.Drawing.Point(1066, 72);
            this.checkedListBoxPartsToRead.Name = "checkedListBoxPartsToRead";
            this.checkedListBoxPartsToRead.Size = new System.Drawing.Size(100, 184);
            this.checkedListBoxPartsToRead.TabIndex = 4;
            this.checkedListBoxPartsToRead.TabStop = false;
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
            this.checkBoxShowStartTime.TabStop = false;
            this.checkBoxShowStartTime.Text = "Oplæs starttid";
            this.checkBoxShowStartTime.UseVisualStyleBackColor = true;
            this.checkBoxShowStartTime.CheckedChanged += new System.EventHandler(this.checkBoxShowStartTime_CheckedChanged);
            // 
            // labelPartsPlayed
            // 
            this.labelPartsPlayed.AutoSize = true;
            this.labelPartsPlayed.Location = new System.Drawing.Point(960, 53);
            this.labelPartsPlayed.Name = "labelPartsPlayed";
            this.labelPartsPlayed.Size = new System.Drawing.Size(66, 13);
            this.labelPartsPlayed.TabIndex = 12;
            this.labelPartsPlayed.Text = "Spil stemmer";
            // 
            // labelPartsRead
            // 
            this.labelPartsRead.AutoSize = true;
            this.labelPartsRead.Location = new System.Drawing.Point(1063, 53);
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
            this.checkBoxShowHarmonies.TabStop = false;
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
            this.checkBoxPlayHarmonies.TabStop = false;
            this.checkBoxPlayHarmonies.Text = "Spil Becifringer";
            this.checkBoxPlayHarmonies.UseVisualStyleBackColor = true;
            this.checkBoxPlayHarmonies.CheckedChanged += new System.EventHandler(this.checkBoxPlayHarmonies_CheckedChanged);
            // 
            // checkBoxOplæsBecifringskoder
            // 
            this.checkBoxOplæsBecifringskoder.AutoSize = true;
            this.checkBoxOplæsBecifringskoder.Location = new System.Drawing.Point(979, 446);
            this.checkBoxOplæsBecifringskoder.Name = "checkBoxOplæsBecifringskoder";
            this.checkBoxOplæsBecifringskoder.Size = new System.Drawing.Size(132, 17);
            this.checkBoxOplæsBecifringskoder.TabIndex = 16;
            this.checkBoxOplæsBecifringskoder.TabStop = false;
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
            this.menuStripFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripFile.Location = new System.Drawing.Point(0, 0);
            this.menuStripFile.Name = "menuStripFile";
            this.menuStripFile.Size = new System.Drawing.Size(1276, 24);
            this.menuStripFile.TabIndex = 17;
            this.menuStripFile.Text = "Filer";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator,
            this.toolStripSeparator1,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.fileToolStripMenuItem.Text = "&Filer";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.openToolStripMenuItem.Text = "Å&bn MusicXml fil";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(203, 6);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(203, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(203, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator3,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator4,
            this.selectAllToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.editToolStripMenuItem.Text = "&Rediger";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.redoToolStripMenuItem.Text = "&Redo";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(141, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(141, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stemmerToolStripMenuItem,
            this.stemmerSomSpillesToolStripMenuItem,
            this.stemmerSomOplæsesToolStripMenuItem,
            this.stemmerMedLyrikoplæsningToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(34, 20);
            this.toolStripMenuItem1.Text = "&Vis";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // stemmerToolStripMenuItem
            // 
            this.stemmerToolStripMenuItem.Name = "stemmerToolStripMenuItem";
            this.stemmerToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.stemmerToolStripMenuItem.Text = "Stemmer";
            this.stemmerToolStripMenuItem.Click += new System.EventHandler(this.stemmerToolStripMenuItem_Click);
            // 
            // stemmerSomSpillesToolStripMenuItem
            // 
            this.stemmerSomSpillesToolStripMenuItem.Name = "stemmerSomSpillesToolStripMenuItem";
            this.stemmerSomSpillesToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.stemmerSomSpillesToolStripMenuItem.Text = "Stemmer (lydafspilning)";
            this.stemmerSomSpillesToolStripMenuItem.Click += new System.EventHandler(this.stemmerSomSpillesToolStripMenuItem_Click);
            // 
            // stemmerSomOplæsesToolStripMenuItem
            // 
            this.stemmerSomOplæsesToolStripMenuItem.Name = "stemmerSomOplæsesToolStripMenuItem";
            this.stemmerSomOplæsesToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.stemmerSomOplæsesToolStripMenuItem.Text = "Stemmer (nodeoplæsning)";
            this.stemmerSomOplæsesToolStripMenuItem.Click += new System.EventHandler(this.stemmerSomOplæsesToolStripMenuItem_Click);
            // 
            // stemmerMedLyrikoplæsningToolStripMenuItem
            // 
            this.stemmerMedLyrikoplæsningToolStripMenuItem.Name = "stemmerMedLyrikoplæsningToolStripMenuItem";
            this.stemmerMedLyrikoplæsningToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.stemmerMedLyrikoplæsningToolStripMenuItem.Text = "Stemmer (lyrikoplæsning)";
            this.stemmerMedLyrikoplæsningToolStripMenuItem.Click += new System.EventHandler(this.stemmerMedLyrikoplæsningToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logFileToolStripMenuItem,
            this.museScoreToolStripMenuItem,
            this.xmlFilToolStripMenuItem,
            this.fortolketXMLFilToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.toolsToolStripMenuItem.Text = "Værk&tøjer";
            // 
            // logFileToolStripMenuItem
            // 
            this.logFileToolStripMenuItem.Name = "logFileToolStripMenuItem";
            this.logFileToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.logFileToolStripMenuItem.Text = "Log fil";
            this.logFileToolStripMenuItem.Click += new System.EventHandler(this.logFileToolStripMenuItem_Click);
            // 
            // museScoreToolStripMenuItem
            // 
            this.museScoreToolStripMenuItem.Name = "museScoreToolStripMenuItem";
            this.museScoreToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.museScoreToolStripMenuItem.Text = "MusicXml fil (i MuseScore)";
            this.museScoreToolStripMenuItem.Click += new System.EventHandler(this.museScoreToolStripMenuItem_Click);
            // 
            // xmlFilToolStripMenuItem
            // 
            this.xmlFilToolStripMenuItem.Name = "xmlFilToolStripMenuItem";
            this.xmlFilToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.xmlFilToolStripMenuItem.Text = "MusicXml fil (som XML)";
            this.xmlFilToolStripMenuItem.Click += new System.EventHandler(this.xmlFilToolStripMenuItem_Click);
            // 
            // fortolketXMLFilToolStripMenuItem
            // 
            this.fortolketXMLFilToolStripMenuItem.Name = "fortolketXMLFilToolStripMenuItem";
            this.fortolketXMLFilToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.fortolketXMLFilToolStripMenuItem.Text = "MusicXml fil (fortolket)";
            this.fortolketXMLFilToolStripMenuItem.Click += new System.EventHandler(this.fortolketXMLFilToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.indexToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.helpToolStripMenuItem.Text = "&Hjælp";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(119, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // checkBoxReadMeasureNumbers
            // 
            this.checkBoxReadMeasureNumbers.AutoSize = true;
            this.checkBoxReadMeasureNumbers.Checked = true;
            this.checkBoxReadMeasureNumbers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxReadMeasureNumbers.Location = new System.Drawing.Point(982, 286);
            this.checkBoxReadMeasureNumbers.Name = "checkBoxReadMeasureNumbers";
            this.checkBoxReadMeasureNumbers.Size = new System.Drawing.Size(111, 17);
            this.checkBoxReadMeasureNumbers.TabIndex = 18;
            this.checkBoxReadMeasureNumbers.TabStop = false;
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
            this.checkBoxPlayMeasureNumbers.TabStop = false;
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
            this.checkBoxReadEndEvents.TabStop = false;
            this.checkBoxReadEndEvents.Text = "Oplæs EndEvents";
            this.checkBoxReadEndEvents.UseVisualStyleBackColor = true;
            this.checkBoxReadEndEvents.CheckedChanged += new System.EventHandler(this.checkBoxReadEndEvents_CheckedChanged);
            // 
            // checkedListBoxPartsToReadLyrics
            // 
            this.checkedListBoxPartsToReadLyrics.AccessibleName = "Oplæs lyrik";
            this.checkedListBoxPartsToReadLyrics.FormattingEnabled = true;
            this.checkedListBoxPartsToReadLyrics.Location = new System.Drawing.Point(1172, 72);
            this.checkedListBoxPartsToReadLyrics.Name = "checkedListBoxPartsToReadLyrics";
            this.checkedListBoxPartsToReadLyrics.Size = new System.Drawing.Size(100, 184);
            this.checkedListBoxPartsToReadLyrics.TabIndex = 5;
            this.checkedListBoxPartsToReadLyrics.TabStop = false;
            this.checkedListBoxPartsToReadLyrics.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxPartsToReadLyrics_ItemCheck);
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
            this.checkBoxReadPitch.TabStop = false;
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
            this.checkBoxReadOctave.TabStop = false;
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
            this.checkBoxReadDuration.TabStop = false;
            this.checkBoxReadDuration.Text = "Oplæs varighed";
            this.checkBoxReadDuration.UseVisualStyleBackColor = true;
            this.checkBoxReadDuration.CheckedChanged += new System.EventHandler(this.checkBoxReadDuration_CheckedChanged);
            // 
            // labelReadLyrics
            // 
            this.labelReadLyrics.AutoSize = true;
            this.labelReadLyrics.Location = new System.Drawing.Point(1169, 52);
            this.labelReadLyrics.Name = "labelReadLyrics";
            this.labelReadLyrics.Size = new System.Drawing.Size(59, 13);
            this.labelReadLyrics.TabIndex = 28;
            this.labelReadLyrics.Text = "Oplæs lyrik";
            // 
            // checkedListBoxParts
            // 
            this.checkedListBoxParts.FormattingEnabled = true;
            this.checkedListBoxParts.Location = new System.Drawing.Point(837, 73);
            this.checkedListBoxParts.Name = "checkedListBoxParts";
            this.checkedListBoxParts.Size = new System.Drawing.Size(120, 184);
            this.checkedListBoxParts.TabIndex = 3;
            this.checkedListBoxParts.TabStop = false;
            this.checkedListBoxParts.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxParts_ItemCheck);
            // 
            // labelSelectParts
            // 
            this.labelSelectParts.AutoSize = true;
            this.labelSelectParts.Location = new System.Drawing.Point(834, 53);
            this.labelSelectParts.Name = "labelSelectParts";
            this.labelSelectParts.Size = new System.Drawing.Size(74, 13);
            this.labelSelectParts.TabIndex = 29;
            this.labelSelectParts.Text = "Vælg stemmer";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1276, 496);
            this.Controls.Add(this.labelSelectParts);
            this.Controls.Add(this.checkedListBoxParts);
            this.Controls.Add(this.labelReadLyrics);
            this.Controls.Add(this.checkBoxReadDuration);
            this.Controls.Add(this.checkBoxReadOctave);
            this.Controls.Add(this.checkBoxReadPitch);
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
            this.Controls.Add(this.listBoxFiltered);
            this.Controls.Add(this.listBoxTimes);
            this.Controls.Add(this.menuStripFile);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStripFile;
            this.Name = "Form1";
            this.Text = "MusikLæser";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPlaySpeed)).EndInit();
            this.menuStripFile.ResumeLayout(false);
            this.menuStripFile.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox listBoxTimes;
        private System.Windows.Forms.ListBox listBoxFiltered;
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
        private System.Windows.Forms.CheckBox checkBoxReadPitch;
        private System.Windows.Forms.CheckBox checkBoxReadOctave;
        private System.Windows.Forms.CheckBox checkBoxReadDuration;
        private System.Windows.Forms.Label labelReadLyrics;
        private System.Windows.Forms.CheckedListBox checkedListBoxParts;
        private System.Windows.Forms.Label labelSelectParts;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem museScoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xmlFilToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fortolketXMLFilToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem stemmerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stemmerSomSpillesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stemmerSomOplæsesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stemmerMedLyrikoplæsningToolStripMenuItem;
    }
}

