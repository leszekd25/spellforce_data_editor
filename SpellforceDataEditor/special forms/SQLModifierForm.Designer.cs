namespace SpellforceDataEditor.special_forms
{
    partial class SQLModifierForm
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
            this.ButtonRtsCoopSpawnGroups = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButtonRtsCoopSpawnGroups
            // 
            this.ButtonRtsCoopSpawnGroups.Location = new System.Drawing.Point(12, 63);
            this.ButtonRtsCoopSpawnGroups.Name = "ButtonRtsCoopSpawnGroups";
            this.ButtonRtsCoopSpawnGroups.Size = new System.Drawing.Size(189, 23);
            this.ButtonRtsCoopSpawnGroups.TabIndex = 0;
            this.ButtonRtsCoopSpawnGroups.Text = "GdRtsCoopSpawnGroup.lua";
            this.ButtonRtsCoopSpawnGroups.UseVisualStyleBackColor = true;
            this.ButtonRtsCoopSpawnGroups.Click += new System.EventHandler(this.ButtonRtsCoopSpawnGroups_Click);
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 40);
            this.label1.TabIndex = 1;
            this.label1.Text = "Currently there\'s only one Lua file you can modify, but that will grow to include" +
    " all sql_ lua scripts";
            // 
            // SQLModifierForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(213, 92);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButtonRtsCoopSpawnGroups);
            this.Name = "SQLModifierForm";
            this.Text = "SQLModifierForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonRtsCoopSpawnGroups;
        private System.Windows.Forms.Label label1;
    }
}