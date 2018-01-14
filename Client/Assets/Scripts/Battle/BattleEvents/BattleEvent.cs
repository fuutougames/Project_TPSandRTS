
public class BattleEvent
{
    /// <summary>
    /// event will dispatch when an enemy is killed
    /// params : #1 uid, #2 enemy config
    /// </summary>
    public const string ENEMY_KILLED = "BATTLE_EVENT_ENEMY_KILLED";

    /// <summary>
    /// event will dispatch when special enemy is trying to escape from battlefield
    /// params : #1 uid, #2 enemy config
    /// </summary>
    public const string ENEMY_START_ESCAPE = "BATTLE_EVENT_ENEMY_START_ESCAPE";

    /// <summary>
    /// event will dispatch when special enemy is escaped from battlefield
    /// params : #1 uid, #2 enemy config
    /// </summary>
    public const string ENEMY_ESCAPED = "BATTLE_EVENT_ENEMY_ESCAPED";

    /// <summary>
    /// event will dispatch when enemy spot you or your ally.
    /// event will dispatch when special trap is triggered by you or your ally
    /// params : None
    /// </summary>
    public const string ALARM_TRIGGERED = "BATTLE_EVENT_ALARM_TRIGGERED";

    /// <summary>
    /// event will dispatch when reach some special location
    /// params : #1 point uid, #2 point config
    /// </summary>
    public const string REACH_POINT = "BATTLE_EVENT_REACH_POINT";

    /// <summary>
    /// event will dispatch when special object is destroyed
    /// params : #1 object uid
    /// </summary>
    public const string TARGET_DESTROY = "BATTLE_EVENT_TARGET_DESTROY";

    /// <summary>
    /// event will dispatch when an ally is killed
    /// params : #1 pawn uid
    /// </summary>
    public const string ALLY_FALLEN = "BATTLE_EVENT_ALLY_FALLEN";

    /// <summary>
    /// event will dispatch when one of your pawns is evacuated
    /// params : #1 pawn uid
    /// </summary>
    public const string ALLY_EVACUATE = "BATTLE_EVENT_ALLY_EVACUATE";

    /// <summary>
    /// event will dispatch when all your pawns is evacuated
    /// params : None
    /// </summary>
    public const string ALLY_ALL_EVACUATE = "BATTLE_EVENT_ALLY_ALL_EVACUATE";
}
