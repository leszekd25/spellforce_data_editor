using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Text.RegularExpressions;

namespace SpellforceDataEditor
{
    public partial class Form1 : Form
    {
        //Variables and other useful stuff START
        FileStream fs;
        BinaryReader reader;
        string[] offsetLengthStr=new string[49]; //offset lengths in Hex
        int baseIndex = int.Parse("14", NumberStyles.HexNumber), offsetBreak = int.Parse("C", NumberStyles.HexNumber); //baseIndex=bytes till first break starts; offsetBreak=bytes between every offset(contains length amongst other things I don't care about)
        int[] offsetLength = new int[49]; //offset length in Integer for use in program
        int[] offsetStartPosition = new int[49]; //offset start position
        int[] offsetEntryLength=new int[49] { 76, 75, 6, 47, 5, 5, 22, 36, 4, 16, 6, 5, 69, 4, 566, 27, 3, 64, 5, 5, 4, 11, 5, 23, 11428, 5, 4, 9, 4, 6, 5, 3, 15, 54, 2463, 11, 6, 71, 13, 3, 4, 6, 17, 5, 4, 4, 2, 90, 4 };
        int[] offsetLengthPosition = new int[49];
        string[] entrySource;
        string[] lbT;
        bool tbChange = false, sbChange = false;
        bool isNumber;
        int output;
        string file;
        //Variables and other useful stuff END

        private void replacePosition_Click(object sender, EventArgs e)
        {
            long position = fs.Position;
            int index = comboBox1.SelectedIndex, numEntries = offsetLength[index] / offsetEntryLength[index];
            fs.Position = offsetStartPosition[index] + int.Parse(boxPosition.Text) - 1;
            byte[] replace = new byte[1];
            replace[0] = Convert.ToByte(int.Parse(boxReplace.Text));
            for(int i = 0; i < numEntries ; i++)
            {
                fs.Write(replace, 0, 1);
                fs.Position += offsetEntryLength[index] - 1;
            }
            fs.Position = position;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            long position = fs.Position;
            int index = comboBox1.SelectedIndex, numEntries = offsetLength[index] / offsetEntryLength[index], selIndex = listBox1.SelectedIndex;

            int numberEnt = listBox2.Items.Count;

            fs.Position = offsetStartPosition[index] + int.Parse(boxPosition.Text) - 1;
            byte[] replace = new byte[1];
            replace[0] = Convert.ToByte(int.Parse(boxReplace.Text));
            //listBox1.SelectedIndex = 0;
            for (int i = 0; i < numberEnt; i++)
            {
                listBox2.SelectedIndex = i;
                listBox1.Text = listBox2.Text;
                fs.Position += int.Parse(boxPosition.Text) - 1;
                    fs.Write(replace, 0, 1);
            }
            listBox1.SelectedIndex = selIndex;
        }

        public Form1()
        {
            InitializeComponent();
            Text = "Spellforce Data Editor";
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fs != null) { fs.Dispose(); } //Releases file 

            openFD.Title = "Open Gamedata";
            openFD.InitialDirectory = "Desktop";
            openFD.FileName = "";
            if (openFD.ShowDialog() != DialogResult.Cancel)
            {
                file = openFD.FileName;
                if(MessageBox.Show("Create a backup?", "", MessageBoxButtons.YesNo) != DialogResult.No)
                {
                    File.Copy(file, file + ".Backup", true);
                }

                open();

                /*fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite);
                reader = new BinaryReader(fs);

                label42.Text = file;

                fs.Position = baseIndex + offsetBreak - 3; //Set position to last byte of first Length (first Length is always at the same position, well atleast it should be :D)
                for (int j = 0; j < 49; j++) //Loop for reading the offset lengths
                {
                    for (int i = 0; i < 4; i++)
                    {
                        offsetLengthStr[j] += string.Format("{0:X2} ", reader.ReadByte());
                        fs.Position -= 2;
                    }
                    offsetLength[j] = int.Parse(offsetLengthStr[j].Replace(" ", ""), NumberStyles.HexNumber); //convert created string to integer for use in program
                    offsetLengthPosition[j] = Convert.ToInt32(fs.Position+1);
                    fs.Position += 4 + offsetLength[j] + offsetBreak; //correct position and set to next length (current position + length + offset break)
                }

                fs.Position = baseIndex + offsetBreak;
                for (int k = 0; k < 49; k++) //Loop for reading the start position for all offsets
                {
                    offsetStartPosition[k] = Convert.ToInt32(fs.Position);
                    fs.Position += offsetLength[k] + offsetBreak;
                }
                comboBox1.Enabled = true;
                listBox1.Enabled = true;*/
                reloadTextStrip.Enabled = true;
                backupCreateStrip.Enabled = true;
            }
        }

        private void open()
        {
            Array.Clear(offsetLengthStr, 0, offsetLengthStr.Length);

            fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite);
            reader = new BinaryReader(fs);

            label42.Text = file;

            fs.Position = baseIndex + offsetBreak - 3; //Set position to last byte of first Length (first Length is always at the same position, well atleast it should be :D)
            for (int j = 0; j < 49; j++) //Loop for reading the offset lengths
            {
                for (int i = 0; i < 4; i++)
                {
                    offsetLengthStr[j] += string.Format("{0:X2} ", reader.ReadByte());
                    fs.Position -= 2;
                }
                offsetLength[j] = int.Parse(offsetLengthStr[j].Replace(" ", ""), NumberStyles.HexNumber); //convert created string to integer for use in program
                offsetLengthPosition[j] = Convert.ToInt32(fs.Position + 1);
                fs.Position += 4 + offsetLength[j] + offsetBreak; //correct position and set to next length (current position + length + offset break)
            }

            fs.Position = baseIndex + offsetBreak;
            for (int k = 0; k < 49; k++) //Loop for reading the start position for all offsets
            {
                offsetStartPosition[k] = Convert.ToInt32(fs.Position);
                fs.Position += offsetLength[k] + offsetBreak;
            }
            comboBox1.Enabled = true;
            listBox1.Enabled = true;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            int selectedOffset = comboBox1.SelectedIndex, selectedEntry = listBox1.SelectedIndex;
            string[] offsetLengthTemp = new string[4];
            fs.Position = offsetLengthPosition[comboBox1.SelectedIndex];
            for(int i = 3; i >= 0; i--)
            {
                offsetLengthTemp[i] = string.Format("{0:X2}", reader.ReadByte());
            }
            string offsetLengthString = offsetLengthTemp[0] + offsetLengthTemp[1] + offsetLengthTemp[2] + offsetLengthTemp[3];
            fs.Position -= 4;
            offsetLength[comboBox1.SelectedIndex] = int.Parse(offsetLengthString, NumberStyles.HexNumber)+offsetEntryLength[comboBox1.SelectedIndex];
            offsetLengthString = string.Format("{0:X8}", offsetLength[comboBox1.SelectedIndex]);
            byte[] byteTemp = new byte[4];
            int j = 0;
            for (int i = 3; i >= 0; i--)
            {
                byteTemp[j] = Convert.ToByte(int.Parse(offsetLengthString.Substring(i * 2, 2),NumberStyles.HexNumber));
                j++;
            }
            fs.Write(byteTemp, 0, 4);
            //
            fs.Position = offsetStartPosition[comboBox1.SelectedIndex] + listBox1.SelectedIndex * offsetEntryLength[comboBox1.SelectedIndex];
            int baseLength = Convert.ToInt32(fs.Position);
            fs.Position = 0;
            byte[] p1Bytes = new byte[baseLength + offsetEntryLength[comboBox1.SelectedIndex]];
            p1Bytes = reader.ReadBytes(baseLength + offsetEntryLength[comboBox1.SelectedIndex]);
            byte[] addBytes = new byte[offsetEntryLength[comboBox1.SelectedIndex]];
            byte[] p2Bytes = new byte[fs.Length - baseLength - offsetEntryLength[comboBox1.SelectedIndex]];
            p2Bytes = reader.ReadBytes(Convert.ToInt32(fs.Length - baseLength - offsetEntryLength[comboBox1.SelectedIndex]));
            fs.Position = 0;

            /*int[] front = { 1, 2, 3, 4 };
            int[] back = { 5, 6, 7, 8 };
            int[] combined = front.Concat(back).ToArray();*/

            byte[] allBytes = p1Bytes.Concat(addBytes).Concat(p2Bytes).ToArray();
            fs.Dispose();
            File.WriteAllBytes(file, allBytes);
            //fs.Write(p1Bytes, 0, p1Bytes.Length);
            //fs.Write(addBytes, 0, addBytes.Length);//fs länge begrenzt
            //fs.Write(p2Bytes, 0, p2Bytes.Length);
            //comboBox1.SelectedIndex = -1;
            open();
            loadList(selectedOffset);
            comboBox1.SelectedIndex = selectedOffset;
            loadList(selectedOffset);
            listBox1.SelectedIndex = selectedEntry + 1;
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            int selectedOffset = comboBox1.SelectedIndex, selectedEntry = listBox1.SelectedIndex;
            string[] offsetLengthTemp = new string[4];
            fs.Position = offsetLengthPosition[comboBox1.SelectedIndex];
            for (int i = 3; i >= 0; i--)
            {
                offsetLengthTemp[i] = string.Format("{0:X2}", reader.ReadByte());
            }
            string offsetLengthString = offsetLengthTemp[0] + offsetLengthTemp[1] + offsetLengthTemp[2] + offsetLengthTemp[3];
            fs.Position -= 4;
            offsetLength[comboBox1.SelectedIndex] = int.Parse(offsetLengthString, NumberStyles.HexNumber) + offsetEntryLength[comboBox1.SelectedIndex];
            offsetLengthString = string.Format("{0:X8}", offsetLength[comboBox1.SelectedIndex]);
            byte[] byteTemp = new byte[4];
            int j = 0;
            for (int i = 3; i >= 0; i--)
            {
                byteTemp[j] = Convert.ToByte(int.Parse(offsetLengthString.Substring(i * 2, 2), NumberStyles.HexNumber));
                j++;
            }
            fs.Write(byteTemp, 0, 4);

            fs.Position = offsetStartPosition[comboBox1.SelectedIndex] + listBox1.SelectedIndex * offsetEntryLength[comboBox1.SelectedIndex];
            int baseLength = Convert.ToInt32(fs.Position);
            fs.Position = 0;
            byte[] p1Bytes = new byte[baseLength];
            p1Bytes = reader.ReadBytes(baseLength);
            byte[] addBytes = new byte[offsetEntryLength[comboBox1.SelectedIndex]];

            for (int i = 0; i < offsetEntryLength[comboBox1.SelectedIndex]; i++)
            {
                addBytes[i] = Convert.ToByte(int.Parse(entrySource[i], NumberStyles.HexNumber));
            }

            byte[] p2Bytes = new byte[fs.Length - baseLength];
            p2Bytes = reader.ReadBytes(Convert.ToInt32(fs.Length - baseLength));
            fs.Position = 0;

            byte[] allBytes = p1Bytes.Concat(addBytes).Concat(p2Bytes).ToArray();
            fs.Dispose();
            File.WriteAllBytes(file, allBytes);

            open();
            loadList(selectedOffset);
            comboBox1.SelectedIndex = selectedOffset;
            loadList(selectedOffset);
            listBox1.SelectedIndex = selectedEntry + 1;
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            //Get and write new length
            int selectedOffset = comboBox1.SelectedIndex, selectedEntry = listBox1.SelectedIndex;
            string[] offsetLengthTemp = new string[4];
            fs.Position = offsetLengthPosition[comboBox1.SelectedIndex];
            for (int i = 3; i >= 0; i--)
            {
                offsetLengthTemp[i] = string.Format("{0:X2}", reader.ReadByte());
            }
            string offsetLengthString = offsetLengthTemp[0] + offsetLengthTemp[1] + offsetLengthTemp[2] + offsetLengthTemp[3];
            fs.Position -= 4;
            offsetLength[comboBox1.SelectedIndex] = int.Parse(offsetLengthString, NumberStyles.HexNumber) - offsetEntryLength[comboBox1.SelectedIndex];
            offsetLengthString = string.Format("{0:X8}", offsetLength[comboBox1.SelectedIndex]);
            byte[] byteTemp = new byte[4];
            int j = 0;
            for (int i = 3; i >= 0; i--)
            {
                byteTemp[j] = Convert.ToByte(int.Parse(offsetLengthString.Substring(i * 2, 2), NumberStyles.HexNumber));
                j++;
            }
            fs.Write(byteTemp, 0, 4);
            //Get and write new length

            fs.Position = offsetStartPosition[comboBox1.SelectedIndex] + listBox1.SelectedIndex * offsetEntryLength[comboBox1.SelectedIndex];
            int baseLength = Convert.ToInt32(fs.Position);
            fs.Position = 0;
            byte[] p1Bytes = new byte[baseLength];
            p1Bytes = reader.ReadBytes(baseLength);

            fs.Position += offsetEntryLength[comboBox1.SelectedIndex];
            byte[] p2Bytes = new byte[fs.Length - baseLength - offsetEntryLength[comboBox1.SelectedIndex]];
            p2Bytes = reader.ReadBytes(Convert.ToInt32(fs.Length - baseLength - offsetEntryLength[comboBox1.SelectedIndex]));
            fs.Position = 0;

            byte[] allBytes = p1Bytes.Concat(p2Bytes).ToArray();
            fs.Dispose();
            File.WriteAllBytes(file, allBytes);

            open();
            loadList(selectedOffset);
            comboBox1.SelectedIndex = selectedOffset;
            loadList(selectedOffset);
            listBox1.SelectedIndex = selectedEntry - 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                checkBox1.Checked = false;
                disableAll();
                testBox6.Clear();
                testBox2.Text = offsetLengthStr[comboBox1.SelectedIndex];

