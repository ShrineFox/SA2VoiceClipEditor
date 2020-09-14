namespace SA2VoiceClipEditor
{
    partial class SA2VoiceClipEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SA2VoiceClipEditorForm));
            this.btn_ExtractAFS = new System.Windows.Forms.Button();
            this.btn_RepackAFS = new System.Windows.Forms.Button();
            this.btn_RepackCSB = new System.Windows.Forms.Button();
            this.btn_ExtractCSB = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.chkBox_Convert = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btn_ExtractAFS
            // 
            this.btn_ExtractAFS.AllowDrop = true;
            this.btn_ExtractAFS.Location = new System.Drawing.Point(12, 12);
            this.btn_ExtractAFS.Name = "btn_ExtractAFS";
            this.btn_ExtractAFS.Size = new System.Drawing.Size(175, 163);
            this.btn_ExtractAFS.TabIndex = 0;
            this.btn_ExtractAFS.Text = "Extract AFS";
            this.btn_ExtractAFS.UseVisualStyleBackColor = true;
            this.btn_ExtractAFS.DragDrop += new System.Windows.Forms.DragEventHandler(this.ExtractAFS_DragDrop);
            this.btn_ExtractAFS.DragEnter += new System.Windows.Forms.DragEventHandler(this.ExtractAFS_DragEnter);
            // 
            // btn_RepackAFS
            // 
            this.btn_RepackAFS.AllowDrop = true;
            this.btn_RepackAFS.Location = new System.Drawing.Point(12, 181);
            this.btn_RepackAFS.Name = "btn_RepackAFS";
            this.btn_RepackAFS.Size = new System.Drawing.Size(175, 44);
            this.btn_RepackAFS.TabIndex = 1;
            this.btn_RepackAFS.Text = "Repack AFS";
            this.btn_RepackAFS.UseVisualStyleBackColor = true;
            this.btn_RepackAFS.DragDrop += new System.Windows.Forms.DragEventHandler(this.RepackAFS_DragDrop);
            this.btn_RepackAFS.DragEnter += new System.Windows.Forms.DragEventHandler(this.RepackAFS_DragEnter);
            // 
            // btn_RepackCSB
            // 
            this.btn_RepackCSB.AllowDrop = true;
            this.btn_RepackCSB.Location = new System.Drawing.Point(193, 181);
            this.btn_RepackCSB.Name = "btn_RepackCSB";
            this.btn_RepackCSB.Size = new System.Drawing.Size(175, 44);
            this.btn_RepackCSB.TabIndex = 3;
            this.btn_RepackCSB.Text = "Repack CSB";
            this.btn_RepackCSB.UseVisualStyleBackColor = true;
            this.btn_RepackCSB.DragDrop += new System.Windows.Forms.DragEventHandler(this.RepackCSB_DragDrop);
            this.btn_RepackCSB.DragEnter += new System.Windows.Forms.DragEventHandler(this.RepackCSB_DragEnter);
            // 
            // btn_ExtractCSB
            // 
            this.btn_ExtractCSB.AllowDrop = true;
            this.btn_ExtractCSB.Location = new System.Drawing.Point(193, 12);
            this.btn_ExtractCSB.Name = "btn_ExtractCSB";
            this.btn_ExtractCSB.Size = new System.Drawing.Size(175, 163);
            this.btn_ExtractCSB.TabIndex = 2;
            this.btn_ExtractCSB.Text = "Extract CSB";
            this.btn_ExtractCSB.UseVisualStyleBackColor = true;
            this.btn_ExtractCSB.DragDrop += new System.Windows.Forms.DragEventHandler(this.ExtractCSB_DragDrop);
            this.btn_ExtractCSB.DragEnter += new System.Windows.Forms.DragEventHandler(this.ExtractCSB_DragEnter);
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Location = new System.Drawing.Point(195, 240);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(0, 17);
            this.lbl_Status.TabIndex = 4;
            // 
            // chkBox_Convert
            // 
            this.chkBox_Convert.AutoSize = true;
            this.chkBox_Convert.Location = new System.Drawing.Point(12, 230);
            this.chkBox_Convert.Name = "chkBox_Convert";
            this.chkBox_Convert.Size = new System.Drawing.Size(137, 38);
            this.chkBox_Convert.TabIndex = 5;
            this.chkBox_Convert.Text = "Convert to WAV\r\n(can take awhile)";
            this.chkBox_Convert.UseVisualStyleBackColor = true;
            // 
            // SA2VoiceClipEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 268);
            this.Controls.Add(this.chkBox_Convert);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.btn_RepackCSB);
            this.Controls.Add(this.btn_ExtractCSB);
            this.Controls.Add(this.btn_RepackAFS);
            this.Controls.Add(this.btn_ExtractAFS);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SA2VoiceClipEditorForm";
            this.Text = "SA2 Voice Clip Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_ExtractAFS;
        private System.Windows.Forms.Button btn_RepackAFS;
        private System.Windows.Forms.Button btn_RepackCSB;
        private System.Windows.Forms.Button btn_ExtractCSB;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.CheckBox chkBox_Convert;
    }
}

