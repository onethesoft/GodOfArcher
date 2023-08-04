using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene
    {
        get
        {
            if(_currentScene == null)
                _currentScene = GameObject.FindObjectOfType<BaseScene>();
            else if( _currentScene.SceneType.ToString() != SceneManager.GetActiveScene().name)
                _currentScene = GameObject.FindObjectOfType<BaseScene>();
            return _currentScene;
         
        }
    }
    BaseScene _currentScene = null;
    
    string GetSceneName(Define.Scene type)
    {
        return System.Enum.GetName(typeof(Define.Scene), type);
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }
}
