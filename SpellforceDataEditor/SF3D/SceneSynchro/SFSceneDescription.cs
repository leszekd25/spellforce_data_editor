/*
 * SFSceneDescription consists of multiple SFSceneDescriptionLine structures and a SFSceneDescriptionMeta object
 * Each SFSceneDescriptionLine is a command to be parsed
 * SFSceneDescriptionMeta contains scene metadata, additonal info for scene
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SF3D.SceneSynchro
{
    public enum SCENE_ITEM_TYPE { OBJ_SIMPLE = 0, OBJ_ANIMATED, OBJ_BONE, OBJ_POSITION, OBJ_ROTATION, SCENE_ANIM };

    public struct SFSceneDescriptionLine
    {
        public SCENE_ITEM_TYPE type;
        public List<String> args;

        public SFSceneDescriptionLine(SCENE_ITEM_TYPE t, string[] a)
        {
            type = t;
            args = a.ToList<String>();
        }

        public bool is_valid()
        {
            if (args == null)
                return false;
            if (type == SCENE_ITEM_TYPE.OBJ_SIMPLE)
                return args.Count == 3; //mesh name, parent name, obj name
            if (type == SCENE_ITEM_TYPE.OBJ_ANIMATED)
                return args.Count == 3; //skel name, parent name, obj name
            if (type == SCENE_ITEM_TYPE.OBJ_BONE)
                return args.Count == 3; //obj_anim name, obj_anim skel bone name, obj name
            if (type == SCENE_ITEM_TYPE.OBJ_POSITION)
                return args.Count == 4; //obj name, x, y, z
            if (type == SCENE_ITEM_TYPE.SCENE_ANIM)
                return args.Count == 1; //1 if animated, 0 otherwise
            return false;
        }
    }

    public class SFSceneDescriptionMeta
    {
        public bool is_animated = false;     //true if scene contains an animation
        public float duration = 0f;
        public bool animation_repeat = true;
        public Dictionary<string, string> obj_to_anim = new Dictionary<string, string>(); //list of all objects with corresponding animations
    }

    public class SFSceneDescription
    {
        public SFSceneDescriptionMeta meta { get; } = new SFSceneDescriptionMeta();
        List<SFSceneDescriptionLine> lines = new List<SFSceneDescriptionLine>();

        public void add_line(SFSceneDescriptionLine sl)
        {
            lines.Add(sl);
        }

        public void add_line(SCENE_ITEM_TYPE t, string[] a)
        {
            lines.Add(new SFSceneDescriptionLine(t, a));
        }

        public List<SFSceneDescriptionLine> get_lines()
        {
            return lines;
        }
    }
}
