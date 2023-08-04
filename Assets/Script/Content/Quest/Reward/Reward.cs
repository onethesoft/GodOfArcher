using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reward : ScriptableObject
{
    [SerializeField]
    private Sprite _icon;
    [SerializeField]
    private Sprite _background;
    [SerializeField]
    private string _description;
    [SerializeField]
    private int _quantity;
    

    public Sprite Icon => _icon;
    public Sprite BackgroundIcon => _background;
    public string Description => _description;
    public int Quantity => _quantity;

    public abstract string GetId();
   
    public abstract void Give(Quest quest);
}
