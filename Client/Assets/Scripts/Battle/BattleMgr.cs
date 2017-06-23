using System.Collections.Generic;
using Battle.Data;
using Boo.Lang;
using Common;

namespace Battle
{
    public class BattleMgr : MonoSingleton<BattleMgr>
    {
        private BattleData _BattleData;
        public BattleData BData { get { return _BattleData; } }

        private BattleSceneData _SceneData;
        public BattleSceneData SceneData { get { return _SceneData; } }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            // check if any character need to take damage;
        }
    }
}
