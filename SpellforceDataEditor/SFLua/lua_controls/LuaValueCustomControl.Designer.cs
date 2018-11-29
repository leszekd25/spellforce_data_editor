namespace SpellforceDataEditor.SFLua.lua_controls
{
    partial class LuaValueCustomControl
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
            this.luaParams = new SpellforceDataEditor.SFLua.lua_controls.LuaValueComplexControl();
            this.ButtonCustomCode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // luaParams
            // 
            this.luaParams.AllowRemove = false;
            this.luaParams.ID = -1;
            this.luaParams.Location = new System.Drawing.Point(40, 24);
            this.luaParams.Margin = new System.Windows.Forms.Padding(0);
            this.luaParams.Name = "luaParams";
            this.luaParams.ParentControl = null;
            this.luaParams.Size = new System.Drawing.Size(240, 60);
            this.luaParams.TabIndex = 1;
            // 
            // ButtonCustomCode
            // 
            this.ButtonCustomCode.Location = new System.Drawing.Point(191, 2);
            this.ButtonCustomCode.Name = "ButtonCustomCode";
            this.ButtonCustomCode.Size = new System.Drawing.Size(86, 23);
            this.ButtonCustomCode.TabIndex = 2;
            this.ButtonCustomCode.Text = "Custom code...";
            this.ButtonCustomCode.UseVisualStyleBackColor = true;
            this.ButtonCustomCode.Click += new System.EventHandler(this.ButtonCustomCode_Click);
            // 
            // LuaValueCustomControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.ButtonCustomCode);
            this.Controls.Add(this.luaParams);
            this.Name = "LuaValueCustomControl";
            this.Size = new System.Drawing.Size(280, 84);
            this.Controls.SetChildIndex(this.luaParams, 0);
            this.Controls.SetChildIndex(this.ButtonCustomCode, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LuaValueComplexControl luaParams;
        private System.Windows.Forms.Button ButtonCustomCode;
    }
}
