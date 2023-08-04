using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class UI_SkillButton : UI_Base
{
    enum Images
    {
        Skill_CooltimeImage
        
    }
    enum Texts
    {
        Skill_CooltimeText
    }

    

    SkillData _skill;
  
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));


        _skill = FindObjectOfType<GameData>().SkillSet.First();
        _skill.Reset();

        GetText((int)Texts.Skill_CooltimeText).text = $"스킬 {Managers.Game.GetSkillCoolTime: 0.0}";

        AddUIEvent(gameObject, (data) => {
            if (Managers.Game.IsAuto == false && _skill.Status == Define.SK_Status.Ready)
                _skill.Activate();
        });
     

    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    private void Update()
    {
        if(_skill.Status == Define.SK_Status.Ready)
        {
            GetText((int)Texts.Skill_CooltimeText).text = $"스킬 {_skill.CoolTime: 0.0}";
            if (Managers.Scene.CurrentScene.CurrentState == BaseScene.GameState.Run)
            {
                if(Managers.Game.IsAuto)
                    _skill.Activate();
                return;
            }
        }
        else if(_skill.Status == Define.SK_Status.Cooldown)
        {
            _skill.AddCoolDown(Time.deltaTime);
            GetImage((int)Images.Skill_CooltimeImage).fillAmount = (float)(_skill.CurrentCoolTime / _skill.CoolTime);
            GetText((int)Texts.Skill_CooltimeText).text = $"스킬 {_skill.CurrentCoolTime: 0.0}s";

        }
        else if(_skill.Status == Define.SK_Status.GlobalCooldown)
        {
            _skill.AddCoolDown(Time.deltaTime);
            GetImage((int)Images.Skill_CooltimeImage).fillAmount = 0.0f;
            GetText((int)Texts.Skill_CooltimeText).text = $"스킬 {0.0}s";
         
        }
        
    }

    private void OnDestroy()
    {
        _skill = null;
    }


}
