using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ServiceStatus 
{
    public enum Status
    {
        Normal,         // ����
        Checking        // ������
    }


    // ���� ���Ӱ����� Ŭ���̾�Ʈ ������
    public List<ServiceVersion> AcceptedVersion;

    // ���� ���� ����
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
