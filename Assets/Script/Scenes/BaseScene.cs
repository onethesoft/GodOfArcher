using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;
    public abstract UI_Scene GetSceneUI();
    private CameraController _camera;
    public enum GameState
    {
        Load = 1,
        Run,
        ProcessReward,
        End,
        Wait
    }
    protected GameState _state;
    public GameState CurrentState
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
        }
    }

    void Awake()
    {
        Init();
    }
    protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";

        _camera = (CameraController)GameObject.FindObjectOfType(typeof(CameraController));
        if (_camera == null)
            _camera = Util.GetOrAddComponent<CameraController>(Managers.Resource.Instantiate("Camera/Main Camera"));

        foreach (EquipmentSlot slot in Managers.Game.GetEquipment("Pet").SlotList)
            slot.UpdateSlot();
        

        
    }
    public CameraController GetCamera()
    {
        return _camera;
    }

    public abstract void Clear();
   

   


}
