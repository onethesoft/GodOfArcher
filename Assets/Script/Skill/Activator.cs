using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activator : ScriptableObject
{
    public abstract void DoActivate(SkillData data);
}
