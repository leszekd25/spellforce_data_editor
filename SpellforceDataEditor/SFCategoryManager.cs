using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor
{
    class SFCategoryManager
    {
        private SFCategory[] categories;
        private int categoryNumber = 49;
        private Byte[] mainHeader;
        public SFCategoryManager()
        {
            categories = new SFCategory[categoryNumber];
            for(int i = 1; i <= categoryNumber; i++)
            {
                categories[i-1] = Assembly.GetExecutingAssembly().CreateInstance("SpellforceDataEditor.SFCategory" + i.ToString()) as SFCategory;
            }
            mainHeader = new Byte[20];
        }
        public SFCategory get_category(int index)
        {
            return categories[index];
        }
        public void load_cff(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            mainHeader = br.ReadBytes(mainHeader.Length);
            foreach(SFCategory category in categories)
            {
                category.read(br);
            }
        }
        public int get_category_number()
        {
            return categoryNumber;
        }
    }
}
