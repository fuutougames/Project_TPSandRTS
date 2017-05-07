using System.Collections.Generic;

public struct WindowInfo
{
    public int ModuleID;
    public string ResName;
    public bool UniqeWindow;
}

public static class WindowInfoMgr
{
    private static Dictionary<int, WindowInfo> m_dictWinInfoMapper = new Dictionary<int, WindowInfo>()
    {
        {
            UIModule.LOGIN,
            new WindowInfo
            {
                ModuleID = UIModule.LOGIN,
                ResName = "UGUILogin",
                UniqeWindow = true
            }
        }
    };

    public static WindowInfo GetWindowInfo(int moduleId)
    {
        WindowInfo info;
        if (m_dictWinInfoMapper.TryGetValue(moduleId, out info))
        {
            return info;
        }
        return default(WindowInfo);
    }
}
