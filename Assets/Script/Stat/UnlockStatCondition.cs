using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnlockStatCondition : ScriptableObject
{
    public abstract bool IsUnlock(object Playerdata);
}
