using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SFEngine.SFMap
{
    public enum SFMapType { CAMPAIGN, COOP, MULTIPLAYER }

    public enum SFMapNPCType { UNIT, OBJECT }

    public class SFMapSpawn
    {
        public int bindstone_index;
        public SFCoord pos;
        public ushort text_id;
        public short unknown;
    }

    public class SFMapTeamPlayer
    {
        public int player_id;     // spawn index in spawn array
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
        public byte[] data = null;

        public void FromBitmap(System.Drawing.Bitmap bmp)
        {
            width = bmp.Width;
            height = bmp.Height;
            data = new byte[width * height * 3]; // RGB8 
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            var col = bmp.GetPixel(j, height - 1 - i);
                            bw.Write(col.B);
                            bw.Write(col.G);
                            bw.Write(col.R);
                        }
                    }
                }
            }
        }

        public System.Drawing.Bitmap ToBitmap()
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height);
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            byte b = br.ReadByte();
                            byte g = br.ReadByte();
                            byte r = br.ReadByte();
                            bmp.SetPixel(j, height - 1 - i, System.Drawing.Color.FromArgb(r, g, b));
                        }
                    }
                }
            }

            return bmp;
        }
    }

    public enum SFMapMinimapSource { ORIGINAL = 0, EDITOR, CUSTOM }

    public class SFMapMetaData
    {
        public SFMapType map_type;
        public int player_count = 0;   // actually spawn points for players, each bound to exactly one bindstone
        public List<SFMapSpawn> spawns = new List<SFMapSpawn>();

        public SFMapMinimap original_minimap = null;
        public SFMapMinimap new_minimap = null;
        public SFMapMinimap custom_minimap = null;
        public SFMapMinimapSource minimap_source = SFMapMinimapSource.ORIGINAL;

        public List<SFMapCoopSpawnParameters> coop_spawn_params = new List<SFMapCoopSpawnParameters>()
            {
                new SFMapCoopSpawnParameters(),
                new SFMapCoopSpawnParameters(),
                new SFMapCoopSpawnParameters()
            };
        public List<SFMapCoopAISpawn> coop_spawns = null;
        public List<SFMapMultiplayerTeamComposition> multi_teams = null;

        public SFMapMetaData()
        {
            coop_spawn_params[0].param1 = 1; coop_spawn_params[1].param1 = 1.5f; coop_spawn_params[2].param1 = 2;
            coop_spawn_params[0].param2 = 1; coop_spawn_params[1].param2 = 1.5f; coop_spawn_params[2].param2 = 2;
            coop_spawn_params[0].param3 = 1; coop_spawn_params[1].param3 = 0.7f; coop_spawn_params[2].param3 = 0.5f;
            coop_spawn_params[0].param4 = 1; coop_spawn_params[1].param4 = 0.7f; coop_spawn_params[2].param4 = 0.5f;
        }

        public void Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapMetaData.Unload() called");
        }

        public int CreateNewPlayer(SFCoord pos)
        {
            spawns.Add(new SFMapSpawn());
            spawns[spawns.Count - 1].pos = pos;
            return spawns.Count - 1;
        }

        public int FindPlayerBySpawnPos(SFCoord pos)
        {
            if (spawns == null)
            {
                spawns = new List<SFMapSpawn>();
            }

            for (int i = 0; i < spawns.Count; i++)
            {
                if (pos == spawns[i].pos)
                {
                    return i;
                }
            }

            return Utility.NO_INDEX;
        }

        public int FindPlayerByBindstoneIndex(int index)
        {
            if (spawns == null)
            {
                spawns = new List<SFMapSpawn>();
            }

            for (int i = 0; i < spawns.Count; i++)
            {
                if (index == spawns[i].bindstone_index)
                {
                    return i;
                }
            }

            return Utility.NO_INDEX;
        }


        public bool GetCoopAISpawnByObject(SFMapObject o, ref SFMapCoopAISpawn sp)
        {
            foreach (SFMapCoopAISpawn cs in coop_spawns)
            {
                if (cs.spawn_obj == o)
                {
                    sp = cs;
                    return true;
                }
            }

            return false;
        }

        // only use this to add team comps!
        public void InsertTeamComp(SFMapMultiplayerTeamComposition tc)
        {
            for (int i = 0; i < multi_teams.Count; i++)
            {
                if (tc.team_count == multi_teams[i].team_count)
                {
                    throw new Exception("SFMapMetaData.InsertTeamComp(): Can't add another team comp with same team count!");
                }
                else if (tc.team_count < multi_teams[i].team_count)
                {
                    multi_teams.Insert(i, tc);
                    return;
                }
            }

            // if new team comp has the biggest team count, append the list with it
            multi_teams.Add(tc);
        }

        public SFMapMultiplayerTeamComposition GetTeamCompByTeamNumber(int num)
        {
            for (int i = 0; i < multi_teams.Count; i++)
            {
                if (multi_teams[i].team_count == num)
                {
                    return multi_teams[i];
                }
            }

            return null;
        }

        public byte[] TeamsToArray(int min_teams, bool include_strings)
        {
            using (MemoryStream ms = new MemoryStream(4096))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = min_teams; i <= 4; i++)
                    {
                        SFMapMultiplayerTeamComposition team = GetTeamCompByTeamNumber(i);
                        if (team == null)
                        {
                            bw.Write(0);
                        }
                        else
                        {
                            for (int j = 0; j < i; j++)
                            {
                                bw.Write(team.players[j].Count);
                                for (int k = 0; k < team.players[j].Count; k++)
                                {
                                    bw.Write((short)spawns[team.players[j][k].player_id].pos.x);
                                    bw.Write((short)spawns[team.players[j][k].player_id].pos.y);
                                    bw.Write((short)team.players[j][k].text_id);
                                    if (include_strings)
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

        public bool IsPlayerActive(int p_id)
        {
            if (multi_teams == null)
            {
                return false;
            }

            foreach (SFMapMultiplayerTeamComposition teamcomp in multi_teams)
            {
                foreach (List<SFMapTeamPlayer> list in teamcomp.players)
                {
                    foreach (SFMapTeamPlayer player in list)
                    {
                        if (p_id == player.player_id)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
