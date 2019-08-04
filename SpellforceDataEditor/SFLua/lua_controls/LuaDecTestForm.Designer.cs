namespace SpellforceDataEditor.SFLua.lua_controls
{
    partial class LuaDecTestForm
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
            this.Progress = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.ScriptsFound = new System.Windows.Forms.Label();
            this.ScriptsDecompiled = new System.Windows.Forms.Label();
            this.ScriptsFailed = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Progress
            // 
            this.Progress.Location = new System.Drawing.Point(12, 132);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(212, 25);
            this.Progress.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Scripts found: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Successfully decompiled:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Failed to decompile:";
            // 
            // ButtonOK
            // 
            this.ButtonOK.Enabled = false;
            this.ButtonOK.Location = new System.Drawing.Point(144, 163);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(80, 23);
            this.ButtonOK.TabIndex = 7;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(212, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "START";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ScriptsFound
            // 
            this.ScriptsFound.AutoSize = true;
            this.ScriptsFound.Location = new System.Drawing.Point(189, 47);
            this.ScriptsFound.Name = "ScriptsFound";
            this.ScriptsFound.Size = new System.Drawing.Size(0, 13);
            this.ScriptsFound.TabIndex = 9;
            // 
            // ScriptsDecompiled
            // 
            this.ScriptsDecompiled.AutoSize = true;
            this.ScriptsDecompiled.Location = new System.Drawing.Point(189, 76);
            this.ScriptsDecompiled.Name = "ScriptsDecompiled";
            this.ScriptsDecompiled.Size = new System.Drawing.Size(0, 13);
            this.ScriptsDecompiled.TabIndex = 10;
            // 
            // ScriptsFailed
            // 
            this.ScriptsFailed.AutoSize = true;
            this.ScriptsFailed.Location = new System.Drawing.Point(189, 104);
            this.ScriptsFailed.Name = "ScriptsFailed";
            this.ScriptsFailed.Size = new System.Drawing.Size(0, 13);
            this.ScriptsFailed.TabIndex = 11;
            // 
            // LuaDecTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(236, 197);
            this.Controls.Add(this.ScriptsFailed);
            this.Controls.Add(this.ScriptsDecompiled);
            this.Controls.Add(this.ScriptsFound);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Progress);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LuaDecTestForm";
            this.Text = "Decompiler test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar Progress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label ScriptsFound;
        private System.Windows.Forms.Label ScriptsDecompiled;
        private System.Windows.Forms.Label ScriptsFailed;
    }
}