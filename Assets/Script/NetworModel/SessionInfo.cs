using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayFab.Internal;
using Newtonsoft.Json.Linq;

[Serializable]
public class SessionInfo 
{
    public string DeviceId;
    public DateTime LastAccessTime;

    public string ToJson()
    {
        JObject obj = new JObject();
        obj.Add("DeviceId", DeviceId);
        obj.Add("LastAccessTime", LastAccessTime.ToString(PlayFabUtil._defaultDateTimeFormats[PlayFabUtil.DEFAULT_UTC_OUTPUT_INDEX]));
        return obj.ToString();
    }

    public static SessionInfo FromJson(string json)
    {
        var obj = JObject.Parse(json);
        SessionInfo sessoin = new SessionInfo { DeviceId = obj["DeviceId"].ToString(), LastAccessTime = DateTime.Parse(obj["LastAccessTime"].ToString(),  null, System.Globalization.DateTimeStyles.RoundtripKind)  };
        return sessoin;
    }
}
