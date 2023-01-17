namespace SFEngine.SFMap
{
    public class SFMapQuickSelectHelper
    {
        public ushort[] ID { get; } = new ushort[10];

        public SFMapQuickSelectHelper()
        {
            ID.Initialize();
        }
    }
}
