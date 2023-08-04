using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Category", fileName = "Category_")]
public class Category : ScriptableObject, IEquatable<Category>
{
    [SerializeField]
    private string _codeName;
    [SerializeField]
    private string _displayName;

    public string CodeName => _codeName;
    public string DisplayName => _displayName;
    public Category(string CodeName , string DisplayName)
    {
        _codeName = CodeName;
        _displayName = DisplayName;
    }

    #region Operator
    bool IEquatable<Category>.Equals(Category other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(other, this))
            return true;
        if (GetType() != other.GetType())
            return false;

        return _codeName == other.CodeName;
    }

    public override int GetHashCode()
    {

        return (_codeName, _displayName).GetHashCode();
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    // Ex. category.CodeName == "Kill => category == "Kill"
    public static bool operator ==(Category lhs, string rhs)
    {
        if (lhs is null)
            return ReferenceEquals(rhs, null);

        return lhs.CodeName == rhs || lhs.DisplayName == rhs;
    }
    public static bool operator !=(Category lhs, string rhs)
    {
        return !(lhs == rhs);
    }
    #endregion
}
