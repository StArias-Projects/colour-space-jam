using System.Collections.Generic;
using UnityEngine;

public struct Stats 
{
    public uint time;
    public uint projectilesReflected;
    public Dictionary<EnemyType, uint> enemiesKilled;
}
