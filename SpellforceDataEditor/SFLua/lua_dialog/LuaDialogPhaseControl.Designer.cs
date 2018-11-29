namespace SpellforceDataEditor.SFLua.lua_dialog
{
    partial class LuaDialogPhaseControl
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
            this.ButtonAddSay = new System.Windows.Forms.Button();
            this.PanelControls = new System.Windows.Forms.Panel();
            this.LabelPhase = new System.Windows.Forms.Label();
            this.TextBoxPhase = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ButtonAddSay
            // 
            this.ButtonAddSay.Location = new System.Drawing.Point(272, 3);
            this.ButtonAddSay.Name = "ButtonAddSay";
            this.ButtonAddSay.Size = new System.Drawing.Size(75, 23);
            this.ButtonAddSay.TabIndex = 2;
            this.ButtonAddSay.Text = "New Say";
            this.ButtonAddSay.UseVisualStyleBackColor = true;
            this.ButtonAddSay.Click += new System.EventHandler(this.ButtonAddSay_Click);
            // 
            // PanelControls
            // 
            this.PanelControls.AutoScroll = true;
            this.PanelControls.BackColor = System.Drawing.SystemColors.Window;
            this.PanelControls.Location = new System.Drawing.Point(4, 54);
            this.PanelControls.Name = "PanelControls";
            this.PanelControls.Size = new System.Drawing.Size(343, 273);
            this.PanelControls.TabIndex = 3;
            // 
            // LabelPhase
            // 
            this.LabelPhase.AutoSize = true;
            this.LabelPhase.Location = new System.Drawing.Point(3, 32);
            this.LabelPhase.Name = "LabelPhase";
            this.LabelPhase.Size = new System.Drawing.Size(51, 13);
            this.LabelPhase.TabIndex = 4;
            this.LabelPhase.Text = "Phase ID";
            // 
            // TextBoxPhase
            // 
            this.TextBoxPhase.Location = new System.Drawing.Point(272, 29);
            this.TextBoxPhase.Name = "TextBoxPhase";
            this.TextBoxPhase.Size = new System.Drawing.Size(75, 20);
            this.TextBoxPhase.TabIndex = 5;
            // 
            // LuaDialogPhaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.PeachPuff;
            this.Controls.Add(this.TextBoxPhase);
            this.Controls.Add(this.LabelPhase);
            this.Controls.Add(this.PanelControls);
            this.Controls.Add(this.ButtonAddSay);
            this.Name = "LuaDialogPhaseControl";
            this.Size = new System.Drawing.Size(350, 330);
            this.Controls.SetChildIndex(this.ButtonAddSay, 0);
            this.Controls.SetChildIndex(this.PanelControls, 0);
            this.Controls.SetChildIndex(this.LabelPhase, 0);
            this.Controls.SetChildIndex(this.TextBoxPhase, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonAddSay;
        private System.Windows.Forms.Panel PanelControls;
        private System.Windows.Forms.Label LabelPhase;
        private System.Windows.Forms.TextBox TextBoxPhase;
    }
}
