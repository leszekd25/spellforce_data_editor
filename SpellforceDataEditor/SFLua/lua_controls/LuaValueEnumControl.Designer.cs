namespace SpellforceDataEditor.SFLua.lua_controls
{
    partial class LuaValueEnumControl
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
            this.ComboValue = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // ComboValue
            // 
            this.ComboValue.FormattingEnabled = true;
            this.ComboValue.Location = new System.Drawing.Point(100, 2);
            this.ComboValue.Name = "ComboValue";
            this.ComboValue.Size = new System.Drawing.Size(121, 21);
            this.ComboValue.TabIndex = 1;
            this.ComboValue.SelectedIndexChanged += new System.EventHandler(this.ComboValue_SelectedIndexChanged);
            // 
            // LuaValueEnumControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.ComboValue);
            this.Name = "LuaValueEnumControl";
            this.Controls.SetChildIndex(this.ComboValue, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ComboValue;
    }
}
