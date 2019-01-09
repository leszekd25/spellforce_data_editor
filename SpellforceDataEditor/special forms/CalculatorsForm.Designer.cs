namespace SpellforceDataEditor.special_forms
{
    partial class CalculatorsForm
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
            this.ComboCalcMode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CalcPanel = new System.Windows.Forms.Panel();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.ButtonCalculate = new System.Windows.Forms.Button();
            this.TextResult = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ComboCalcMode
            // 
            this.ComboCalcMode.FormattingEnabled = true;
            this.ComboCalcMode.Items.AddRange(new object[] {
            "Hit chance"});
            this.ComboCalcMode.Location = new System.Drawing.Point(101, 12);
            this.ComboCalcMode.Name = "ComboCalcMode";
            this.ComboCalcMode.Size = new System.Drawing.Size(121, 21);
            this.ComboCalcMode.TabIndex = 0;
            this.ComboCalcMode.SelectedIndexChanged += new System.EventHandler(this.ComboCalcMode_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Calculator mode";
            // 
            // CalcPanel
            // 
            this.CalcPanel.Location = new System.Drawing.Point(15, 41);
            this.CalcPanel.Margin = new System.Windows.Forms.Padding(0);
            this.CalcPanel.Name = "CalcPanel";
            this.CalcPanel.Size = new System.Drawing.Size(400, 300);
            this.CalcPanel.TabIndex = 3;
            // 
            // ButtonClose
            // 
            this.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonClose.Location = new System.Drawing.Point(12, 410);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(75, 23);
            this.ButtonClose.TabIndex = 4;
            this.ButtonClose.Text = "Close";
            this.ButtonClose.UseVisualStyleBackColor = true;
            // 
            // ButtonCalculate
            // 
            this.ButtonCalculate.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCalculate.Location = new System.Drawing.Point(340, 410);
            this.ButtonCalculate.Name = "ButtonCalculate";
            this.ButtonCalculate.Size = new System.Drawing.Size(75, 23);
            this.ButtonCalculate.TabIndex = 5;
            this.ButtonCalculate.Text = "Calculate";
            this.ButtonCalculate.UseVisualStyleBackColor = true;
            this.ButtonCalculate.Click += new System.EventHandler(this.ButtonCalculate_Click);
            // 
            // TextResult
            // 
            this.TextResult.Location = new System.Drawing.Point(15, 344);
            this.TextResult.Multiline = true;
            this.TextResult.Name = "TextResult";
            this.TextResult.ReadOnly = true;
            this.TextResult.Size = new System.Drawing.Size(400, 60);
            this.TextResult.TabIndex = 6;
            // 
            // CalculatorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonClose;
            this.ClientSize = new System.Drawing.Size(430, 445);
            this.Controls.Add(this.TextResult);
            this.Controls.Add(this.ButtonCalculate);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.CalcPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ComboCalcMode);
            this.Name = "CalculatorsForm";
            this.Text = "CalculatorsForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CalculatorsForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ComboCalcMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel CalcPanel;
        private System.Windows.Forms.Button ButtonClose;
        private System.Windows.Forms.Button ButtonCalculate;
        private System.Windows.Forms.TextBox TextResult;
    }
}