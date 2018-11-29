namespace SpellforceDataEditor.SFLua.lua_controls
{
    partial class LuaValueColorControl
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
            this.ColorDialog = new System.Windows.Forms.ColorDialog();
            this.ButtonColorPick = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonColorPick
            // 
            this.ButtonColorPick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(51)))), ((int)(((byte)(0)))));
            this.ButtonColorPick.Location = new System.Drawing.Point(146, 1);
            this.ButtonColorPick.Name = "ButtonColorPick";
            this.ButtonColorPick.Size = new System.Drawing.Size(75, 23);
            this.ButtonColorPick.TabIndex = 1;
            this.ButtonColorPick.UseVisualStyleBackColor = false;
            this.ButtonColorPick.Click += new System.EventHandler(this.ButtonColorPick_Click);
            // 
            // LuaValueColorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.ButtonColorPick);
            this.Name = "LuaValueColorControl";
            this.Controls.SetChildIndex(this.ButtonColorPick, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog ColorDialog;
        private System.Windows.Forms.Button ButtonColorPick;
    }
}
