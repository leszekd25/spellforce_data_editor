using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public delegate void OnButtonPress(int id);

    public partial class MapTerrainTextureControl : UserControl
    {
        public int ID { get; set; } = SFEngine.Utility.NO_INDEX;
        public OnButtonPress delegate_onpress = null;

        public MapTerrainTextureControl()
        {
            InitializeComponent();
        }

        private void MapTerrainTextureControl_Resize(object sender, EventArgs e)
        {
            ButtonTextureImage.Size = new Size(this.Width - 6, this.Width - 6);
            ButtonTextureImage.Location = new Point(3, 3);
            ButtonTextureID.Size = new Size(this.Width - 6, 16);
            ButtonTextureID.Location = new Point(3, this.Width);
        }

        public void SetImage(Image im, int tex_id)
        {
            ButtonTextureImage.Image = im;
            ButtonTextureID.Text = tex_id.ToString();
        }

        public void ResizeWidth(int w)
        {
            Size = new Size(w, w + 16);
        }

        private void ButtonTextureImage_Click(object sender, EventArgs e)
        {
            delegate_onpress?.Invoke(ID);
        }
    }
}
