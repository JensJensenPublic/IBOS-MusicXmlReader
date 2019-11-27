namespace MusicXmlReader
{
    partial class GeneralSettingsForms
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
            this.labelMusicXmlFile = new System.Windows.Forms.Label();
            this.labelBrailleMusicPath = new System.Windows.Forms.Label();
            this.textBoxMusicXmlFile = new System.Windows.Forms.TextBox();
            this.textBoxBrailleMusicPath = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelMusicXmlFile
            // 
            this.labelMusicXmlFile.AutoSize = true;
            this.labelMusicXmlFile.Location = new System.Drawing.Point(30, 36);
            this.labelMusicXmlFile.Name = "labelMusicXmlFile";
            this.labelMusicXmlFile.Size = new System.Drawing.Size(68, 13);
            this.labelMusicXmlFile.TabIndex = 0;
            this.labelMusicXmlFile.Text = "MusicXml file";
            // 
            // labelBrailleMusicPath
            // 
            this.labelBrailleMusicPath.AutoSize = true;
            this.labelBrailleMusicPath.Location = new System.Drawing.Point(30, 77);
            this.labelBrailleMusicPath.Name = "labelBrailleMusicPath";
            this.labelBrailleMusicPath.Size = new System.Drawing.Size(90, 13);
            this.labelBrailleMusicPath.TabIndex = 1;
            this.labelBrailleMusicPath.Text = "Braille Music path";
            // 
            // textBoxMusicXmlFile
            // 
            this.textBoxMusicXmlFile.Location = new System.Drawing.Point(172, 29);
            this.textBoxMusicXmlFile.Name = "textBoxMusicXmlFile";
            this.textBoxMusicXmlFile.Size = new System.Drawing.Size(643, 20);
            this.textBoxMusicXmlFile.TabIndex = 2;
            // 
            // textBoxBrailleMusicPath
            // 
            this.textBoxBrailleMusicPath.Location = new System.Drawing.Point(172, 74);
            this.textBoxBrailleMusicPath.Name = "textBoxBrailleMusicPath";
            this.textBoxBrailleMusicPath.Size = new System.Drawing.Size(643, 20);
            this.textBoxBrailleMusicPath.TabIndex = 3;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(740, 226);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(605, 226);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // GeneralSettingsForms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 261);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxBrailleMusicPath);
            this.Controls.Add(this.textBoxMusicXmlFile);
            this.Controls.Add(this.labelBrailleMusicPath);
            this.Controls.Add(this.labelMusicXmlFile);
            this.Name = "GeneralSettingsForms";
            this.Text = "GeneralSettingsForms";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMusicXmlFile;
        private System.Windows.Forms.Label labelBrailleMusicPath;
        private System.Windows.Forms.TextBox textBoxMusicXmlFile;
        private System.Windows.Forms.TextBox textBoxBrailleMusicPath;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}