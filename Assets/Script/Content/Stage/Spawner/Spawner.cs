using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Spawner : ScriptableObject
{
    public abstract System.Numerics.BigInteger GetClearGold(int stage);
    public abstract System.Numerics.BigInteger GetMonsterHP(int stage);
    public abstract void DoSpawn();

    public abstract Vector3 GetSpawnPosition(int i);
}

