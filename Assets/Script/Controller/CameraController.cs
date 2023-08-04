using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    bool _isShake = false;
    public enum Direction
    {
        ShakeHorizontal,
        ShakeVertical
    }
    public void DoShake(Direction ShakeDirection)
    {
        if(_isShake == false)
        {
            _isShake = true;
            GetComponent<DOTweenAnimation>().DORestartById(ShakeDirection.ToString());
            
        }
        //Debug.Log(_isShake);
        
    }
    public void OnCompleteDoShake()
    {
        _isShake = false;
    }
    private void OnDestroy()
    {
        foreach(string direction in System.Enum.GetNames(typeof(Direction)))
            GetComponent<DOTweenAnimation>().DOKillAllById(direction);
        
        
    }
}
