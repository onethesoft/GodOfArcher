using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ServiceVersion : IEquatable<ServiceVersion>
{
    public int Major;
    public int Minor;
    public int Patch;

    public override bool Equals(object obj)
    {
        return Equals(obj as ServiceVersion);
    }

    public bool Equals(ServiceVersion other)
    {
        return Major == other.Major && Minor == other.Minor && Patch == other.Patch;
    }

    public static bool operator == (ServiceVersion a , ServiceVersion b)
    {
        return a.Equals(b);
    }
    public static bool operator !=(ServiceVersion a, ServiceVersion b)
    {
        return a.Equals(b);
    }

    public override int GetHashCode()
    {
        return (Major, Minor , Patch).GetHashCode();
    }
}
