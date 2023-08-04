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

        GameObject.FindObjectOfType<GameData>().OpenPopup(this);

        UI_CharacterInfoItem _Rating = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _Rating.HeadText = "µî±Þ";
        _Rating.ContentText = Managers.Game.GetDisplayRank();

        UI_CharacterInfoItem _Damage = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _Damage.HeadText = "°ø°Ý·Â";
        _Damage.ContentText = Util.GetBigIntegerUnit(Managers.Game.Damage);

        UI_CharacterInfoItem _CriticalRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CriticalRate.HeadText = "Ä¡¸íÈ®·ü";
        _CriticalRate.ContentText = ((float)(Managers.Game.GetCriticalHitRate)/ 10.0f).ToString()+ " %";
        //

        UI_CharacterInfoItem _CoolTime = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CoolTime.HeadText = "½ºÅ³ÄðÅ¸ÀÓ °¨¼Ò";
        _CoolTime.ContentText = $"{Managers.Game.GetSkillCoolTimeString}%";

        UI_CharacterInfoItem _GoldDropRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _GoldDropRate.HeadText = "°ñµå È¹µæ·ü";
        _GoldDropRate.ContentText = $"{Managers.Game.GetGoldDropRate}%";

        UI_CharacterInfoItem _JumpingRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _JumpingRate.HeadText = "Á¡ÇÎ È®·ü";
        _JumpingRate.ContentText = $"{Managers.Game.GetJumpingRate}%";

        UI_CharacterInfoItem _PierceCountRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _PierceCountRate.HeadText = "Ãß°¡ Å¸°Ý È®·ü";
        _PierceCountRate.ContentText = $"{100}%";

        UI_CharacterInfoItem _DPS = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _DPS.HeadText = "DPS";
        _DPS.ContentText = Util.GetBigIntegerUnit(Managers.Game.DPS);

        UI_CharacterInfoItem _AttackSpeed = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _AttackSpeed.HeadText = "°ø°Ý¼Óµµ";
        _AttackSpeed.ContentText = Managers.Game.AttackSpeed.ToString() + " %";

        UI_CharacterInfoItem _CriticalHit = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CriticalHit.HeadText = "Ä¡¸íÇÇÇØ";
        _CriticalHit.ContentText = Util.GetBigIntegerUnit(Managers.Game.CriticalHitMultiplier) + " %";

        UI_CharacterInfoItem _SkillAttack = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _SkillAttack.HeadText = "½ºÅ³°ø°Ý·Â Áõ°¡";
        _SkillAttack.ContentText = $"{Managers.Game.GetSkillMultipier - 100}%";

        UI_CharacterInfoItem _CPDropRate = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _CPDropRate.HeadText = "CP È¹µæ·ü";
        _CPDropRate.ContentText = $"{Managers.Game.GetCraftDropRate}%";

        UI_CharacterInfoItem _JumpingCount = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _JumpingCount.HeadText = "Á¡ÇÎ Ãþ";
        _JumpingCount.ContentText = Managers.Game.GetJumpingCount.ToString();

        UI_CharacterInfoItem _PierceCount = Util.GetOrAddComponent<UI_CharacterInfoItem>(Managers.Resource.Instantiate("UI/SubItem/CharacterInfoPopup/UI_CharacterInfoItem", Get<GameObject>((int)GameObjects.ContentPanel).transform));
        _PierceCount.HeadText = "Ãß°¡ Å¸°Ý";
        _PierceCount.ContentText = $"¸ó½ºÅÍ Å¸°Ý + {Managers.Game.GetPierceCount}";

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => { ClosePopupUI(); });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => { ClosePopupUI(); });

    }
}
