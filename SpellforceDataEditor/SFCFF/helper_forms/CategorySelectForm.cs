using SFEngine.SFCFF;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFCFF.helper_forms
{
    public partial class CategorySelectForm : Form
    {
        public Tuple<ushort, ushort> CategoryID = Tuple.Create<ushort, ushort>(0, 0);

        public CategorySelectForm()
        {
            InitializeComponent();
        }

        private void CategorySelectForm_Load(object sender, EventArgs e)
        {
            if (!SFCategoryManager.ready)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            Dictionary<Tuple<ushort, ushort>, ChunkFormatInfo> ChunkFormats = new Dictionary<Tuple<ushort, ushort>, ChunkFormatInfo>();
            foreach (var i in SFCategory.ChunkFormats)
            {
                ChunkFormats.Add(i.Key, i.Value);
            }

            // remove categories that exist
            SFGameData gd = SFCategoryManager.gamedata;

            foreach (var cat in gd.categories)
            {
                Tuple<ushort, ushort> k = new Tuple<ushort, ushort>((ushort)cat.Value.category_id, (ushort)cat.Value.category_type);
                if (ChunkFormats.ContainsKey(k))
                {
                    ChunkFormats.Remove(k);
                }
            }

            // list remaining categories
            foreach (var i in ChunkFormats)
            {
                // for now, only one version per category, so this is fine
                ListCategories.Items.Add(Tuple.Create(i.Key.Item1, i.Value.Name));
            }
        }

        private void ListCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListCategories.SelectedIndex == -1)
            {
                CategoryID = Tuple.Create<ushort, ushort>(0, 0);
                ButtonOK.Enabled = false;
                return;
            }

            ButtonOK.Enabled = true;

            // for now, only one version per category, so this is fine
            ushort cat_id = ((Tuple<ushort, string>)ListCategories.SelectedItem).Item1;
            ushort cat_type = UInt16.MaxValue;
            foreach (var i in SFCategory.ChunkFormats)
            {
                if (i.Key.Item1 == cat_id)
                {
                    cat_type = i.Key.Item2;
                    break;
                }
            }
            if (cat_type == UInt16.MaxValue)
            {
                CategoryID = Tuple.Create<ushort, ushort>(0, 0);
                ButtonOK.Enabled = false;
                return;
            }

            CategoryID = Tuple.Create<ushort, ushort>(cat_id, cat_type);
        }
    }
}
