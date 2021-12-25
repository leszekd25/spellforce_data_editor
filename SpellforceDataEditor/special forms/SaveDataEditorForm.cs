using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFEngine.SFChunk;
using System.IO;

namespace SpellforceDataEditor.special_forms
{


    public partial class SaveDataEditorForm : Form
    {
        public SaveDataEditorForm()
        {
            InitializeComponent();
        }

        private string GetFileChunkDescription(SFChunkFileChunk sfcfc)
        {
            return "Chunk ID " + sfcfc.header.ChunkID.ToString() + " Type " + sfcfc.header.ChunkDataType.ToString() + " Occ #" + (sfcfc.header.ChunkOccurence + 1).ToString();
        }

        private void UnpackToNode(TreeNode tn, SFChunkFile sfcf)
        {
            foreach(var c in sfcf.GetAllChunks())
            {
                int i = tn.Nodes.Add(new TreeNode(GetFileChunkDescription(c)));
                tn.Nodes[i].Tag = c;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenSave.ShowDialog() != DialogResult.OK)
                return;

            // clear
            TreeChunks.Nodes.Clear();

            // load
            TreeChunks.Nodes.Add(new TreeNode("root"));
            SFChunkFile sfcf = new SFChunkFile();
            sfcf.OpenFile(OpenSave.FileName);
            UnpackToNode(TreeChunks.Nodes[0], sfcf);
            sfcf.Close();
        }

        private void TreeChunks_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode tn = e.Node;
            if (tn == null)
                return;

            if (tn.Tag == null)
                return;
            if (!(tn.Tag is SFChunkFileChunk))
                return;

            SFChunkFileChunk sfcfc = (SFChunkFileChunk)(tn.Tag);
            LabelChunkData.Text = "Chunk ID: " + sfcfc.header.ChunkID.ToString()
                + ", chunk type: " + sfcfc.header.ChunkDataType.ToString()
                + ", chunk occurence: " + sfcfc.header.ChunkOccurence.ToString()
                + ", is compressed: " + sfcfc.header.ChunkIsPacked.ToString()
                + ", chunk data size: " + sfcfc.header.ChunkDataLength.ToString()
                + ", unpacked data size: " + sfcfc.get_original_data_length().ToString();
        }

        private List<int> find_potential_chunkfile_pos(byte[] data)
        {
            List<int> ret = new List<int>();
            using(MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int i = 0;
                    while(i<(br.BaseStream.Length-4))
                    {
                        br.BaseStream.Position = i;
                        if (br.ReadByte() == 0x12)
                        {
                            br.BaseStream.Position = i;
                            if (br.ReadInt32() == -579674862)
                                ret.Add(i);
                        }
                        i++;
                    }
                }
            }

            return ret;
        }

        private void ButtonUnpack_Click(object sender, EventArgs e)
        {
            TreeNode tn = TreeChunks.SelectedNode;
            if (tn == null)
                return;

            if (tn.Tag == null)
                return;
            if (!(tn.Tag is SFChunkFileChunk))
                return;

            SFChunkFileChunk sfcfc = (SFChunkFileChunk)(tn.Tag);
            SFChunkFile sfcf = null;
            try
            {
                byte[] dt = sfcfc.get_raw_data();
                List<int> offset = find_potential_chunkfile_pos(dt);
                foreach(var o in offset)
                {
                    byte[] dt2 = dt.Skip(o).ToArray();

                    sfcf = new SFChunkFile();

                    if (sfcf.OpenRaw(dt2) != 0)
                    {
                        sfcf.Close();
                        sfcf = null;
                    }
                    else
                        break;
                }
            }
            catch(Exception)
            {
                if(sfcf != null)
                    sfcf.Close();
                sfcf = null;
            }

            if (sfcf == null)
                return;

            UnpackToNode(tn, sfcf);
            sfcf.Close();
        }

        private void ButtonExtract_Click(object sender, EventArgs e)
        {
            TreeNode tn = TreeChunks.SelectedNode;
            if (tn == null)
                return;

            if (tn.Tag == null)
                return;
            if (!(tn.Tag is SFChunkFileChunk))
                return;

            SFChunkFileChunk sfcfc = (SFChunkFileChunk)(tn.Tag);
            byte[] dt = sfcfc.get_raw_data();
            if (dt == null)
                return;

            FileStream fs = new FileStream("chunk_output", FileMode.Create, FileAccess.Write);
            fs.Write(dt, 0, dt.Length);
            fs.Close();
        }
    }
}
