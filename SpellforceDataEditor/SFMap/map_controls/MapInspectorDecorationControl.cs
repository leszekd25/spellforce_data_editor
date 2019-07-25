using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapInspectorDecorationControl : SpellforceDataEditor.SFMap.map_controls.MapInspectorBaseControl
    {
        HashSet<SFCoord> selection = new HashSet<SFCoord>();
        bool is_adding = false;

        int selected_group = -1;
        int selected_group_element = -1;

        public MapInspectorDecorationControl()
        {
            InitializeComponent();
        }

        private void MapInspectorDecorationControl_VisibleChanged(object sender, EventArgs e)
        {
            if (map == null)
                return;

            if (Visible)
            {
                ReloadGroupList();

                SelectGroup(selected_group);

                map.heightmap.OverlaySetVisible("DecorationTile", true);
            }
            else
                map.heightmap.OverlaySetVisible("DecorationTile", false);

            MainForm.mapedittool.update_render = true;
        }

        private void ReloadGroupList()
        {
            ListGroups.Items.Clear();
            ListGroupElements.Items.Clear();

            ListGroups.Items.Add("No group");
            for (int i = 1; i < 255; i++)
                ListGroups.Items.Add(i.ToString() + ". " + GetGroupString(map.decoration_manager.dec_groups[i]));
        }

        private string GetGroupString(SFMapDecorationGroup dec)
        {
            if (dec.dec_used == 0)
                return "(Unused)";
            return dec.dec_used.ToString() + " elements";
        }

        private void ReloadGroupElementList(int group_index)
        {
            ListGroupElements.Items.Clear();

            if ((group_index <= 0) || (group_index >= 255))
                return;
            SFMapDecorationGroup dec = map.decoration_manager.dec_groups[group_index];

            // index 0 is always empty
            for (int i = 1; i <= dec.dec_used; i++)
                ListGroupElements.Items.Add(GetGroupElementString(dec, i));
        }

        private string GetGroupElementString(SFMapDecorationGroup dec, int i)
        {
            if ((i < 0) || (i >= 30))
                return "";
            return dec.dec_id[i].ToString() + " (" + SFCFF.SFCategoryManager.GetObjectName(dec.dec_id[i]) + ")";
        }

        private void SelectGroup(int group_index)
        {
            if (group_index < 0)
                group_index = -1;
            if (group_index >= 255)
                group_index = -1;

            selected_group = group_index;
            selected_group_element = -1;
            ReloadGroupElementList(selected_group);

            map.heightmap.OverlayClear("DecorationTile");
            if((selected_group != -1)&&(selected_group != 0))
            {
                int k = 0;
                for(int i = 0; i < 1048576; i++)
                {
                    if (map.decoration_manager.dec_assignment[i] == selected_group)
                    {
                        map.heightmap.OverlayAdd("DecorationTile", map.decoration_manager.GetFixedDecPosition(i));
                        k += 1;
                    }
                }
                System.Diagnostics.Debug.WriteLine("DEC COUNT " + k.ToString());
                map.heightmap.RebuildOverlay(new SFCoord(0, 0), new SFCoord(map.width - 1, map.height - 1), "DecorationTile");
            }
            MainForm.mapedittool.update_render = true;
        }

        private void ListGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectGroup(ListGroups.SelectedIndex);
        }

        private void SelectGroupElement(int element_index)
        {
            if (selected_group == -1)
                return;
            SFMapDecorationGroup dec = map.decoration_manager.dec_groups[selected_group];

            if (element_index < 1)
                element_index = -1;
            if (element_index > dec.dec_used)
                element_index = -1;
            selected_group_element = element_index;

            if(selected_group_element == -1)
            {
                DecorationID.Text = "";
                DecorationWeight.Text = "";
            }
            else
            {
                DecorationID.Text = dec.dec_id[selected_group_element].ToString();
                DecorationWeight.Text = dec.weight[selected_group_element].ToString();
            }
        }


        private void ListGroupElements_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectGroupElement(ListGroupElements.SelectedIndex + 1);
        }

        public override void OnMouseDown(SFCoord clicked_pos, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                if (selected_group == -1)
                    return;

                is_adding = true;

                int size = (int)Math.Ceiling(DecorationBrush.brush.size);
                DecorationBrush.brush.center = clicked_pos;
                SFCoord topleft = new SFCoord(clicked_pos.x - size, clicked_pos.y - size);
                SFCoord bottomright = new SFCoord(clicked_pos.x + size, clicked_pos.y + size);
                if (topleft.x < 0)
                    topleft.x = 0;
                if (topleft.y < 0)
                    topleft.y = 0;
                if (bottomright.x >= map.width)
                    bottomright.x = (short)(map.width - 1);
                if (bottomright.y >= map.height)
                    bottomright.y = (short)(map.height - 1);

                for (int i = topleft.x; i <= bottomright.x; i++)
                {
                    for (int j = topleft.y; j <= bottomright.y; j++)
                    {
                        SFCoord p = new SFCoord(i, j);
                        if (DecorationBrush.brush.GetStrengthAt(p) == 0)
                            continue;

                        selection.Add(p);
                        map.heightmap.OverlayAdd("DecorationTile", new SFCoord(p.x, map.height - p.y - 1));
                    }
                }

                map.heightmap.RebuildOverlay(topleft, bottomright, "DecorationTile");
            }
            else if(button == MouseButtons.Right)
            {
                byte selected_dec_group = map.decoration_manager.GetFixedDecAssignment(new SFCoord(clicked_pos.x, map.height - clicked_pos.y - 1));
                ListGroups.SelectedIndex = selected_dec_group;
            }
        }

        public override void OnMouseUp()
        {
            if(is_adding)
            {
                HashSet<SFCoord> inverted_selection = new HashSet<SFCoord>();
                foreach (SFCoord p in selection)
                    inverted_selection.Add(new SFCoord(p.x, map.height - p.y - 1));

                map.decoration_manager.ModifyDecorations(inverted_selection, selected_group);

                selection.Clear();
                is_adding = false;

                MainForm.mapedittool.update_render = true;
            }
        }

        private void DecorationID_Validated(object sender, EventArgs e)
        {
            if (selected_group == -1)
                return;
            if (selected_group_element == -1)
                return;

            if (Utility.TryParseUInt16(DecorationID.Text) == 0)
                DecorationID.Text = map.decoration_manager.dec_groups[selected_group].dec_id[selected_group_element].ToString();
            else
                map.decoration_manager.dec_groups[selected_group].dec_id[selected_group_element] = Utility.TryParseUInt16(DecorationID.Text);

            ListGroupElements.Items[selected_group_element - 1] = GetGroupElementString(map.decoration_manager.dec_groups[selected_group], selected_group_element);
            map.decoration_manager.ModifyDecorations((byte)selected_group);
            MainForm.mapedittool.update_render = true;
        }

        private void DecorationWeight_Validated(object sender, EventArgs e)
        {
            if (selected_group == -1)
                return;
            if (selected_group_element == -1)
                return;

            if(Utility.TryParseUInt8(DecorationWeight.Text) == 0)
                DecorationWeight.Text = map.decoration_manager.dec_groups[selected_group].weight[selected_group_element].ToString();
            else
                map.decoration_manager.dec_groups[selected_group].weight[selected_group_element] = Utility.TryParseUInt8(DecorationWeight.Text);
        }

        private void ButtonAddElement_Click(object sender, EventArgs e)
        {
            if (selected_group == -1)
                return;

            SFMapDecorationGroup dec = map.decoration_manager.dec_groups[selected_group];
            ushort last_dec_id = dec.dec_id[dec.dec_used];
            byte w = 255;

            if(dec.AddDecoration(last_dec_id, w) != 0)
                return;
            map.decoration_manager.ModifyDecorations((byte)selected_group);
            ReloadGroupElementList(selected_group);
            ListGroupElements.SelectedIndex = dec.dec_used-1;
            ListGroups.Items[selected_group] = GetGroupString(map.decoration_manager.dec_groups[selected_group]);
            MainForm.mapedittool.update_render = true;
        }

        private void ButtonRemoveElement_Click(object sender, EventArgs e)
        {
            if (selected_group == -1)
                return;

            SFMapDecorationGroup dec = map.decoration_manager.dec_groups[selected_group];

            if (dec.RemoveDecoration(selected_group_element) != 0)
                return;
            if (selected_group_element > dec.dec_used)
                selected_group_element = dec.dec_used;
            map.decoration_manager.ModifyDecorations((byte)selected_group);
            ReloadGroupElementList(selected_group);
            ListGroupElements.SelectedIndex = selected_group_element - 1;
            ListGroups.Items[selected_group] = GetGroupString(map.decoration_manager.dec_groups[selected_group]);
            MainForm.mapedittool.update_render = true;
        }
    }
}
