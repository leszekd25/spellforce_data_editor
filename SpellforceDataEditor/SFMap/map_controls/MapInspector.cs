using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapInspector : UserControl
    {
        public SFEngine.SFMap.SFMap map = null;
        public SFMap.SFMapSelectionHelper selection_helper = null;

        public MapInspector()
        {
            InitializeComponent();
        }

        public virtual void OnSelect(object o)
        {

        }
    }
}
