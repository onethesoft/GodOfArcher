using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "아이템/컨테이너")]
public class Container : BaseItem
{
    public delegate void OnChangedStatus(Status prevStatus, Status status);
    public event OnChangedStatus OnChanged = null;
    public enum Status
    {
        Normal,
        UnLocking,
        CompleteUnLocked
    }
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
        Container _ret = Instantiate(this);
        _ret._RemainingUses = 1;
        _ret._isStackable = true;
        return _ret;
    }
}
