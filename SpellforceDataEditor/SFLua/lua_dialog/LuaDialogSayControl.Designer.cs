namespace SpellforceDataEditor.SFLua.lua_dialog
{
    partial class LuaDialogSayControl
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
            this.ButtonAnswer = new System.Windows.Forms.Button();
            this.StringText = new SpellforceDataEditor.SFLua.lua_controls.LuaValueStringControl();
            this.StringTag = new SpellforceDataEditor.SFLua.lua_controls.LuaValueStringControl();
            this.ColorColor = new SpellforceDataEditor.SFLua.lua_controls.LuaValueColorControl();
            this.PanelControls = new System.Windows.Forms.Panel();
            this.Conditions = new SpellforceDataEditor.SFLua.lua_controls.LuaValueComplexControl();
            this.Actions = new SpellforceDataEditor.SFLua.lua_controls.LuaValueComplexControl();
            this.LabelChooseAnswer = new System.Windows.Forms.Label();
            this.CheckBoxChooseAnswer = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ButtonAnswer
            // 
            this.ButtonAnswer.Location = new System.Drawing.Point(122, 2);
            this.ButtonAnswer.Name = "ButtonAnswer";
            this.ButtonAnswer.Size = new System.Drawing.Size(75, 23);
            this.ButtonAnswer.TabIndex = 1;
            this.ButtonAnswer.Text = "Add Answer";
            this.ButtonAnswer.UseVisualStyleBackColor = true;
            this.ButtonAnswer.Click += new System.EventHandler(this.ButtonAnswer_Click);
            // 
            // StringText
            // 
            this.StringText.AllowRemove = false;
            this.StringText.Default = null;
            this.StringText.ID = -1;
            this.StringText.Important = false;
            this.StringText.Location = new System.Drawing.Point(0, 26);
            this.StringText.Margin = new System.Windows.Forms.Padding(0);
            this.StringText.Name = "StringText";
            this.StringText.ParentControl = null;
            this.StringText.PSChar = "";
            this.StringText.Size = new System.Drawing.Size(200, 24);
            this.StringText.TabIndex = 2;
            this.StringText.Value = null;
            // 
            // StringTag
            // 
            this.StringTag.AllowRemove = false;
            this.StringTag.Default = null;
            this.StringTag.ID = -1;
            this.StringTag.Important = false;
            this.StringTag.Location = new System.Drawing.Point(0, 50);
            this.StringTag.Margin = new System.Windows.Forms.Padding(0);
            this.StringTag.Name = "StringTag";
            this.StringTag.ParentControl = null;
            this.StringTag.PSChar = "";
            this.StringTag.Size = new System.Drawing.Size(200, 24);
            this.StringTag.TabIndex = 3;
            this.StringTag.Value = null;
            // 
            // ColorColor
            // 
            this.ColorColor.AllowRemove = false;
            this.ColorColor.Default = null;
            this.ColorColor.ID = -1;
            this.ColorColor.Important = false;
            this.ColorColor.Location = new System.Drawing.Point(0, 74);
            this.ColorColor.Margin = new System.Windows.Forms.Padding(0);
            this.ColorColor.Name = "ColorColor";
            this.ColorColor.ParentControl = null;
            this.ColorColor.PSChar = "";
            this.ColorColor.Size = new System.Drawing.Size(200, 24);
            this.ColorColor.TabIndex = 4;
            this.ColorColor.Value = null;
            // 
            // PanelControls
            // 
            this.PanelControls.AutoScroll = true;
            this.PanelControls.BackColor = System.Drawing.SystemColors.Window;
            this.PanelControls.Location = new System.Drawing.Point(3, 241);
            this.PanelControls.Name = "PanelControls";
            this.PanelControls.Size = new System.Drawing.Size(236, 166);
            this.PanelControls.TabIndex = 5;
            // 
            // Conditions
            // 
            this.Conditions.AllowRemove = false;
            this.Conditions.ID = -1;
            this.Conditions.Important = false;
            this.Conditions.Location = new System.Drawing.Point(0, 118);
            this.Conditions.Margin = new System.Windows.Forms.Padding(0);
            this.Conditions.Name = "Conditions";
            this.Conditions.ParentControl = null;
            this.Conditions.PSChar = "";
            this.Conditions.Size = new System.Drawing.Size(240, 60);
            this.Conditions.TabIndex = 6;
            // 
            // Actions
            // 
            this.Actions.AllowRemove = false;
            this.Actions.ID = -1;
            this.Actions.Important = false;
            this.Actions.Location = new System.Drawing.Point(-1, 178);
            this.Actions.Margin = new System.Windows.Forms.Padding(0);
            this.Actions.Name = "Actions";
            this.Actions.ParentControl = null;
            this.Actions.PSChar = "";
            this.Actions.Size = new System.Drawing.Size(240, 60);
            this.Actions.TabIndex = 7;
            // 
            // LabelChooseAnswer
            // 
            this.LabelChooseAnswer.AutoSize = true;
            this.LabelChooseAnswer.Location = new System.Drawing.Point(3, 102);
            this.LabelChooseAnswer.Name = "LabelChooseAnswer";
            this.LabelChooseAnswer.Size = new System.Drawing.Size(115, 13);
            this.LabelChooseAnswer.TabIndex = 8;
            this.LabelChooseAnswer.Text = "Allow choosing answer";
            // 
            // CheckBoxChooseAnswer
            // 
            this.CheckBoxChooseAnswer.AutoSize = true;
            this.CheckBoxChooseAnswer.Location = new System.Drawing.Point(182, 101);
            this.CheckBoxChooseAnswer.Name = "CheckBoxChooseAnswer";
            this.CheckBoxChooseAnswer.Size = new System.Drawing.Size(15, 14);
            this.CheckBoxChooseAnswer.TabIndex = 9;
            this.CheckBoxChooseAnswer.UseVisualStyleBackColor = true;
            // 
            // LuaDialogSayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.Khaki;
            this.Controls.Add(this.CheckBoxChooseAnswer);
            this.Controls.Add(this.LabelChooseAnswer);
            this.Controls.Add(this.Actions);
            this.Controls.Add(this.Conditions);
            this.Controls.Add(this.PanelControls);
            this.Controls.Add(this.ColorColor);
            this.Controls.Add(this.StringTag);
            this.Controls.Add(this.StringText);
            this.Controls.Add(this.ButtonAnswer);
            this.Name = "LuaDialogSayControl";
            this.Size = new System.Drawing.Size(242, 411);
            this.Controls.SetChildIndex(this.ButtonAnswer, 0);
            this.Controls.SetChildIndex(this.StringText, 0);
            this.Controls.SetChildIndex(this.StringTag, 0);
            this.Controls.SetChildIndex(this.ColorColor, 0);
            this.Controls.SetChildIndex(this.PanelControls, 0);
            this.Controls.SetChildIndex(this.Conditions, 0);
            this.Controls.SetChildIndex(this.Actions, 0);
            this.Controls.SetChildIndex(this.LabelChooseAnswer, 0);
            this.Controls.SetChildIndex(this.CheckBoxChooseAnswer, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonAnswer;
        private lua_controls.LuaValueStringControl StringText;
        private lua_controls.LuaValueStringControl StringTag;
        private lua_controls.LuaValueColorControl ColorColor;
        private System.Windows.Forms.Panel PanelControls;
        private lua_controls.LuaValueComplexControl Conditions;
        private lua_controls.LuaValueComplexControl Actions;
        private System.Windows.Forms.Label LabelChooseAnswer;
        private System.Windows.Forms.CheckBox CheckBoxChooseAnswer;
    }
}
