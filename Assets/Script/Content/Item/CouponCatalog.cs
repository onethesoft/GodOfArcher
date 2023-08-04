using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ÄíÆù/ÄíÆùÄ«Å»·Î±×", fileName = "Coupon_")]
public class CouponCatalog : ScriptableObject
{
    [SerializeField]
    public string CouponCode;

    [SerializeField]
    public string CouponContainerId;

    [SerializeField]
    public string CouponKeyId;

    // Seconds minimum is 5
    // for CouponKey Timeperiod
    [SerializeField]
    int _TimePeriod;

    public int? TimePeriod { 
        get 
        {
            if (_TimePeriod < 5)
                return null;
            else
                return _TimePeriod;
        } 
    }

    [SerializeField]
    public string CouponeReceiveTicketId;
}
