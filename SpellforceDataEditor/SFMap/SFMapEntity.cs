﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapEntity
    {
        public int id = -1;
        public int game_id = -1;
        public int npc_id = 0;
        public SFCoord grid_position = new SFCoord(0, 0);
        public int angle = 0;
        public SF3D.SceneSynchro.SceneNode node = null;

        public virtual string GetName() { return ""; }

        public override string ToString()
        {
            return GetName();
        }
    }
}
