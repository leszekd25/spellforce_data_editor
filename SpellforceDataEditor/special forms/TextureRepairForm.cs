using System;
using System.Windows.Forms;
using System.IO;
using SFEngine.SF3D;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.special_forms
{
    public partial class TextureRepairForm : Form
    {
        string TextureDirectorySource = "";
        string TextureDirectoryDestination = "";

        public TextureRepairForm()
        {
            InitializeComponent();
        }

        private void ButtonTexDirectorySource_Click(object sender, EventArgs e)
        {
            if(SelectFolderDialog.ShowDialog() == DialogResult.OK)
            {
                TextureDirectorySource = SelectFolderDialog.SelectedPath;
                TextBoxTexDirectorySource.Text = TextureDirectorySource;
                ButtonRepairStart.Enabled = ((TextureDirectorySource != "") && (TextureDirectoryDestination != ""));
            }
        }

        private void ButtonTexDirectoryDestination_Click(object sender, EventArgs e)
        {
            if (SelectFolderDialog.ShowDialog() == DialogResult.OK)
            {
                TextureDirectoryDestination = SelectFolderDialog.SelectedPath;
                TextBoxTexDirectoryDestination.Text = TextureDirectoryDestination;
                ButtonRepairStart.Enabled = ((TextureDirectorySource != "") && (TextureDirectoryDestination != ""));
            }
        }

        public void UpdateStatus(int total_textures, int broken_textures, int fixed_textures)
        {
            LabelDescription.Text = "Total textures: " + total_textures.ToString() + "\r\nBroken textures: " + broken_textures.ToString() + "\r\nFixed textures:" + fixed_textures.ToString();
        }

        private void ButtonRepairStart_Click(object sender, EventArgs e)
        {
            if(!SFEngine.SF3D.SFRender.SFRenderEngine.initialized)
            {
                UpdateStatus(0, 0, 0);
                return;
            }
            // 1. find all textures in the folder
            string[] list_items = Directory.GetFiles(TextureDirectorySource, "*.dds", SearchOption.TopDirectoryOnly);
            if(list_items.Length == 0)
            {
                UpdateStatus(0, 0, 0);
                return;
            }
            Progress.Minimum = 0;
            Progress.Value = 0;
            Progress.Maximum = list_items.Length;

            int total_tex = list_items.Length;
            int broken_textures = 0;
            int fixed_textures = 0;

            // 2. one by one, fix textures
            for(int n = 0; n < list_items.Length; n++)
            {
                Progress.Value += 1;

                string fname = list_items[n];
                // 2.1. check if mipmap count matches texture size
                bool need_to_fix = false;
                uint[] header = new uint[32];
                try
                {
                    using (FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            br.BaseStream.Position = 0;

                            for (int i = 0; i < 32; i++)
                            {
                                header[i] = br.ReadUInt32();
                            }
                            if (header[0] != 0x20534444)
                            {
                                broken_textures += 1;
                                continue;
                            }

                            int width = (int)header[3];
                            int height = (int)header[4];
                            int mipMapC = (int)header[7];

                            int fixed_mipmaps = 1;
                            while ((width > 1) && (height > 1))
                            {
                                width /= 2;
                                height /= 2;
                                fixed_mipmaps += 1;
                            }
                            if(fixed_mipmaps != mipMapC)
                            {
                                need_to_fix = true;
                                broken_textures += 1;
                            }
                        }
                    }


                    if (need_to_fix)
                    {
                        SFTexture tex = new SFTexture();
                        byte[] data = File.ReadAllBytes(fname);

                        if(tex.Load(data, 0, new SFTexture.SFTextureLoadArgs() { IgnoreMipmapSettings = true }) != 0)
                        {
                            continue;
                        }
                        tex.Init();

                        // generate mipmaps
                        SFEngine.SF3D.SFRender.SFRenderEngine.SetTexture(0, TextureTarget.Texture2D, tex.tex_id);
                        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                        int mipcount = 1 + (int)Math.Floor(Math.Log(Math.Max(header[3], header[4]), 2));
                        int block_size = (tex.internal_format == InternalFormat.CompressedRgbaS3tcDxt1Ext ? 8 : 16); // only dxt1, dxt3 or dxt5; 
                        int size = 0;
                        int w = (int)header[3];
                        int h = (int)header[4];
                        // calculate size
                        for(int i = 0; i < mipcount; i++)
                        {
                            size += ((w + 3) / 4) * ((h + 3) / 4) * block_size;
                            w /= 2;
                            h /= 2;
                        }

                        // get mipmap data
                        byte[] mipmap_data = new byte[size];
                        w = (int)header[3];
                        h = (int)header[4];
                        int offset = 0;
                        for (int i = 0; i < mipcount; i++)
                        {
                            GL.GetCompressedTexImage(TextureTarget.Texture2D, i, ref mipmap_data[offset]);
                            offset += ((w + 3) / 4) * ((h + 3) / 4) * block_size;
                            w /= 2;
                            h /= 2;
                        }

                        // update header
                        header[5] = (uint)(((w + 3) / 4) * ((h + 3) / 4) * block_size);
                        header[7] = (uint)mipcount;
                        header[27] = 0x00401008;   // texture, mipmap, complex

                        // save the file
                        string new_fname = TextureDirectoryDestination + "\\" + Path.GetFileName(fname);
                        using (FileStream fs = new FileStream(new_fname, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                for(int i = 0; i < 32; i++)
                                {
                                    bw.Write(header[i]);
                                }
                                bw.Write(mipmap_data);
                            }
                        }

                        fixed_textures += 1;
                    }
                }
                catch(Exception)
                {
                    broken_textures += 1;
                    continue;
                }
            }

            UpdateStatus(total_tex, broken_textures, fixed_textures);
        }
    }
}
