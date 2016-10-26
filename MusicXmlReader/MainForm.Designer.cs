namespace MusicXmlReader
{
    partial class MainForm
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
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMusicXmlFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.museScoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sibeliusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openXMLFileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLogFileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inspectAsXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAsInterpretedXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userSettingsTreeView = new System.Windows.Forms.TreeView();
            this.listBoxTimes = new System.Windows.Forms.ListBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.textBoxBraille = new System.Windows.Forms.TextBox();
            this.textBoxText = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.textBoxNormalText = new System.Windows.Forms.TextBox();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(1219, 24);
            this.MenuStrip.TabIndex = 0;
            this.MenuStrip.TabStop = true;
            this.MenuStrip.Text = "menuStrip1";
            // 
            // filesToolStripMenuItem
            // 
            this.filesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMusicXmlFileToolStripMenuItem});
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            this.filesToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.filesToolStripMenuItem.Text = "&Files";
            // 
            // openMusicXmlFileToolStripMenuItem
            // 
            this.openMusicXmlFileToolStripMenuItem.Name = "openMusicXmlFileToolStripMenuItem";
            this.openMusicXmlFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openMusicXmlFileToolStripMenuItem.Text = "&Open MusicXml File";
            this.openMusicXmlFileToolStripMenuItem.Click += new System.EventHandler(this.openMusicXmlFileToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.museScoreToolStripMenuItem,
            this.sibeliusToolStripMenuItem,
            this.logfileToolStripMenuItem,
            this.openXMLFileLocationToolStripMenuItem,
            this.openLogFileLocationToolStripMenuItem,
            this.inspectAsXMLToolStripMenuItem,
            this.viewAsInterpretedXMLToolStripMenuItem,
            this.saveAsTextToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // museScoreToolStripMenuItem
            // 
            this.museScoreToolStripMenuItem.Name = "museScoreToolStripMenuItem";
            this.museScoreToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.museScoreToolStripMenuItem.Text = "Start MuseScore";
            this.museScoreToolStripMenuItem.Click += new System.EventHandler(this.museScoreToolStripMenuItem_Click);
            // 
            // sibeliusToolStripMenuItem
            // 
            this.sibeliusToolStripMenuItem.Name = "sibeliusToolStripMenuItem";
            this.sibeliusToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.sibeliusToolStripMenuItem.Text = "Start Sibelius";
            this.sibeliusToolStripMenuItem.Click += new System.EventHandler(this.sibeliusToolStripMenuItem_Click);
            // 
            // logfileToolStripMenuItem
            // 
            this.logfileToolStripMenuItem.Name = "logfileToolStripMenuItem";
            this.logfileToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.logfileToolStripMenuItem.Text = "Explore log file";
            this.logfileToolStripMenuItem.Click += new System.EventHandler(this.logfileToolStripMenuItem_Click);
            // 
            // openXMLFileLocationToolStripMenuItem
            // 
            this.openXMLFileLocationToolStripMenuItem.Name = "openXMLFileLocationToolStripMenuItem";
            this.openXMLFileLocationToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.openXMLFileLocationToolStripMenuItem.Text = "Open XML file location";
            this.openXMLFileLocationToolStripMenuItem.Click += new System.EventHandler(this.openXMLFileLocationToolStripMenuItem_Click);
            // 
            // openLogFileLocationToolStripMenuItem
            // 
            this.openLogFileLocationToolStripMenuItem.Name = "openLogFileLocationToolStripMenuItem";
            this.openLogFileLocationToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.openLogFileLocationToolStripMenuItem.Text = "Open log file location";
            this.openLogFileLocationToolStripMenuItem.Click += new System.EventHandler(this.openLogFileLocationToolStripMenuItem_Click);
            // 
            // inspectAsXMLToolStripMenuItem
            // 
            this.inspectAsXMLToolStripMenuItem.Name = "inspectAsXMLToolStripMenuItem";
            this.inspectAsXMLToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.inspectAsXMLToolStripMenuItem.Text = "Explore as raw XML";
            this.inspectAsXMLToolStripMenuItem.Click += new System.EventHandler(this.inspectAsXMLToolStripMenuItem_Click);
            // 
            // viewAsInterpretedXMLToolStripMenuItem
            // 
            this.viewAsInterpretedXMLToolStripMenuItem.Name = "viewAsInterpretedXMLToolStripMenuItem";
            this.viewAsInterpretedXMLToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.viewAsInterpretedXMLToolStripMenuItem.Text = "Explore as interpreted XML";
            this.viewAsInterpretedXMLToolStripMenuItem.Click += new System.EventHandler(this.viewAsInterpretedXMLToolStripMenuItem_Click);
            // 
            // saveAsTextToolStripMenuItem
            // 
            this.saveAsTextToolStripMenuItem.Name = "saveAsTextToolStripMenuItem";
            this.saveAsTextToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.saveAsTextToolStripMenuItem.Text = "Save as text";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // userSettingsTreeView
            // 
            this.userSettingsTreeView.CheckBoxes = true;
            this.userSettingsTreeView.Location = new System.Drawing.Point(10, 60);
            this.userSettingsTreeView.Name = "userSettingsTreeView";
            this.userSettingsTreeView.Size = new System.Drawing.Size(200, 420);
            this.userSettingsTreeView.TabIndex = 1;
            // 
            // listBoxTimes
            // 
            this.listBoxTimes.FormattingEnabled = true;
            this.listBoxTimes.Location = new System.Drawing.Point(220, 60);
            this.listBoxTimes.Name = "listBoxTimes";
            this.listBoxTimes.Size = new System.Drawing.Size(993, 420);
            this.listBoxTimes.TabIndex = 3;
            this.listBoxTimes.SelectedIndexChanged += new System.EventHandler(this.listBoxTimes_SelectedIndexChanged);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(13, 27);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "ButtonStart";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(135, 27);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 4;
            this.buttonStop.Text = "ButtonStop";
            this.buttonStop.UseVisualStyleBackColor = true;
            // 
            // textBoxBraille
            // 
            this.textBoxBraille.BackColor = System.Drawing.Color.Black;
            this.textBoxBraille.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxBraille.ForeColor = System.Drawing.Color.White;
            this.textBoxBraille.Location = new System.Drawing.Point(10, 511);
            this.textBoxBraille.Name = "textBoxBraille";
            this.textBoxBraille.Size = new System.Drawing.Size(1200, 62);
            this.textBoxBraille.TabIndex = 5;
            this.textBoxBraille.TabStop = false;
            // 
            // textBoxText
            // 
            this.textBoxText.Location = new System.Drawing.Point(10, 579);
            this.textBoxText.Name = "textBoxText";
            this.textBoxText.Size = new System.Drawing.Size(1200, 20);
            this.textBoxText.TabIndex = 6;
            this.textBoxText.TabStop = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(220, 28);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(993, 20);
            this.textBoxMessage.TabIndex = 7;
            this.textBoxMessage.TabStop = false;
            // 
            // textBoxNormalText
            // 
            this.textBoxNormalText.Location = new System.Drawing.Point(10, 485);
            this.textBoxNormalText.Name = "textBoxNormalText";
            this.textBoxNormalText.Size = new System.Drawing.Size(1203, 20);
            this.textBoxNormalText.TabIndex = 8;
            this.textBoxNormalText.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1219, 601);
            this.Controls.Add(this.textBoxNormalText);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.textBoxText);
            this.Controls.Add(this.textBoxBraille);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.listBoxTimes);
            this.Controls.Add(this.userSettingsTreeView);
            this.Controls.Add(this.MenuStrip);
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.TreeView userSettingsTreeView;
        private System.Windows.Forms.ListBox listBoxTimes;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.TextBox textBoxBraille;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxText;
        private System.Windows.Forms.ToolStripMenuItem openMusicXmlFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.ToolStripMenuItem museScoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sibeliusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openXMLFileLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogFileLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inspectAsXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewAsInterpretedXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsTextToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxNormalText;
    }
}

