using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "스킬/Activator/SkillActivator", fileName = "기본스킬")]
public class SkillActivator : Activator
{
    List<Vector3> _skillPos = new List<Vector3>();

    public override void DoActivate(SkillData data)
    {
        _skillPos.Clear();

        if (Managers.Game.GetTotalTarget().Count == 0) return;

        System.Numerics.BigInteger _skillDamage = Managers.Game.GetSkillDamage;

        foreach (GameObject monsters in Managers.Game.GetTotalTarget())
        {
            if (Managers.Setting.ShowEffect)
                if (_skillPos.All(x => x.x != monsters.transform.position.x))
                {

                    Managers.Resource.Instantiate(data.Effect).transform.position = monsters.transform.position;
                    _skillPos.Add(monsters.transform.position);


                }

            MonsterController _controller = monsters.GetComponent<MonsterController>();

            Managers.Game.Hit(_controller, Define.DamageType.Skill, _skillDamage);

        }
    }

    
}
