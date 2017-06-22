using Battle.Data;
using Common;

namespace Battle
{
    public class BattleMgr : Singleton<BattleMgr>
    {
        private BattleSceneData _SceneData;
        public BattleSceneData SceneData { get { return _SceneData; } }

    }
}
