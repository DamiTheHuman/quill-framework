using UnityEngine;

public enum BossPhase
{
    [Tooltip("The idle phase before entry")]
    Idle,
    [Tooltip("When the boss object is to move towards its start position")]
    Entry,
    [Tooltip("When the boss is actively attacking the opponent")]
    Attacking,
    [Tooltip("Generally when the boss is exploding")]
    Exploding,
    [Tooltip("When the boss has been defeated and exits the screen")]
    Exit,
}