                fs.Position = offsetStartPosition[comboBox1.SelectedIndex];

                testBox4.Text = string.Format("{0:X2} ", fs.Position);

                loadList(comboBox1.SelectedIndex);
                listBox1.SelectedIndex = 0;
                loadEntry();
            }
        }

        private void exportListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamWriter listWriter = new StreamWriter(Application.StartupPath + "//ExportedList.txt");
            foreach (var item in listBox1.Items) { listWriter.WriteLine(item.ToString()); }
            listWriter.Dispose();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) { save(); }
            sbChange = false; tbChange = false;

            buttonSave.Enabled = true;
            buttonAdd.Enabled = true;
            buttonCopy.Enabled = true;
            buttonRemove.Enabled = true;
            checkBox1.Enabled = true;

            loadEntry();
            label43.Text = listBox1.Text;

            testBox4.Text = string.Format("{0:X2} ", fs.Position);

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    index1Load();
                    break;
                case 1:
                    index2Load();
                    break;
                case 3:
                    index4Load();
                    break;
                case 4:
                    index5Load();
                    break;
                case 5:
                    index6Load();
                    break;
                case 6:
                    index7Load();
                    break;
                case 7:
                    index8Load();
                    break;
                case 8:
                    index9Load();
                    break;
                case 9:
                    index10Load();
                    break;
                case 10:
                    index11Load();
                    break;
                case 11:
                    index12Load();
                    break;
                case 12:
                    index13Load();
                    break;
                case 13:
                    index14Load();
                    break;
                case 14:
                    index15Load();
                    break;
                case 15:
                    index16Load();
                    break;
                case 16:
                    index17Load();
                    break;
                case 17:
                    index18Load();
                    break;
                case 18:
                    index19Load();
                    break;
                case 19:
                    index20Load();
                    break;
                case 20:
                    index21Load();
                    break;
                case 21:
                    index22Load();
                    break;
                case 22:
                    index23Load();
                    break;
                case 23:
                    //index24Load();
                    break;
                case 25:
                    index26Load();
                    break;
                case 27:
                    index28Load();
                    break;
                case 28:
                    index29Load();
                    break;
                case 29:
                    index30Load();
                    break;
                case 32:
                    index33Load();
                    break;
                case 35:
                    index36Load();
                    break;
                case 40:
                    index41Load();
                    break;
                case 41:
                    index42Load();
                    break;
                case 42:
                    index43Load();
                    break;
                case 47:
                    index48Load();
                    break;
                case 48:
                    index49Load();
                    break;
                default: break;
            }
        }

        //Load the selected entry
        private void loadEntry()
        {
            sourceBox.Clear();
            fs.Position = offsetStartPosition[comboBox1.SelectedIndex] + listBox1.SelectedIndex * offsetEntryLength[comboBox1.SelectedIndex];
            entrySource = new string[offsetEntryLength[comboBox1.SelectedIndex]];
            if (offsetEntryLength[comboBox1.SelectedIndex] <= 100 || comboBox1.SelectedIndex == 14)
            {
                for (int i = 0; i < offsetEntryLength[comboBox1.SelectedIndex]; i++)
                {
                    entrySource[i] = string.Format("{0:X2} ", reader.ReadByte());
                    sourceBox.Text += entrySource[i];
                }
            }
            else { sourceBox.Text = "Offset area can't be edited at the moment."; }
            fs.Position = offsetStartPosition[comboBox1.SelectedIndex] + listBox1.SelectedIndex * offsetEntryLength[comboBox1.SelectedIndex];
        }

        private void testBox6_TextChanged(object sender, EventArgs e)
        {

            listBox2.Items.Clear();
            foreach (string entry in listBox1.Items) if (entry.Contains(testBox6.Text) && testBox6.Text != "") { listBox2.Items.Add(entry); }
            listBox2.Sorted = true;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Text = listBox2.Text; //listBox2.Text
        }

        private void loadList(int index)
        {
            string temp = "";
            listBox1.Items.Clear();
            fs.Position = offsetStartPosition[index];
            switch (index)
            {
                case 0:
                    StreamReader sReader1 = new StreamReader(Application.StartupPath + "\\SpellEffects.txt");
                    Hashtable nameList = new Hashtable();
                    string textLine="";
                    do
                    {
                        textLine = sReader1.ReadLine();
                        nameList[textLine.Substring(0, 4).Replace(" ","")] = textLine.Substring(4, textLine.Length - 4);

                    } while (sReader1.Peek() != -1);
                    
                    int key;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 4;
                        key = int.Parse(temp.Substring(6, 2) + temp.Substring(4, 2), NumberStyles.HexNumber);
                        int ID = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = ID.ToString() + " " + nameList[ID.ToString()].ToString();
                        }
                        catch
                        {
                            temp = ID.ToString();
                        }
                        //fs.Position -= 7;
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader1.Close();
                    break;

                case 1:
                    StreamReader sReader2 = new StreamReader(Application.StartupPath + "\\Spells.txt");
                    Hashtable nameList2 = new Hashtable();
                    string textLine2 = "";
                    do
                    {
                        textLine2 = sReader2.ReadLine();
                        nameList2[textLine2.Substring(0, 4).Replace(" ", "")] = textLine2.Substring(4, textLine2.Length - 4);

                    } while (sReader2.Peek() != -1);

                    int key2;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 4;
                        key2 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key2.ToString() + " " + nameList2[key2.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key2.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader2.Close();
                    break;

                case 3:
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            temp += string.Format("{0:X2} ", reader.ReadByte());
                        }
                        fs.Position -= 2;
                        temp = int.Parse(temp.Substring(3, 2) + temp.Substring(0, 2), NumberStyles.HexNumber).ToString();
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    break;

                case 4:
                    StreamReader sReader5 = new StreamReader(Application.StartupPath + "\\Skills.txt");
                    Hashtable nameList5 = new Hashtable();
                    string textLine5 = "";

                    do
                    {
                        textLine5 = sReader5.ReadLine();
                        nameList5[textLine5.Substring(0, 6).Replace(" ", "")] = textLine5.Substring(6, textLine5.Length - 6);

                    } while (sReader5.Peek() != -1);

                    int key5, statsID;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 5;
                        key5 = int.Parse(temp.Substring(8, 2), NumberStyles.HexNumber);
                        statsID = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = statsID.ToString() + "\t " + nameList5[temp.Substring(4, 4)].ToString() + " " + key5.ToString();
                        }
                        catch
                        {
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader5.Close();
                    break;

                case 5:
                    StreamReader sReader6 = new StreamReader(Application.StartupPath + "\\SpellEffects.txt");
                    Hashtable nameList6 = new Hashtable();
                    string textLine6 = "";

                    do
                    {
                        textLine6 = sReader6.ReadLine();
                        nameList6[textLine6.Substring(0, 5).Replace(" ", "")] = textLine6.Substring(5, textLine6.Length - 5);

                    } while (sReader6.Peek() != -1);

                    int key6;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 5;
                        key6 = int.Parse(temp.Substring(8, 2) + temp.Substring(6, 2), NumberStyles.HexNumber);
                        statsID = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = statsID.ToString() + "\t (" + int.Parse(temp.Substring(4, 2), NumberStyles.HexNumber) + ")   " + nameList6[key6.ToString()].ToString();
                        }
                        catch
                        {
                            temp = statsID.ToString() + "\t (" + int.Parse(temp.Substring(4, 2), NumberStyles.HexNumber) + ")   ";
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader6.Close();
                    break;

                case 6:
                    StreamReader sReader7 = new StreamReader(Application.StartupPath + "\\Items.txt");
                    Hashtable nameList7 = new Hashtable();
                    string textLine7 = "";
                    do
                    {
                        textLine7 = sReader7.ReadLine();
                        nameList7[textLine7.Substring(0, 4).Replace(" ", "")] = textLine7.Substring(8, textLine7.Length - 8);

                    } while (sReader7.Peek() != -1);

                    int key7;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 2;
                        key7 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key7.ToString() + " " + nameList7[key7.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key7.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader7.Close();
                    break;

                case 7:
                    StreamReader sReader8 = new StreamReader(Application.StartupPath + "\\Items.txt");
                    Hashtable nameList8 = new Hashtable();
                    string textLine8 = "";
                    do
                    {
                        textLine8 = sReader8.ReadLine();
                        nameList8[textLine8.Substring(0, 4).Replace(" ", "")] = textLine8.Substring(8, textLine8.Length - 8);

                    } while (sReader8.Peek() != -1);

                    int key8;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 3;
                        key8 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key8.ToString() + " " + nameList8[key8.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key8.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader8.Close();
                    break;

                case 8:
                    StreamReader sReader9 = new StreamReader(Application.StartupPath + "\\Items.txt");
                    Hashtable nameList9 = new Hashtable();
                    string textLine9 = "";
                    do
                    {
                        textLine9 = sReader9.ReadLine();
                        nameList9[textLine9.Substring(0, 4).Replace(" ", "")] = textLine9.Substring(8, textLine9.Length - 8);

                    } while (sReader9.Peek() != -1);

                    int key9;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 2;
                        key9 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key9.ToString() + " " + nameList9[key9.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key9.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader9.Close();
                    break;

                case 9:
                    StreamReader sReader10 = new StreamReader(Application.StartupPath + "\\Items.txt");
                    Hashtable nameList10 = new Hashtable();
                    string textLine10 = "";
                    do
                    {
                        textLine10 = sReader10.ReadLine();
                        nameList10[textLine10.Substring(0, 4).Replace(" ", "")] = textLine10.Substring(8, textLine10.Length - 8);

                    } while (sReader10.Peek() != -1);

                    int key10;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 3;
                        key10 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key10.ToString() + " " + nameList10[key10.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key10.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader10.Close();
                    break;

                case 10:
                    StreamReader sReader11 = new StreamReader(Application.StartupPath + "\\Items.txt");
                    Hashtable nameList11 = new Hashtable();
                    string textLine11 = "";
                    do
                    {
                        textLine11 = sReader11.ReadLine();
                        nameList11[textLine11.Substring(0, 4).Replace(" ", "")] = textLine11.Substring(8, textLine11.Length - 8);

                    } while (sReader11.Peek() != -1);

                    int key11;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 3;
                        key11 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key11.ToString() + " " + nameList11[key11.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key11.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader11.Close();
                    break;

                case 11:
                    fs.Position = offsetStartPosition[index];
                    StreamReader sReader12 = new StreamReader(Application.StartupPath + "\\Items.txt");
                    Hashtable nameList12 = new Hashtable();
                    string textLine12 = "";
                    do
                    {
                        textLine12 = sReader12.ReadLine();
                        nameList12[textLine12.Substring(0, 4).Replace(" ", "")] = textLine12.Substring(8, textLine12.Length - 8);

                    } while (sReader12.Peek() != -1);

                    StreamReader sReader12A = new StreamReader(Application.StartupPath + "\\SpellEffects.txt");
                    Hashtable nameList12A = new Hashtable();
                    string textLine12A = "";
                    do
                    {
                        textLine12A = sReader12A.ReadLine();
                        nameList12A[textLine12A.Substring(0, 4).Replace(" ", "")] = textLine12A.Substring(4, textLine12A.Length - 4);

                    } while (sReader12A.Peek() != -1);

                    int key12, key12A;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 5;
                        key12 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        key12A = int.Parse(temp.Substring(8, 2) + temp.Substring(6, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key12.ToString() + " " + nameList12[key12.ToString()].ToString() + " " + nameList12A[key12A.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key12.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader12.Close();
                    sReader12A.Close();
                    break;

                case 12:
                    StreamReader sReader13 = new StreamReader(Application.StartupPath + "\\Items.txt");
                    Hashtable nameList13 = new Hashtable();
                    string textLine13 = "";
                    do
                    {
                        textLine13 = sReader13.ReadLine();
                        nameList13[textLine13.Substring(0, 4).Replace(" ", "")] = textLine13.Substring(8, textLine13.Length - 8);

                    } while (sReader13.Peek() != -1);

                    int key13;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 2;
                        key13 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key13.ToString() + " " + nameList13[key13.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key13.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader13.Close();
                    break;

                case 13:
                    StreamReader sReader14 = new StreamReader(Application.StartupPath + "\\Items.txt");
                    Hashtable nameList14 = new Hashtable();
                    string textLine14 = "";
                    do
                    {
                        textLine14 = sReader14.ReadLine();
                        nameList14[textLine14.Substring(0, 4).Replace(" ", "")] = textLine14.Substring(8, textLine14.Length - 8);

                    } while (sReader14.Peek() != -1);

                    int key14;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 2;
                        key14 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key14.ToString() + " " + nameList14[key14.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key14.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader14.Close();
                    break;

                case 14:
                    string tempByte;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        temp += string.Format("{0:X2}", reader.ReadByte()) + string.Format("{0:X2}", reader.ReadByte());
                        temp = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber).ToString()+" ";
                        fs.Position -= 2;

                        fs.Position += 53;
                        for (int j = 0; j <= offsetEntryLength[index]-54; j++)
                        {
                            tempByte = string.Format("{0:X2}", reader.ReadByte());
                            if (tempByte != "00") { temp += Convert.ToChar(int.Parse(tempByte, NumberStyles.HexNumber)); }
                        }
                        //fs.Position -= 3;


                        listBox1.Items.Add(temp);
                        temp = "";
                        //fs.Position += offsetEntryLength[index]-512;
                    }
                    break;

                case 15:

                    StreamReader sReader = new StreamReader(Application.StartupPath + "\\langID.txt");
                    string langID = sReader.ReadToEnd();
                    sReader.Dispose();

                    StreamReader sReader16 = new StreamReader(Application.StartupPath + "\\Names.txt");
                    Hashtable nameList16 = new Hashtable();
                    string textLine16 = "";
                    do
                    {
                        textLine16 = sReader16.ReadLine();
                        nameList16[textLine16.Substring(0, 12).Replace(" ", "")] = textLine16.Substring(12, textLine16.Length - 12);

                    } while (sReader16.Peek() != -1);

                    int key16, nameID;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 9;
                        key16 = int.Parse(temp.Substring(0, 2), NumberStyles.HexNumber);
                        nameID = int.Parse(temp.Substring(16, 2) + temp.Substring(14, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key16.ToString() + " " + nameList16[langID + nameID.ToString()].ToString();
                        }
                        catch
                        {
                            if (langID == "00") { temp = key16.ToString() + " " + nameList16["01" + nameID.ToString()].ToString(); }
                            if (langID != "00") { temp = key16.ToString() + " " + nameList16["00" + nameID.ToString()].ToString(); }
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader16.Close();
                    break;

                case 16:
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        temp = int.Parse(string.Format("{0:X2}", reader.ReadByte()), NumberStyles.HexNumber).ToString();
                        fs.Position -= 1;
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    break;

                case 17:
                    StreamReader sReader18 = new StreamReader(Application.StartupPath + "\\Units.txt");
                    Hashtable nameList18 = new Hashtable();
                    string textLine18 = "";
                    do
                    {
                        textLine18 = sReader18.ReadLine();
                        nameList18[textLine18.Substring(0, 4).Replace(" ", "")] = textLine18.Substring(8, textLine18.Length - 8);

                    } while (sReader18.Peek() != -1);

                    int key18;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 6;
                        key18 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key18.ToString() + " " + nameList18[key18.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key18.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader18.Close();
                    break;

                case 18:
                    string[] slot = new string[7] { "Helmet", "Right hand", "Chest", "Left hand", "Right ring", "Legs", "Left ring" };
                    StreamReader sReader19 = new StreamReader(Application.StartupPath + "\\Units.txt");
                    Hashtable nameList19 = new Hashtable();
                    string textLine19="";
                    do
                    {
                        textLine19 = sReader19.ReadLine();
                        nameList19[textLine19.Substring(0, 4).Replace(" ","")] = textLine19.Substring(8, textLine19.Length - 8);

                    } while (sReader19.Peek() != -1);
                    
                    int key19, e;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 3;
                        key19 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        e = int.Parse(temp.Substring(4, 2),NumberStyles.HexNumber);
                        try
                        {
                            if (e > 6) { temp = key19.ToString() + " " + nameList19[key19.ToString()].ToString(); }
                            else { temp = key19.ToString() + " " + nameList19[key19.ToString()].ToString() + " " + slot[e]; }
                        }
                        catch
                        {
                            temp = key19.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader19.Close();
                    break;

                case 19:
                    fs.Position = offsetStartPosition[index];
                    StreamReader sReader20 = new StreamReader(Application.StartupPath + "\\Units.txt");
                    Hashtable nameList20 = new Hashtable();
                    string textLine20 = "";
                    do
                    {
                        textLine20 = sReader20.ReadLine();
                        nameList20[textLine20.Substring(0, 4).Replace(" ", "")] = textLine20.Substring(8, textLine20.Length - 8);

                    } while (sReader20.Peek() != -1);

                    StreamReader sReader20A = new StreamReader(Application.StartupPath + "\\SpellEffects.txt");
                    Hashtable nameList20A = new Hashtable();
                    string textLine20A = "";
                    do
                    {
                        textLine20A = sReader20A.ReadLine();
                        nameList20A[textLine20A.Substring(0, 4).Replace(" ", "")] = textLine20A.Substring(4, textLine20A.Length - 4);

                    } while (sReader20A.Peek() != -1);

                    int key20, key20A;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 5;
                        key20 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        key20A = int.Parse(temp.Substring(8, 2) + temp.Substring(6, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key20.ToString() + " " + nameList20[key20.ToString()].ToString() + " " + nameList20A[key20A.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key20.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader20.Close();
                    sReader20A.Close();
                    break;

                case 20:
                    fs.Position = offsetStartPosition[index];
                    StreamReader sReader21 = new StreamReader(Application.StartupPath + "\\Units.txt");
                    Hashtable nameList21 = new Hashtable();
                    string textLine21 = "";
                    do
                    {
                        textLine21 = sReader21.ReadLine();
                        nameList21[textLine21.Substring(0, 4).Replace(" ", "")] = textLine21.Substring(8, textLine21.Length - 8);

                    } while (sReader21.Peek() != -1);

                    int key21;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 3;
                        key21 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key21.ToString() + " " + nameList21[key21.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key21.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader21.Close();
                    break;

                case 21:
                    StreamReader sReader22 = new StreamReader(Application.StartupPath + "\\Items.txt");
                    Hashtable nameList22 = new Hashtable();
                    string textLine22 = "";
                    do
                    {
                        textLine22 = sReader22.ReadLine();
                        nameList22[textLine22.Substring(0, 4).Replace(" ", "")] = textLine22.Substring(8, textLine22.Length - 8);

                    } while (sReader22.Peek() != -1);

                    StreamReader sReader22A = new StreamReader(Application.StartupPath + "\\Units.txt");
                    Hashtable nameList22A = new Hashtable();
                    string textLine22A = "";
                    do
                    {
                        textLine22A = sReader22A.ReadLine();
                        nameList22A[textLine22A.Substring(0, 4).Replace(" ", "")] = textLine22A.Substring(8, textLine22A.Length - 8);

                    } while (sReader22A.Peek() != -1);

                    int item1, item2, item3, unitID, lootSlot;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 11; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 11;
                        unitID = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        lootSlot = int.Parse(temp.Substring(4, 2), NumberStyles.HexNumber);
                        item1 = int.Parse(temp.Substring(8, 2) + temp.Substring(6, 2), NumberStyles.HexNumber);
                        item2 = int.Parse(temp.Substring(14, 2) + temp.Substring(12, 2), NumberStyles.HexNumber);
                        item3 = int.Parse(temp.Substring(20, 2) + temp.Substring(18, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = nameList22A[unitID.ToString()].ToString() + " Slot " + lootSlot.ToString() + ": " + nameList22[item1.ToString()].ToString();// + " | " + nameList22[item2.ToString()].ToString() + " | " + nameList22[item3.ToString()].ToString() + " | ";
                            if (item2 != 0) { temp += " | " + nameList22[item2.ToString()].ToString(); }
                            if (item3 != 0) { temp += " | " + nameList22[item3.ToString()].ToString(); }
                        }
                        catch
                        {
                            temp = unitID.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader22.Close();
                    break;

                case 22:
                    StreamReader sReader23 = new StreamReader(Application.StartupPath + "\\Units.txt");
                    Hashtable nameList23 = new Hashtable();
                    string textLine23 = "";
                    do
                    {
                        textLine23 = sReader23.ReadLine();
                        nameList23[textLine23.Substring(0, 8).Replace(" ", "")] = textLine23.Substring(8, textLine23.Length - 8);

                    } while (sReader23.Peek() != -1);

                    StreamReader sReader23A = new StreamReader(Application.StartupPath + "\\Buildings.txt");
                    Hashtable nameList23A = new Hashtable();
                    string textLine23A = "";
                    do
                    {
                        textLine23A = sReader23A.ReadLine();
                        nameList23A[textLine23A.Substring(0, 4).Replace(" ", "")] = textLine23A.Substring(4, textLine23A.Length - 4);

                    } while (sReader23A.Peek() != -1);

                    int unitID23, buildingID23;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        //fs.Position -= 11;
                        unitID23 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        buildingID23 = int.Parse(temp.Substring(8, 2) + temp.Substring(6, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = unitID23.ToString() + " " + nameList23[unitID23.ToString()].ToString() + " | " + buildingID23.ToString() + " " + nameList23A[buildingID23.ToString()].ToString();
                        }
                        catch
                        {
                            temp = unitID23.ToString() + " | " + buildingID23.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        //fs.Position += offsetEntryLength[index];
                    }
                    sReader23.Close();
                    break;

                case 23:
                    fs.Position = offsetStartPosition[index];
                    StreamReader sReader24 = new StreamReader(Application.StartupPath + "\\Buildings.txt");
                    Hashtable nameList24 = new Hashtable();
                    string textLine24 = "";
                    do
                    {
                        textLine24 = sReader24.ReadLine();
                        nameList24[textLine24.Substring(0, 4).Replace(" ", "")] = textLine24.Substring(4, textLine24.Length - 4);

                    } while (sReader24.Peek() != -1);

                    int key24;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 2;
                        key24 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key24.ToString() + " " + nameList24[key24.ToString()].ToString();
                        }
                        catch
                        {
                            temp = key24.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader24.Close();
                    break;

                case 25:
                    fs.Position = offsetStartPosition[index];
                    StreamReader sReader26 = new StreamReader(Application.StartupPath + "\\Buildings.txt");
                    Hashtable nameList26 = new Hashtable();
                    string textLine26 = "";
                    do
                    {
                        textLine26 = sReader26.ReadLine();
                        nameList26[textLine26.Substring(0, 4).Replace(" ", "")] = textLine26.Substring(4, textLine26.Length - 4);

                    } while (sReader26.Peek() != -1);

                    StreamReader sReader26A = new StreamReader(Application.StartupPath + "\\Resources.txt");
                    Hashtable nameList26A = new Hashtable();
                    string textLine26A = "";
                    do
                    {
                        textLine26A = sReader26A.ReadLine();
                        nameList26A[textLine26A.Substring(0, 3).Replace(" ", "")] = textLine26A.Substring(3, textLine26A.Length - 3);

                    } while (sReader26A.Peek() != -1);

                    int key26;
                    string key26A;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        //fs.Position -= 2;
                        key26 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        key26A = temp.Substring(4, 2);
                        try
                        {
                            temp = key26.ToString() + " " + nameList26[key26.ToString()].ToString() + " " + nameList26A[key26A].ToString();
                        }
                        catch
                        {
                            temp = key26.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        //fs.Position += offsetEntryLength[index];
                    }
                    sReader26.Close();
                    break;

                case 27:
                    StreamReader sReader28 = new StreamReader(Application.StartupPath + "\\Skills.txt");
                    Hashtable nameList28 = new Hashtable();
                    string textLine28 = "";
                    /*do
                    {
                        textLine28 = sReader28.ReadLine();
                        nameList28[textLine28.Substring(0, 3).Replace(" ", "")] = textLine28.Substring(3, textLine28.Length - 3);

                    } while (sReader28.Peek() != -1);*/

                    for(int i = 0; i < 8; i++)
                    {
                        textLine28 = sReader28.ReadLine();
                        nameList28[textLine28.Substring(0, 3).Replace(" ", "")] = textLine28.Substring(6, textLine28.Length - 6);
                    }

                    int key28;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 2;
                        key28 = int.Parse(temp.Substring(2,2), NumberStyles.HexNumber);
                        try
                        {
                            temp = nameList28[temp.Substring(0, 2)].ToString() + " Level " + key28.ToString();
                        }
                        catch
                        {
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader28.Close();
                    break;

                case 28:
                    StreamReader sReader29 = new StreamReader(Application.StartupPath + "\\Units.txt");
                    Hashtable nameList29 = new Hashtable();

                    string textLine29 = "";
                    do
                    {
                        textLine29 = sReader29.ReadLine();
                        nameList29[textLine29.Substring(0, 4).Replace(" ", "")] = textLine29.Substring(8, textLine29.Length - 8);

                    } while (sReader29.Peek() != -1);

                    int key29;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 4;
                        key29 = int.Parse(temp.Substring(6, 2) + temp.Substring(4, 2), NumberStyles.HexNumber);
                        //temp = key29.ToString() + " " + nameList29[key29.ToString()].ToString() + " " + temp.Substring(0, 2) + " " + temp.Substring(2, 2);
                        try
                        {
                            temp = key29.ToString() + " " + nameList29[key29.ToString()].ToString() + " " + int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        }
                        catch
                        {
                            temp= key29.ToString() + " " + int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader29.Close();
                    break;

                case 29:
                    StreamReader sReader30 = new StreamReader(Application.StartupPath + "\\Merchants.txt");
                    Hashtable nameList30 = new Hashtable();
                    string textLine30 = "";
                    do
                    {
                        textLine30 = sReader30.ReadLine();
                        nameList30[textLine30.Substring(0, 4).Replace(" ", "")] = textLine30.Substring(4, textLine30.Length - 4);

                    } while (sReader30.Peek() != -1);

                    StreamReader sReader30A = new StreamReader(Application.StartupPath + "\\Items.txt");
                    Hashtable nameList30A = new Hashtable();
                    string textLine30A = "";
                    do
                    {
                        textLine30A = sReader30A.ReadLine();
                        nameList30A[textLine30A.Substring(0, 4).Replace(" ", "")] = textLine30A.Substring(4, textLine30A.Length - 4);

                    } while (sReader30A.Peek() != -1);

                    int key30, key30A;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 4;
                        key30 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        key30A = int.Parse(temp.Substring(6, 2) + temp.Substring(4, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber).ToString() + " " + nameList30[key30.ToString()].ToString();
                            temp = key30.ToString() + " " + nameList30[key30.ToString()].ToString() + " " + nameList30A[key30A.ToString()].ToString();
                        }
                        catch
                        {
                            temp = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber).ToString();
                            temp = key30.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    break;

                case 32:
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        temp = "Level " + int.Parse(string.Format("{0:X2}", reader.ReadByte()), NumberStyles.HexNumber).ToString();
                        fs.Position -= 1;
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    break;

                case 35:
                    StreamReader sReader36 = new StreamReader(Application.StartupPath + "\\Items.txt");
                    Hashtable nameList36 = new Hashtable();
                    string textLine36 = "";
                    do
                    {
                        textLine36 = sReader36.ReadLine();
                        nameList36[textLine36.Substring(0, 4).Replace(" ", "")] = textLine36.Substring(8, textLine36.Length - 8);

                    } while (sReader36.Peek() != -1);

                    StreamReader sReader36A = new StreamReader(Application.StartupPath + "\\Objects.txt");
                    Hashtable nameList36A = new Hashtable();
                    string textLine36A = "";
                    do
                    {
                        textLine36A = sReader36A.ReadLine();
                        try
                        {
                            nameList36A[textLine36A.Substring(0, 5).Replace(" ", "")] = textLine36A.Substring(6, textLine36A.Length - 6);
                        }
                        catch
                        {
                            nameList36A[textLine36A.Substring(0, textLine36A.Length).Replace(" ", "")] = "No text";
                        }

                    } while (sReader36A.Peek() != -1);

                    //int item1, item2, item3, unitID, lootSlot;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 11; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 11;
                        unitID = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        lootSlot = int.Parse(temp.Substring(4, 2), NumberStyles.HexNumber);
                        item1 = int.Parse(temp.Substring(8, 2) + temp.Substring(6, 2), NumberStyles.HexNumber);
                        item2 = int.Parse(temp.Substring(14, 2) + temp.Substring(12, 2), NumberStyles.HexNumber);
                        item3 = int.Parse(temp.Substring(20, 2) + temp.Substring(18, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = nameList36A[unitID.ToString()].ToString() + " Slot " + lootSlot.ToString() + ": " + nameList36[item1.ToString()].ToString();// + " | " + nameList22[item2.ToString()].ToString() + " | " + nameList22[item3.ToString()].ToString() + " | ";
                            if (item2 != 0) { temp += " | " + nameList36[item2.ToString()].ToString(); }
                            if (item3 != 0) { temp += " | " + nameList36[item3.ToString()].ToString(); }
                        }
                        catch
                        {
                            temp = unitID.ToString();
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader36.Close();
                    break;

                case 40:

                    StreamReader tempReader41 = new StreamReader(Application.StartupPath + "\\langID.txt");
                    string langID41 = tempReader41.ReadToEnd();
                    tempReader41.Dispose();

                    StreamReader sReader41 = new StreamReader(Application.StartupPath + "\\Names.txt");
                    Hashtable nameList41 = new Hashtable();
                    string textLine41 = "";
                    do
                    {
                        textLine41 = sReader41.ReadLine();
                        nameList41[textLine41.Substring(0, 12).Replace(" ", "")] = textLine41.Substring(12, textLine41.Length - 12);

                    } while (sReader41.Peek() != -1);

                    int key41, nameID41;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        //fs.Position -= 9;
                        key41 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        nameID41 = int.Parse(temp.Substring(6, 2) + temp.Substring(4, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key41.ToString() + " " + nameList41[langID41 + nameID41.ToString()].ToString();
                        }
                        catch (System.NullReferenceException)
                        {
                            temp = key41.ToString();
                        }
                        catch
                        {
                            if (langID41 == "00") { temp = key41.ToString() + " " + nameList41["01" + nameID41.ToString()].ToString(); }
                            if (langID41 != "00") { temp = key41.ToString() + " " + nameList41["00" + nameID41.ToString()].ToString(); }
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        //fs.Position += offsetEntryLength[index];
                    }
                    sReader41.Close();
                    break;

                case 41:

                    StreamReader tempReader = new StreamReader(Application.StartupPath + "\\langID.txt");
                    string langID42 = tempReader.ReadToEnd();
                    tempReader.Dispose();

                    StreamReader sReader42 = new StreamReader(Application.StartupPath + "\\Names.txt");
                    Hashtable nameList42 = new Hashtable();
                    string textLine42 = "";
                    do
                    {
                        textLine42 = sReader42.ReadLine();
                        nameList42[textLine42.Substring(0, 12).Replace(" ", "")] = textLine42.Substring(12, textLine42.Length - 12);

                    } while (sReader42.Peek() != -1);

                    int key42, nameID42A, nameID42B;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        //fs.Position -= 9;
                        key42 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        nameID42A = int.Parse(temp.Substring(6, 2) + temp.Substring(4, 2), NumberStyles.HexNumber);
                        nameID42B = int.Parse(temp.Substring(10, 2) + temp.Substring(8, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key42.ToString() + " " + nameList42[langID42 + nameID42A.ToString()].ToString() + " | " + nameList42[langID42 + nameID42B.ToString()].ToString();
                        }
                        catch (System.NullReferenceException)
                        {
                            temp = key42.ToString();
                        }
                        catch
                        {
                            if (langID42 == "00") { temp = key42.ToString() + " " + nameList42["01" + nameID42A.ToString()].ToString() + " | " + nameList42["01" + nameID42B.ToString()].ToString(); }
                            if (langID42 != "00") { temp = key42.ToString() + " " + nameList42["00" + nameID42A.ToString()].ToString() + " | " + nameList42["00" + nameID42B.ToString()].ToString(); }
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        //fs.Position += offsetEntryLength[index];
                    }
                    sReader42.Close();
                    break;

                case 42:

                    StreamReader tempReader43 = new StreamReader(Application.StartupPath + "\\langID.txt");
                    string langID43 = tempReader43.ReadToEnd();
                    tempReader43.Dispose();

                    StreamReader sReader43 = new StreamReader(Application.StartupPath + "\\Names.txt");
                    Hashtable nameList43 = new Hashtable();
                    string textLine43 = "";
                    do
                    {
                        textLine43 = sReader43.ReadLine();
                        nameList43[textLine43.Substring(0, 12).Replace(" ", "")] = textLine43.Substring(12, textLine43.Length - 12);

                    } while (sReader43.Peek() != -1);

                    int key43, nameID43;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 17; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        //fs.Position -= 9;
                        key43 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        nameID43 = int.Parse(temp.Substring(20, 2) + temp.Substring(18, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key43.ToString() + " " + nameList43[langID43 + nameID43.ToString()].ToString();
                        }
                        catch (System.NullReferenceException)
                        {
                            temp = key43.ToString();
                        }
                        catch
                        {
                            if (langID43 == "00") { temp = key43.ToString() + " " + nameList43["01" + nameID43.ToString()].ToString(); }
                            if (langID43 != "00") { temp = key43.ToString() + " " + nameList43["00" + nameID43.ToString()].ToString(); }
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        //fs.Position += offsetEntryLength[index];
                    }
                    sReader43.Close();
                    break;

                case 47:

                    StreamReader tempReader2 = new StreamReader(Application.StartupPath + "\\langID.txt");
                    string langID48 = tempReader2.ReadToEnd();
                    tempReader2.Dispose();

                    StreamReader sReader48 = new StreamReader(Application.StartupPath + "\\Names.txt");
                    Hashtable nameList48 = new Hashtable();
                    string textLine48 = "";
                    do
                    {
                        textLine48 = sReader48.ReadLine();
                        nameList48[textLine48.Substring(0, 12).Replace(" ", "")] = textLine48.Substring(12, textLine48.Length - 12);

                    } while (sReader48.Peek() != -1);

                    int key48, nameID48;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        fs.Position -= 6;
                        key48 = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber);
                        nameID48 = int.Parse(temp.Substring(10, 2) + temp.Substring(8, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key48.ToString() + " " + nameList48[langID48 + nameID48.ToString()].ToString();
                        }
                        catch (System.NullReferenceException)
                        {
                            temp = key48.ToString();
                        }
                        catch
                        {
                            if (langID48 == "00") { temp = key48.ToString() + " " + nameList48["01" + nameID48.ToString()].ToString(); }
                            if (langID48 != "00") { temp = key48.ToString() + " " + nameList48["00" + nameID48.ToString()].ToString(); }
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    sReader48.Close();
                    break;

                case 48:

                    StreamReader tempReader3 = new StreamReader(Application.StartupPath + "\\langID.txt");
                    string langID49 = tempReader3.ReadToEnd();
                    tempReader3.Dispose();

                    StreamReader sReader49 = new StreamReader(Application.StartupPath + "\\Names.txt");
                    Hashtable nameList49 = new Hashtable();
                    string textLine49 = "";
                    do
                    {
                        textLine49 = sReader49.ReadLine();
                        nameList49[textLine49.Substring(0, 12).Replace(" ", "")] = textLine49.Substring(12, textLine49.Length - 12);

                    } while (sReader49.Peek() != -1);

                    int key49, nameID49;
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        //fs.Position -= 6;
                        key49 = int.Parse(temp.Substring(0, 2), NumberStyles.HexNumber);
                        nameID49 = int.Parse(temp.Substring(4, 2) + temp.Substring(2, 2), NumberStyles.HexNumber);
                        try
                        {
                            temp = key49.ToString() + " " + nameList49[langID49 + nameID49.ToString()].ToString();
                        }
                        catch (System.NullReferenceException)
                        {
                            temp = key49.ToString();
                        }
                        catch
                        {
                            if (langID49 == "00") { temp = key49.ToString() + " " + nameList49["01" + nameID49.ToString()].ToString(); }
                            if (langID49 != "00") { temp = key49.ToString() + " " + nameList49["00" + nameID49.ToString()].ToString(); }
                        }
                        listBox1.Items.Add(temp);
                        temp = "";
                        //fs.Position += offsetEntryLength[index];
                    }
                    sReader49.Close();
                    break;

                default:
                    for (int i = 0; i < (offsetLength[index] / offsetEntryLength[index]); i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            temp += string.Format("{0:X2}", reader.ReadByte());
                        }
                        temp = int.Parse(temp.Substring(2, 2) + temp.Substring(0, 2), NumberStyles.HexNumber).ToString();
                        fs.Position -= 2;
                        listBox1.Items.Add(temp);
                        temp = "";
                        fs.Position += offsetEntryLength[index];
                    }
                    break;
            }
        }

        private void reloadUnitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamReader sReader = new StreamReader(Application.StartupPath + "\\langID.txt");
            string langID = sReader.ReadToEnd();

            StreamWriter writer = new StreamWriter(Application.StartupPath + "\\Names.txt");

            reader = new BinaryReader(fs);
            string[] nameSource = new string[offsetEntryLength[14]];
            string[] temp = new string[offsetLength[14] / offsetEntryLength[14]];
            fs.Position = offsetStartPosition[14];
            for (int j = 0; j < offsetLength[14] / offsetEntryLength[14]; j++)
            {

                Array.Clear(nameSource, 0, nameSource.Length);
                
                for (int i = 0; i < offsetEntryLength[14]; i++)
                {
                    nameSource[i] = string.Format("{0:X2}", reader.ReadByte());
                }
                temp[j] = int.Parse(nameSource[1] + nameSource[0], NumberStyles.HexNumber).ToString() + "    ";
                if (int.Parse(temp[j]) < 10) { temp[j] += " "; }
                if (int.Parse(temp[j]) < 100) { temp[j] += " "; }
                if (int.Parse(temp[j]) < 1000) { temp[j] += " "; }
                if (int.Parse(temp[j]) < 10000) { temp[j] += " "; }

                for (int i = 54; i < offsetEntryLength[14] - 54; i++)
                {
                    if (nameSource[i] != "00") { temp[j] += Convert.ToChar(int.Parse(nameSource[i], NumberStyles.HexNumber)); }
                }
                writer.WriteLine(nameSource[2]+" "+temp[j]);
                writer.Flush();
            }
            writer.Close();
            //Break
            sReader = new StreamReader(Application.StartupPath + "\\Names.txt");
            Hashtable nameList = new Hashtable();
            string textLine = "";
            do
            {
                textLine = sReader.ReadLine();
                nameList[textLine.Substring(0, 12).Replace(" ", "")] = textLine.Substring(12, textLine.Length - 12);

            } while (sReader.Peek() != -1);

            writer = new StreamWriter(Application.StartupPath + "\\Units.txt");

            fs.Position = offsetStartPosition[17];

            nameSource = new string[offsetEntryLength[17]];
            string tempLine="";
            int key, unitID;
            for (int i = 0; i < offsetLength[17] / offsetEntryLength[17]; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    tempLine += string.Format("{0:X2}", reader.ReadByte());
                }
                fs.Position -= 6;
                key = int.Parse(tempLine.Substring(6, 2) + tempLine.Substring(4, 2), NumberStyles.HexNumber);
                unitID = int.Parse(tempLine.Substring(2, 2) + tempLine.Substring(0, 2), NumberStyles.HexNumber);
                tempLine = unitID + "   ";
                if (unitID < 10) { tempLine += " "; }
                if (unitID < 100) { tempLine += " "; }
                if (unitID < 1000) { tempLine += " "; }
                if (unitID < 10000) { tempLine += " "; }
                try
                {
                    tempLine += nameList[langID + key.ToString()].ToString();
                }
                catch
                {
                    tempLine += nameList["00" + key.ToString()].ToString();
                }
                writer.WriteLine(tempLine);
                writer.Flush();
                tempLine = "";
                fs.Position += offsetEntryLength[17];
            }
            writer.Close();
            //Break
            //Buildings
            sReader = new StreamReader(Application.StartupPath + "\\Names.txt");
            nameList = new Hashtable();
            textLine = "";
            do
            {
                textLine = sReader.ReadLine();
                nameList[textLine.Substring(0, 12).Replace(" ", "")] = textLine.Substring(12, textLine.Length - 12);

            } while (sReader.Peek() != -1);

            writer = new StreamWriter(Application.StartupPath + "\\Buildings.txt");

            fs.Position = offsetStartPosition[23];

            nameSource = new string[offsetEntryLength[23]];
            tempLine = ""; //langID = "01";
            //int key, unitID;
            for (int i = 0; i < offsetLength[23] / offsetEntryLength[23]; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    tempLine += string.Format("{0:X2}", reader.ReadByte());
                }
                fs.Position -= 9;
                key = int.Parse(tempLine.Substring(16, 2) + tempLine.Substring(14, 2), NumberStyles.HexNumber);
                unitID = int.Parse(tempLine.Substring(2, 2) + tempLine.Substring(0, 2), NumberStyles.HexNumber);
                tempLine = unitID.ToString();
                if (unitID < 10) { tempLine += " "; }
                if (unitID < 100) { tempLine += " "; }
                if (unitID < 1000) { tempLine += " "; }
                //if (unitID < 10000) { tempLine += " "; }
                try
                {
                    tempLine += nameList[langID + key.ToString()].ToString();
                }
                catch
                {
                    tempLine += nameList["00" + key.ToString()].ToString();
                }
                writer.WriteLine(tempLine);
                writer.Flush();
                tempLine = "";
                fs.Position += offsetEntryLength[23];
            }
            writer.Close();
            //Buildings
            sReader = new StreamReader(Application.StartupPath + "\\Names.txt");
            nameList = new Hashtable();
            textLine = "";
            do
            {
                textLine = sReader.ReadLine();
                nameList[textLine.Substring(0, 12).Replace(" ", "")] = textLine.Substring(12, textLine.Length - 12);

            } while (sReader.Peek() != -1);

            writer = new StreamWriter(Application.StartupPath + "\\Spells.txt");

            fs.Position = offsetStartPosition[1];

            nameSource = new string[offsetEntryLength[1]];
            tempLine = ""; //langID = "01";
            //int key, unitID;
            for (int i = 0; i < offsetLength[1] / offsetEntryLength[1]; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    tempLine += string.Format("{0:X2}", reader.ReadByte());
                }
                fs.Position -= 6;
                key = int.Parse(tempLine.Substring(6, 2) + tempLine.Substring(4, 2), NumberStyles.HexNumber);
                unitID = int.Parse(tempLine.Substring(2, 2) + tempLine.Substring(0, 2), NumberStyles.HexNumber);
                tempLine = unitID.ToString();
                if (unitID < 10) { tempLine += " "; }
                if (unitID < 100) { tempLine += " "; }
                if (unitID < 1000) { tempLine += " "; }
                //if (unitID < 10000) { tempLine += " "; }
                try
                {
                    tempLine += nameList[langID + key.ToString()].ToString();
                }
                catch
                {
                    tempLine += nameList["00" + key.ToString()].ToString();
                }
                writer.WriteLine(tempLine);
                writer.Flush();
                tempLine = "";
                fs.Position += offsetEntryLength[1];
            }
            writer.Close();
            //Break
            sReader = new StreamReader(Application.StartupPath + "\\Spells.txt");
            nameList = new Hashtable();
            textLine = "";
            do
            {
                textLine = sReader.ReadLine();
                nameList[textLine.Substring(0, 4).Replace(" ", "")] = textLine.Substring(4, textLine.Length - 4);

            } while (sReader.Peek() != -1);

            writer = new StreamWriter(Application.StartupPath + "\\SpellEffects.txt");

            fs.Position = offsetStartPosition[0];

            nameSource = new string[offsetEntryLength[0]];
            tempLine = ""; //langID = "01";
            //int key, unitID;
            for (int i = 0; i < offsetLength[0] / offsetEntryLength[0]; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    tempLine += string.Format("{0:X2}", reader.ReadByte());
                }
                fs.Position -= 7;
                key = int.Parse(tempLine.Substring(6, 2) + tempLine.Substring(4, 2), NumberStyles.HexNumber);
                unitID = int.Parse(tempLine.Substring(2, 2) + tempLine.Substring(0, 2), NumberStyles.HexNumber);
                int level = int.Parse(tempLine.Substring(12, 2),NumberStyles.HexNumber);
                tempLine = unitID.ToString();
                if (unitID < 10) { tempLine += " "; }
                if (unitID < 100) { tempLine += " "; }
                if (unitID < 1000) { tempLine += " "; }
                if (unitID < 10000) { tempLine += " "; }
                try
                {
                    tempLine += nameList[key.ToString()].ToString() + " Level " + level.ToString();
                }
                catch
                {
                    tempLine += nameList[key.ToString()].ToString() + " Level " + level.ToString();
                }
                writer.WriteLine(tempLine);
                writer.Flush();
                tempLine = "";
                fs.Position += offsetEntryLength[0];
            }
            writer.Close();
            //Break
            sReader = new StreamReader(Application.StartupPath + "\\Names.txt");
            nameList = new Hashtable();
            textLine = "";
            do
            {
                textLine = sReader.ReadLine();
                nameList[textLine.Substring(0, 12).Replace(" ", "")] = textLine.Substring(12, textLine.Length - 12);

            } while (sReader.Peek() != -1);

            writer = new StreamWriter(Application.StartupPath + "\\Items.txt");

            fs.Position = offsetStartPosition[6];

            nameSource = new string[offsetEntryLength[6]];
            tempLine = ""; //langID = "01";
            //int key, unitID;
            for (int i = 0; i < offsetLength[6] / offsetEntryLength[6]; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    tempLine += string.Format("{0:X2}", reader.ReadByte());
                }
                fs.Position -= 6;
                key = int.Parse(tempLine.Substring(10, 2) + tempLine.Substring(8, 2), NumberStyles.HexNumber);
                unitID = int.Parse(tempLine.Substring(2, 2) + tempLine.Substring(0, 2), NumberStyles.HexNumber);
                tempLine = unitID + "   ";
                if (unitID < 10) { tempLine += " "; }
                if (unitID < 100) { tempLine += " "; }
                if (unitID < 1000) { tempLine += " "; }
                if (unitID < 10000) { tempLine += " "; }
                try
                {
                    tempLine += nameList[langID + key.ToString()].ToString();
                }
                catch
                {
                    if (langID == "00") { tempLine += nameList["01" + key.ToString()].ToString(); }
                    if (langID == "01") { tempLine += nameList["00" + key.ToString()].ToString(); }
                }
                writer.WriteLine(tempLine);
                writer.Flush();
                tempLine = "";
                fs.Position += offsetEntryLength[6];
            }
            writer.Close();
            //Break
            sReader = new StreamReader(Application.StartupPath + "\\Units.txt");
            nameList = new Hashtable();
            textLine = "";
            do
            {
                textLine = sReader.ReadLine();
                nameList[textLine.Substring(0, 8).Replace(" ", "")] = textLine.Substring(8, textLine.Length - 8);

            } while (sReader.Peek() != -1);

            writer = new StreamWriter(Application.StartupPath + "\\Merchants.txt");

            fs.Position = offsetStartPosition[28];

            nameSource = new string[offsetEntryLength[28]];
            tempLine = ""; //langID = "01";
            //int key, unitID;
            for (int i = 0; i < offsetLength[28] / offsetEntryLength[28]; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    tempLine += string.Format("{0:X2}", reader.ReadByte());
                }
                fs.Position -= 4;
                key = int.Parse(tempLine.Substring(2, 2) + tempLine.Substring(0, 2), NumberStyles.HexNumber);
                unitID = int.Parse(tempLine.Substring(6, 2) + tempLine.Substring(4, 2), NumberStyles.HexNumber);
                tempLine = key.ToString();
                if (key < 10) { tempLine += " "; }
                if (key < 100) { tempLine += " "; }
                if (key < 1000) { tempLine += " "; }
                //if (unitID < 10000) { tempLine += " "; }

                    tempLine += unitID + " " + nameList[unitID.ToString()].ToString();

                writer.WriteLine(tempLine);
                writer.Flush();
                tempLine = "";
                fs.Position += offsetEntryLength[28];
            }
            writer.Close();
            //Chests
            writer = new StreamWriter(Application.StartupPath + "\\Objects.txt");

            fs.Position = offsetStartPosition[33];
            string writeLine="";
            tempLine = "";
            for (int i = 0; i < offsetLength[33] / offsetEntryLength[33]; i++)
            {
                for (int j = 0; j < offsetEntryLength[33]; j++)
                {
                    tempLine += string.Format("{0:X2}", reader.ReadByte());
                }
                //fs.Position -= offsetEntryLength[33];
                key = int.Parse(tempLine.Substring(2, 2) + tempLine.Substring(0, 2), NumberStyles.HexNumber);
                writeLine = key.ToString() + " ";
                if (key < 10) { writeLine += " "; }
                if (key < 100) { writeLine += " "; }
                if (key < 1000) { writeLine += " "; }
                if (key < 10000) { writeLine += " "; }

                for (int k = 14; k < 96; k+=2) { if (tempLine.Substring(k, 2) != "00") { writeLine += Convert.ToChar(int.Parse(tempLine.Substring(k, 2), NumberStyles.HexNumber)); } }

                writer.WriteLine(writeLine);
                writer.Flush();
                tempLine = "";
                writeLine ="";
                //fs.Position += offsetEntryLength[33];
            }
            writer.Close();
            //Chests
        }


        private void buttonSave_Click(object sender, EventArgs e)
        {
            save();
        }

        private void save()
        {
            string saveBytes = sourceBox.Text.Replace(" ", "");
            byte[] writeBytes = new byte[offsetEntryLength[comboBox1.SelectedIndex]];
            for (int i = 0; i < offsetEntryLength[comboBox1.SelectedIndex]; i++)
            {
                writeBytes[i] = Convert.ToByte(int.Parse(saveBytes.Substring(i * 2, 2), NumberStyles.HexNumber));
            }

            fs.Write(writeBytes, 0, offsetEntryLength[comboBox1.SelectedIndex]);
            fs.Position -= offsetEntryLength[comboBox1.SelectedIndex];
        }

        //Convert hex from textbox to integer
        private string toInt1(string textbox)
        {
            isNumber = int.TryParse(textbox, out output);
            if (isNumber)
            {
                string temp = string.Format("{0:X2}", int.Parse(textbox));
                textbox = temp.Substring(temp.Length - 2, 2) + " ";
            }
            else
            {
                textbox = "00 ";
            }
            return textbox;
        }

        private string toInt2(string textbox)
        {
            isNumber=int.TryParse(textbox,out output);
            if (isNumber)
            {
                string temp = string.Format("{0:X4}", int.Parse(textbox));
                textbox = temp.Substring(temp.Length - 2, 2) + " " + temp.Substring(temp.Length - 4, 2) + " ";
            }
            else
            {
                textbox = "00 00 ";
            }
            return textbox;
        }

        private string toInt4(string textbox)
        {
            isNumber = int.TryParse(textbox, out output);
            if (isNumber)
            {
                string temp = string.Format("{0:X8}", int.Parse(textbox));
                textbox = temp.Substring(temp.Length - 2, 2) + " " + temp.Substring(temp.Length - 4, 2) + " " + temp.Substring(temp.Length - 6, 2) + " " + temp.Substring(temp.Length - 8, 2) + " ";
            }
            else
            {
                textbox = "00 00 00 00 ";
            }
            return textbox;
        }

        //Entry label and text change
        private void index1Load()
        {
            lbT = new string[10] { "Spell/Skill effect ID", "Spell/Skill type ID", "Spell/Skill requirements", "Mana cost", "Casting time (ms)", "Recast time (ms)", "Min cast range", "Max cast range", "Casting Type", "Spell/Skill stats" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = /*entrySource[0] + entrySource[1];*/int.Parse(entrySource[1].Replace(" ","") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = /*entrySource[2] + entrySource[3];*/int.Parse(entrySource[3].Replace(" ", "") + entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = entrySource[4] + entrySource[5] + entrySource[6];
            label4.Visible = true; label4.Text = lbT[2]; textBox4.Visible = true; textBox4.Text = entrySource[7] + entrySource[8] + entrySource[9];
            label5.Visible = true; label5.Text = lbT[2]; textBox5.Visible = true; textBox5.Text = entrySource[10] + entrySource[11] + entrySource[12];
            label6.Visible = true; label6.Text = lbT[2]; textBox6.Visible = true; textBox6.Text = entrySource[13] + entrySource[14] + entrySource[15];
            label7.Visible = true; label7.Text = lbT[3]; textBox7.Visible = true; textBox7.Text = /*entrySource[16] + entrySource[17];*/ int.Parse(entrySource[17].Replace(" ", "") + entrySource[16].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label8.Visible = true; label8.Text = lbT[4]; textBox8.Visible = true; textBox8.Text = int.Parse(entrySource[21].Replace(" ", "") + entrySource[20].Replace(" ", "") + entrySource[19].Replace(" ", "") + entrySource[18].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label9.Visible = true; label9.Text = lbT[5]; textBox9.Visible = true; textBox9.Text = int.Parse(entrySource[25].Replace(" ", "") + entrySource[24].Replace(" ", "") + entrySource[23].Replace(" ", "") + entrySource[22].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label10.Visible = true; label10.Text = lbT[6]; textBox10.Visible = true; textBox10.Text = int.Parse(entrySource[27].Replace(" ", "") + entrySource[26].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label11.Visible = true; label11.Text = lbT[7]; textBox11.Visible = true; textBox11.Text = int.Parse(entrySource[29].Replace(" ", "") + entrySource[28].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label12.Visible = true; label12.Text = lbT[8]; textBox12.Visible = true; textBox12.Text = entrySource[30] + entrySource[31];;
            label13.Visible = true; label13.Text = lbT[9]; textBox13.Visible = true; textBox13.Text = entrySource[32] + entrySource[33] + entrySource[34] + entrySource[35];
            label14.Visible = true; label14.Text = lbT[9]; textBox14.Visible = true; textBox14.Text = entrySource[36] + entrySource[37] + entrySource[38] + entrySource[39];
            label15.Visible = true; label15.Text = lbT[9]; textBox15.Visible = true; textBox15.Text = entrySource[40] + entrySource[41] + entrySource[42] + entrySource[43];
            label16.Visible = true; label16.Text = lbT[9]; textBox16.Visible = true; textBox16.Text = entrySource[44] + entrySource[45] + entrySource[46] + entrySource[47];
            label17.Visible = true; label17.Text = lbT[9]; textBox17.Visible = true; textBox17.Text = entrySource[48] + entrySource[49] + entrySource[50] + entrySource[51];
            label18.Visible = true; label18.Text = lbT[9]; textBox18.Visible = true; textBox18.Text = entrySource[52] + entrySource[53] + entrySource[54] + entrySource[55];
            label19.Visible = true; label19.Text = lbT[9]; textBox19.Visible = true; textBox19.Text = entrySource[56] + entrySource[57] + entrySource[58] + entrySource[59];
            label20.Visible = true; label20.Text = lbT[9]; textBox20.Visible = true; textBox20.Text = entrySource[60] + entrySource[61] + entrySource[62] + entrySource[63];
            label21.Visible = true; label21.Text = lbT[9]; textBox21.Visible = true; textBox21.Text = entrySource[64] + entrySource[65] + entrySource[66] + entrySource[67];
            label22.Visible = true; label22.Text = lbT[9]; textBox22.Visible = true; textBox22.Text = entrySource[68] + entrySource[69] + entrySource[70] + entrySource[71];
            label23.Visible = true; label23.Text = lbT[9]; textBox23.Visible = true; textBox23.Text = entrySource[72] + entrySource[73] + entrySource[74] + entrySource[75];
        }

        private void index2Load()
        {
            lbT = new string[6] { "Spell/Skill type ID", "Spell/Skill name ID", "Spell/Skill stats from spellline.lua", "Some sorting system?", "UI name", "unknown" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[3].Replace(" ", "") + entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = entrySource[4] + entrySource[5];
            label21.Visible = true; label21.Text = lbT[3]; textBox21.Visible = true; textBox21.Text = entrySource[6] + entrySource[7] + entrySource[8];
            richTextBox1.Visible = true; richTextBox1.Clear(); for (int i = 9; i < 73; i++) { if (entrySource[i] != "00") { richTextBox1.Text += Convert.ToChar(int.Parse(entrySource[i], NumberStyles.HexNumber)); } }
            label22.Visible = true; label22.Text = lbT[5]; textBox22.Visible = true; textBox22.Text = entrySource[73] + entrySource[74];

        }

        private void index4Load()
        {
            lbT = new string[24] { "Unit stats ID", "Unit level", "Unit race ID", "Agility", "Charisma", "Dexterity", "Intelligence", "Stamina", "Strength", "Wisdom", "Unknown", "Fire resistance", "Ice resistance", "Black resistance", "Mind resistance", "Walk speed", "Fight speed", "Cast speed", "Unit size", "Unknown", "Spawning base time", "Head gender", "Head ID", "Equipment slots ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[3].Replace(" ", "") + entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = entrySource[4];
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = int.Parse(entrySource[6].Replace(" ", "") + entrySource[5].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = int.Parse(entrySource[8].Replace(" ", "") + entrySource[7].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = int.Parse(entrySource[10].Replace(" ", "") + entrySource[9].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label7.Visible = true; label7.Text = lbT[6]; textBox7.Visible = true; textBox7.Text = int.Parse(entrySource[12].Replace(" ", "") + entrySource[11].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label8.Visible = true; label8.Text = lbT[7]; textBox8.Visible = true; textBox8.Text = int.Parse(entrySource[14].Replace(" ", "") + entrySource[13].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label9.Visible = true; label9.Text = lbT[8]; textBox9.Visible = true; textBox9.Text = int.Parse(entrySource[16].Replace(" ", "") + entrySource[15].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label10.Visible = true; label10.Text = lbT[9]; textBox10.Visible = true; textBox10.Text = int.Parse(entrySource[18].Replace(" ", "") + entrySource[17].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label11.Visible = true; label11.Text = lbT[10]; textBox11.Visible = true; textBox11.Text = entrySource[19] + entrySource[20];
            label12.Visible = true; label12.Text = lbT[11]; textBox12.Visible = true; textBox12.Text = int.Parse(entrySource[22].Replace(" ", "") + entrySource[21].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label13.Visible = true; label13.Text = lbT[12]; textBox13.Visible = true; textBox13.Text = int.Parse(entrySource[24].Replace(" ", "") + entrySource[23].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label14.Visible = true; label14.Text = lbT[13]; textBox14.Visible = true; textBox14.Text = int.Parse(entrySource[26].Replace(" ", "") + entrySource[25].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label15.Visible = true; label15.Text = lbT[14]; textBox15.Visible = true; textBox15.Text = int.Parse(entrySource[28].Replace(" ", "") + entrySource[27].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label16.Visible = true; label16.Text = lbT[15]; textBox16.Visible = true; textBox16.Text = int.Parse(entrySource[30].Replace(" ", "") + entrySource[29].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label17.Visible = true; label17.Text = lbT[16]; textBox17.Visible = true; textBox17.Text = int.Parse(entrySource[32].Replace(" ", "") + entrySource[31].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label18.Visible = true; label18.Text = lbT[17]; textBox18.Visible = true; textBox18.Text = int.Parse(entrySource[34].Replace(" ", "") + entrySource[33].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label19.Visible = true; label19.Text = lbT[18]; textBox19.Visible = true; textBox19.Text = int.Parse(entrySource[36].Replace(" ", "") + entrySource[35].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label20.Visible = true; label20.Text = lbT[19]; textBox20.Visible = true; textBox20.Text = entrySource[37] + entrySource[38];
            label21.Visible = true; label21.Text = lbT[20]; textBox21.Visible = true; textBox21.Text = int.Parse(entrySource[42].Replace(" ", "") + entrySource[41].Replace(" ", "") + entrySource[40].Replace(" ", "") + entrySource[39].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label22.Visible = true; label22.Text = lbT[21]; textBox22.Visible = true; textBox22.Text = entrySource[43];
            label23.Visible = true; label23.Text = lbT[22]; textBox23.Visible = true; textBox23.Text = entrySource[44] + entrySource[45];
            label24.Visible = true; label24.Text = lbT[23]; textBox24.Visible = true; textBox24.Text = entrySource[46];
        }

        private void index5Load()
        {
            lbT = new string[2] { "Unit stats ID", "Skill" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse((entrySource[1] + entrySource[0]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = entrySource[2] + entrySource[3] + entrySource[4];
        }

        private void index6Load()
        {
            lbT = new string[3] { "Unit stats ID", "Spell number", "Spell ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse((entrySource[1] + entrySource[0]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse((entrySource[4] + entrySource[3]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index7Load()
        {
            lbT = new string[10] { "Item ID", "Item type", "Item name ID", "Unit stats ID", "Army unit ID", "Building ID", "Unknown", "Selling price", "Buying price", "Item set ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse((entrySource[1] + entrySource[0]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = entrySource[2] + entrySource[3];
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse((entrySource[5] + entrySource[4]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = int.Parse((entrySource[7] + entrySource[6]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = int.Parse((entrySource[9] + entrySource[8]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = int.Parse((entrySource[11] + entrySource[10]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label7.Visible = true; label7.Text = lbT[6]; textBox7.Visible = true; textBox7.Text = entrySource[12];
            label8.Visible = true; label8.Text = lbT[7]; textBox8.Visible = true; textBox8.Text = int.Parse((entrySource[16] + entrySource[15] + entrySource[14] + entrySource[13]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label9.Visible = true; label9.Text = lbT[8]; textBox9.Visible = true; textBox9.Text = int.Parse((entrySource[20] + entrySource[19] + entrySource[18] + entrySource[17]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label10.Visible = true; label10.Text = lbT[9]; textBox10.Visible = true; textBox10.Text = entrySource[21];
        }

        private void index8Load()
        {
            lbT = new string[18] { "Item ID", "strength", "stamina", "agility", "dexterity", "HP", "charisma", "intelligence", "wisdom", "mana", "armor class", "fire resist", "ice resist", "black resist", "mind resist", "run speed", "fight speed", "cast speed" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[3].Replace(" ", "") + entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[5].Replace(" ", "") + entrySource[4].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = int.Parse(entrySource[7].Replace(" ", "") + entrySource[6].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = int.Parse(entrySource[9].Replace(" ", "") + entrySource[8].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = int.Parse(entrySource[11].Replace(" ", "") + entrySource[10].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label7.Visible = true; label7.Text = lbT[6]; textBox7.Visible = true; textBox7.Text = int.Parse(entrySource[13].Replace(" ", "") + entrySource[12].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label8.Visible = true; label8.Text = lbT[7]; textBox8.Visible = true; textBox8.Text = int.Parse(entrySource[15].Replace(" ", "") + entrySource[14].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label9.Visible = true; label9.Text = lbT[8]; textBox9.Visible = true; textBox9.Text = int.Parse(entrySource[17].Replace(" ", "") + entrySource[16].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label10.Visible = true; label10.Text = lbT[9]; textBox10.Visible = true; textBox10.Text = int.Parse(entrySource[19].Replace(" ", "") + entrySource[18].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label11.Visible = true; label11.Text = lbT[10]; textBox11.Visible = true; textBox11.Text = int.Parse(entrySource[21].Replace(" ", "") + entrySource[20].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label12.Visible = true; label12.Text = lbT[11]; textBox12.Visible = true; textBox12.Text = int.Parse(entrySource[23].Replace(" ", "") + entrySource[22].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label13.Visible = true; label13.Text = lbT[12]; textBox13.Visible = true; textBox13.Text = int.Parse(entrySource[25].Replace(" ", "") + entrySource[24].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label14.Visible = true; label14.Text = lbT[13]; textBox14.Visible = true; textBox14.Text = int.Parse(entrySource[27].Replace(" ", "") + entrySource[26].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label15.Visible = true; label15.Text = lbT[14]; textBox15.Visible = true; textBox15.Text = int.Parse(entrySource[29].Replace(" ", "") + entrySource[28].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label16.Visible = true; label16.Text = lbT[15]; textBox16.Visible = true; textBox16.Text = int.Parse(entrySource[31].Replace(" ", "") + entrySource[30].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label17.Visible = true; label17.Text = lbT[16]; textBox17.Visible = true; textBox17.Text = int.Parse(entrySource[33].Replace(" ", "") + entrySource[32].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label18.Visible = true; label18.Text = lbT[17]; textBox18.Visible = true; textBox18.Text = int.Parse(entrySource[35].Replace(" ", "") + entrySource[34].Replace(" ", ""), NumberStyles.HexNumber).ToString();

        }

        private void index9Load()
        {
            lbT = new string[2] { "Scroll/Rune in inventory ID", "Spell/Rune added ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[3].Replace(" ", "") + entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index10Load()
        {
            lbT = new string[8] { "Item ID", "Min damage", "Max damage", "Min range", "Max range", "Weapon speed", "Weapon type", "Weapon material" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[3].Replace(" ", "") + entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[5].Replace(" ", "") + entrySource[4].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = int.Parse(entrySource[7].Replace(" ", "") + entrySource[6].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = int.Parse(entrySource[9].Replace(" ", "") + entrySource[8].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = int.Parse(entrySource[11].Replace(" ", "") + entrySource[10].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label7.Visible = true; label7.Text = lbT[6]; textBox7.Visible = true; textBox7.Text = entrySource[12] + entrySource[13];
            label8.Visible = true; label8.Text = lbT[7]; textBox8.Visible = true; textBox8.Text = entrySource[14] + entrySource[15];
        }

        private void index11Load()
        {
            lbT = new string[3] { "Item ID", "Requirement Number", "Item requirements" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[2].Replace(" ", "")).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = entrySource[3] + entrySource[4] + entrySource[5];
        }

        private void index12Load()
        {
            lbT = new string[3] { "Item ID", "Effect number", "Effect ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[2]).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[4].Replace(" ", "") + entrySource[3].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index13Load()
        {
            lbT = new string[3] { "Item ID", "UI Number", "Unknown" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse((entrySource[1] + entrySource[0]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = entrySource[2];
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = entrySource[67] + entrySource[68];
            richTextBox1.Visible = true; richTextBox1.Clear(); for (int i = 3; i < 67; i++) { if (entrySource[i] != "00") { richTextBox1.Text += Convert.ToChar(int.Parse(entrySource[i], NumberStyles.HexNumber)); } }
        }

        private void index14Load()
        {
            lbT = new string[2] { "Item ID", "Spell effect ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[3].Replace(" ", "") + entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index15Load()
        {
            lbT = new string[3] { "Text ID", "Language ID", "Dialogue number" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse((entrySource[1] + entrySource[0]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = entrySource[2];
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = entrySource[3];
            textBox21.Visible = true; textBox21.Clear(); for (int j = 4; j < 54; j++) { textBox21.Text += entrySource[j]; }
            richTextBox1.Visible = true; richTextBox1.Clear(); for (int i = 54; i < offsetEntryLength[14] - 54; i++) { if (entrySource[i] != "00") { richTextBox1.Text += Convert.ToChar(int.Parse(entrySource[i], NumberStyles.HexNumber)); } }    
        }

        private void index16Load()
        {
            lbT = new string[6] { "Race ID", "Unknown", "Race name ID", "Unknown", "Stats from lua", "Unknown"};
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = entrySource[1] + entrySource[2] + entrySource[3] + entrySource[4] + entrySource[5] + entrySource[6];
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse((entrySource[8] + entrySource[7]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = entrySource[9] + entrySource[10] + entrySource[11] + entrySource[12] + entrySource[13] + entrySource[14] + entrySource[15];
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = entrySource[16] + entrySource[17] + entrySource[18];
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = entrySource[19] + entrySource[20] + entrySource[21] + entrySource[22] + entrySource[23] + entrySource[24] + entrySource[25] + entrySource[26];
        }

        private void index17Load()
        {
            lbT = new string[2] { "Head ID", "Unknown"};
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = entrySource[0];
            label2.Visible = true; label2.Text = lbT[0]; textBox2.Visible = true; textBox2.Text = entrySource[1];
            label3.Visible = true; label3.Text = lbT[1]; textBox3.Visible = true; textBox3.Text = entrySource[2];
        }

        private void index18Load()
        {
            lbT = new string[10] { "Unit ID", "Name ID", "Unit Stats ID", "Unit EXP", "Unknown", "HP Factor", "Unknown", "Unit AC" ,"Unit Name in Gamedata", "Unknown"};
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse((entrySource[1] + entrySource[0]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse((entrySource[3] + entrySource[2]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse((entrySource[5] + entrySource[4]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = int.Parse((entrySource[9] + entrySource[8] + entrySource[7] + entrySource[6]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = entrySource[10] + entrySource[11];
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = entrySource[12] + entrySource[13] + entrySource[14] + entrySource[15];
            label7.Visible = true; label7.Text = lbT[6]; textBox7.Visible = true; textBox7.Text = entrySource[16] + entrySource[17] + entrySource[18] + entrySource[19] + entrySource[20];
            label8.Visible = true; label8.Text = lbT[7]; textBox8.Visible = true; textBox8.Text = int.Parse((entrySource[22] + entrySource[21]).Replace(" ",""),NumberStyles.HexNumber).ToString();
            label9.Visible = true; label9.Text = lbT[8]; textBox9.Visible = true; textBox9.Clear(); for (int i = 23; i <= 62; i++) { textBox9.Text += Convert.ToChar(int.Parse(entrySource[i],NumberStyles.HexNumber)); } //entrySource[23] + entrySource[24] + entrySource[25] + entrySource[26] + entrySource[27] + entrySource[28] + entrySource[29] + entrySource[30] + entrySource[31] + entrySource[32] + entrySource[33] + entrySource[34] + entrySource[35] + entrySource[36] + entrySource[37] + entrySource[38] + entrySource[39] + entrySource[40] + entrySource[41] + entrySource[42] + entrySource[43] + entrySource[44] + entrySource[45] + entrySource[46] + entrySource[47] + entrySource[48] + entrySource[49] + entrySource[50] + entrySource[51] + entrySource[52] + entrySource[53] + entrySource[54] + entrySource[55] + entrySource[56] + entrySource[57] + entrySource[58] + entrySource[59] + entrySource[60] + entrySource[61] + entrySource[62];
            label10.Visible = true; label10.Text = lbT[9]; textBox10.Visible = true; textBox10.Text = entrySource[63];
        }

        private void index19Load()
        {
            lbT = new string[3] { "Unit ID", "Equipment Slot", "Item ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = entrySource[2];
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[4].Replace(" ", "") + entrySource[3].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index20Load()
        {
            lbT = new string[3] { "Unit ID", "Spell/Skill number", "Spell ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[2]).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[4].Replace(" ", "") + entrySource[3].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index21Load()
        {
            lbT = new string[3] { "Unit ID", "Resource ID", "Resource amount" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = entrySource[2];
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[3].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index22Load()
        {
            lbT = new string[7] { "Unit ID", "Slot number", "1st item", "1st item dropchance %", "2nd item ID", "2nd item dropchance %", "3rd item ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[4].Replace(" ", "") + entrySource[3].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = int.Parse(entrySource[5].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = int.Parse(entrySource[7].Replace(" ", "") + entrySource[6].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = int.Parse(entrySource[8].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label7.Visible = true; label7.Text = lbT[6]; textBox7.Visible = true; textBox7.Text = int.Parse(entrySource[10].Replace(" ", "") + entrySource[9].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index23Load()
        {
            lbT = new string[3] { "Unit ID", "Requirement number", "Building ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse((entrySource[1] + entrySource[0]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse((entrySource[4] + entrySource[3]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index24Load()
        {
            lbT = new string[8] { "Building ID", "Race ID", "Enter Slot", "Slot amount", "HP", "Name ID", "Buildtime", "Work Requirement"};
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[1].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = int.Parse(entrySource[3].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = int.Parse(entrySource[4].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = int.Parse(entrySource[5].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label7.Visible = true; label7.Text = lbT[6]; textBox7.Visible = true; textBox7.Text = int.Parse(entrySource[6].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label8.Visible = true; label8.Text = lbT[7]; textBox8.Visible = true; textBox8.Text = int.Parse(entrySource[7].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index26Load()
        {
            lbT = new string[3] { "Building ID", "Resource ID", "Resource amount" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = entrySource[2];
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse((entrySource[4] + entrySource[3]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index28Load()
        {
            lbT = new string[9] { "Skill", "Level", "Strength", "Stamina", "Agility", "Dexterity", "Charisma", "Intelligence", "Wisdom" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[1].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = int.Parse(entrySource[3].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = int.Parse(entrySource[4].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = int.Parse(entrySource[5].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label7.Visible = true; label7.Text = lbT[6]; textBox7.Visible = true; textBox7.Text = int.Parse(entrySource[6].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label8.Visible = true; label8.Text = lbT[7]; textBox8.Visible = true; textBox8.Text = int.Parse(entrySource[7].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label9.Visible = true; label9.Text = lbT[8]; textBox9.Visible = true; textBox9.Text = int.Parse(entrySource[8].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index29Load()
        {
            lbT = new string[2] { "Merchant ID", "Unit ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[3].Replace(" ", "") + entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index30Load()
        {
            lbT = new string[3] { "Merchant ID", "Item ID", "Item Quantity" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[3].Replace(" ", "") + entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[5].Replace(" ", "") + entrySource[4].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index33Load()
        {
            lbT = new string[8] { "Level", "HP Factor", "MP Factor", "EXP", "Max ATT points", "Max skill level", "Weapon factor", "Armorclass factor" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse((entrySource[2] + entrySource[1]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse((entrySource[4] + entrySource[3]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = int.Parse((entrySource[8] + entrySource[7] + entrySource[6] + entrySource[5]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = int.Parse(entrySource[9].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = int.Parse(entrySource[10].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label7.Visible = true; label7.Text = lbT[6]; textBox7.Visible = true; textBox7.Text = int.Parse((entrySource[12] + entrySource[11]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label8.Visible = true; label8.Text = lbT[7]; textBox8.Visible = true; textBox8.Text = int.Parse((entrySource[14] + entrySource[13]).Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index36Load()
        {
            lbT = new string[7] { "Chest ID", "Slot number", "1st item", "1st item dropchance %", "2nd item ID", "2nd item dropchance %", "3rd item ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[4].Replace(" ", "") + entrySource[3].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = int.Parse(entrySource[5].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = int.Parse(entrySource[7].Replace(" ", "") + entrySource[6].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = int.Parse(entrySource[8].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label7.Visible = true; label7.Text = lbT[6]; textBox7.Visible = true; textBox7.Text = int.Parse(entrySource[10].Replace(" ", "") + entrySource[9].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index41Load()
        {
            lbT = new string[2] { "Description ID", "Text ID" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[3].Replace(" ", "") + entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index42Load()
        {
            lbT = new string[3] { "Button/Menu ID", "Text ID 1", "Text ID 2" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[3].Replace(" ", "") + entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[5].Replace(" ", "") + entrySource[4].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index43Load()
        {
            lbT = new string[8] { "Quest ID", "Unknown", "Previous Quest ID", "Unknown", "Unknown", "Title ID", "Description ID", "Unknown" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = entrySource[2] + entrySource[3];
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[5].Replace(" ", "") + entrySource[4].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = entrySource[7] + entrySource[6];
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = entrySource[8];
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = int.Parse(entrySource[10].Replace(" ", "") + entrySource[9].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label7.Visible = true; label7.Text = lbT[6]; textBox7.Visible = true; textBox7.Text = int.Parse(entrySource[12].Replace(" ", "") + entrySource[11].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label8.Visible = true; label8.Text = lbT[7]; textBox8.Visible = true; textBox8.Text = entrySource[13] + entrySource[14] + entrySource[15] + entrySource[16];
        }

        private void index48Load()
        {
            lbT = new string[13] { "Button ID", "Building ID", "Text ID", "Description ID", "Wood", "Stone", "Iron", "Lenya", "Aria", "Moonsilver", "Food", "UI name", "Build time (ms)" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[1].Replace(" ", "") + entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[3].Replace(" ", "") + entrySource[2].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = int.Parse(entrySource[5].Replace(" ", "") + entrySource[4].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label4.Visible = true; label4.Text = lbT[3]; textBox4.Visible = true; textBox4.Text = int.Parse(entrySource[7].Replace(" ", "") + entrySource[6].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label5.Visible = true; label5.Text = lbT[4]; textBox5.Visible = true; textBox5.Text = int.Parse(entrySource[9].Replace(" ", "") + entrySource[8].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label6.Visible = true; label6.Text = lbT[5]; textBox6.Visible = true; textBox6.Text = int.Parse(entrySource[11].Replace(" ", "") + entrySource[10].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label7.Visible = true; label7.Text = lbT[6]; textBox7.Visible = true; textBox7.Text = int.Parse(entrySource[13].Replace(" ", "") + entrySource[12].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label21.Visible = true; label21.Text = lbT[7]; textBox21.Visible = true; textBox21.Text = int.Parse(entrySource[15].Replace(" ", "") + entrySource[14].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label22.Visible = true; label22.Text = lbT[8]; textBox22.Visible = true; textBox22.Text = int.Parse(entrySource[17].Replace(" ", "") + entrySource[16].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label23.Visible = true; label23.Text = lbT[9]; textBox23.Visible = true; textBox23.Text = int.Parse(entrySource[19].Replace(" ", "") + entrySource[18].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label24.Visible = true; label24.Text = lbT[10]; textBox24.Visible = true; textBox24.Text = int.Parse(entrySource[21].Replace(" ", "") + entrySource[20].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            richTextBox1.Visible = true; richTextBox1.Clear(); for (int i = 22; i < 86; i++) { if (entrySource[i] != "00") { richTextBox1.Text += Convert.ToChar(int.Parse(entrySource[i], NumberStyles.HexNumber)); } }

            label25.Visible = true; label25.Text = lbT[12]; textBox25.Visible = true; textBox25.Text = int.Parse(entrySource[89].Replace(" ", "") + entrySource[88].Replace(" ", "") + entrySource[87].Replace(" ", "") + entrySource[86].Replace(" ", ""), NumberStyles.HexNumber).ToString();
        }

        private void index49Load()
        {
            lbT = new string[3] { "Set ID", "Text ID", "Unknown" };
            label1.Visible = true; label1.Text = lbT[0]; textBox1.Visible = true; textBox1.Text = int.Parse(entrySource[0].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label2.Visible = true; label2.Text = lbT[1]; textBox2.Visible = true; textBox2.Text = int.Parse(entrySource[2].Replace(" ", "") + entrySource[1].Replace(" ", ""), NumberStyles.HexNumber).ToString();
            label3.Visible = true; label3.Text = lbT[2]; textBox3.Visible = true; textBox3.Text = entrySource[3];
        }

        //Disable Labels and Textboxes
        private void disableAll()
        {
            richTextBox1.Visible = false;
            textBox1.Visible = false; label1.Visible = false;
            textBox2.Visible = false; label2.Visible = false;
            textBox3.Visible = false; label3.Visible = false;
            textBox4.Visible = false; label4.Visible = false;
            textBox5.Visible = false; label5.Visible = false;
            textBox6.Visible = false; label6.Visible = false;
            textBox7.Visible = false; label7.Visible = false;
            textBox8.Visible = false; label8.Visible = false;
            textBox9.Visible = false; label9.Visible = false;
            textBox10.Visible = false; label10.Visible = false;
            textBox11.Visible = false; label11.Visible = false;
            textBox12.Visible = false; label12.Visible = false;
            textBox13.Visible = false; label13.Visible = false;
            textBox14.Visible = false; label14.Visible = false;
            textBox15.Visible = false; label15.Visible = false;
            textBox16.Visible = false; label16.Visible = false;
            textBox17.Visible = false; label17.Visible = false;
            textBox18.Visible = false; label18.Visible = false;
            textBox19.Visible = false; label19.Visible = false;
            textBox20.Visible = false; label20.Visible = false;
            textBox21.Visible = false; label21.Visible = false;
            textBox22.Visible = false; label22.Visible = false;
            textBox23.Visible = false; label23.Visible = false;
            textBox24.Visible = false; label24.Visible = false;
            textBox25.Visible = false; label25.Visible = false;
            textBox26.Visible = false; label26.Visible = false;
            textBox27.Visible = false; label27.Visible = false;
            textBox28.Visible = false; label28.Visible = false;
            textBox29.Visible = false; label29.Visible = false;
            textBox30.Visible = false; label30.Visible = false;
            textBox31.Visible = false; label31.Visible = false;
            textBox32.Visible = false; label32.Visible = false;
            textBox33.Visible = false; label33.Visible = false;
            textBox34.Visible = false; label34.Visible = false;
            textBox35.Visible = false; label35.Visible = false;
            textBox36.Visible = false; label36.Visible = false;
            textBox37.Visible = false; label37.Visible = false;
            textBox38.Visible = false; label38.Visible = false;
            textBox39.Visible = false; label39.Visible = false;
            textBox40.Visible = false; label40.Visible = false;
        }

        //Reload content of sourceBox
        private void reloadSourceBox()
        {
            if (sbChange)
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text) + textBox3.Text + textBox4.Text + textBox5.Text + textBox6.Text + toInt2(textBox7.Text) + toInt4(textBox8.Text) + toInt4(textBox9.Text) + toInt2(textBox10.Text) + toInt2(textBox11.Text) + textBox12.Text + textBox13.Text + textBox14.Text + textBox15.Text + textBox16.Text + textBox17.Text + textBox18.Text + textBox19.Text + textBox20.Text + toInt4(textBox21.Text) + textBox22.Text + textBox23.Text; break;
                    case 1: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text) + textBox3.Text + textBox21.Text + asciiToHex64(richTextBox1.Text) + textBox22.Text; break;
                    case 3: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text) + textBox3.Text + toInt2(textBox4.Text) + toInt2(textBox5.Text) + toInt2(textBox6.Text) + toInt2(textBox7.Text) + toInt2(textBox8.Text) + toInt2(textBox9.Text) + toInt2(textBox10.Text) + textBox11.Text + toInt2(textBox12.Text) + toInt2(textBox13.Text) + toInt2(textBox14.Text) + toInt2(textBox15.Text) + toInt2(textBox16.Text) + toInt2(textBox17.Text) + toInt2(textBox18.Text) + toInt2(textBox19.Text) + textBox20.Text + toInt4(textBox21.Text) + textBox22.Text + textBox23.Text + textBox24.Text; break;
                    case 4: sourceBox.Text = toInt2(textBox1.Text) + textBox2.Text; break;
                    case 5: sourceBox.Text = toInt2(textBox1.Text) + toInt1(textBox2.Text) + toInt2(textBox3.Text); break;
                    case 6: sourceBox.Text = toInt2(textBox1.Text) + textBox2.Text + toInt2(textBox3.Text) + toInt2(textBox4.Text) + toInt2(textBox5.Text) + toInt2(textBox6.Text) + textBox7.Text + toInt4(textBox8.Text) + toInt4(textBox9.Text) + textBox10.Text; break;
                    case 7: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text) + toInt2(textBox3.Text) + toInt2(textBox4.Text) + toInt2(textBox5.Text) + toInt2(textBox6.Text) + toInt2(textBox7.Text) + toInt2(textBox8.Text) + toInt2(textBox9.Text) + toInt2(textBox10.Text) + toInt2(textBox11.Text) + toInt2(textBox12.Text) + toInt2(textBox13.Text) + toInt2(textBox14.Text) + toInt2(textBox15.Text) + toInt2(textBox16.Text) + toInt2(textBox17.Text) + toInt2(textBox18.Text); break;
                    case 8: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text); break;
                    case 9: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text) + toInt2(textBox3.Text) + toInt2(textBox4.Text) + toInt2(textBox5.Text) + toInt2(textBox6.Text) + textBox7.Text + textBox8.Text; break;
                    case 10: sourceBox.Text = toInt2(textBox1.Text) + toInt1(textBox2.Text) + textBox3.Text; break;
                    case 11: sourceBox.Text = toInt2(textBox1.Text) + toInt1(textBox2.Text) + toInt2(textBox3.Text); break;
                    case 12: sourceBox.Text = toInt2(textBox1.Text) + textBox2.Text + asciiToHex64(richTextBox1.Text) + textBox3.Text; break;
                    case 13: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text); break;
                    case 14: sourceBox.Text = toInt2(textBox1.Text) + textBox2.Text + textBox3.Text + textBox21.Text + asciiToHex512(richTextBox1.Text); break;
                    case 15: sourceBox.Text = toInt1(textBox1.Text) + textBox2.Text + toInt2(textBox3.Text) + textBox4.Text + textBox5.Text + textBox6.Text; break;
                    case 16: sourceBox.Text = textBox1.Text + textBox2.Text + textBox3.Text; break;
                    case 17: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text) + toInt2(textBox3.Text) + toInt4(textBox4.Text) + textBox5.Text + textBox6.Text + textBox7.Text + toInt2(textBox8.Text) + asciiToHex40(textBox9.Text) + textBox10.Text;  break;
                    case 18: sourceBox.Text = toInt2(textBox1.Text) + textBox2.Text + toInt2(textBox3.Text); break;
                    case 19: sourceBox.Text = toInt2(textBox1.Text) + toInt1(textBox2.Text) + toInt2(textBox3.Text); break;
                    case 20: sourceBox.Text = toInt2(textBox1.Text) + textBox2.Text + toInt1(textBox3.Text); break;
                    case 21: sourceBox.Text = toInt2(textBox1.Text) + toInt1(textBox2.Text) + toInt2(textBox3.Text) + toInt1(textBox4.Text) + toInt2(textBox5.Text) + toInt1(textBox6.Text) + toInt2(textBox7.Text); break;
                    case 22: sourceBox.Text = toInt2(textBox1.Text) + toInt1(textBox2.Text) + toInt2(textBox3.Text); break;
                    case 25: sourceBox.Text = toInt2(textBox1.Text) + textBox2.Text + toInt2(textBox3.Text); break;
                    case 27: sourceBox.Text = toInt1(textBox1.Text) + toInt1(textBox2.Text) + toInt1(textBox3.Text) + toInt1(textBox4.Text) + toInt1(textBox5.Text) + toInt1(textBox6.Text) + toInt1(textBox7.Text) + toInt1(textBox8.Text) + toInt1(textBox9.Text); break;
                    case 28: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text); break;
                    case 29: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text) + toInt2(textBox3.Text); break;
                    case 32: sourceBox.Text = toInt1(textBox1.Text) + toInt2(textBox2.Text) + toInt2(textBox3.Text) + toInt4(textBox4.Text) + toInt1(textBox5.Text) + toInt1(textBox6.Text) + toInt2(textBox7.Text) + toInt2(textBox8.Text); break;
                    case 35: sourceBox.Text = toInt2(textBox1.Text) + toInt1(textBox2.Text) + toInt2(textBox3.Text) + toInt1(textBox4.Text) + toInt2(textBox5.Text) + toInt1(textBox6.Text) + toInt2(textBox7.Text); break;
                    case 40: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text); break;
                    case 41: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text) + toInt2(textBox3.Text); break;
                    case 42: sourceBox.Text = toInt2(textBox1.Text) + textBox2.Text + toInt2(textBox3.Text) + textBox4.Text + textBox5.Text + toInt2(textBox6.Text) + toInt2(textBox7.Text) + textBox8.Text; break;
                    case 47: sourceBox.Text = toInt2(textBox1.Text) + toInt2(textBox2.Text) + toInt2(textBox3.Text) + toInt2(textBox4.Text) + toInt2(textBox5.Text) + toInt2(textBox6.Text) + toInt2(textBox7.Text) + toInt2(textBox21.Text) + toInt2(textBox22.Text) + toInt2(textBox23.Text) + toInt2(textBox24.Text) + asciiToHex64(richTextBox1.Text) + toInt4(textBox25.Text); break;
                    case 48: sourceBox.Text = toInt1(textBox1.Text) + toInt2(textBox2.Text) + textBox3.Text; break;
                    default: sourceBox.Text = textBox1.Text + textBox2.Text + textBox3.Text + textBox4.Text + textBox5.Text + textBox6.Text + textBox7.Text + textBox8.Text + textBox9.Text + textBox10.Text + textBox11.Text + textBox12.Text + textBox13.Text + textBox14.Text + textBox15.Text + textBox16.Text + textBox17.Text + textBox18.Text + textBox19.Text + textBox20.Text + textBox21.Text + textBox22.Text + textBox23.Text + textBox24.Text + textBox25.Text + textBox26.Text + textBox27.Text + textBox28.Text + textBox29.Text + textBox30.Text + textBox31.Text + textBox32.Text + textBox33.Text + textBox34.Text + textBox35.Text + textBox36.Text + textBox37.Text + textBox38.Text + textBox39.Text + textBox40.Text; break;
                }
            }
        }

        private string asciiToHex40(string textbox)
        {
            char[] tem = new char[40];
            Array.Clear(tem, 0, tem.Length);
            string temp = "";
            int val;
            for (int i = 0; i < 40; i++)
            {
                try
                {
                    tem[i] = Convert.ToChar(textbox.Substring(i, 1));
                    val = Convert.ToInt32(tem[i]);
                }
                catch (System.Exception)
                {
                    val = 0;
                }
                temp += string.Format("{0:X2} ", val);
            }
            textbox = temp;
            return textbox;
        }

        private string asciiToHex64(string textbox)
        {
            char[] tem = new char[64];
            Array.Clear(tem, 0, tem.Length);
            string temp = "";
            int val;
            for (int i = 0; i < 64; i++)
            {
                try
                {
                    tem[i] = Convert.ToChar(textbox.Substring(i, 1));
                    val = Convert.ToInt32(tem[i]);
                }
                catch (System.Exception)
                {
                    val = 0;
                }
                temp += string.Format("{0:X2} ", val);
            }
            textbox = temp;
            return textbox;
        }

        private string asciiToHex512(string textbox)
        {
            char[] tem = new char[512];
            Array.Clear(tem, 0, tem.Length);
            string temp = "";
            int val;
            for (int i = 0; i < 512; i++)
            {
                try
                {
                    tem[i] = Convert.ToChar(textbox.Substring(i, 1));
                    val = Convert.ToInt32(tem[i]);
                }
                catch (System.Exception)
                {
                    val = 0;
                }
                temp += string.Format("{0:X2} ", val);
            }
            textbox = temp;
            return textbox;
        }

        //Reload content of Textboxes
        private void reloadTextBoxes()
        {
            
            if (sourceBox.Text.Length == offsetEntryLength[comboBox1.SelectedIndex] * 3 && tbChange)
            {
                for (int i = 0; i < offsetEntryLength[comboBox1.SelectedIndex]; i++)
                {
                    entrySource[i] = sourceBox.Text.Substring(i * 3, 3);
                }
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        index1Load();
                        break;
                    case 1:
                        index2Load();
                        break;
                    case 3:
                        index4Load();
                        break;
                    case 4:
                        index5Load();
                        break;
                    case 5:
                        index6Load();
                        break;
                    case 6:
                        index7Load();
                        break;
                    case 7:
                        index8Load();
                        break;
                    case 8:
                        index9Load();
                        break;
                    case 9:
                        index10Load();
                        break;
                    case 10:
                        index11Load();
                        break;
                    case 11:
                        index12Load();
                        break;
                    case 12:
                        index13Load();
                        break;
                    case 13:
                        index14Load();
                        break;
                    case 14:
                        index15Load();
                        break;
                    case 15:
                        index16Load();
                        break;
                    case 16:
                        index17Load();
                        break;
                    case 17:
                        index18Load();
                        break;
                    case 18:
                        index19Load();
                        break;
                    case 19:
                        index20Load();
                        break;
                    case 20:
                        index21Load();
                        break;
                    case 21:
                        index22Load();
                        break;
                    case 22:
                        index23Load();
                        break;
                    case 25:
                        index26Load();
                        break;
                    case 27:
                        index28Load();
                        break;
                    case 28:
                        index29Load();
                        break;
                    case 29:
                        index30Load();
                        break;
                    case 32:
                        index33Load();
                        break;
                    case 35:
                        index36Load();
                        break;
                    case 40:
                        index41Load();
                        break;
                    case 41:
                        index42Load();
                        break;
                    case 42:
                        index43Load();
                        break;
                    case 47:
                        index48Load();
                        break;
                    case 48:
                        index49Load();
                        break;
                    default: break;
                }
            }
        }

        //SourceBox changes reload textboxes
        private void sourceBox_TextChanged(object sender, EventArgs e)
        {
            if (sourceBox.Focused) { sbChange = false; tbChange = true; }
            reloadTextBoxes();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        //Textbox changes reload sourceBox START
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (textBox6.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (textBox7.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if (textBox8.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (textBox9.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            if (textBox10.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            if (textBox11.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            if (textBox12.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            if (textBox13.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            if (textBox14.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            if (textBox15.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            if (textBox16.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            if (textBox17.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            if (textBox18.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            if (textBox19.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {
            if (textBox20.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {
            if (textBox21.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            if (textBox22.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox23_TextChanged(object sender, EventArgs e)
        {
            if (textBox23.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox24_TextChanged(object sender, EventArgs e)
        {
            if (textBox24.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox25_TextChanged(object sender, EventArgs e)
        {
            if (textBox25.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox26_TextChanged(object sender, EventArgs e)
        {
            if (textBox26.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox27_TextChanged(object sender, EventArgs e)
        {
            if (textBox27.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox28_TextChanged(object sender, EventArgs e)
        {
            if (textBox28.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox29_TextChanged(object sender, EventArgs e)
        {
            if (textBox29.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox30_TextChanged(object sender, EventArgs e)
        {
            if (textBox30.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox31_TextChanged(object sender, EventArgs e)
        {
            if (textBox31.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox32_TextChanged(object sender, EventArgs e)
        {
            if (textBox32.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox33_TextChanged(object sender, EventArgs e)
        {
            if (textBox33.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox34_TextChanged(object sender, EventArgs e)
        {
            if (textBox34.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox35_TextChanged(object sender, EventArgs e)
        {
            if (textBox35.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox36_TextChanged(object sender, EventArgs e)
        {
            if (textBox36.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox37_TextChanged(object sender, EventArgs e)
        {
            if (textBox37.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox38_TextChanged(object sender, EventArgs e)
        {
            if (textBox38.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox39_TextChanged(object sender, EventArgs e)
        {
            if (textBox39.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void textBox40_TextChanged(object sender, EventArgs e)
        {
            if (textBox40.Focused) { sbChange = true; tbChange = false; }
            reloadSourceBox();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void selectNextIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count > 0) { if (listBox2.SelectedIndex < listBox2.Items.Count - 1) { listBox2.SelectedIndex += 1; } }
            else if (listBox1.SelectedIndex < listBox1.Items.Count - 1) { listBox1.SelectedIndex += 1; }
        }

        private void selectPreviousIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count > 0) { if (listBox2.SelectedIndex > 0) { listBox2.SelectedIndex -= 1; } }
            else if (listBox1.SelectedIndex > 0) { listBox1.SelectedIndex -= 1; }
        }

        private void createBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.Copy(file, file + ".Backup", true);
        }

        //Textbox changes reload sourceBox END
    }
}
