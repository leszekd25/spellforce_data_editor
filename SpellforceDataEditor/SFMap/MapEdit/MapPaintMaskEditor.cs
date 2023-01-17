using SFEngine.SFMap;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapPaintMaskEditor : MapEditor
    {
        bool first_click = false;
        SFCoord first_position = new SFCoord(-1, -1);
        public MapBrush Brush { get; set; }
        public HashSet<SFCoord> cells = new HashSet<SFCoord>();

        public override void OnMousePress(SFCoord pos, MouseButtons button, ref special_forms.SpecialKeysPressed specials)
        {
            if ((button != MouseButtons.Left) && (button != MouseButtons.Right))
            {
                return;
            }

            if (!first_click)
            {
                first_click = true;
                first_position = pos;
                switch (MainForm.mapedittool.mask_source)
                {
                    case special_forms.MapEditorForm.MapMaskSource.FEATURE:
                        {
                            if (button == MouseButtons.Left)
                            {
                                switch (MainForm.mapedittool.mask_feature)
                                {
                                    case special_forms.MapEditorForm.MapMaskFeature.BUILDING:
                                        MainForm.mapedittool.external_SelectMaskFeature(map.FindBuildingApprox(first_position));
                                        break;
                                    case special_forms.MapEditorForm.MapMaskFeature.LAKE:
                                        int lake_index = map.lake_manager.GetLakeIndexAt(first_position);
                                        if (lake_index == SFEngine.Utility.NO_INDEX)
                                        {
                                            MainForm.mapedittool.external_DeselectMaskFeature();
                                        }
                                        else
                                        {
                                            MainForm.mapedittool.external_SelectMaskFeature(map.lake_manager.lakes[lake_index]);
                                        }

                                        break;
                                    case special_forms.MapEditorForm.MapMaskFeature.OBJECT:
                                        // objects
                                        SFMapObject ob = map.FindObjectApprox(first_position);
                                        if (ob != null)
                                        {
                                            MainForm.mapedittool.external_SelectMaskFeature(ob);
                                            break;
                                        }
                                        // monuments, bindstones
                                        SFMapInteractiveObject int_ob = null;
                                        for (int i = 0; i < map.int_object_manager.int_objects.Count; i++)
                                        {
                                            if (SFCoord.Distance(map.int_object_manager.int_objects[i].grid_position, first_position) <= 3)
                                            {
                                                int_ob = map.int_object_manager.int_objects[i];
                                                MainForm.mapedittool.external_SelectMaskFeature(int_ob);
                                                break;
                                            }
                                        }
                                        if (int_ob != null)
                                        {
                                            break;
                                        }
                                        // portals
                                        SFMapPortal ptl = null;
                                        for (int i = 0; i < map.portal_manager.portals.Count; i++)
                                        {
                                            if (SFCoord.Distance(map.portal_manager.portals[i].grid_position, first_position) <= 3)
                                            {
                                                ptl = map.portal_manager.portals[i];
                                                MainForm.mapedittool.external_SelectMaskFeature(ptl);
                                                break;
                                            }
                                        }
                                        if (ptl != null)
                                        {
                                            break;
                                        }

                                        // nothing selected
                                        MainForm.mapedittool.external_DeselectMaskFeature();
                                        break;
                                    case special_forms.MapEditorForm.MapMaskFeature.WALKABLE:
                                        MainForm.mapedittool.external_SelectMaskFeature(first_position);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else if (button == MouseButtons.Right)
                            {
                                if (MainForm.mapedittool.mask_feature == special_forms.MapEditorForm.MapMaskFeature.LAKE)
                                {
                                    int lake_index = map.lake_manager.GetLakeIndexAt(first_position);
                                    if (lake_index == SFEngine.Utility.NO_INDEX)
                                    {
                                        MainForm.mapedittool.external_DeselectMaskFeature();
                                    }
                                    else
                                    {
                                        SFMapLake lake = map.lake_manager.lakes[lake_index];
                                        ushort lvl = (ushort)(map.heightmap.GetHeightAt(lake.start.x, lake.start.y) + lake.z_diff);
                                        ushort h2 = map.heightmap.GetHeightAt(pos.x, pos.y);
                                        if (h2 < lvl)
                                        {
                                            MainForm.mapedittool.external_MaskSetValue(lvl - h2);
                                        }
                                    }
                                }
                                else
                                {
                                    MainForm.mapedittool.external_DeselectMaskFeature();
                                }
                            }
                        }
                        MainForm.mapedittool.EvaluateMask();
                        break;

                    default:
                        break;
                }
            }

            switch (MainForm.mapedittool.mask_source)
            {
                case special_forms.MapEditorForm.MapMaskSource.PAINT:
                    {
                        int size = (int)Brush.size;
                        Brush.center = pos;
                        SFCoord topleft = new SFCoord(pos.x - size, pos.y - size);
                        SFCoord bottomright = new SFCoord(pos.x + size, pos.y + size);
                        if (topleft.x < 0)
                        {
                            topleft.x = 0;
                        }

                        if (topleft.y < 0)
                        {
                            topleft.y = 0;
                        }

                        if (bottomright.x >= map.width)
                        {
                            bottomright.x = (short)(map.width - 1);
                        }

                        if (bottomright.y >= map.height)
                        {
                            bottomright.y = (short)(map.height - 1);
                        }

                        if (button == MouseButtons.Left)
                        {
                            for (int i = topleft.x; i <= bottomright.x; i++)
                            {
                                for (int j = topleft.y; j <= bottomright.y; j++)
                                {
                                    SFCoord p = new SFCoord(i, j);
                                    if (Brush.GetInvertedDistanceNormalized(p) == 1f)
                                    {
                                        continue;
                                    }

                                    map.heightmap.SetFlag(p, SFMapHeightMapFlag.EDITOR_SELECTION, true);
                                    cells.Add(p);
                                }
                            }
                        }
                        else
                        {
                            for (int i = topleft.x; i <= bottomright.x; i++)
                            {
                                for (int j = topleft.y; j <= bottomright.y; j++)
                                {
                                    SFCoord p = new SFCoord(i, j);
                                    if (Brush.GetInvertedDistanceNormalized(p) == 1f)
                                    {
                                        continue;
                                    }

                                    map.heightmap.SetFlag(p, SFMapHeightMapFlag.EDITOR_SELECTION, false);
                                    cells.Remove(p);
                                }
                            }
                        }

                        map.heightmap.RefreshOverlay();
                    }
                    break;
                case special_forms.MapEditorForm.MapMaskSource.HEIGHT:
                    ushort height = map.heightmap.GetHeightAt(pos.x, pos.y);
                    MainForm.mapedittool.external_MaskSetValue(height);
                    break;
                case special_forms.MapEditorForm.MapMaskSource.SLOPE:
                    float h = map.heightmap.GetVertexNormal(pos.x, pos.y).Y;
                    MainForm.mapedittool.external_MaskSetValue((int)(Math.Acos(h) * 180 / Math.PI));
                    break;
                case special_forms.MapEditorForm.MapMaskSource.TEXTURE:
                    byte tex = map.heightmap.GetTile(pos);
                    MainForm.mapedittool.external_MaskSetValue(tex);
                    break;
                case special_forms.MapEditorForm.MapMaskSource.ATTRIBUTE:
                    {
                        switch (MainForm.mapedittool.mask_attribute)
                        {
                            case special_forms.MapEditorForm.MapMaskAttribute.LAKE:
                                {
                                    int lake_index = map.lake_manager.GetLakeIndexAt(first_position);
                                    if (lake_index != SFEngine.Utility.NO_INDEX)
                                    {
                                        SFMapLake lake = map.lake_manager.lakes[lake_index];
                                        ushort lvl = (ushort)(map.heightmap.GetHeightAt(lake.start.x, lake.start.y) + lake.z_diff);
                                        ushort h2 = map.heightmap.GetHeightAt(pos.x, pos.y);
                                        if (h2 < lvl)
                                        {
                                            MainForm.mapedittool.external_MaskSetValue(lvl - h2);
                                        }
                                    }
                                }
                                break;
                            default:
                                return;
                        }
                    }
                    break;
                default:
                    return;
            }
        }

        public override void OnMouseUp(MouseButtons b)
        {
            if ((b != MouseButtons.Left) && (b != MouseButtons.Right))
            {
                return;
            }

            switch (MainForm.mapedittool.mask_source)
            {
                case special_forms.MapEditorForm.MapMaskSource.PAINT:
                case special_forms.MapEditorForm.MapMaskSource.HEIGHT:
                case special_forms.MapEditorForm.MapMaskSource.SLOPE:
                case special_forms.MapEditorForm.MapMaskSource.TEXTURE:
                    MainForm.mapedittool.EvaluateMask();
                    break;
                case special_forms.MapEditorForm.MapMaskSource.ATTRIBUTE:
                    switch (MainForm.mapedittool.mask_attribute)
                    {
                        case special_forms.MapEditorForm.MapMaskAttribute.LAKE:
                            MainForm.mapedittool.EvaluateMask();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            first_click = false;
            SFCoord first_position = new SFCoord(-1, -1);

            base.OnMouseUp(b);
        }
    }
}
