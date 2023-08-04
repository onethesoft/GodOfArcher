using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Buff : UI_Base
{
    [SerializeField]
    Buff _adbuff;

    [SerializeField]
    Buff _iapbuff;

    [SerializeField]
    Buff _beginnerbuff;

    WaitForSeconds _delay;
   
    enum Images
    {
        Icon
    }
    enum Texts
    {
        Description
    }
    Coroutine _coroutine = null;
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        _delay = new WaitForSeconds(1.0f);

        Debug.Assert(_adbuff != null, "UI_Buff buff not setup");
        Debug.Assert(_iapbuff != null, "UI_Buff Iapbuff not setup");
        Debug.Assert(_beginnerbuff != null, "UI_Buff beginnerbuff not setup");

        GetImage((int)Images.Icon).sprite = _adbuff.Icon;

        UpdateStatus();
        GetText((int)Texts.Description).fontSize = _adbuff.FontSize;

        _adbuff.OffOutline.AddOutline(gameObject);

        Managers.Game.GetInventory().OnItemChanged -= UpdateStatus;
        Managers.Game.GetInventory().OnItemChanged += UpdateStatus;

        _coroutine = StartCoroutine(ActionUpdate());
        
        

        AddUIEvent(gameObject, (data) => {
            if (!(Managers.Scene.CurrentScene is MainScene))
                return;
            if (Managers.Game.GetInventory().IsFindItem(x => (x.ItemId == _adbuff.ItemId) || (x.ItemId == _iapbuff.ItemId) || (x.ItemId == _beginnerbuff.ItemId)))
                return;


            UI_AdConfirm _showmessage = Managers.UI.ShowPopupUI<UI_AdConfirm>();
            _showmessage.TitleText = "버프 광고";
            _showmessage.ContentText = "버프는 30분간 유지됩니다.";
            _showmessage.OnOK += () => {

                //Managers.Ad.ShowRewardVideoIronSource
                if(Managers.Ad.ShowAd(_adbuff.ItemId , ()=> {
                    Debug.Log("BuffButton " + GlobalTime.Now);
                    List<BaseItem> _buffs = Managers.Item.GrantItemToUser(_adbuff.ItemId);

                    if (Managers.Network.IS_ENABLE_NETWORK == true)
                        Managers.Network.GrantItems(_buffs.Select(x => x.ToGrantItem()).ToList(), (result) => {
                            Managers.Game.GetInventory().Find(x => x.ItemId == _adbuff.ItemId).Setup(Managers.Player.GetPlayer(Managers.Game.PlayerId).Payload.UserInventory.Where(x => x.ItemId == _adbuff.ItemId).FirstOrDefault());

                        });
                }))
                {

                }
                
                
            };
           
        });


    }
    
    IEnumerator ActionUpdate()
    {
        while (true)
        {
            yield return _delay;
            UpdateStatus();
        }
    }
    void UpdateStatus()
    {
        
        string descriptionText;

        if(Managers.Game.GetInventory().IsFindItem(_iapbuff.ItemId))
        {
            Buff _findBuff = Managers.Game.GetInventory().Find(x=>x.ItemId == _iapbuff.ItemId) as Buff;
            string timeStamp = "--:--";
            if (_findBuff.RemainingTime().HasValue == true)
            {
                if (_findBuff.RemainingTime().Value.TotalMinutes > 1.0f)
                    timeStamp = _findBuff.RemainingTime().Value.ToString(@"hh\:mm");
                else
                    timeStamp = _findBuff.RemainingTime().Value.ToString(@"mm\:ss");
            }

            descriptionText = $" {_adbuff.Description.Replace("\\n", System.Environment.NewLine)}{(_findBuff.type == Buff.Type.AutoAttack ? "" : "x")}{(_findBuff.StatModifier.Count != 0 ? _findBuff.StatModifier[0].Value.ToString() : "") }{System.Environment.NewLine} {timeStamp}";

            GetComponent<Image>().sprite = _adbuff.OnBackground;
            GetText((int)Texts.Description).text = descriptionText;
        }
        else if (Managers.Game.GetInventory().IsFindItem(_beginnerbuff.ItemId))
        {
            Buff _findBuff = Managers.Game.GetInventory().Find(x => x.ItemId == _beginnerbuff.ItemId) as Buff;
            string timeStamp = "--:--";
            if (_findBuff.RemainingTime().HasValue == true)
            {
                if (_findBuff.RemainingTime().Value.TotalMinutes > 1.0f)
                    timeStamp = _findBuff.RemainingTime().Value.ToString(@"hh\:mm");
                else
                    timeStamp = _findBuff.RemainingTime().Value.ToString(@"mm\:ss");
            }

            descriptionText = $" {_beginnerbuff.Description.Replace("\\n", System.Environment.NewLine)}{(_findBuff.type == Buff.Type.AutoAttack ? "" : "x")}{(_findBuff.StatModifier.Count != 0 ? _findBuff.StatModifier[0].Value.ToString() : "") }{System.Environment.NewLine} {timeStamp}";

            GetComponent<Image>().sprite = _beginnerbuff.OnBackground;
            GetText((int)Texts.Description).text = descriptionText;
        }
        else if(Managers.Game.GetInventory().IsFindItem(_adbuff.ItemId))
        {
            Buff _findBuff = Managers.Game.GetInventory().Find(x => x.ItemId == _adbuff.ItemId) as Buff;

            string timeStamp = "--:--";
            if (_findBuff.RemainingTime().HasValue == true)
            {
                if (_findBuff.RemainingTime().Value.TotalMinutes > 1.0f)
                    timeStamp = _findBuff.RemainingTime().Value.ToString(@"hh\:mm");
                else
                    timeStamp = _findBuff.RemainingTime().Value.ToString(@"mm\:ss");
            }

            descriptionText = $" {_adbuff.Description.Replace("\\n", System.Environment.NewLine)}{(_findBuff.type == Buff.Type.AutoAttack ? "" : "x")}{(_findBuff.StatModifier.Count != 0 ? _findBuff.StatModifier[0].Value.ToString() : "") }{System.Environment.NewLine} {timeStamp}";

            GetComponent<Image>().sprite = _adbuff.OnBackground;
            GetText((int)Texts.Description).text = descriptionText;
        }
        else
        {
            descriptionText = $" {_adbuff.Description.Replace("\\n", System.Environment.NewLine)}{(_adbuff.type == Buff.Type.AutoAttack ? "" : "x")}{(_adbuff.StatModifier.Count != 0 ? _adbuff.StatModifier[0].Value.ToString() : "") }{System.Environment.NewLine}30M";
            GetComponent<Image>().sprite = _adbuff.OffBackground;
            GetText((int)Texts.Description).text = descriptionText;
        }
        
        
            
      
    }

    private void OnDestroy()
    {
        StopCoroutine(_coroutine);
        Managers.Game.GetInventory().OnItemChanged -= UpdateStatus;
    }


}
