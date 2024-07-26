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
            label1 = new System.Windows.Forms.Label();
            textBox1 = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            CheckHelmet = new System.Windows.Forms.CheckBox();
            HelmetID = new System.Windows.Forms.TextBox();
            HelmetName = new System.Windows.Forms.Label();
            RightHandName = new System.Windows.Forms.Label();
            RightHandID = new System.Windows.Forms.TextBox();
            CheckRightHand = new System.Windows.Forms.CheckBox();
            label7 = new System.Windows.Forms.Label();
            ChestName = new System.Windows.Forms.Label();
            ChestID = new System.Windows.Forms.TextBox();
            CheckChest = new System.Windows.Forms.CheckBox();
            label9 = new System.Windows.Forms.Label();
            LeftHandName = new System.Windows.Forms.Label();
            LeftHandID = new System.Windows.Forms.TextBox();
            CheckLeftHand = new System.Windows.Forms.CheckBox();
            label11 = new System.Windows.Forms.Label();
            RightRingName = new System.Windows.Forms.Label();
            RightRingID = new System.Windows.Forms.TextBox();
            CheckRightRing = new System.Windows.Forms.CheckBox();
            label13 = new System.Windows.Forms.Label();
            LegsName = new System.Windows.Forms.Label();
            LegsID = new System.Windows.Forms.TextBox();
            CheckLegs = new System.Windows.Forms.CheckBox();
            label15 = new System.Windows.Forms.Label();
            LeftRingName = new System.Windows.Forms.Label();
            LeftRingID = new System.Windows.Forms.TextBox();
            CheckLeftRing = new System.Windows.Forms.CheckBox();
            label17 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(69, 25);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(43, 15);
            label1.TabIndex = 19;
            label1.Text = "Unit ID";
            // 
            // textBox1
            // 
            textBox1.BackColor = System.Drawing.Color.DarkOrange;
            textBox1.Location = new System.Drawing.Point(122, 22);
            textBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox1.Name = "textBox1";
            textBox1.ShortcutsEnabled = false;
            textBox1.Size = new System.Drawing.Size(146, 23);
            textBox1.TabIndex = 16;
            textBox1.Leave += textBox1_TextChanged;
            textBox1.MouseDown += textBox1_MouseDown;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(44, 55);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(46, 15);
            label4.TabIndex = 22;
            label4.Tag = "0";
            label4.Text = "Helmet";
            // 
            // CheckHelmet
            // 
            CheckHelmet.AutoCheck = false;
            CheckHelmet.AutoSize = true;
            CheckHelmet.Location = new System.Drawing.Point(98, 55);
            CheckHelmet.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CheckHelmet.Name = "CheckHelmet";
            CheckHelmet.Size = new System.Drawing.Size(15, 14);
            CheckHelmet.TabIndex = 23;
            CheckHelmet.Tag = "0";
            CheckHelmet.UseVisualStyleBackColor = true;
            CheckHelmet.Click += CheckItem_Click;
            // 
            // HelmetID
            // 
            HelmetID.BackColor = System.Drawing.Color.DarkOrange;
            HelmetID.Enabled = false;
            HelmetID.Location = new System.Drawing.Point(122, 52);
            HelmetID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            HelmetID.Name = "HelmetID";
            HelmetID.ShortcutsEnabled = false;
            HelmetID.Size = new System.Drawing.Size(146, 23);
            HelmetID.TabIndex = 24;
            HelmetID.Tag = "0";
            HelmetID.Text = "0";
            HelmetID.Leave += TextBoxItem_Validated;
            HelmetID.MouseDown += LeftRingID_MouseDown;
            // 
            // HelmetName
            // 
            HelmetName.AutoSize = true;
            HelmetName.Location = new System.Drawing.Point(276, 55);
            HelmetName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            HelmetName.Name = "HelmetName";
            HelmetName.Size = new System.Drawing.Size(0, 15);
            HelmetName.TabIndex = 25;
            HelmetName.Tag = "0";
            // 
            // RightHandName
            // 
            RightHandName.AutoSize = true;
            RightHandName.Location = new System.Drawing.Point(276, 85);
            RightHandName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            RightHandName.Name = "RightHandName";
            RightHandName.Size = new System.Drawing.Size(0, 15);
            RightHandName.TabIndex = 29;
            RightHandName.Tag = "1";
            // 
            // RightHandID
            // 
            RightHandID.BackColor = System.Drawing.Color.DarkOrange;
            RightHandID.Enabled = false;
            RightHandID.Location = new System.Drawing.Point(122, 82);
            RightHandID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RightHandID.Name = "RightHandID";
            RightHandID.ShortcutsEnabled = false;
            RightHandID.Size = new System.Drawing.Size(146, 23);
            RightHandID.TabIndex = 28;
            RightHandID.Tag = "1";
            RightHandID.Text = "0";
            RightHandID.Leave += TextBoxItem_Validated;
            RightHandID.MouseDown += LeftRingID_MouseDown;
            // 
            // CheckRightHand
            // 
            CheckRightHand.AutoCheck = false;
            CheckRightHand.AutoSize = true;
            CheckRightHand.Location = new System.Drawing.Point(98, 85);
            CheckRightHand.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CheckRightHand.Name = "CheckRightHand";
            CheckRightHand.Size = new System.Drawing.Size(15, 14);
            CheckRightHand.TabIndex = 27;
            CheckRightHand.Tag = "1";
            CheckRightHand.UseVisualStyleBackColor = true;
            CheckRightHand.Click += CheckItem_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(22, 85);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(65, 15);
            label7.TabIndex = 26;
            label7.Tag = "1";
            label7.Text = "Right hand";
            // 
            // ChestName
            // 
            ChestName.AutoSize = true;
            ChestName.Location = new System.Drawing.Point(276, 115);
            ChestName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            ChestName.Name = "ChestName";
            ChestName.Size = new System.Drawing.Size(0, 15);
            ChestName.TabIndex = 33;
            ChestName.Tag = "2";
            // 
            // ChestID
            // 
            ChestID.BackColor = System.Drawing.Color.DarkOrange;
            ChestID.Enabled = false;
            ChestID.Location = new System.Drawing.Point(122, 112);
            ChestID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ChestID.Name = "ChestID";
            ChestID.ShortcutsEnabled = false;
            ChestID.Size = new System.Drawing.Size(146, 23);
            ChestID.TabIndex = 32;
            ChestID.Tag = "2";
            ChestID.Text = "0";
            ChestID.Leave += TextBoxItem_Validated;
            ChestID.MouseDown += LeftRingID_MouseDown;
            // 
            // CheckChest
            // 
            CheckChest.AutoCheck = false;
            CheckChest.AutoSize = true;
            CheckChest.Location = new System.Drawing.Point(98, 115);
            CheckChest.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CheckChest.Name = "CheckChest";
            CheckChest.Size = new System.Drawing.Size(15, 14);
            CheckChest.TabIndex = 31;
            CheckChest.Tag = "2";
            CheckChest.UseVisualStyleBackColor = true;
            CheckChest.Click += CheckItem_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(51, 115);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(37, 15);
            label9.TabIndex = 30;
            label9.Tag = "2";
            label9.Text = "Chest";
            // 
            // LeftHandName
            // 
            LeftHandName.AutoSize = true;
            LeftHandName.Location = new System.Drawing.Point(276, 145);
            LeftHandName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LeftHandName.Name = "LeftHandName";
            LeftHandName.Size = new System.Drawing.Size(0, 15);
            LeftHandName.TabIndex = 37;
            LeftHandName.Tag = "3";
            // 
            // LeftHandID
            // 
            LeftHandID.BackColor = System.Drawing.Color.DarkOrange;
            LeftHandID.Enabled = false;
            LeftHandID.Location = new System.Drawing.Point(122, 142);
            LeftHandID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LeftHandID.Name = "LeftHandID";
            LeftHandID.ShortcutsEnabled = false;
            LeftHandID.Size = new System.Drawing.Size(146, 23);
            LeftHandID.TabIndex = 36;
            LeftHandID.Tag = "3";
            LeftHandID.Text = "0";
            LeftHandID.Leave += TextBoxItem_Validated;
            LeftHandID.MouseDown += LeftRingID_MouseDown;
            // 
            // CheckLeftHand
            // 
            CheckLeftHand.AutoCheck = false;
            CheckLeftHand.AutoSize = true;
            CheckLeftHand.Location = new System.Drawing.Point(98, 145);
            CheckLeftHand.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CheckLeftHand.Name = "CheckLeftHand";
            CheckLeftHand.Size = new System.Drawing.Size(15, 14);
            CheckLeftHand.TabIndex = 35;
            CheckLeftHand.Tag = "3";
            CheckLeftHand.UseVisualStyleBackColor = true;
            CheckLeftHand.Click += CheckItem_Click;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(30, 145);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(57, 15);
            label11.TabIndex = 34;
            label11.Tag = "3";
            label11.Text = "Left hand";
            // 
            // RightRingName
            // 
            RightRingName.AutoSize = true;
            RightRingName.Location = new System.Drawing.Point(276, 175);
            RightRingName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            RightRingName.Name = "RightRingName";
            RightRingName.Size = new System.Drawing.Size(0, 15);
            RightRingName.TabIndex = 41;
            RightRingName.Tag = "4";
            // 
            // RightRingID
            // 
            RightRingID.BackColor = System.Drawing.Color.DarkOrange;
            RightRingID.Enabled = false;
            RightRingID.Location = new System.Drawing.Point(122, 172);
            RightRingID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RightRingID.Name = "RightRingID";
            RightRingID.ShortcutsEnabled = false;
            RightRingID.Size = new System.Drawing.Size(146, 23);
            RightRingID.TabIndex = 40;
            RightRingID.Tag = "4";
            RightRingID.Text = "0";
            RightRingID.Leave += TextBoxItem_Validated;
            RightRingID.MouseDown += LeftRingID_MouseDown;
            // 
            // CheckRightRing
            // 
            CheckRightRing.AutoCheck = false;
            CheckRightRing.AutoSize = true;
            CheckRightRing.Location = new System.Drawing.Point(98, 175);
            CheckRightRing.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CheckRightRing.Name = "CheckRightRing";
            CheckRightRing.Size = new System.Drawing.Size(15, 14);
            CheckRightRing.TabIndex = 39;
            CheckRightRing.Tag = "4";
            CheckRightRing.UseVisualStyleBackColor = true;
            CheckRightRing.Click += CheckItem_Click;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(30, 175);
            label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(59, 15);
            label13.TabIndex = 38;
            label13.Tag = "4";
            label13.Text = "Right ring";
            // 
            // LegsName
            // 
            LegsName.AutoSize = true;
            LegsName.Location = new System.Drawing.Point(276, 205);
            LegsName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LegsName.Name = "LegsName";
            LegsName.Size = new System.Drawing.Size(0, 15);
            LegsName.TabIndex = 45;
            LegsName.Tag = "5";
            // 
            // LegsID
            // 
            LegsID.BackColor = System.Drawing.Color.DarkOrange;
            LegsID.Enabled = false;
            LegsID.Location = new System.Drawing.Point(122, 202);
            LegsID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LegsID.Name = "LegsID";
            LegsID.ShortcutsEnabled = false;
            LegsID.Size = new System.Drawing.Size(146, 23);
            LegsID.TabIndex = 44;
            LegsID.Tag = "5";
            LegsID.Text = "0";
            LegsID.Leave += TextBoxItem_Validated;
            LegsID.MouseDown += LeftRingID_MouseDown;
            // 
            // CheckLegs
            // 
            CheckLegs.AutoCheck = false;
            CheckLegs.AutoSize = true;
            CheckLegs.Location = new System.Drawing.Point(98, 205);
            CheckLegs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CheckLegs.Name = "CheckLegs";
            CheckLegs.Size = new System.Drawing.Size(15, 14);
            CheckLegs.TabIndex = 43;
            CheckLegs.Tag = "5";
            CheckLegs.UseVisualStyleBackColor = true;
            CheckLegs.Click += CheckItem_Click;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(56, 205);
            label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(31, 15);
            label15.TabIndex = 42;
            label15.Tag = "5";
            label15.Text = "Legs";
            // 
            // LeftRingName
            // 
            LeftRingName.AutoSize = true;
            LeftRingName.Location = new System.Drawing.Point(276, 235);
            LeftRingName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LeftRingName.Name = "LeftRingName";
            LeftRingName.Size = new System.Drawing.Size(0, 15);
            LeftRingName.TabIndex = 49;
            LeftRingName.Tag = "6";
            // 
            // LeftRingID
            // 
            LeftRingID.BackColor = System.Drawing.Color.DarkOrange;
            LeftRingID.Enabled = false;
            LeftRingID.Location = new System.Drawing.Point(122, 232);
            LeftRingID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LeftRingID.Name = "LeftRingID";
            LeftRingID.ShortcutsEnabled = false;
            LeftRingID.Size = new System.Drawing.Size(146, 23);
            LeftRingID.TabIndex = 48;
            LeftRingID.Tag = "6";
            LeftRingID.Text = "0";
            LeftRingID.Leave += TextBoxItem_Validated;
            LeftRingID.MouseDown += LeftRingID_MouseDown;
            // 
            // CheckLeftRing
            // 
            CheckLeftRing.AutoCheck = false;
            CheckLeftRing.AutoSize = true;
            CheckLeftRing.Location = new System.Drawing.Point(98, 235);
            CheckLeftRing.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CheckLeftRing.Name = "CheckLeftRing";
            CheckLeftRing.Size = new System.Drawing.Size(15, 14);
            CheckLeftRing.TabIndex = 47;
            CheckLeftRing.Tag = "6";
            CheckLeftRing.UseVisualStyleBackColor = true;
            CheckLeftRing.Click += CheckItem_Click;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(38, 235);
            label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(51, 15);
            label17.TabIndex = 46;
            label17.Tag = "6";
            label17.Text = "Left ring";
            // 
            // Control19
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(LeftRingName);
            Controls.Add(LeftRingID);
            Controls.Add(CheckLeftRing);
            Controls.Add(label17);
            Controls.Add(LegsName);
            Controls.Add(LegsID);
            Controls.Add(CheckLegs);
            Controls.Add(label15);
            Controls.Add(RightRingName);
            Controls.Add(RightRingID);
            Controls.Add(CheckRightRing);
            Controls.Add(label13);
            Controls.Add(LeftHandName);
            Controls.Add(LeftHandID);
            Controls.Add(CheckLeftHand);
            Controls.Add(label11);
            Controls.Add(ChestName);
            Controls.Add(ChestID);
            Controls.Add(CheckChest);
            Controls.Add(label9);
            Controls.Add(RightHandName);
            Controls.Add(RightHandID);
            Controls.Add(CheckRightHand);
            Controls.Add(label7);
            Controls.Add(HelmetName);
            Controls.Add(HelmetID);
            Controls.Add(CheckHelmet);
            Controls.Add(label4);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control19";
            ResumeLayout(false);
            PerformLayout();
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
