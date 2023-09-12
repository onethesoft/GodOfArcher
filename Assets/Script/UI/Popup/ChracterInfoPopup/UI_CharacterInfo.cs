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
        _Rating.HeadText = "���";
        _Rating.ContentText = $"{Managers.Game.GetDisplayRank()} + {_playerData.ReviveLevel}";

        UI_CharacterInfoItem _Damage = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _Damage.HeadText = "���ݷ� ����";
        _Damage.ContentText = Util.GetBigIntegerUnit(Managers.Game.Damage);

        UI_CharacterInfoItem _AttackSpeed = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _AttackSpeed.HeadText = "���ݼӵ� ����";
        _AttackSpeed.ContentText = Managers.Game.AttackSpeed.ToString() + " %";

        UI_CharacterInfoItem _CriticalRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CriticalRate.HeadText = "ġ��Ȯ�� ����";
        _CriticalRate.ContentText = ((float)(Managers.Game.GetCriticalHitRate)/ 10.0f).ToString()+ " %";

         UI_CharacterInfoItem _CriticalHit = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CriticalHit.HeadText = "ġ������ ����";
        _CriticalHit.ContentText = Util.GetBigIntegerUnit(Managers.Game.CriticalHitMultiplier) + " %";
        //

        UI_CharacterInfoItem _CoolTime = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CoolTime.HeadText = "��ų��Ÿ�� ����";
        _CoolTime.ContentText = $"{Managers.Game.GetSkillCoolTimeString}%";

        UI_CharacterInfoItem _SkillAttack = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _SkillAttack.HeadText = "��ų���ݷ� ����";
        _SkillAttack.ContentText = $"{Managers.Game.GetSkillMultipier - 100}%";

        UI_CharacterInfoItem _GoldDropRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _GoldDropRate.HeadText = "���ȹ��� ����";
        _GoldDropRate.ContentText = $"{Managers.Game.GetGoldDropRate}%";

         UI_CharacterInfoItem _CPDropRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CPDropRate.HeadText = "CPȹ��� ����";
        _CPDropRate.ContentText = $"{Managers.Game.GetCraftDropRate}%";

        UI_CharacterInfoItem _DPS = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _DPS.HeadText = "DPS";
        _DPS.ContentText = Util.GetBigIntegerUnit(Managers.Game.DPS);

        UI_CharacterInfoItem _JumpingRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _JumpingRate.HeadText = "���� Ȯ��";
        _JumpingRate.ContentText = $"{Managers.Game.GetJumpingRate}%";

        UI_CharacterInfoItem _JumpingCount = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _JumpingCount.HeadText = "���� ��";
        _JumpingCount.ContentText = Managers.Game.GetJumpingCount.ToString();

        UI_CharacterInfoItem _PierceCountRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _PierceCountRate.HeadText = "�߰� Ÿ�� Ȯ��";
        _PierceCountRate.ContentText = $"{100}%";

        UI_CharacterInfoItem _PierceCount = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _PierceCount.HeadText = "�߰� Ÿ��";
        _PierceCount.ContentText = $"���� Ÿ�� + {Managers.Game.GetPierceCount}";

        UI_CharacterInfoItem _AttackAmp= Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _AttackAmp.HeadText = "���ݷ� ����";
        _AttackAmp.ContentText = $"{Managers.Game.GetAttackAmp }%";

        UI_CharacterInfoItem _CriticalHitAmp = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CriticalHitAmp.HeadText = "ġ������ ����";
        _CriticalHitAmp.ContentText = $"{Managers.Game.GetCriticalHitAmp }%";

        UI_CharacterInfoItem _SkillAttackAmp = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _SkillAttackAmp.HeadText = "��ų���ݷ� ����";
        _SkillAttackAmp.ContentText = $"{Managers.Game.GetSkillAttackAmp}%";

        UI_CharacterInfoItem _AllAttackAmp = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _AllAttackAmp.HeadText = "��� ���� ����";
        _AllAttackAmp.ContentText = $"{Managers.Game.GetAllAttackAmp }%";

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => { ClosePopupUI(); });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => { ClosePopupUI(); });

    }
}
