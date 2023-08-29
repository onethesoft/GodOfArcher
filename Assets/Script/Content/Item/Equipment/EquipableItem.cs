using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipableItem : BaseItem
{
    public enum Rank
    {
        F,
        E,
        D,
        C,
        B,
        A,
        S,
        L,
        M,
        I,
        U,
        G
    }

    [SerializeField]
    protected int _EquipCount = 0;
    public int EquipCount => _EquipCount;

    [SerializeField]
    protected int _Level = 0;
    public int Level
    {
        get { return _Level; }
    }

    #region UI Data
    [SerializeField]
    private Sprite _Icon = null;
    public Sprite Icon => _Icon;

    [SerializeField]
    private Sprite _IconBackground = null;
    public Sprite IconBackground
    {
        get { return _IconBackground; }
        set { _IconBackground = value; }
    }

    [SerializeField]
    private Sprite _Background = null;
    public Sprite Background
    {
        get { return _Background; }
        set { _Background = value; }
    }

    [SerializeField]
    private OutlineData _IconBackgroundOutline;
    public OutlineData IconBackgroundOutline
    {
        get { return _IconBackgroundOutline; }
        set { _IconBackgroundOutline = value; }
    }

    [SerializeField]
    private Sprite _DescriptionBackgroundSprite;
    public Sprite DescriptionBackground
    {
        get { return _DescriptionBackgroundSprite; }
        set { _DescriptionBackgroundSprite = value; }
    }



    [SerializeField]
    private Sprite _DescriptionBackgroundTexture = null;
    public Sprite DescriptionBackgroundTexture
    {
        get { return _DescriptionBackgroundTexture; }
        set { _DescriptionBackgroundTexture = value; }
    }

    [SerializeField]
    private OutlineData _DescriptionBackgroundOutline = null;
    public OutlineData DescriptionBackgroundOutline
    {
        get { return _DescriptionBackgroundOutline; }
        set { _DescriptionBackgroundOutline = value; }
    }


    [SerializeField]
    private Color _DescriptionBackgroundColor;
    public Color DescriptionBackgroundColor
    {
        get { return _DescriptionBackgroundColor; }
        set { _DescriptionBackgroundColor = value; }
    }

    [SerializeField]
    private string _SpriteCollectionId = null;
    public string SpriteCollectionId => _SpriteCollectionId;

    [SerializeField]
    private OutlineData _DisplayNameOutLine = null;
    public OutlineData DisplayNameOutLine
    {
        get { return _DisplayNameOutLine; }
        set { _DisplayNameOutLine = value; }
    }
    #endregion

    [SerializeField]
    protected List<StatModifier> _statModifiers;

    public IReadOnlyList<StatModifier> StatModifiers => _statModifiers;

    public virtual void Equipped()
    {
        if (!_RemainingUses.HasValue) return;

        if (_EquipCount < _RemainingUses.Value)
        {
            _EquipCount++;
        }
    }

    public virtual void UnEquipped()
    {
        if (_EquipCount > 0)
        {
            _EquipCount--;
        }
    }

    public override int? GetUsableCount()
    {
        return _RemainingUses - _EquipCount;
    }



    

}
