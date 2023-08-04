using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="아이템/우편")]
public class Mail : BaseItem
{
    public delegate void OnChangedStatus(Status prevStatus, Status status);
    public event OnChangedStatus OnChanged = null;
    public enum Status
    {
        Normal,
        UnLocking,
        CompleteUnLocked
    }

    [SerializeField]
    Sprite _background;

    public Sprite BackgroundIcon => _background;

    [SerializeField]
    Sprite _icon;

    public Sprite Icon => _icon;

    [SerializeField]
    Collection<BaseItem> _grantedItems;

    public Collection<BaseItem> Items => _grantedItems;

    [SerializeField]
    Collection<Currency> _grantedCurreyncies;

    public Collection<Currency> Currencies => _grantedCurreyncies;


    Status _status = Status.Normal;
    public Status status
    {
        get
        {
            return _status;
        }
        private set
        {
            Status prev = _status;
            _status = value;
            OnChanged?.Invoke(prev, _status);
        }
        
    }

    public void UnLocking()
    {
        status = Status.UnLocking;
    }
    public void CompleteUnLock()
    {
        status = Status.CompleteUnLocked;
    }
    public override BaseItem Clone()
    {
        Mail _ret = Instantiate(this);
        _ret._RemainingUses = 1;
        _ret._isStackable = false;
        return _ret;
    }
}
