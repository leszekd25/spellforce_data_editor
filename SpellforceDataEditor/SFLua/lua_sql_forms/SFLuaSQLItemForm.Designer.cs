namespace SpellforceDataEditor.SFLua.lua_sql_forms
{
    partial class SFLuaSQLItemForm
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
            this.ListItems = new System.Windows.Forms.ListBox();
            this.ButtonCancelChanges = new System.Windows.Forms.Button();
            this.ButtonSaveChanges = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.MMC = new System.Windows.Forms.TextBox();
            this.MFC = new System.Windows.Forms.TextBox();
            this.MMW = new System.Windows.Forms.TextBox();
            this.MFW = new System.Windows.Forms.TextBox();
            this.SelectionSize = new System.Windows.Forms.TextBox();
            this.ShadowRNG = new System.Windows.Forms.TextBox();
            this.AnimSet = new System.Windows.Forms.TextBox();
            this.Race = new System.Windows.Forms.TextBox();
            this.Cat = new System.Windows.Forms.TextBox();
            this.SubCat = new System.Windows.Forms.TextBox();
            this.ButtonAdd = new System.Windows.Forms.Button();
            this.ButtonRemove = new System.Windows.Forms.Button();
            this.ItemID = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ListItems
            // 
            this.ListItems.FormattingEnabled = true;
            this.ListItems.Location = new System.Drawing.Point(12, 38);
            this.ListItems.Name = "ListItems";
            this.ListItems.Size = new System.Drawing.Size(220, 251);
            this.ListItems.TabIndex = 0;
            this.ListItems.SelectedIndexChanged += new System.EventHandler(this.ListItems_SelectedIndexChanged);
            // 
            // ButtonCancelChanges
            // 
            this.ButtonCancelChanges.Location = new System.Drawing.Point(12, 334);
            this.ButtonCancelChanges.Name = "ButtonCancelChanges";
            this.ButtonCancelChanges.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancelChanges.TabIndex = 1;
            this.ButtonCancelChanges.Text = "Cancel";
            this.ButtonCancelChanges.UseVisualStyleBackColor = true;
            this.ButtonCancelChanges.Click += new System.EventHandler(this.ButtonCancelChanges_Click);
            // 
            // ButtonSaveChanges
            // 
            this.ButtonSaveChanges.Location = new System.Drawing.Point(608, 334);
            this.ButtonSaveChanges.Name = "ButtonSaveChanges";
            this.ButtonSaveChanges.Size = new System.Drawing.Size(75, 23);
            this.ButtonSaveChanges.TabIndex = 2;
            this.ButtonSaveChanges.Text = "Apply";
            this.ButtonSaveChanges.UseVisualStyleBackColor = true;
            this.ButtonSaveChanges.Click += new System.EventHandler(this.ButtonSaveChanges_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Items";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(238, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Mesh (male cold)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(238, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Mesh (female cold)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(238, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Mesh (male warm)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(238, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Mesh (female warm)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(238, 145);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Shadow RNG (?)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(238, 196);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Animation set";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(238, 170);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Selection size (?)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(238, 273);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(117, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Subcategory (unused?)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(238, 222);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Race (?)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(238, 247);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(99, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Category (unused?)";
            // 
            // MMC
            // 
            this.MMC.Location = new System.Drawing.Point(361, 38);
            this.MMC.Name = "MMC";
            this.MMC.Size = new System.Drawing.Size(322, 20);
            this.MMC.TabIndex = 14;
            this.MMC.Validated += new System.EventHandler(this.MMC_Validated);
            // 
            // MFC
            // 
            this.MFC.Location = new System.Drawing.Point(361, 64);
            this.MFC.Name = "MFC";
            this.MFC.Size = new System.Drawing.Size(322, 20);
            this.MFC.TabIndex = 15;
            this.MFC.Validated += new System.EventHandler(this.MFC_Validated);
            // 
            // MMW
            // 
            this.MMW.Location = new System.Drawing.Point(361, 90);
            this.MMW.Name = "MMW";
            this.MMW.Size = new System.Drawing.Size(322, 20);
            this.MMW.TabIndex = 16;
            this.MMW.Validated += new System.EventHandler(this.MMW_Validated);
            // 
            // MFW
            // 
            this.MFW.Location = new System.Drawing.Point(361, 116);
            this.MFW.Name = "MFW";
            this.MFW.Size = new System.Drawing.Size(322, 20);
            this.MFW.TabIndex = 17;
            this.MFW.Validated += new System.EventHandler(this.MFW_Validated);
            // 
            // SelectionSize
            // 
            this.SelectionSize.Location = new System.Drawing.Point(361, 167);
            this.SelectionSize.Name = "SelectionSize";
            this.SelectionSize.Size = new System.Drawing.Size(100, 20);
            this.SelectionSize.TabIndex = 18;
            this.SelectionSize.Validated += new System.EventHandler(this.SelectionSize_Validated);
            // 
            // ShadowRNG
            // 
            this.ShadowRNG.Location = new System.Drawing.Point(361, 142);
            this.ShadowRNG.Name = "ShadowRNG";
            this.ShadowRNG.Size = new System.Drawing.Size(100, 20);
            this.ShadowRNG.TabIndex = 19;
            this.ShadowRNG.Validated += new System.EventHandler(this.ShadowRNG_Validated);
            // 
            // AnimSet
            // 
            this.AnimSet.Location = new System.Drawing.Point(361, 193);
            this.AnimSet.Name = "AnimSet";
            this.AnimSet.Size = new System.Drawing.Size(322, 20);
            this.AnimSet.TabIndex = 20;
            this.AnimSet.Validated += new System.EventHandler(this.AnimSet_Validated);
            // 
            // Race
            // 
            this.Race.Location = new System.Drawing.Point(361, 219);
            this.Race.Name = "Race";
            this.Race.Size = new System.Drawing.Size(100, 20);
            this.Race.TabIndex = 22;
            this.Race.Validated += new System.EventHandler(this.Race_Validated);
            // 
            // Cat
            // 
            this.Cat.Location = new System.Drawing.Point(361, 244);
            this.Cat.Name = "Cat";
            this.Cat.Size = new System.Drawing.Size(100, 20);
            this.Cat.TabIndex = 21;
            this.Cat.Validated += new System.EventHandler(this.Cat_Validated);
            // 
            // SubCat
            // 
            this.SubCat.Location = new System.Drawing.Point(361, 270);
            this.SubCat.Name = "SubCat";
            this.SubCat.Size = new System.Drawing.Size(100, 20);
            this.SubCat.TabIndex = 24;
            this.SubCat.Validated += new System.EventHandler(this.SubCat_Validated);
            // 
            // ButtonAdd
            // 
            this.ButtonAdd.Location = new System.Drawing.Point(76, 295);
            this.ButtonAdd.Name = "ButtonAdd";
            this.ButtonAdd.Size = new System.Drawing.Size(75, 23);
            this.ButtonAdd.TabIndex = 25;
            this.ButtonAdd.Text = "Add";
            this.ButtonAdd.UseVisualStyleBackColor = true;
            this.ButtonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // ButtonRemove
            // 
            this.ButtonRemove.Location = new System.Drawing.Point(157, 295);
            this.ButtonRemove.Name = "ButtonRemove";
            this.ButtonRemove.Size = new System.Drawing.Size(75, 23);
            this.ButtonRemove.TabIndex = 26;
            this.ButtonRemove.Text = "Remove";
            this.ButtonRemove.UseVisualStyleBackColor = true;
            this.ButtonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // ItemID
            // 
            this.ItemID.Location = new System.Drawing.Point(12, 295);
            this.ItemID.Name = "ItemID";
            this.ItemID.Size = new System.Drawing.Size(58, 20);
            this.ItemID.TabIndex = 27;
            this.ItemID.Validated += new System.EventHandler(this.ItemID_Validated);
            // 
            // SFLuaSQLItemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 368);
            this.Controls.Add(this.ItemID);
            this.Controls.Add(this.ButtonRemove);
            this.Controls.Add(this.ButtonAdd);
            this.Controls.Add(this.SubCat);
            this.Controls.Add(this.Race);
            this.Controls.Add(this.Cat);
            this.Controls.Add(this.AnimSet);
            this.Controls.Add(this.ShadowRNG);
            this.Controls.Add(this.SelectionSize);
            this.Controls.Add(this.MFW);
            this.Controls.Add(this.MMW);
            this.Controls.Add(this.MFC);
            this.Controls.Add(this.MMC);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButtonSaveChanges);
            this.Controls.Add(this.ButtonCancelChanges);
            this.Controls.Add(this.ListItems);
            this.Name = "SFLuaSQLItemForm";
            this.Text = "sql_item";
            this.Load += new System.EventHandler(this.SFLuaSQLItemForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ListItems;
        private System.Windows.Forms.Button ButtonCancelChanges;
        private System.Windows.Forms.Button ButtonSaveChanges;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox MMC;
        private System.Windows.Forms.TextBox MFC;
        private System.Windows.Forms.TextBox MMW;
        private System.Windows.Forms.TextBox MFW;
        private System.Windows.Forms.TextBox SelectionSize;
        private System.Windows.Forms.TextBox ShadowRNG;
        private System.Windows.Forms.TextBox AnimSet;
        private System.Windows.Forms.TextBox Race;
        private System.Windows.Forms.TextBox Cat;
        private System.Windows.Forms.TextBox SubCat;
        private System.Windows.Forms.Button ButtonAdd;
        private System.Windows.Forms.Button ButtonRemove;
        private System.Windows.Forms.TextBox ItemID;
    }
}