namespace MusicBrailleReader
{
    partial class MusicBrailleReaderMainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openUsingNOTAProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.octoBraille1252ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aSCIIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uNICODEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uft8ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uft16ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utf32ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pEFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openUsingBrailleOrchProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTestFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exporterSomMusicXmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exporterSomTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iBOSMusicXmlReaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.museScoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logfileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regressionTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regressionReferenceLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyRenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transscribeHøjskolesangbogenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jAWSSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jAWSSettingsDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jAWSSettingsUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.listBoxOffsets = new System.Windows.Forms.ListBox();
            this.textBoxBraille = new System.Windows.Forms.TextBox();
            this.userSettingsTreeView = new System.Windows.Forms.TreeView();
            this.textBoxText = new System.Windows.Forms.TextBox();
            this.textBoxStatusInformation = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.exportAsSeparateScoresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.octoBraillebrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aSCIIToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uNICODEToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pEFToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.utf8ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utf16ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utf32ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1146, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // filesToolStripMenuItem
            // 
            this.filesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openUsingNOTAProfileToolStripMenuItem,
            this.openUsingBrailleOrchProfileToolStripMenuItem,
            this.openTestFileToolStripMenuItem,
            this.exporterSomMusicXmlToolStripMenuItem,
            this.exporterSomTextToolStripMenuItem,
            this.exitToolStripMenuItem,
            this.exportAsSeparateScoresToolStripMenuItem});
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            this.filesToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.filesToolStripMenuItem.Text = "Files";
            // 
            // openUsingNOTAProfileToolStripMenuItem
            // 
            this.openUsingNOTAProfileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.octoBraille1252ToolStripMenuItem,
            this.aSCIIToolStripMenuItem,
            this.uNICODEToolStripMenuItem,
            this.pEFToolStripMenuItem});
            this.openUsingNOTAProfileToolStripMenuItem.Name = "openUsingNOTAProfileToolStripMenuItem";
            this.openUsingNOTAProfileToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openUsingNOTAProfileToolStripMenuItem.Text = "Open using NOTA profile ";
            this.openUsingNOTAProfileToolStripMenuItem.Click += new System.EventHandler(this.openUsingNOTAProfileToolStripMenuItem_Click);
            // 
            // octoBraille1252ToolStripMenuItem
            // 
            this.octoBraille1252ToolStripMenuItem.Name = "octoBraille1252ToolStripMenuItem";
            this.octoBraille1252ToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.octoBraille1252ToolStripMenuItem.Text = "OctoBraille_1252";
            this.octoBraille1252ToolStripMenuItem.Click += new System.EventHandler(this.octoBraille1252ToolStripMenuItem_Click);
            // 
            // aSCIIToolStripMenuItem
            // 
            this.aSCIIToolStripMenuItem.Name = "aSCIIToolStripMenuItem";
            this.aSCIIToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.aSCIIToolStripMenuItem.Text = "ASCII";
            this.aSCIIToolStripMenuItem.Click += new System.EventHandler(this.aSCIIToolStripMenuItem_Click);
            // 
            // uNICODEToolStripMenuItem
            // 
            this.uNICODEToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uft8ToolStripMenuItem,
            this.uft16ToolStripMenuItem,
            this.utf32ToolStripMenuItem});
            this.uNICODEToolStripMenuItem.Name = "uNICODEToolStripMenuItem";
            this.uNICODEToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.uNICODEToolStripMenuItem.Text = "UNICODE";
            // 
            // uft8ToolStripMenuItem
            // 
            this.uft8ToolStripMenuItem.Name = "uft8ToolStripMenuItem";
            this.uft8ToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.uft8ToolStripMenuItem.Text = "Uft-8";
            this.uft8ToolStripMenuItem.Click += new System.EventHandler(this.uft8ToolStripMenuItem_Click);
            // 
            // uft16ToolStripMenuItem
            // 
            this.uft16ToolStripMenuItem.Name = "uft16ToolStripMenuItem";
            this.uft16ToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.uft16ToolStripMenuItem.Text = "Uft-16";
            this.uft16ToolStripMenuItem.Click += new System.EventHandler(this.uft16ToolStripMenuItem_Click);
            // 
            // utf32ToolStripMenuItem
            // 
            this.utf32ToolStripMenuItem.Name = "utf32ToolStripMenuItem";
            this.utf32ToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.utf32ToolStripMenuItem.Text = "Utf-32";
            this.utf32ToolStripMenuItem.Click += new System.EventHandler(this.utf32ToolStripMenuItem_Click);
            // 
            // pEFToolStripMenuItem
            // 
            this.pEFToolStripMenuItem.Name = "pEFToolStripMenuItem";
            this.pEFToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.pEFToolStripMenuItem.Text = "PEF";
            this.pEFToolStripMenuItem.Click += new System.EventHandler(this.pEFToolStripMenuItem_Click);
            // 
            // openUsingBrailleOrchProfileToolStripMenuItem
            // 
            this.openUsingBrailleOrchProfileToolStripMenuItem.Name = "openUsingBrailleOrchProfileToolStripMenuItem";
            this.openUsingBrailleOrchProfileToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openUsingBrailleOrchProfileToolStripMenuItem.Text = "Open using BrailleOrch profile";
            this.openUsingBrailleOrchProfileToolStripMenuItem.Click += new System.EventHandler(this.openUsingBrailleOrchProfileToolStripMenuItem_Click);
            // 
            // openTestFileToolStripMenuItem
            // 
            this.openTestFileToolStripMenuItem.Name = "openTestFileToolStripMenuItem";
            this.openTestFileToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openTestFileToolStripMenuItem.Text = "Open test file";
            this.openTestFileToolStripMenuItem.Click += new System.EventHandler(this.openTestFileToolStripMenuItem_Click);
            // 
            // exporterSomMusicXmlToolStripMenuItem
            // 
            this.exporterSomMusicXmlToolStripMenuItem.Name = "exporterSomMusicXmlToolStripMenuItem";
            this.exporterSomMusicXmlToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.exporterSomMusicXmlToolStripMenuItem.Text = "Exporter som MusicXml";
            this.exporterSomMusicXmlToolStripMenuItem.Click += new System.EventHandler(this.exporterSomMusicXmlToolStripMenuItem_Click);
            // 
            // exporterSomTextToolStripMenuItem
            // 
            this.exporterSomTextToolStripMenuItem.Name = "exporterSomTextToolStripMenuItem";
            this.exporterSomTextToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.exporterSomTextToolStripMenuItem.Text = "Exporter som text";
            this.exporterSomTextToolStripMenuItem.Click += new System.EventHandler(this.exporterSomTextToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iBOSMusicXmlReaderToolStripMenuItem,
            this.museScoreToolStripMenuItem,
            this.logFileToolStripMenuItem,
            this.logfileLocationToolStripMenuItem,
            this.regressionTestToolStripMenuItem,
            this.regressionReferenceLocationToolStripMenuItem,
            this.copyRenameToolStripMenuItem,
            this.transscribeHøjskolesangbogenToolStripMenuItem,
            this.jAWSSettingsToolStripMenuItem,
            this.jAWSSettingsDirectoryToolStripMenuItem,
            this.jAWSSettingsUpdateToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // iBOSMusicXmlReaderToolStripMenuItem
            // 
            this.iBOSMusicXmlReaderToolStripMenuItem.Name = "iBOSMusicXmlReaderToolStripMenuItem";
            this.iBOSMusicXmlReaderToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.iBOSMusicXmlReaderToolStripMenuItem.Text = "IBOS MusicXmlReader";
            this.iBOSMusicXmlReaderToolStripMenuItem.Click += new System.EventHandler(this.iBOSMusicXmlReaderToolStripMenuItem_Click);
            // 
            // museScoreToolStripMenuItem
            // 
            this.museScoreToolStripMenuItem.Name = "museScoreToolStripMenuItem";
            this.museScoreToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.museScoreToolStripMenuItem.Text = "MuseScore";
            this.museScoreToolStripMenuItem.Click += new System.EventHandler(this.museScoreToolStripMenuItem_Click);
            // 
            // logFileToolStripMenuItem
            // 
            this.logFileToolStripMenuItem.Name = "logFileToolStripMenuItem";
            this.logFileToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.logFileToolStripMenuItem.Text = "LogFile";
            this.logFileToolStripMenuItem.Click += new System.EventHandler(this.logFileToolStripMenuItem_Click);
            // 
            // logfileLocationToolStripMenuItem
            // 
            this.logfileLocationToolStripMenuItem.Name = "logfileLocationToolStripMenuItem";
            this.logfileLocationToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.logfileLocationToolStripMenuItem.Text = "Logfile location";
            this.logfileLocationToolStripMenuItem.Click += new System.EventHandler(this.logfileLocationToolStripMenuItem_Click);
            // 
            // regressionTestToolStripMenuItem
            // 
            this.regressionTestToolStripMenuItem.Name = "regressionTestToolStripMenuItem";
            this.regressionTestToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.regressionTestToolStripMenuItem.Text = "Regression test";
            this.regressionTestToolStripMenuItem.Click += new System.EventHandler(this.regressionTestToolStripMenuItem_Click);
            // 
            // regressionReferenceLocationToolStripMenuItem
            // 
            this.regressionReferenceLocationToolStripMenuItem.Name = "regressionReferenceLocationToolStripMenuItem";
            this.regressionReferenceLocationToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.regressionReferenceLocationToolStripMenuItem.Text = "Regression reference location";
            this.regressionReferenceLocationToolStripMenuItem.Click += new System.EventHandler(this.regressionReferenceLocationToolStripMenuItem_Click);
            // 
            // copyRenameToolStripMenuItem
            // 
            this.copyRenameToolStripMenuItem.Name = "copyRenameToolStripMenuItem";
            this.copyRenameToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.copyRenameToolStripMenuItem.Text = "CopyRename Højskolesangbogen";
            this.copyRenameToolStripMenuItem.Click += new System.EventHandler(this.copyRenameToolStripMenuItem_Click);
            // 
            // transscribeHøjskolesangbogenToolStripMenuItem
            // 
            this.transscribeHøjskolesangbogenToolStripMenuItem.Name = "transscribeHøjskolesangbogenToolStripMenuItem";
            this.transscribeHøjskolesangbogenToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.transscribeHøjskolesangbogenToolStripMenuItem.Text = "Transscribe Højskolesangbogen";
            this.transscribeHøjskolesangbogenToolStripMenuItem.Click += new System.EventHandler(this.transscribeHøjskolesangbogenToolStripMenuItem_Click);
            // 
            // jAWSSettingsToolStripMenuItem
            // 
            this.jAWSSettingsToolStripMenuItem.Name = "jAWSSettingsToolStripMenuItem";
            this.jAWSSettingsToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.jAWSSettingsToolStripMenuItem.Text = "JAWS Settings";
            this.jAWSSettingsToolStripMenuItem.Click += new System.EventHandler(this.jAWSSettingsToolStripMenuItem_Click);
            // 
            // jAWSSettingsDirectoryToolStripMenuItem
            // 
            this.jAWSSettingsDirectoryToolStripMenuItem.Name = "jAWSSettingsDirectoryToolStripMenuItem";
            this.jAWSSettingsDirectoryToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.jAWSSettingsDirectoryToolStripMenuItem.Text = "JAWS Settings directory";
            this.jAWSSettingsDirectoryToolStripMenuItem.Click += new System.EventHandler(this.jAWSSettingsDirectoryToolStripMenuItem_Click);
            // 
            // jAWSSettingsUpdateToolStripMenuItem
            // 
            this.jAWSSettingsUpdateToolStripMenuItem.Name = "jAWSSettingsUpdateToolStripMenuItem";
            this.jAWSSettingsUpdateToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.jAWSSettingsUpdateToolStripMenuItem.Text = "JAWS Settings restore defaults";
            this.jAWSSettingsUpdateToolStripMenuItem.Click += new System.EventHandler(this.jAWSSettingsUpdateToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // listBoxOffsets
            // 
            this.listBoxOffsets.AccessibleName = "Punktnodeliste";
            this.listBoxOffsets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxOffsets.FormattingEnabled = true;
            this.listBoxOffsets.Location = new System.Drawing.Point(258, 89);
            this.listBoxOffsets.Name = "listBoxOffsets";
            this.listBoxOffsets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxOffsets.Size = new System.Drawing.Size(876, 316);
            this.listBoxOffsets.TabIndex = 0;
            this.listBoxOffsets.SelectedIndexChanged += new System.EventHandler(this.listBoxDecodedAsText_SelectedIndexChanged);
            // 
            // textBoxBraille
            // 
            this.textBoxBraille.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBraille.BackColor = System.Drawing.Color.Black;
            this.textBoxBraille.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxBraille.ForeColor = System.Drawing.Color.White;
            this.textBoxBraille.Location = new System.Drawing.Point(12, 437);
            this.textBoxBraille.Name = "textBoxBraille";
            this.textBoxBraille.Size = new System.Drawing.Size(1122, 62);
            this.textBoxBraille.TabIndex = 0;
            this.textBoxBraille.TabStop = false;
            // 
            // userSettingsTreeView
            // 
            this.userSettingsTreeView.AccessibleName = "Note Filter";
            this.userSettingsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.userSettingsTreeView.CheckBoxes = true;
            this.userSettingsTreeView.Location = new System.Drawing.Point(12, 27);
            this.userSettingsTreeView.Name = "userSettingsTreeView";
            this.userSettingsTreeView.Size = new System.Drawing.Size(240, 378);
            this.userSettingsTreeView.TabIndex = 1;
            // 
            // textBoxText
            // 
            this.textBoxText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxText.Location = new System.Drawing.Point(12, 504);
            this.textBoxText.Name = "textBoxText";
            this.textBoxText.Size = new System.Drawing.Size(1122, 20);
            this.textBoxText.TabIndex = 0;
            this.textBoxText.TabStop = false;
            // 
            // textBoxStatusInformation
            // 
            this.textBoxStatusInformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStatusInformation.Location = new System.Drawing.Point(12, 530);
            this.textBoxStatusInformation.Name = "textBoxStatusInformation";
            this.textBoxStatusInformation.Size = new System.Drawing.Size(1122, 20);
            this.textBoxStatusInformation.TabIndex = 0;
            this.textBoxStatusInformation.TabStop = false;
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Location = new System.Drawing.Point(12, 411);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(1122, 20);
            this.textBox3.TabIndex = 8;
            this.textBox3.TabStop = false;
            // 
            // exportAsSeparateScoresToolStripMenuItem
            // 
            this.exportAsSeparateScoresToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.octoBraillebrlToolStripMenuItem,
            this.aSCIIToolStripMenuItem1,
            this.uNICODEToolStripMenuItem1,
            this.pEFToolStripMenuItem1});
            this.exportAsSeparateScoresToolStripMenuItem.Name = "exportAsSeparateScoresToolStripMenuItem";
            this.exportAsSeparateScoresToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.exportAsSeparateScoresToolStripMenuItem.Text = "Export as separate scores";
            // 
            // octoBraillebrlToolStripMenuItem
            // 
            this.octoBraillebrlToolStripMenuItem.Name = "octoBraillebrlToolStripMenuItem";
            this.octoBraillebrlToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.octoBraillebrlToolStripMenuItem.Text = "OctoBraille (.brl)";
            this.octoBraillebrlToolStripMenuItem.Click += new System.EventHandler(this.octoBraillebrlToolStripMenuItem_Click);
            // 
            // aSCIIToolStripMenuItem1
            // 
            this.aSCIIToolStripMenuItem1.Name = "aSCIIToolStripMenuItem1";
            this.aSCIIToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.aSCIIToolStripMenuItem1.Text = "ASCII (.brf)";
            this.aSCIIToolStripMenuItem1.Click += new System.EventHandler(this.aSCIIToolStripMenuItem1_Click);
            // 
            // uNICODEToolStripMenuItem1
            // 
            this.uNICODEToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.utf8ToolStripMenuItem,
            this.utf16ToolStripMenuItem,
            this.utf32ToolStripMenuItem1});
            this.uNICODEToolStripMenuItem1.Name = "uNICODEToolStripMenuItem1";
            this.uNICODEToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.uNICODEToolStripMenuItem1.Text = "UNICODE";
            // 
            // pEFToolStripMenuItem1
            // 
            this.pEFToolStripMenuItem1.Name = "pEFToolStripMenuItem1";
            this.pEFToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.pEFToolStripMenuItem1.Text = "PEF";
            this.pEFToolStripMenuItem1.Click += new System.EventHandler(this.pEFToolStripMenuItem1_Click);
            // 
            // utf8ToolStripMenuItem
            // 
            this.utf8ToolStripMenuItem.Name = "utf8ToolStripMenuItem";
            this.utf8ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.utf8ToolStripMenuItem.Text = "Utf-8 (.txt)";
            this.utf8ToolStripMenuItem.Click += new System.EventHandler(this.utf8ToolStripMenuItem_Click);
            // 
            // utf16ToolStripMenuItem
            // 
            this.utf16ToolStripMenuItem.Name = "utf16ToolStripMenuItem";
            this.utf16ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.utf16ToolStripMenuItem.Text = "Utf-16 (.txt)";
            this.utf16ToolStripMenuItem.Click += new System.EventHandler(this.utf16ToolStripMenuItem_Click);
            // 
            // utf32ToolStripMenuItem1
            // 
            this.utf32ToolStripMenuItem1.Name = "utf32ToolStripMenuItem1";
            this.utf32ToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.utf32ToolStripMenuItem1.Text = "Utf-32 (.txt)";
            this.utf32ToolStripMenuItem1.Click += new System.EventHandler(this.utf32ToolStripMenuItem1_Click);
            // 
            // MusicBrailleReaderMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1146, 557);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBoxStatusInformation);
            this.Controls.Add(this.textBoxText);
            this.Controls.Add(this.userSettingsTreeView);
            this.Controls.Add(this.textBoxBraille);
            this.Controls.Add(this.listBoxOffsets);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimizeBox = false;
            this.Name = "MusicBrailleReaderMainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MusicBrailleReaderMainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ListBox listBoxOffsets;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem museScoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logfileLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iBOSMusicXmlReaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem regressionTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTestFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem regressionReferenceLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyRenameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem transscribeHøjskolesangbogenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openUsingNOTAProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openUsingBrailleOrchProfileToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxBraille;
        private System.Windows.Forms.TreeView userSettingsTreeView;
        private System.Windows.Forms.ToolStripMenuItem exporterSomMusicXmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exporterSomTextToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxText;
        private System.Windows.Forms.TextBox textBoxStatusInformation;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ToolStripMenuItem jAWSSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jAWSSettingsDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jAWSSettingsUpdateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem octoBraille1252ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aSCIIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uNICODEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pEFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uft8ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uft16ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utf32ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAsSeparateScoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem octoBraillebrlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aSCIIToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uNICODEToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem utf8ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utf16ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utf32ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pEFToolStripMenuItem1;
    }
}

