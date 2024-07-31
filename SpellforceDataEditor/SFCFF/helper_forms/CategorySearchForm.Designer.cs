namespace SpellforceDataEditor.SFCFF.helper_forms
{
    partial class CategorySearchForm
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
            listResults = new System.Windows.Forms.ListBox();
            SuspendLayout();
            // 
            // listResults
            // 
            listResults.FormattingEnabled = true;
            listResults.ItemHeight = 15;
            listResults.Location = new System.Drawing.Point(12, 12);
            listResults.Name = "listResults";
            listResults.Size = new System.Drawing.Size(410, 454);
            listResults.TabIndex = 0;
            listResults.SelectedIndexChanged += listResults_SelectedIndexChanged;
            // 
            // CategorySearchForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(434, 480);
            Controls.Add(listResults);
            Name = "CategorySearchForm";
            Text = "Search results";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ListBox listResults;
    }
}