using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;


public class AnalyticsManager
{
    public void Init()
    {
        //  AnalyticsService.internalInstance.InternalTick(); unused
        AnalyticsService.Instance.SetAnalyticsEnabled(false);
    }
}
