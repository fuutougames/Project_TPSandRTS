
namespace GameEvents
{
    public class WinStartUpData
    {
        public int ModuleID;
        // start up param which will pass to WindowBase.StartUp;
        public object[] Params;

        public WinStartUpData()
        {
            //Params = new Dictionary<string, string>();
        }
    }
}
