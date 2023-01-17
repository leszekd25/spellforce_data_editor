namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control19
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CheckHelmet = new System.Windows.Forms.CheckBox();
            this.HelmetID = new System.Windows.Forms.TextBox();
            this.HelmetName = new System.Windows.Forms.Label();
            this.RightHandName = new System.Windows.Forms.Label();
            this.RightHandID = new System.Windows.Forms.TextBox();
            this.CheckRightHand = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ChestName = new System.Windows.Forms.Label();
            this.ChestID = new System.Windows.Forms.TextBox();
            this.CheckChest = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.LeftHandName = new System.Windows.Forms.Label();
            this.LeftHandID = new System.Windows.Forms.TextBox();
            this.CheckLeftHand = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.RightRingName = new System.Windows.Forms.Label();
            this.RightRingID = new System.Windows.Forms.TextBox();
            this.CheckRightRing = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.LegsName = new System.Windows.Forms.Label();
            this.LegsID = new System.Windows.Forms.TextBox();
            this.CheckLegs = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.LeftRingName = new System.Windows.Forms.Label();
            this.LeftRingID = new System.Windows.Forms.TextBox();
            this.CheckLeftRing = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Unit ID";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.DarkOrange;
            this.textBox1.Location = new System.Drawing.Point(105, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(126, 20);
            this.textBox1.TabIndex = 16;
            this.textBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBox1_MouseDown);
            this.textBox1.Leave += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Helmet";
            // 
            // CheckHelmet
            // 
            this.CheckHelmet.AutoSize = true;
            this.CheckHelmet.Location = new System.Drawing.Point(84, 48);
            this.CheckHelmet.Name = "CheckHelmet";
            this.CheckHelmet.Size = new System.Drawing.Size(15, 14);
            this.CheckHelmet.TabIndex = 23;
            this.CheckHelmet.Tag = "0";
            this.CheckHelmet.UseVisualStyleBackColor = true;
            this.CheckHelmet.Click += new System.EventHandler(this.CheckItem_Click);
            // 
            // HelmetID
            // 
            this.HelmetID.BackColor = System.Drawing.Color.DarkOrange;
            this.HelmetID.Enabled = false;
            this.HelmetID.Location = new System.Drawing.Point(105, 45);
            this.HelmetID.Name = "HelmetID";
            this.HelmetID.Size = new System.Drawing.Size(126, 20);
            this.HelmetID.TabIndex = 24;
            this.HelmetID.Tag = "0";
            this.HelmetID.Text = "0";
            this.HelmetID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextboxItem_MouseDown);
            this.HelmetID.Leave += new System.EventHandler(this.TextBoxItem_Validated);
            // 
            // HelmetName
            // 
            this.HelmetName.AutoSize = true;
            this.HelmetName.Location = new System.Drawing.Point(237, 48);
            this.HelmetName.Name = "HelmetName";
            this.HelmetName.Size = new System.Drawing.Size(0, 13);
            this.HelmetName.TabIndex = 25;
            // 
            // RightHandName
            // 
            this.RightHandName.AutoSize = true;
            this.RightHandName.Location = new System.Drawing.Point(237, 74);
            this.RightHandName.Name = "RightHandName";
            this.RightHandName.Size = new System.Drawing.Size(0, 13);
            this.RightHandName.TabIndex = 29;
            // 
            // RightHandID
            // 
            this.RightHandID.BackColor = System.Drawing.Color.DarkOrange;
            this.RightHandID.Enabled = false;
            this.RightHandID.Location = new System.Drawing.Point(105, 71);
            this.RightHandID.Name = "RightHandID";
            this.RightHandID.Size = new System.Drawing.Size(126, 20);
            this.RightHandID.TabIndex = 28;
            this.RightHandID.Tag = "1";
            this.RightHandID.Text = "0";
            this.RightHandID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextboxItem_MouseDown);
            this.RightHandID.Leave += new System.EventHandler(this.TextBoxItem_Validated);
            // 
            // CheckRightHand
            // 
            this.CheckRightHand.AutoSize = true;
            this.CheckRightHand.Location = new System.Drawing.Point(84, 74);
            this.CheckRightHand.Name = "CheckRightHand";
            this.CheckRightHand.Size = new System.Drawing.Size(15, 14);
            this.CheckRightHand.TabIndex = 27;
            this.CheckRightHand.Tag = "1";
            this.CheckRightHand.UseVisualStyleBackColor = true;
            this.CheckRightHand.Click += new System.EventHandler(this.CheckItem_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Right hand";
            // 
            // ChestName
            // 
            this.ChestName.AutoSize = true;
            this.ChestName.Location = new System.Drawing.Point(237, 100);
            this.ChestName.Name = "ChestName";
            this.ChestName.Size = new System.Drawing.Size(0, 13);
            this.ChestName.TabIndex = 33;
            // 
            // ChestID
            // 
            this.ChestID.BackColor = System.Drawing.Color.DarkOrange;
            this.ChestID.Enabled = false;
            this.ChestID.Location = new System.Drawing.Point(105, 97);
            this.ChestID.Name = "ChestID";
            this.ChestID.Size = new System.Drawing.Size(126, 20);
            this.ChestID.TabIndex = 32;
            this.ChestID.Tag = "2";
            this.ChestID.Text = "0";
            this.ChestID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextboxItem_MouseDown);
            this.ChestID.Leave += new System.EventHandler(this.TextBoxItem_Validated);
            // 
            // CheckChest
            // 
            this.CheckChest.AutoSize = true;
            this.CheckChest.Location = new System.Drawing.Point(84, 100);
            this.CheckChest.Name = "CheckChest";
            this.CheckChest.Size = new System.Drawing.Size(15, 14);
            this.CheckChest.TabIndex = 31;
            this.CheckChest.Tag = "2";
            this.CheckChest.UseVisualStyleBackColor = true;
            this.CheckChest.Click += new System.EventHandler(this.CheckItem_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(44, 100);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 30;
            this.label9.Text = "Chest";
            // 
            // LeftHandName
            // 
            this.LeftHandName.AutoSize = true;
            this.LeftHandName.Location = new System.Drawing.Point(237, 126);
            this.LeftHandName.Name = "LeftHandName";
            this.LeftHandName.Size = new System.Drawing.Size(0, 13);
            this.LeftHandName.TabIndex = 37;
            // 
            // LeftHandID
            // 
            this.LeftHandID.BackColor = System.Drawing.Color.DarkOrange;
            this.LeftHandID.Enabled = false;
            this.LeftHandID.Location = new System.Drawing.Point(105, 123);
            this.LeftHandID.Name = "LeftHandID";
            this.LeftHandID.Size = new System.Drawing.Size(126, 20);
            this.LeftHandID.TabIndex = 36;
            this.LeftHandID.Tag = "3";
            this.LeftHandID.Text = "0";
            this.LeftHandID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextboxItem_MouseDown);
            this.LeftHandID.Leave += new System.EventHandler(this.TextBoxItem_Validated);
            // 
            // CheckLeftHand
            // 
            this.CheckLeftHand.AutoSize = true;
            this.CheckLeftHand.Location = new System.Drawing.Point(84, 126);
            this.CheckLeftHand.Name = "CheckLeftHand";
            this.CheckLeftHand.Size = new System.Drawing.Size(15, 14);
            this.CheckLeftHand.TabIndex = 35;
            this.CheckLeftHand.Tag = "3";
            this.CheckLeftHand.UseVisualStyleBackColor = true;
            this.CheckLeftHand.Click += new System.EventHandler(this.CheckItem_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(26, 126);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 13);
            this.label11.TabIndex = 34;
            this.label11.Text = "Left hand";
            // 
            // RightRingName
            // 
            this.RightRingName.AutoSize = true;
            this.RightRingName.Location = new System.Drawing.Point(237, 152);
            this.RightRingName.Name = "RightRingName";
            this.RightRingName.Size = new System.Drawing.Size(0, 13);
            this.RightRingName.TabIndex = 41;
            // 
            // RightRingID
            // 
            this.RightRingID.BackColor = System.Drawing.Color.DarkOrange;
            this.RightRingID.Enabled = false;
            this.RightRingID.Location = new System.Drawing.Point(105, 149);
            this.RightRingID.Name = "RightRingID";
            this.RightRingID.Size = new System.Drawing.Size(126, 20);
            this.RightRingID.TabIndex = 40;
            this.RightRingID.Tag = "4";
            this.RightRingID.Text = "0";
            this.RightRingID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextboxItem_MouseDown);
            this.RightRingID.Leave += new System.EventHandler(this.TextBoxItem_Validated);
            // 
            // CheckRightRing
            // 
            this.CheckRightRing.AutoSize = true;
            this.CheckRightRing.Location = new System.Drawing.Point(84, 152);
            this.CheckRightRing.Name = "CheckRightRing";
            this.CheckRightRing.Size = new System.Drawing.Size(15, 14);
            this.CheckRightRing.TabIndex = 39;
            this.CheckRightRing.Tag = "4";
            this.CheckRightRing.UseVisualStyleBackColor = true;
            this.CheckRightRing.Click += new System.EventHandler(this.CheckItem_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(26, 152);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 13);
            this.label13.TabIndex = 38;
            this.label13.Text = "Right ring";
            // 
            // LegsName
            // 
            this.LegsName.AutoSize = true;
            this.LegsName.Location = new System.Drawing.Point(237, 178);
            this.LegsName.Name = "LegsName";
            this.LegsName.Size = new System.Drawing.Size(0, 13);
            this.LegsName.TabIndex = 45;
            // 
            // LegsID
            // 
            this.LegsID.BackColor = System.Drawing.Color.DarkOrange;
            this.LegsID.Enabled = false;
            this.LegsID.Location = new System.Drawing.Point(105, 175);
            this.LegsID.Name = "LegsID";
            this.LegsID.Size = new System.Drawing.Size(126, 20);
            this.LegsID.TabIndex = 44;
            this.LegsID.Tag = "5";
            this.LegsID.Text = "0";
            this.LegsID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextboxItem_MouseDown);
            this.LegsID.Leave += new System.EventHandler(this.TextBoxItem_Validated);
            // 
            // CheckLegs
            // 
            this.CheckLegs.AutoSize = true;
            this.CheckLegs.Location = new System.Drawing.Point(84, 178);
            this.CheckLegs.Name = "CheckLegs";
            this.CheckLegs.Size = new System.Drawing.Size(15, 14);
            this.CheckLegs.TabIndex = 43;
            this.CheckLegs.Tag = "5";
            this.CheckLegs.UseVisualStyleBackColor = true;
            this.CheckLegs.Click += new System.EventHandler(this.CheckItem_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(48, 178);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(30, 13);
            this.label15.TabIndex = 42;
            this.label15.Text = "Legs";
            // 
            // LeftRingName
            // 
            this.LeftRingName.AutoSize = true;
            this.LeftRingName.Location = new System.Drawing.Point(237, 204);
            this.LeftRingName.Name = "LeftRingName";
            this.LeftRingName.Size = new System.Drawing.Size(0, 13);
            this.LeftRingName.TabIndex = 49;
            // 
            // LeftRingID
            // 
            this.LeftRingID.BackColor = System.Drawing.Color.DarkOrange;
            this.LeftRingID.Enabled = false;
            this.LeftRingID.Location = new System.Drawing.Point(105, 201);
            this.LeftRingID.Name = "LeftRingID";
            this.LeftRingID.Size = new System.Drawing.Size(126, 20);
            this.LeftRingID.TabIndex = 48;
            this.LeftRingID.Tag = "6";
            this.LeftRingID.Text = "0";
            this.LeftRingID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TextboxItem_MouseDown);
            this.LeftRingID.Leave += new System.EventHandler(this.TextBoxItem_Validated);
            // 
            // CheckLeftRing
            // 
            this.CheckLeftRing.AutoSize = true;
            this.CheckLeftRing.Location = new System.Drawing.Point(84, 204);
            this.CheckLeftRing.Name = "CheckLeftRing";
            this.CheckLeftRing.Size = new System.Drawing.Size(15, 14);
            this.CheckLeftRing.TabIndex = 47;
            this.CheckLeftRing.Tag = "6";
            this.CheckLeftRing.UseVisualStyleBackColor = true;
            this.CheckLeftRing.Click += new System.EventHandler(this.CheckItem_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(33, 204);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(45, 13);
            this.label17.TabIndex = 46;
            this.label17.Text = "Left ring";
            // 
            // Control19
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LeftRingName);
            this.Controls.Add(this.LeftRingID);
            this.Controls.Add(this.CheckLeftRing);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.LegsName);
            this.Controls.Add(this.LegsID);
            this.Controls.Add(this.CheckLegs);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.RightRingName);
            this.Controls.Add(this.RightRingID);
            this.Controls.Add(this.CheckRightRing);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.LeftHandName);
            this.Controls.Add(this.LeftHandID);
            this.Controls.Add(this.CheckLeftHand);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.ChestName);
            this.Controls.Add(this.ChestID);
            this.Controls.Add(this.CheckChest);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.RightHandName);
            this.Controls.Add(this.RightHandID);
            this.Controls.Add(this.CheckRightHand);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.HelmetName);
            this.Controls.Add(this.HelmetID);
            this.Controls.Add(this.CheckHelmet);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Name = "Control19";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox CheckHelmet;
        private System.Windows.Forms.TextBox HelmetID;
        private System.Windows.Forms.Label HelmetName;
        private System.Windows.Forms.Label RightHandName;
        private System.Windows.Forms.TextBox RightHandID;
        private System.Windows.Forms.CheckBox CheckRightHand;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label ChestName;
        private System.Windows.Forms.TextBox ChestID;
        private System.Windows.Forms.CheckBox CheckChest;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label LeftHandName;
        private System.Windows.Forms.TextBox LeftHandID;
        private System.Windows.Forms.CheckBox CheckLeftHand;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label RightRingName;
        private System.Windows.Forms.TextBox RightRingID;
        private System.Windows.Forms.CheckBox CheckRightRing;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label LegsName;
        private System.Windows.Forms.TextBox LegsID;
        private System.Windows.Forms.CheckBox CheckLegs;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label LeftRingName;
        private System.Windows.Forms.TextBox LeftRingID;
        private System.Windows.Forms.CheckBox CheckLeftRing;
        private System.Windows.Forms.Label label17;
    }
}
