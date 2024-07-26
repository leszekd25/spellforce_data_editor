namespace SpellforceDataEditor.SFMap
{
    public class SFMapQuickSelectHelper
    {
        public short cat_id;
        public ushort[] ID { get; } = new ushort[10];

        public SFMapQuickSelectHelper()
        {
            ID.Initialize();
        }
    }
}
