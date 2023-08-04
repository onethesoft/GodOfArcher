using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "아이템/유물재료")]
public class Artifactpiece : BaseItem
{
    [SerializeField]
    Artifact.type _type;

    public Artifact.type Type => _type;

    [SerializeField]
    Sprite _icon;
    public Sprite Icon => _icon;

    [SerializeField]
    Sprite _Background;
    public Sprite Background => _Background;

    [SerializeField]
    Color _BackgroundColor;
    public Color BackgroundColor => _BackgroundColor;

    public override BaseItem Clone()
    {
        Artifactpiece _ret = Instantiate(this);
        if (!_ret._RemainingUses.HasValue)
            _ret._RemainingUses = 1;

        return _ret;
    }
}
