namespace SpellforceDataEditor.SFLua.lua_controls
{
    partial class LuaValueIntControl
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
            this.TextBoxValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TextBoxValue
            // 
            this.TextBoxValue.Location = new System.Drawing.Point(121, 2);
            this.TextBoxValue.Name = "TextBoxValue";
            this.TextBoxValue.Size = new System.Drawing.Size(100, 20);
            this.TextBoxValue.TabIndex = 1;
            this.TextBoxValue.TextChanged += new System.EventHandler(this.TextBoxValue_TextChanged);
            // 
            // LuaValueIntControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.TextBoxValue);
            this.Name = "LuaValueIntControl";
            this.Controls.SetChildIndex(this.TextBoxValue, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxValue;
    }
}
