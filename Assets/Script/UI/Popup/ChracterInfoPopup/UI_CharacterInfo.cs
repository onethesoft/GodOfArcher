using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterInfo : UI_Popup
{

    enum Buttons
    {
        Close ,
        Exit
    }
    enum GameObjects
    {
        ContentPanel
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        GameData _playerData = FindObjectOfType<GameData>();
        _playerData.OpenPopup(this);

        UI_CharacterInfoItem _Rating = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _Rating.HeadText = "등급";
        _Rating.ContentText = $"{Managers.Game.GetDisplayRank()} + {_playerData.ReviveLevel}";

        UI_CharacterInfoItem _Damage = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _Damage.HeadText = "공격력 증가";
        _Damage.ContentText = Util.GetBigIntegerUnit(Managers.Game.Damage);

        UI_CharacterInfoItem _AttackSpeed = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _AttackSpeed.HeadText = "공격속도 증가";
        _AttackSpeed.ContentText = Managers.Game.AttackSpeed.ToString() + " %";

        UI_CharacterInfoItem _CriticalRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CriticalRate.HeadText = "치명확률 증가";
        _CriticalRate.ContentText = ((float)(Managers.Game.GetCriticalHitRate)/ 10.0f).ToString()+ " %";

         UI_CharacterInfoItem _CriticalHit = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CriticalHit.HeadText = "치명피해 증가";
        _CriticalHit.ContentText = Util.GetBigIntegerUnit(Managers.Game.CriticalHitMultiplier) + " %";
        //

        UI_CharacterInfoItem _CoolTime = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CoolTime.HeadText = "스킬쿨타임 감소";
        _CoolTime.ContentText = $"{Managers.Game.GetSkillCoolTimeString}%";

        UI_CharacterInfoItem _SkillAttack = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _SkillAttack.HeadText = "스킬공격력 증가";
        _SkillAttack.ContentText = $"{Managers.Game.GetSkillMultipier - 100}%";

        UI_CharacterInfoItem _GoldDropRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _GoldDropRate.HeadText = "골드획득률 증가";
        _GoldDropRate.ContentText = $"{Managers.Game.GetGoldDropRate}%";

         UI_CharacterInfoItem _CPDropRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CPDropRate.HeadText = "CP획득률 증가";
        _CPDropRate.ContentText = $"{Managers.Game.GetCraftDropRate}%";

        UI_CharacterInfoItem _DPS = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _DPS.HeadText = "DPS";
        _DPS.ContentText = Util.GetBigIntegerUnit(Managers.Game.DPS);

        UI_CharacterInfoItem _JumpingRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _JumpingRate.HeadText = "점핑 확률";
        _JumpingRate.ContentText = $"{Managers.Game.GetJumpingRate}%";

        UI_CharacterInfoItem _JumpingCount = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _JumpingCount.HeadText = "점핑 층";
        _JumpingCount.ContentText = Managers.Game.GetJumpingCount.ToString();

        UI_CharacterInfoItem _PierceCountRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _PierceCountRate.HeadText = "추가 타격 확률";
        _PierceCountRate.ContentText = $"{100}%";

        UI_CharacterInfoItem _PierceCount = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _PierceCount.HeadText = "추가 타격";
        _PierceCount.ContentText = $"몬스터 타격 + {Managers.Game.GetPierceCount}";

        UI_CharacterInfoItem _AttackAmp= Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _AttackAmp.HeadText = "공격력 증폭";
        _AttackAmp.ContentText = $"{Managers.Game.GetAttackAmp }%";

        UI_CharacterInfoItem _CriticalHitAmp = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CriticalHitAmp.HeadText = "치명피해 증폭";
        _CriticalHitAmp.ContentText = $"{Managers.Game.GetCriticalHitAmp }%";

        UI_CharacterInfoItem _SkillAttackAmp = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _SkillAttackAmp.HeadText = "스킬공격력 증폭";
        _SkillAttackAmp.ContentText = $"{Managers.Game.GetSkillAttackAmp}%";

        UI_CharacterInfoItem _AllAttackAmp = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _AllAttackAmp.HeadText = "모든 피해 증폭";
        _AllAttackAmp.ContentText = $"{Managers.Game.GetAllAttackAmp }%";

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => { ClosePopupUI(); });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => { ClosePopupUI(); });

    }
}
