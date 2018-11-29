namespace SpellforceDataEditor.special_forms.utility_forms
{
    partial class ShowCodeForm
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
            this.TextBoxCode = new System.Windows.Forms.TextBox();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TextBoxCode
            // 
            this.TextBoxCode.AcceptsReturn = true;
            this.TextBoxCode.AcceptsTab = true;
            this.TextBoxCode.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.TextBoxCode.Location = new System.Drawing.Point(12, 12);
            this.TextBoxCode.Multiline = true;
            this.TextBoxCode.Name = "TextBoxCode";
            this.TextBoxCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBoxCode.Size = new System.Drawing.Size(485, 433);
            this.TextBoxCode.TabIndex = 0;
            this.TextBoxCode.WordWrap = false;
            // 
            // ButtonOk
            // 
            this.ButtonOk.Location = new System.Drawing.Point(422, 451);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(75, 23);
            this.ButtonOk.TabIndex = 2;
            this.ButtonOk.Text = "OK";
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // ShowCodeForm
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(509, 486);
            this.ControlBox = false;
            this.Controls.Add(this.ButtonOk);
            this.Controls.Add(this.TextBoxCode);
            this.Name = "ShowCodeForm";
            this.Text = "Enter Lua code";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxCode;
        private System.Windows.Forms.Button ButtonOk;
    }
}