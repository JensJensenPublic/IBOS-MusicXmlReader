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
            this.labelPartsPlayed = new System.Windows.Forms.Label();
            this.labelPartsRead = new System.Windows.Forms.Label();
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
            this.toolStripMenuItemSpil = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemOplæs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logFileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.musicXmlFileLocationtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.museScoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xmlFilToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fortolketXMLFilToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAfspil = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPause = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkedListBoxPartsToReadLyrics = new System.Windows.Forms.CheckedListBox();
            this.labelReadLyrics = new System.Windows.Forms.Label();
            this.checkedListBoxParts = new System.Windows.Forms.CheckedListBox();
            this.labelSelectParts = new System.Windows.Forms.Label();
            this.checkedListBoxReaderSettings = new System.Windows.Forms.CheckedListBox();
            this.checkedListBoxPlayerSettings = new System.Windows.Forms.CheckedListBox();
            this.labelPlay = new System.Windows.Forms.Label();
            this.labelRead = new System.Windows.Forms.Label();
            this.textBoxBraille = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPlaySpeed)).BeginInit();
            this.menuStripFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxTimes
            // 
            this.listBoxTimes.AccessibleDescription = "";
            this.listBoxTimes.AccessibleName = "";
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
            this.Stop.Text = "&Pause";
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
            this.butonPlayPoly.Text = "&Spil";
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
            this.toolStripMenuItemAfspil,
            this.helpToolStripMenuItem});
            this.menuStripFile.Location = new System.Drawing.Point(0, 0);
            this.menuStripFile.Name = "menuStripFile";
            this.menuStripFile.Size = new System.Drawing.Size(1276, 24);
            this.menuStripFile.TabIndex = 0;
            this.menuStripFile.TabStop = true;
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
            this.stemmerMedLyrikoplæsningToolStripMenuItem,
            this.toolStripMenuItemSpil,
            this.toolStripMenuItemOplæs});
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
            // toolStripMenuItemSpil
            // 
            this.toolStripMenuItemSpil.Name = "toolStripMenuItemSpil";
            this.toolStripMenuItemSpil.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItemSpil.Text = "Spil";
            this.toolStripMenuItemSpil.Click += new System.EventHandler(this.toolStripMenuItemSpil_Click);
            // 
            // toolStripMenuItemOplæs
            // 
            this.toolStripMenuItemOplæs.Name = "toolStripMenuItemOplæs";
            this.toolStripMenuItemOplæs.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItemOplæs.Text = "Oplæs";
            this.toolStripMenuItemOplæs.Click += new System.EventHandler(this.toolStripMenuItemOplæs_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logFileLocationToolStripMenuItem,
            this.logFileToolStripMenuItem,
            this.musicXmlFileLocationtoolStripMenuItem,
            this.museScoreToolStripMenuItem,
            this.xmlFilToolStripMenuItem,
            this.fortolketXMLFilToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.toolsToolStripMenuItem.Text = "Værk&tøjer";
            // 
            // logFileLocationToolStripMenuItem
            // 
            this.logFileLocationToolStripMenuItem.Name = "logFileLocationToolStripMenuItem";
            this.logFileLocationToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.logFileLocationToolStripMenuItem.Text = "Log fil placering";
            this.logFileLocationToolStripMenuItem.Click += new System.EventHandler(this.logFileLocationToolStripMenuItem_Click);
            // 
            // logFileToolStripMenuItem
            // 
            this.logFileToolStripMenuItem.Name = "logFileToolStripMenuItem";
            this.logFileToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.logFileToolStripMenuItem.Text = "Log fil";
            this.logFileToolStripMenuItem.Click += new System.EventHandler(this.logFileToolStripMenuItem_Click);
            // 
            // musicXmlFileLocationtoolStripMenuItem
            // 
            this.musicXmlFileLocationtoolStripMenuItem.Name = "musicXmlFileLocationtoolStripMenuItem";
            this.musicXmlFileLocationtoolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.musicXmlFileLocationtoolStripMenuItem.Text = "MusicXml fil placering";
            this.musicXmlFileLocationtoolStripMenuItem.Click += new System.EventHandler(this.musicXmlFileLocationtoolStripMenuItem_Click);
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
            // toolStripMenuItemAfspil
            // 
            this.toolStripMenuItemAfspil.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemStart,
            this.toolStripMenuItemPause});
            this.toolStripMenuItemAfspil.Name = "toolStripMenuItemAfspil";
            this.toolStripMenuItemAfspil.Size = new System.Drawing.Size(49, 20);
            this.toolStripMenuItemAfspil.Text = "&Afspil";
            // 
            // toolStripMenuItemStart
            // 
            this.toolStripMenuItemStart.Name = "toolStripMenuItemStart";
            this.toolStripMenuItemStart.Size = new System.Drawing.Size(105, 22);
            this.toolStripMenuItemStart.Text = "Start";
            this.toolStripMenuItemStart.Click += new System.EventHandler(this.toolStripMenuItemStart_Click);
            // 
            // toolStripMenuItemPause
            // 
            this.toolStripMenuItemPause.Name = "toolStripMenuItemPause";
            this.toolStripMenuItemPause.Size = new System.Drawing.Size(105, 22);
            this.toolStripMenuItemPause.Text = "Pause";
            this.toolStripMenuItemPause.Click += new System.EventHandler(this.toolStripMenuItemPause_Click);
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
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
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
            this.checkedListBoxParts.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxParts_SelectedIndexChanged);
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
            // checkedListBoxReaderSettings
            // 
            this.checkedListBoxReaderSettings.AccessibleDescription = "Oplæs";
            this.checkedListBoxReaderSettings.AccessibleName = "Oplæs";
            this.checkedListBoxReaderSettings.FormattingEnabled = true;
            this.checkedListBoxReaderSettings.Location = new System.Drawing.Point(963, 285);
            this.checkedListBoxReaderSettings.Name = "checkedListBoxReaderSettings";
            this.checkedListBoxReaderSettings.Size = new System.Drawing.Size(120, 199);
            this.checkedListBoxReaderSettings.TabIndex = 30;
            this.checkedListBoxReaderSettings.TabStop = false;
            this.checkedListBoxReaderSettings.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxReaderSettings_ItemCheck);
            // 
            // checkedListBoxPlayerSettings
            // 
            this.checkedListBoxPlayerSettings.AccessibleDescription = "Spil";
            this.checkedListBoxPlayerSettings.AccessibleName = "Spil";
            this.checkedListBoxPlayerSettings.FormattingEnabled = true;
            this.checkedListBoxPlayerSettings.Location = new System.Drawing.Point(837, 285);
            this.checkedListBoxPlayerSettings.Name = "checkedListBoxPlayerSettings";
            this.checkedListBoxPlayerSettings.Size = new System.Drawing.Size(120, 199);
            this.checkedListBoxPlayerSettings.TabIndex = 31;
            this.checkedListBoxPlayerSettings.TabStop = false;
            this.checkedListBoxPlayerSettings.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxPlayerSettings_ItemCheck);
            // 
            // labelPlay
            // 
            this.labelPlay.AutoSize = true;
            this.labelPlay.Location = new System.Drawing.Point(837, 264);
            this.labelPlay.Name = "labelPlay";
            this.labelPlay.Size = new System.Drawing.Size(24, 13);
            this.labelPlay.TabIndex = 32;
            this.labelPlay.Text = "Spil";
            // 
            // labelRead
            // 
            this.labelRead.AutoSize = true;
            this.labelRead.Location = new System.Drawing.Point(963, 264);
            this.labelRead.Name = "labelRead";
            this.labelRead.Size = new System.Drawing.Size(38, 13);
            this.labelRead.TabIndex = 33;
            this.labelRead.Text = "Oplæs";
            // 
            // textBoxBraille
            // 
            this.textBoxBraille.BackColor = System.Drawing.Color.Black;
            this.textBoxBraille.Font = new System.Drawing.Font("BrailleUnicode6", 36F);
            this.textBoxBraille.ForeColor = System.Drawing.Color.White;
            this.textBoxBraille.Location = new System.Drawing.Point(257, 490);
            this.textBoxBraille.Name = "textBoxBraille";
            this.textBoxBraille.Size = new System.Drawing.Size(574, 68);
            this.textBoxBraille.TabIndex = 34;
            this.textBoxBraille.TabStop = false;
            this.textBoxBraille.TextChanged += new System.EventHandler(this.textBoxBraille_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1276, 578);
            this.Controls.Add(this.textBoxBraille);
            this.Controls.Add(this.labelRead);
            this.Controls.Add(this.labelPlay);
            this.Controls.Add(this.checkedListBoxPlayerSettings);
            this.Controls.Add(this.checkedListBoxReaderSettings);
            this.Controls.Add(this.labelSelectParts);
            this.Controls.Add(this.checkedListBoxParts);
            this.Controls.Add(this.labelReadLyrics);
            this.Controls.Add(this.checkedListBoxPartsToReadLyrics);
            this.Controls.Add(this.labelPartsRead);
            this.Controls.Add(this.labelPartsPlayed);
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
        private System.Windows.Forms.Label labelPartsPlayed;
        private System.Windows.Forms.Label labelPartsRead;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.MenuStrip menuStripFile;
        private System.Windows.Forms.CheckedListBox checkedListBoxPartsToReadLyrics;
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
        private System.Windows.Forms.ToolStripMenuItem logFileLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem musicXmlFileLocationtoolStripMenuItem;
        private System.Windows.Forms.CheckedListBox checkedListBoxReaderSettings;
        private System.Windows.Forms.CheckedListBox checkedListBoxPlayerSettings;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSpil;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOplæs;
        private System.Windows.Forms.Label labelPlay;
        private System.Windows.Forms.Label labelRead;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAfspil;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStart;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPause;
        private System.Windows.Forms.TextBox textBoxBraille;
    }
}

