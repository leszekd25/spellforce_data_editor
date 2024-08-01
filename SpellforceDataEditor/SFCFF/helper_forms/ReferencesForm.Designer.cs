namespace SpellforceDataEditor.SFCFF.helper_forms
{
    partial class ReferencesForm
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
            listBox1 = new System.Windows.Forms.ListBox();
            labelRefElemName = new System.Windows.Forms.Label();
            LabelRefNum = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.HorizontalScrollbar = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new System.Drawing.Point(0, 30);
            listBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listBox1.Name = "listBox1";
            listBox1.Size = new System.Drawing.Size(768, 424);
            listBox1.TabIndex = 0;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // labelRefElemName
            // 
            labelRefElemName.AutoSize = true;
            labelRefElemName.Location = new System.Drawing.Point(14, 10);
            labelRefElemName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelRefElemName.Name = "labelRefElemName";
            labelRefElemName.Size = new System.Drawing.Size(0, 15);
            labelRefElemName.TabIndex = 1;
            // 
            // LabelRefNum
            // 
            LabelRefNum.AutoSize = true;
            LabelRefNum.Location = new System.Drawing.Point(12, 9);
            LabelRefNum.Name = "LabelRefNum";
            LabelRefNum.Size = new System.Drawing.Size(0, 15);
            LabelRefNum.TabIndex = 2;
            // 
            // ReferencesForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(768, 454);
            Controls.Add(LabelRefNum);
            Controls.Add(labelRefElemName);
            Controls.Add(listBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "ReferencesForm";
            Text = "References";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label labelRefElemName;
        private System.Windows.Forms.Label LabelRefNum;
    }
}