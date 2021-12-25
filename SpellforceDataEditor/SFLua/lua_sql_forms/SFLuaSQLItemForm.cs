using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFEngine.SFMap;
using SFEngine.SFCFF;
using SFEngine.SFLua;

namespace SpellforceDataEditor.SFLua.lua_sql_forms
{
    public partial class SFLuaSQLItemForm : Form
    {
        List<int> index_to_key = null;
        int selected_id = SFEngine.Utility.NO_INDEX;

        public SFLuaSQLItemForm()
        {
            InitializeComponent();
        }

        private void SFLuaSQLItemForm_Load(object sender, EventArgs e)
        {
            if (SFLuaEnvironment.items.Load() != 0)
            {
                MessageBox.Show("Could not load script/sql_item.lua");
                this.Close();
                return;
            }

            ReloadItemList();
        }

        private void ReloadItemList()
        {
            ListItems.Items.Clear();
            ListItems.SuspendLayout();

            var items = SFLuaEnvironment.items.items;
            index_to_key = items.Keys.ToList();
            index_to_key.Sort();

            foreach (int i in index_to_key)
                ListItems.Items.Add(i.ToString()+". "+GetItemString(i));

            ListItems.ResumeLayout();
        }

        private string GetItemString(int id)
        {
            if (!SFCategoryManager.ready)
                return "";
            return SFCategoryManager.GetItemName((ushort)id);
        }

        private void ListItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListItems.SelectedIndex == SFEngine.Utility.NO_INDEX)
            {
                MMC.Text = "";
                MFC.Text = "";
                MMW.Text = "";
                MFW.Text = "";
                ShadowRNG.Text = "";
                SelectionSize.Text = "";
                AnimSet.Text = "";
                Race.Text = "";
                Cat.Text = "";
                SubCat.Text = "";
                ItemID.Text = "";
                selected_id = -1;
                return;
            }

            int item_id = index_to_key[ListItems.SelectedIndex];    // even though its a dictionary, they're listed sequentially!
            selected_id = item_id;

            var items = SFLuaEnvironment.items.items;
            if (!items.ContainsKey(item_id))
                return;

