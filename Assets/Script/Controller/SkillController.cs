using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [SerializeField]
    int _ticks;
    public int Tick => _ticks;

    [SerializeField]
    float _tickTime = 0.25f;
    public float TickTime => _tickTime;

    int _progressTicCount = 3;


    private void OnParticleSystemStopped()
    {
        Managers.Resource.Destroy(gameObject);
    }
    private void OnEnable()
    {
        _progressTicCount = _ticks;
       // StartCoroutine(Hit());
    }
    IEnumerator Hit()
    {
        
        yield return new WaitForSeconds(0.1f);
        while (_progressTicCount > 0)
        {
            if (Managers.Game.GetTotalTarget().Count == 0)
                yield break;

            foreach (GameObject obj in Managers.Game.GetTotalTarget())
            {
                MonsterController _controller = obj.GetComponent<MonsterController>();
                if (_controller.State == Define.State.Idle)
                {
                    if(Managers.Game.IsCriticalHit)
                        _controller.OnHit(Managers.Game.CriticalHitDamage, Define.DamageType.Skill);
                    else
                        _controller.OnHit(Managers.Game.Damage, Define.DamageType.Skill);
                } //Managers.Game.IsCriticalHit? Define.DamageType.Critical : Define.DamageType.Normal;
                //_damage = _damageType == Define.DamageType.Normal ? Managers.Game.Damage : Managers.Game.CriticalHitDamage;
                
            }
            
            
            _progressTicCount--;
            yield return new WaitForSeconds(_tickTime);

        }
        
    }
   
}
