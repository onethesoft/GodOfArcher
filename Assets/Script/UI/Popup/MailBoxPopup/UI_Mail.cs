using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Mail : UI_Base
{
    Mail _data;
    public Mail setData { set { _data = value; } }

    public delegate void OnConsumeHandler(UI_Mail consumedMail);
    public OnConsumeHandler OnConsume = null;

    public delegate void OnStatusChangedHandler(UI_Mail mail , Mail.Status prevStatus , Mail.Status currentStatus);
    public OnStatusChangedHandler OnStatusChanged = null;

    public enum Status
    {
        Normal , 
        RequestUnLock
    }



    enum Images
    {
        IconBackground,
        Icon
    }
    enum Texts
    {
        DisplayName,
        Description
    }
    enum Buttons
    {
        Button
    }
    enum GameObjects
    {
        ButtonBlocker
    }

    Mail.Status _status = Mail.Status.Normal;
    public Mail.Status CurrentStatus 
    { 
        get 
        {
            return _data.status;
        } 
        
    }
    
    public void Setup(Mail mail)
    {
        _data = mail;
    }

  


    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        if(_data != null)
        {
            GetText((int)Texts.Description).text = _data.Description;
            GetText((int)Texts.DisplayName).text = _data.DisplayName;

            GetImage((int)Images.Icon).sprite = _data.Icon;
            GetImage((int)Images.IconBackground).sprite = _data.BackgroundIcon;

            UpdateMail();

            _data.OnChanged -= OnMailStatusChangedHandler;
            _data.OnChanged += OnMailStatusChangedHandler;



            _data.OnRevokeNotifier -= OnConsumed;
            _data.OnRevokeNotifier += OnConsumed;
        }

        AddUIEvent(GetButton((int)Buttons.Button).gameObject, (data) => {
            if (_data.status == Mail.Status.UnLocking || _data.status == Mail.Status.CompleteUnLocked)
                return;

            _data.UnLocking();
            Managers.Item.UnLockItems(_data, (grantedItems) => {

                _data.CompleteUnLock();
                _data.Consume(1);

            });
           
            /*
            List<BaseItem> _items = Managers.Item.UnLockItems(_data);
            if (_items.Count != 0)
            {
                UI_RandomboxPopup ui_randombox = Managers.UI.ShowPopupUI<UI_RandomboxPopup>();
                ui_randombox.Setup(_items);
            }
            */

        });
    }

    public void OnConsumed(BaseItem Mail)
    {
        if (Mail == _data)
        {
            _data.OnChanged -= OnMailStatusChangedHandler;
            _data.OnRevokeNotifier -= OnConsumed;
            OnConsume?.Invoke(this);
            Managers.Resource.Destroy(gameObject);
        }
    }

    public void SetEnableButtonBlock(bool isActive)
    {
        Get<GameObject>((int)GameObjects.ButtonBlocker).SetActive(isActive);
    }
    void OnMailStatusChangedHandler(Mail.Status prevStatus, Mail.Status currentStatus)
    {
        UpdateMail();
        OnStatusChanged?.Invoke(this, prevStatus, currentStatus);
    }

    void UpdateMail()
    {
        if(_data != null)
        {
            if(_data.status == Mail.Status.Normal)
            {
                SetEnableButtonBlock(false);
            }
            else
            {
                SetEnableButtonBlock(true);
            }
        }
    }
    private void OnDestroy()
    {
        if (_data != null)
        {
            _data.OnChanged -= OnMailStatusChangedHandler;
            _data.OnRevokeNotifier -= OnConsumed;
        }

        OnConsume = null;
        OnStatusChanged = null;
    }
}
