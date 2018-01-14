using System.Collections.Generic;
using Battle.Data;
using Common;

namespace Battle
{
    public class BattleMgr : MonoSingleton<BattleMgr>
    {
        // 
        private BattleData _BattleData;
        public BattleData BData { get { return _BattleData; } }

        public void Reset()
        {
            if (BData == null)
                _BattleData = new BattleData();
            _BattleData.Reset();
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            Reset();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            // check if any character need to take damage;
            _BattleData.Update();
        }
    }
}
