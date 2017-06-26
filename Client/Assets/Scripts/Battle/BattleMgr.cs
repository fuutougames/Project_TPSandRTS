using System.Collections.Generic;
using Battle.Data;
using Common;

namespace Battle
{
    public class BattleMgr : MonoSingleton<BattleMgr>
    {
        private BattleData _BattleData;
        public BattleData BData { get { return _BattleData; } }

        private BattleSceneData _SceneData;
        public BattleSceneData SceneData { get { return _SceneData; } }

        public void Reset()
        {
            if (BData == null)
                _BattleData = new BattleData();
            _BattleData.Reset();

            if (SceneData == null)
                _SceneData = new BattleSceneData();
            _SceneData.Reset();
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            Reset();
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            // check if any character need to take damage;
            _BattleData.UpdateProjectiles();
        }
    }
}
