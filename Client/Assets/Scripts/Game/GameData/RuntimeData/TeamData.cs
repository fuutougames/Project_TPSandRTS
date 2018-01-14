using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    using Battle.Data;
    public class TeamData : DataModuleBase
    {
        /// <summary>
        /// Team members
        /// </summary>
        private Dictionary<int, PawnData> _TeamMembers;

        /// <summary>
        /// teams which will join the mission
        /// </summary>
        private Dictionary<int, List<int>> _TacticalTeams;
    }
}
