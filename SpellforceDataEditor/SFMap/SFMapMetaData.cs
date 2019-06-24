using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SFMap
{
    public enum SFMapType { CAMPAIGN, COOP, MULTIPLAYER }

    public enum SFMapNPCType { UNIT, OBJECT }

    public class SFMapSpawn
    {
        public SFCoord pos;
        public ushort text_id;
        public short unknown;
    }

    public class SFMapTeamPlayer
    {
        public int player_id;
        public ushort text_id;
        public string coop_map_type;
        public string coop_map_lvl;

        public SFMapTeamPlayer(int p_id, ushort t_id, string s1 = "", string s2 = "")
        {
            player_id = p_id;
            text_id = t_id;
            coop_map_type = s1;
            coop_map_lvl = s2;
        }
    }

    public class SFMapCoopAISpawn
    {
        public SFMapObject spawn_obj;
        public int spawn_id;
        public int spawn_certain; // not sure...

        public SFMapCoopAISpawn()
        {

        }

        public SFMapCoopAISpawn(SFMapObject _obj, int _id, int _certain)
        {
            spawn_obj = _obj;
            spawn_id = _id;
            spawn_certain = _certain;
        }
    }

    public class SFMapCoopSpawnParameters
    {
        public float param1, param2, param3, param4 = 0;
    }

    public class SFMapMultiplayerTeamComposition
    {
        public int team_count = 0;
        public List<List<SFMapTeamPlayer>> players = null;
    }

    public class SFMapMinimap
    {
        public int width, height;
        public byte[] texture_data = null;
        public int tex_id = -1;

        public void GenerateTexture()
        {
            tex_id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex_id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, texture_data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }

    public class SFMapMetaData
    {
        public SFMapType map_type;
        public int player_count = 0;   // actually spawn points for players, each bound to exactly one bindstone
        public List<SFMapSpawn> spawns = null;
        public SFMapMinimap minimap = null;
        public List<SFMapCoopSpawnParameters> coop_spawn_params = null;
        public List<SFMapCoopAISpawn> coop_spawns = null;
        public List<SFMapMultiplayerTeamComposition> multi_teams = null;

        public SFMapMetaData()
        {
            SFLua.SFLuaEnvironment.coop_spawns.Load();
        }

        public void Unload()
        {
            if(minimap != null)
            {
                if (minimap.tex_id != -1)
                    GL.DeleteTexture(minimap.tex_id);
            }
        }

        public int GetPlayerBySpawnPos(SFCoord pos)
        {
            if (spawns == null)
                spawns = new List<SFMapSpawn>();

            for (int i = 0; i < spawns.Count; i++)
                if (pos == spawns[i].pos)
                    return i;

            spawns.Add(new SFMapSpawn());
            spawns[spawns.Count - 1].pos = pos;
            return spawns.Count - 1;
        }

        public bool GetCoopAISpawnByObject(SFMapObject o, ref SFMapCoopAISpawn sp)
        {
            foreach (SFMapCoopAISpawn cs in coop_spawns)
                if (cs.spawn_obj == o)
                {
                    sp = cs;
                    return true;
                }
            return false;
        }

        public SFMapMultiplayerTeamComposition GetTeamCompByTeamNumber(int num)
        {
            for (int i = 0; i < multi_teams.Count; i++)
                if (multi_teams[i].team_count == num)
                    return multi_teams[i];
            return null;
        }

        public byte[] TeamsToArray(int min_teams, bool include_strings)
        {
            using (MemoryStream ms = new MemoryStream(4096))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for(int i = min_teams; i <= 4; i++)
                    {
                        SFMapMultiplayerTeamComposition team = GetTeamCompByTeamNumber(i);
                        if (team == null)
                            bw.Write(0);
                        else
                        {
                            for(int j = 0; j < i; j++)
                            {
                                bw.Write(team.players[j].Count);
                                for(int k = 0; k < team.players[j].Count; k++)
                                {
                                    bw.Write((short)spawns[team.players[j][k].player_id].pos.x);
                                    bw.Write((short)spawns[team.players[j][k].player_id].pos.y);
                                    bw.Write((short)team.players[j][k].text_id);
                                    if(include_strings)
                                    {
                                        Utility.WriteSFString(bw, team.players[j][k].coop_map_type);
                                        Utility.WriteSFString(bw, team.players[j][k].coop_map_lvl);
                                    }
                                }
                            }
                        }
                    }

                    return ms.GetBuffer().Take((int)ms.Position).ToArray();
                }
            }
        }
    }
}
