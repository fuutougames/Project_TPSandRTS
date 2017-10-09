
namespace GameEvents
{
    public class WindowStartUpEvent
    {
        public int ModuleID;
        // start up param which will pass to WindowBase.StartUp;
        public object[] Params;

        public WindowStartUpEvent()
        {
            //Params = new Dictionary<string, string>();
        }
    }
}
