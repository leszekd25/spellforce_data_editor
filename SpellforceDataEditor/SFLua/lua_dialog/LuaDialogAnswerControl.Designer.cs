namespace SpellforceDataEditor.SFLua.lua_dialog
{
    partial class LuaDialogAnswerControl
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
            this.StringText = new SpellforceDataEditor.SFLua.lua_controls.LuaValueStringControl();
            this.StringTag = new SpellforceDataEditor.SFLua.lua_controls.LuaValueStringControl();
            this.ColorColor = new SpellforceDataEditor.SFLua.lua_controls.LuaValueColorControl();
            this.Conditions = new SpellforceDataEditor.SFLua.lua_controls.LuaValueComplexControl();
            this.IntGotoPhase = new SpellforceDataEditor.SFLua.lua_controls.LuaValueIntControl();
            this.SuspendLayout();
            // 
            // StringText
            // 
            this.StringText.AllowRemove = false;
            this.StringText.Default = null;
            this.StringText.ID = -1;
            this.StringText.Important = false;
            this.StringText.Location = new System.Drawing.Point(0, 24);
            this.StringText.Margin = new System.Windows.Forms.Padding(0);
            this.StringText.Name = "StringText";
            this.StringText.ParentControl = null;
            this.StringText.PSChar = "";
            this.StringText.Size = new System.Drawing.Size(200, 24);
            this.StringText.TabIndex = 1;
            this.StringText.Value = null;
            // 
            // StringTag
            // 
            this.StringTag.AllowRemove = false;
            this.StringTag.Default = null;
            this.StringTag.ID = -1;
            this.StringTag.Important = false;
            this.StringTag.Location = new System.Drawing.Point(0, 48);
            this.StringTag.Margin = new System.Windows.Forms.Padding(0);
            this.StringTag.Name = "StringTag";
            this.StringTag.ParentControl = null;
            this.StringTag.PSChar = "";
            this.StringTag.Size = new System.Drawing.Size(200, 24);
            this.StringTag.TabIndex = 2;
            this.StringTag.Value = null;
            // 
            // ColorColor
            // 
            this.ColorColor.AllowRemove = false;
            this.ColorColor.Default = null;
            this.ColorColor.ID = -1;
            this.ColorColor.Important = false;
            this.ColorColor.Location = new System.Drawing.Point(0, 72);
            this.ColorColor.Margin = new System.Windows.Forms.Padding(0);
            this.ColorColor.Name = "ColorColor";
            this.ColorColor.ParentControl = null;
            this.ColorColor.PSChar = "";
            this.ColorColor.Size = new System.Drawing.Size(200, 24);
            this.ColorColor.TabIndex = 3;
            this.ColorColor.Value = null;
            // 
            // Conditions
            // 
            this.Conditions.AllowRemove = false;
            this.Conditions.ID = -1;
            this.Conditions.Important = false;
            this.Conditions.Location = new System.Drawing.Point(-1, 120);
            this.Conditions.Margin = new System.Windows.Forms.Padding(0);
            this.Conditions.Name = "Conditions";
            this.Conditions.ParentControl = null;
            this.Conditions.PSChar = "";
            this.Conditions.Size = new System.Drawing.Size(240, 60);
            this.Conditions.TabIndex = 4;
            // 
            // IntGotoPhase
            // 
            this.IntGotoPhase.AllowRemove = false;
            this.IntGotoPhase.Default = null;
            this.IntGotoPhase.ID = -1;
            this.IntGotoPhase.Important = false;
            this.IntGotoPhase.Location = new System.Drawing.Point(0, 96);
            this.IntGotoPhase.Margin = new System.Windows.Forms.Padding(0);
            this.IntGotoPhase.Name = "IntGotoPhase";
            this.IntGotoPhase.ParentControl = null;
            this.IntGotoPhase.PSChar = "";
            this.IntGotoPhase.Size = new System.Drawing.Size(200, 24);
            this.IntGotoPhase.TabIndex = 5;
            this.IntGotoPhase.Value = null;
            this.IntGotoPhase.MouseDown += new System.Windows.Forms.MouseEventHandler(this.IntGotoPhase_MouseDown);
            // 
            // LuaDialogAnswerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.PaleGreen;
            this.Controls.Add(this.IntGotoPhase);
            this.Controls.Add(this.Conditions);
            this.Controls.Add(this.ColorColor);
            this.Controls.Add(this.StringTag);
            this.Controls.Add(this.StringText);
            this.Name = "LuaDialogAnswerControl";
            this.Size = new System.Drawing.Size(239, 182);
            this.Controls.SetChildIndex(this.StringText, 0);
            this.Controls.SetChildIndex(this.StringTag, 0);
            this.Controls.SetChildIndex(this.ColorColor, 0);
            this.Controls.SetChildIndex(this.Conditions, 0);
            this.Controls.SetChildIndex(this.IntGotoPhase, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private lua_controls.LuaValueStringControl StringText;
        private lua_controls.LuaValueStringControl StringTag;
        private lua_controls.LuaValueColorControl ColorColor;
        private lua_controls.LuaValueComplexControl Conditions;
        private lua_controls.LuaValueIntControl IntGotoPhase;
    }
}
