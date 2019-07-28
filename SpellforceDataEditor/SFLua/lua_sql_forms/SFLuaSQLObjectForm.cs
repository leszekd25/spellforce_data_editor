using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFLua.lua_sql_forms
{
    public partial class SFLuaSQLObjectForm : Form
    {
        bool can_load = true;
        List<int> index_to_key = null;
        int selected_id = -1;

        public SFLuaSQLObjectForm()
        {
            InitializeComponent();
        }

        private void SFLuaSQLObjectForm_Load(object sender, EventArgs e)
        {
            if (SFLuaEnvironment.objects.Load() != 0)
            {
                MessageBox.Show("Could not load script/sql_object.lua");
                can_load = false;
                this.Close();
                return;
            }

            ReloadObjectList();
        }

        private void ReloadObjectList()
        {
            ListObjects.Items.Clear();
            ListObjects.SuspendLayout();

            var objects = SFLuaEnvironment.objects.objects;
            index_to_key = objects.Keys.ToList();
            index_to_key.Sort();

            foreach (int i in index_to_key)
                ListObjects.Items.Add(i.ToString() + ". " + GetObjectString(i));

            ListObjects.ResumeLayout();
        }

        private string GetObjectString(int id)
        {
            if (!SFCFF.SFCategoryManager.ready)
                return "";
            return SFCFF.SFCategoryManager.GetObjectName((ushort)id);
        }

        private void ListObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ListObjects.SelectedIndex == -1)
            {
                ObjName.Text = "";
                Mesh.Items.Clear();
                SelectedMesh.Text = "";
                CastsShadow.Text = "";
                IsBillboarded.Text = "";
                Scale.Text = "";
                SelectionSize.Text = "";
                return;
            }

            int object_id = index_to_key[ListObjects.SelectedIndex];
            selected_id = object_id;

            var objects = SFLuaEnvironment.objects.objects;
            if (!objects.ContainsKey(object_id))
                return;

            ObjName.Text = objects[object_id].Name;

            Mesh.Items.Clear();
            for (int i = 0; i < objects[object_id].Mesh.Count; i++)
                Mesh.Items.Add(objects[object_id].Mesh[i]);

            SelectedMesh.Text = "";
            CastsShadow.SelectedIndex = (objects[object_id].Shadow ? 1 : 0);
            IsBillboarded.SelectedIndex = (objects[object_id].Billboarded ? 1 : 0);
            Scale.Text = objects[object_id].Scale.ToString();
            SelectionSize.Text = objects[object_id].SelectionScaling.ToString();
        }

        private void ObjName_Validated(object sender, EventArgs e)
        {
            if (selected_id == -1)
                return;

            SFLuaEnvironment.objects.objects[selected_id].Name = ObjName.Text.ToString();
        }

        private void Mesh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selected_id == -1)
                return;

            var objects = SFLuaEnvironment.objects.objects;

            if (Mesh.SelectedIndex == -1)
                SelectedMesh.Text = "";
            else
                SelectedMesh.Text = objects[selected_id].Mesh[Mesh.SelectedIndex];
        }

        private void ButtonAddMesh_Click(object sender, EventArgs e)
        {
            if (selected_id == -1)
                return;

            var objects = SFLuaEnvironment.objects.objects;

            objects[selected_id].Mesh.Add("");
            Mesh.Items.Add(Utility.S_MISSING);

            Mesh.SelectedIndex = objects[selected_id].Mesh.Count - 1;
        }

        private void ButtonRemoveMesh_Click(object sender, EventArgs e)
        {
            if (selected_id == -1)
                return;
            if (Mesh.SelectedIndex == -1)
                return;

            var objects = SFLuaEnvironment.objects.objects;
            int cur_index = Mesh.SelectedIndex;

            objects[selected_id].Mesh.RemoveAt(Mesh.SelectedIndex);
            Mesh.Items.RemoveAt(Mesh.SelectedIndex);

            if (cur_index == Mesh.Items.Count)
                cur_index -= 1;
            Mesh.SelectedIndex = cur_index;
        }

        private void SelectedMesh_Validated(object sender, EventArgs e)
        {
            if (selected_id == -1)
                return;
            if (Mesh.SelectedIndex == -1)
                return;

            var objects = SFLuaEnvironment.objects.objects;

            objects[selected_id].Mesh[Mesh.SelectedIndex] = SelectedMesh.Text;
            Mesh.Items[Mesh.SelectedIndex] = SelectedMesh.Text;
        }

        private void CastsShadow_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selected_id == -1)
                return;

            SFLuaEnvironment.objects.objects[selected_id].Shadow = (CastsShadow.SelectedIndex == 1 ? true : false);
        }

        private void IsBillboarded_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selected_id == -1)
                return;

            SFLuaEnvironment.objects.objects[selected_id].Billboarded = (IsBillboarded.SelectedIndex == 1 ? true : false);
        }

        private void Scale_Validated(object sender, EventArgs e)
        {
            if (selected_id == -1)
                return;

            SFLuaEnvironment.objects.objects[selected_id].Scale =
                (double)Utility.TryParseUInt32(Scale.Text,
                                               (uint)SFLuaEnvironment.objects.objects[selected_id].Scale);
        }

        private void SelectionSize_Validated(object sender, EventArgs e)
        {
            if (selected_id == -1)
                return;
            SFLuaEnvironment.objects.objects[selected_id].SelectionScaling =
                Utility.TryParseDouble(SelectionSize.Text,
                                       SFLuaEnvironment.objects.objects[selected_id].SelectionScaling);
        }

        private void ObjectID_Validated(object sender, EventArgs e)
        {
            if (selected_id == -1)
                return;

            int previous_id = selected_id;
            int previous_index = ListObjects.SelectedIndex;
            lua_sql.SFLuaSQLObjectData obj = SFLuaEnvironment.objects.objects[previous_id];
            int new_id = (int)Utility.TryParseUInt32(ObjectID.Text, (uint)previous_id);
            if (new_id == previous_id)
                return;

            SFLuaEnvironment.objects.objects.Remove(selected_id);
            index_to_key.RemoveAt(previous_index);
            ListObjects.Items.RemoveAt(previous_index);

            int new_index = Utility.FindNewIndexOf(index_to_key, new_id);
            if (new_index == -1)
            {
                new_index = previous_index;
                new_id = previous_id;
                ObjectID.Text = previous_id.ToString();
            }

            index_to_key.Insert(new_index, new_id);
            SFLuaEnvironment.objects.objects.Add(new_id, obj);
            ListObjects.Items.Insert(new_index, new_id.ToString() + ". " + GetObjectString(new_id));

            selected_id = new_id;
            ListObjects.SelectedIndex = new_index;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if(index_to_key[0]==0)
            {
                selected_id = 0;
                ListObjects.SelectedIndex = 0;
                return;
            }

            lua_sql.SFLuaSQLObjectData obj = new lua_sql.SFLuaSQLObjectData();
            obj.Name = "";
            obj.Mesh = new List<string>();
            obj.Shadow = false;
            obj.Billboarded = false;
            obj.Scale = 100;
            obj.SelectionScaling = 1;

            index_to_key.Insert(0, 0);
            SFLuaEnvironment.objects.objects.Add(0, obj);
            ListObjects.Items.Insert(0, "0. " + GetObjectString(0));

            selected_id = 0;
            ListObjects.SelectedIndex = 0;
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            if (selected_id == -1)
                return;

            int ind = ListObjects.SelectedIndex;
            SFLuaEnvironment.objects.objects.Remove(selected_id);
            index_to_key.RemoveAt(ListObjects.SelectedIndex);
            ListObjects.Items.RemoveAt(ListObjects.SelectedIndex);

            if (ind == index_to_key.Count)
                ind -= 1;

            ListObjects.SelectedIndex = ind;
        }

        private void ButtonCancelChanges_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ButtonSaveChanges_Click(object sender, EventArgs e)
        {
            if (SFLuaEnvironment.objects.Save() != 0)
            {
                MessageBox.Show("Error while saving sql_object.lua");
            }
            this.Close();
        }
    }
}
