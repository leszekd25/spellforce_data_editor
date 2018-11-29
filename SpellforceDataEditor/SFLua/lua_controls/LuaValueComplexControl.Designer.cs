namespace SpellforceDataEditor.SFLua.lua_controls
{
    partial class LuaValueComplexControl
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
            this.ValuesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // ValuesPanel
            // 
            this.ValuesPanel.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ValuesPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ValuesPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.ValuesPanel.Location = new System.Drawing.Point(37, 26);
            this.ValuesPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ValuesPanel.Name = "ValuesPanel";
            this.ValuesPanel.Size = new System.Drawing.Size(224, 32);
            this.ValuesPanel.TabIndex = 1;
            this.ValuesPanel.WrapContents = false;
            // 
            // LuaValueComplexControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.ValuesPanel);
            this.Name = "LuaValueComplexControl";
            this.Size = new System.Drawing.Size(264, 60);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LuaValueComplexControl_MouseClick);
            this.Controls.SetChildIndex(this.ValuesPanel, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ValuesPanel;
    }
}
