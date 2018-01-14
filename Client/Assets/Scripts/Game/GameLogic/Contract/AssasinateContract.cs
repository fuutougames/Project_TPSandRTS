using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContractLogics
{
    public class AssasinateContract : ContractBase
    {
        private bool _TargetAssasinated = false;

        public AssasinateContract(int confID) : base (confID)
        {
            _Type = CONTRACT_TYPE.ASSASINATE;
        }

        public override void AddListeners()
        {
            base.AddListeners();

            Dispatcher.RegisterHandler(BattleEvent.ENEMY_KILLED, OnEnemyKilled);
            Dispatcher.RegisterHandler(BattleEvent.ENEMY_ESCAPED, OnEnemyEscaped);
            Dispatcher.RegisterHandler(BattleEvent.ALLY_ALL_EVACUATE, OnAllEvacuated);
        }

        public override void RemoveListeners()
        {
            base.RemoveListeners();

            Dispatcher.UnregisterHandler(BattleEvent.ENEMY_KILLED, OnEnemyKilled);
            Dispatcher.UnregisterHandler(BattleEvent.ENEMY_ESCAPED, OnEnemyEscaped);
            Dispatcher.UnregisterHandler(BattleEvent.ALLY_ALL_EVACUATE, OnAllEvacuated);
        }

        public override bool IsContractComplete()
        {
            return base.IsContractComplete();
        }

        private void OnEnemyKilled(params object[] paramArr)
        {
            // if not target killed, return
            if (true)
                return;

            _TargetAssasinated = true;
        }

        private void OnEnemyEscaped(params object[] paramArr)
        {
            // if not target escaped, return
            if (true)
                return;

            // if target escaped, contract failed
            Dispatcher.Dispatch(ContractEvent.CONTRACT_FAILED, _Uid);
        }

        private void OnAllEvacuated(params object[] paramArr)
        {
            // if not evacuate point, return
            if (true)
                return;

            if (!_TargetAssasinated)
                return;

            Dispatcher.Dispatch(ContractEvent.CONTRACT_COMPLETE, _Uid);
        }
    }
}
