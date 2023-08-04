using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ServiceStatus 
{
    public enum Status
    {
        Normal,         // 정상
        Checking        // 점검중
    }


    // 현재 접속가능한 클라이언트 버전들
    public List<ServiceVersion> AcceptedVersion;

    // 현재 서버 상태
    public string State;

    public bool IsChecking()
    {
        if (State == Status.Checking.ToString())
            return true;
        else
            return false;
    }
    public bool IsNormal()
    {
        if (State == Status.Normal.ToString())
            return true;
        else return false;
    }

    public bool IsAcceptVersion(ServiceVersion version)
    {
        return AcceptedVersion.Any(x => x == version);
    }

    
    
}
