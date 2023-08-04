using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_MailBox : UI_Popup
{
    enum Buttons
    {
        Close ,
        Exit,
        AllReceiveButton
    }
    enum GameObjects
    {
        Content,
        AllReceiveButtonBlocker
    }
    List<UI_Mail> _mailList;
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        _mailList = new List<UI_Mail>();
        List<Mail> mails = Managers.Game.GetInventory().FindAll(x => x is Mail).Select(x => x as Mail).ToList();

        Get<GameObject>((int)GameObjects.AllReceiveButtonBlocker).SetActive(false);
       
        foreach (Mail mail in mails)
        {
            if (string.IsNullOrEmpty(mail.ItemInstanceId)) continue;
            UI_Mail _mail = Util.GetOrAddComponent<UI_Mail>(Managers.Resource.Instantiate("UI/SubItem/MailBoxPopup/UI_Mail", Get<GameObject>((int)GameObjects.Content).transform));
            _mail.setData = mail;
            _mail.Setup(mail);
            //_mail.OnStatusChanged -= OnMailStatusChangedHandler;
            //_mail.OnStatusChanged += OnMailStatusChangedHandler;

         


            _mail.OnConsume += (UI_Mail consumed) => 
            {
                consumed.OnConsume = null; 
                _mailList.Remove(consumed);
            };

            _mailList.Add(_mail);
        }

        

        AddUIEvent(GetButton((int)Buttons.Close).gameObject, (data) => { ClosePopupUI(); });
        AddUIEvent(GetButton((int)Buttons.Exit).gameObject, (data) => { ClosePopupUI(); });


        // 루비 모두 수령
        AddUIEvent(GetButton((int)Buttons.AllReceiveButton).gameObject, (data) => {

            List<Mail> _getMails = Managers.Game.GetInventory().FindAll(x => x is Mail).Select(x => x as Mail).ToList();
            if (_getMails.Count == 0)
                return;
            if (_getMails.Any(x => ( x.status == Mail.Status.UnLocking || x.status == Mail.Status.CompleteUnLocked ) ))
                return;

            List<Mail> _itemIds = _getMails.Where(x => x.ItemId.Contains(Define.CurrencyID.Ruby.ToString()) || x.ItemId.Contains("Key")).ToList();
            
            foreach(Mail itemid in _itemIds)
            {
                itemid.UnLocking();

                Managers.Item.UnLockItems(itemid, (grantedItems) => {
                 
                    itemid.CompleteUnLock();
                    itemid.Consume(1);

                });
            }
             

            //Managers.Item.UnlockItemsWithItemId("MailBox_Ruby5000", _getMails);
            



        });
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    
    void OnMailStatusChangedHandler(UI_Mail sender , Mail.Status prevStatus, Mail.Status currentStatus)
    {
        if(currentStatus == Mail.Status.UnLocking)
        {
            foreach (UI_Mail mail in _mailList)
            {
                if(sender != mail)
                    mail.SetEnableButtonBlock(true);
            }
        }
        else if(currentStatus == Mail.Status.CompleteUnLocked)
        {
            foreach (UI_Mail mail in _mailList)
            {
                if (sender != mail)
                    mail.SetEnableButtonBlock(false);
            }
        }
        /*
        if(currentStatus == Mail.Status.RequestUnLock)
        {
            foreach(UI_Mail mail in _mailList)
            {
                mail.SetEnableButtonBlock(true);
            }
        }
        else if(currentStatus == UI_Mail.Status.Normal)
        {
            foreach (UI_Mail mail in _mailList)
            {
                mail.SetEnableButtonBlock(false);
            }
        }
        */
    }

   



}
