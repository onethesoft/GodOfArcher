using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConsumable 
{
    public int Amount();
    
    public void Comsume(int Count);
}
