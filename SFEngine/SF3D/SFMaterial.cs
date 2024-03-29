﻿/*
 * SHMaterial is an important data blob, it contains data for how a model should be drawn
 * It also contains mesh vertex indices it affects
 * */

using OpenTK;

//expand!
namespace SFEngine.SF3D
{
    public enum RenderMode
    {
        ONE_ZERO,
        SRCALPHA_ONE,
        SRCALPHA_INVSRCALPHA,
        DESTCOLOR_ONE,
        ZERO_INVSRCCOLOR,
        DESTCOLOR_INVSRCALPHA,
        ONE_INVSRCALPHA,
        ONE_ONE,
        DESTCOLOR_SRCCOLOR,
        DESTCOLOR_ZERO
    }

    public class SFMaterial
    {
        public int _texID = -1;
        public byte unused_uchar = 0;
        public byte uv_mode = 0;
        public ushort unused_short2 = 0;
        public RenderMode texRenderMode = 0;
        public byte texAlpha = 255;
        public byte matFlags = 7;
        public float matDepthBias = 0;
        public float texTiling = 1f;
        public SFTexture texture = null;
        public Vector4 emission_color = new Vector4(1);
        public float emission_strength = 0.0f;
        public bool apply_shading = true;
        public bool casts_shadow = true;
        public bool transparent_pass = false;
        public bool additive_pass = false;
        public bool water_pass = false;
        public bool apply_shadow = true;
        public bool distance_fade = true;

        public bool yet_to_be_drawn = true;

        new public string ToString()
        {
            return "MATERIAL\nTEX_ID#" + _texID.ToString()
                + "\nUSER DATA#" + unused_uchar.ToString()
                + "\nUV MODE#" + uv_mode.ToString()
                + "\nRENDER MODE#" + texRenderMode.ToString()
                + "\nALPHA#" + texAlpha.ToString()
                + "\nMATERIAL FLAGS#" + matFlags.ToString()
                + "\nDEPTH BIAS#" + matDepthBias.ToString()
                + "\nTEXTURE TILING#" + texTiling.ToString();
        }
    }
}
