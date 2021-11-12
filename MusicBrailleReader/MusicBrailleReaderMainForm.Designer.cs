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
            this.openUsingBrailleOrchProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTestFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tactileMusicXmlReaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iBOSMusicXmlReaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.museScoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logfileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regressionTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regressionReferenceLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyRenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transscribeHøjskolesangbogenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.listBoxOffsets = new System.Windows.Forms.ListBox();
            this.textBoxRawBraille6 = new System.Windows.Forms.TextBox();
            this.textBoxEditResultAsDecodedText = new System.Windows.Forms.TextBox();
            this.textBoxDebugInfo = new System.Windows.Forms.TextBox();
            this.userSettingsTreeView = new System.Windows.Forms.TreeView();
            this.exporterSomMusicXmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.exporterSomMusicXmlToolStripMenuItem});
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            this.filesToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.filesToolStripMenuItem.Text = "Files";
            // 
            // openUsingNOTAProfileToolStripMenuItem
            // 
            this.openUsingNOTAProfileToolStripMenuItem.Name = "openUsingNOTAProfileToolStripMenuItem";
            this.openUsingNOTAProfileToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openUsingNOTAProfileToolStripMenuItem.Text = "Open using NOTA profile ";
            this.openUsingNOTAProfileToolStripMenuItem.Click += new System.EventHandler(this.openUsingNOTAProfileToolStripMenuItem_Click);
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
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tactileMusicXmlReaderToolStripMenuItem,
            this.iBOSMusicXmlReaderToolStripMenuItem,
            this.museScoreToolStripMenuItem,
            this.logFileToolStripMenuItem,
            this.logfileLocationToolStripMenuItem,
            this.regressionTestToolStripMenuItem,
            this.regressionReferenceLocationToolStripMenuItem,
            this.copyRenameToolStripMenuItem,
            this.transscribeHøjskolesangbogenToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // tactileMusicXmlReaderToolStripMenuItem
            // 
            this.tactileMusicXmlReaderToolStripMenuItem.Name = "tactileMusicXmlReaderToolStripMenuItem";
            this.tactileMusicXmlReaderToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.tactileMusicXmlReaderToolStripMenuItem.Text = "Tactile MusicXmlReader";
            this.tactileMusicXmlReaderToolStripMenuItem.Click += new System.EventHandler(this.tactileMusicXmlReaderToolStripMenuItem_Click);
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
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // listBoxOffsets
            // 
            this.listBoxOffsets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxOffsets.FormattingEnabled = true;
            this.listBoxOffsets.Location = new System.Drawing.Point(258, 128);
            this.listBoxOffsets.Name = "listBoxOffsets";
            this.listBoxOffsets.Size = new System.Drawing.Size(876, 329);
            this.listBoxOffsets.TabIndex = 1;
            this.listBoxOffsets.SelectedIndexChanged += new System.EventHandler(this.listBoxDecodedAsText_SelectedIndexChanged);
            // 
            // textBoxRawBraille6
            // 
            this.textBoxRawBraille6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRawBraille6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxRawBraille6.Location = new System.Drawing.Point(258, 91);
            this.textBoxRawBraille6.Name = "textBoxRawBraille6";
            this.textBoxRawBraille6.Size = new System.Drawing.Size(876, 31);
            this.textBoxRawBraille6.TabIndex = 2;
            this.textBoxRawBraille6.TextChanged += new System.EventHandler(this.textBoxRawBraille6_TextChanged);
            this.textBoxRawBraille6.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxRawBraille6_KeyDown);
            this.textBoxRawBraille6.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxRawBraille6_KeyPress);
            this.textBoxRawBraille6.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxRawBraille6_KeyUp);
            // 
            // textBoxEditResultAsDecodedText
            // 
            this.textBoxEditResultAsDecodedText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEditResultAsDecodedText.Location = new System.Drawing.Point(258, 65);
            this.textBoxEditResultAsDecodedText.Name = "textBoxEditResultAsDecodedText";
            this.textBoxEditResultAsDecodedText.Size = new System.Drawing.Size(876, 20);
            this.textBoxEditResultAsDecodedText.TabIndex = 3;
            // 
            // textBoxDebugInfo
            // 
            this.textBoxDebugInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDebugInfo.Location = new System.Drawing.Point(258, 39);
            this.textBoxDebugInfo.Name = "textBoxDebugInfo";
            this.textBoxDebugInfo.Size = new System.Drawing.Size(876, 20);
            this.textBoxDebugInfo.TabIndex = 4;
            // 
            // userSettingsTreeView
            // 
            this.userSettingsTreeView.AccessibleName = "Note Filter";
            this.userSettingsTreeView.CheckBoxes = true;
            this.userSettingsTreeView.Location = new System.Drawing.Point(12, 39);
            this.userSettingsTreeView.Name = "userSettingsTreeView";
            this.userSettingsTreeView.Size = new System.Drawing.Size(240, 418);
            this.userSettingsTreeView.TabIndex = 5;
            // 
            // exporterSomMusicXmlToolStripMenuItem
            // 
            this.exporterSomMusicXmlToolStripMenuItem.Name = "exporterSomMusicXmlToolStripMenuItem";
            this.exporterSomMusicXmlToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.exporterSomMusicXmlToolStripMenuItem.Text = "Exporter som MusicXml";
            this.exporterSomMusicXmlToolStripMenuItem.Click += new System.EventHandler(this.exporterSomMusicXmlToolStripMenuItem_Click);
            // 
            // MusicBrailleReaderMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1146, 470);
            this.Controls.Add(this.userSettingsTreeView);
            this.Controls.Add(this.textBoxDebugInfo);
            this.Controls.Add(this.textBoxEditResultAsDecodedText);
            this.Controls.Add(this.textBoxRawBraille6);
            this.Controls.Add(this.listBoxOffsets);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MusicBrailleReaderMainForm";
            this.Text = "Form1";
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
        private System.Windows.Forms.ToolStripMenuItem tactileMusicXmlReaderToolStripMenuItem;
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
        private System.Windows.Forms.TextBox textBoxRawBraille6;
        private System.Windows.Forms.TextBox textBoxEditResultAsDecodedText;
        private System.Windows.Forms.TextBox textBoxDebugInfo;
        private System.Windows.Forms.TreeView userSettingsTreeView;
        private System.Windows.Forms.ToolStripMenuItem exporterSomMusicXmlToolStripMenuItem;
    }
}