            MMC.Text = items[item_id].MeshMaleCold;
            MFC.Text = items[item_id].MeshFemaleCold;
            MMW.Text = items[item_id].MeshMaleWarm;
            MFW.Text = items[item_id].MeshFemaleWarm;
            ShadowRNG.Text = items[item_id].ShadowRNG.ToString();
            SelectionSize.Text = items[item_id].SelectionSize.ToString();
            AnimSet.Text = items[item_id].AnimSet;
            Race.Text = items[item_id].Race.ToString();
            Cat.Text = items[item_id].Category.ToString();
            SubCat.Text = items[item_id].SubCategory.ToString();
            ItemID.Text = item_id.ToString();
        }

        private void MMC_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
                return;

            SFLuaEnvironment.items.items[selected_id].MeshMaleCold = MMC.Text.ToString();
            if(MMC.Text.ToString()=="")
                SFLuaEnvironment.items.items[selected_id].MeshMaleCold = "<undefined>";
        }

        private void MFC_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
                return;

            SFLuaEnvironment.items.items[selected_id].MeshFemaleCold = MFC.Text.ToString();
            if (MFC.Text.ToString() == "")
                SFLuaEnvironment.items.items[selected_id].MeshFemaleCold = "<undefined>";
        }

        private void MMW_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
                return;

            SFLuaEnvironment.items.items[selected_id].MeshMaleWarm = MMW.Text.ToString();
            if (MMW.Text.ToString() == "")
                SFLuaEnvironment.items.items[selected_id].MeshMaleWarm = "<undefined>";
        }

        private void MFW_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
                return;

            SFLuaEnvironment.items.items[selected_id].MeshFemaleWarm = MFW.Text.ToString();
            if (MFW.Text.ToString() == "")
                SFLuaEnvironment.items.items[selected_id].MeshFemaleWarm = "<undefined>";
        }

        private void ShadowRNG_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
                return;

            SFLuaEnvironment.items.items[selected_id].ShadowRNG = 
                SFEngine.Utility.TryParseDouble(ShadowRNG.Text,
                                       SFLuaEnvironment.items.items[selected_id].ShadowRNG);
        }

        private void SelectionSize_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
                return;

            SFLuaEnvironment.items.items[selected_id].SelectionSize =
                SFEngine.Utility.TryParseDouble(SelectionSize.Text,
                                       SFLuaEnvironment.items.items[selected_id].SelectionSize);
        }

        private void AnimSet_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
                return;

            SFLuaEnvironment.items.items[selected_id].AnimSet = AnimSet.Text.ToString();
        }

        private void Race_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
                return;

            SFLuaEnvironment.items.items[selected_id].Race =
                (int)SFEngine.Utility.TryParseUInt32(Race.Text,
                                             (uint)SFLuaEnvironment.items.items[selected_id].Race);
        }

        private void Cat_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
                return;

            SFLuaEnvironment.items.items[selected_id].Category =
                (int)SFEngine.Utility.TryParseUInt32(Cat.Text,
                                             (uint)SFLuaEnvironment.items.items[selected_id].Category);
        }

        private void SubCat_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
                return;

            SFLuaEnvironment.items.items[selected_id].SubCategory =
                (int)SFEngine.Utility.TryParseUInt32(SubCat.Text,
                                             (uint)SFLuaEnvironment.items.items[selected_id].SubCategory);
        }

        private void ItemID_Validated(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
                return;

            int previous_id = selected_id;
            int previous_index = ListItems.SelectedIndex;
            SFEngine.SFLua.lua_sql.SFLuaSQLItemData item = SFLuaEnvironment.items.items[previous_id];
            int new_id = (int)SFEngine.Utility.TryParseUInt32(ItemID.Text, (uint)previous_id);
            if (new_id == previous_id)
                return;

            SFLuaEnvironment.items.items.Remove(selected_id);
            index_to_key.RemoveAt(previous_index);
            ListItems.Items.RemoveAt(previous_index);

            int new_index = SFEngine.Utility.FindNewIndexOf(index_to_key, new_id);

            // check if the item with provided ID already exists
            if (new_index == SFEngine.Utility.NO_INDEX)
            {
                new_index = previous_index;
                new_id = previous_id;
                ItemID.Text = previous_id.ToString();
            }

            index_to_key.Insert(new_index, new_id);
            SFLuaEnvironment.items.items.Add(new_id, item);
            ListItems.Items.Insert(new_index, new_id.ToString() + ". " + GetItemString(new_id));

            selected_id = new_id;
            ListItems.SelectedIndex = new_index;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if (index_to_key[0] == 0)
            {
                selected_id = 0;
                ListItems.SelectedIndex = 0;
                return;
            }

            SFEngine.SFLua.lua_sql.SFLuaSQLItemData item = new SFEngine.SFLua.lua_sql.SFLuaSQLItemData();
            item.MeshMaleCold = "<undefined>";
            item.MeshFemaleCold = "<undefined>";
            item.MeshMaleWarm = "<undefined>";
            item.MeshFemaleWarm = "<undefined>";
            item.ShadowRNG = 0;
            item.SelectionSize = 0;
            item.AnimSet = "";
            item.Race = 0;
            item.Category = 0;
            item.SubCategory = 0;

            index_to_key.Insert(0, 0);
            SFLuaEnvironment.items.items.Add(0, item);
            ListItems.Items.Insert(0, "0. " + GetItemString(0));

            selected_id = 0;
            ListItems.SelectedIndex = 0;
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            if (selected_id == SFEngine.Utility.NO_INDEX)
                return;

            int ind = ListItems.SelectedIndex;
            SFLuaEnvironment.items.items.Remove(selected_id);
            index_to_key.RemoveAt(ListItems.SelectedIndex);
            ListItems.Items.RemoveAt(ListItems.SelectedIndex);

            if (ind == index_to_key.Count)
                ind -= 1;

            ListItems.SelectedIndex = ind;
        }

        private void ButtonCancelChanges_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ButtonSaveChanges_Click(object sender, EventArgs e)
        {
            if (SFLuaEnvironment.items.Save() != 0)
            {
                MessageBox.Show("Error while saving sql_item.lua");
            }
            this.Close();
        }
    }
}
