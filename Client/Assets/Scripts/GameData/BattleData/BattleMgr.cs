using System.Collections.Generic;
using Battle.Data;
using Boo.Lang;
using Common;

namespace Battle
{
    public class BattleMgr : Singleton<BattleMgr>
    {
        private BattleSceneData _SceneData;
        public BattleSceneData SceneData { get { return _SceneData; } }
    }
}
