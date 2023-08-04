using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Detector : MonoBehaviour
{

    float _time = 0.0f;
    bool _injectDetected = false;
    public void InjectionDetect()
    {
        _injectDetected = true;

        UI_Messagebox _messagebox = Managers.UI.ShowPopupUI<UI_Messagebox>();
        _messagebox.mode = UI_Messagebox.Mode.OK;
        _messagebox.Text = "Injection 감지되었습니다.";
        _messagebox.OK += () => {
            ForceExit();
        };

        Invoke("ForceExit", 5.0f);
    }

    public void TimeCheatingDetect()
    {
        _injectDetected = true;

        UI_Messagebox _messagebox = Managers.UI.ShowPopupUI<UI_Messagebox>();
        _messagebox.mode = UI_Messagebox.Mode.OK;
        _messagebox.Text = "TimeCheating 감지되었습니다.";
        _messagebox.OK += () => {
            ForceExit();
        };

        Invoke("ForceExit", 5.0f);
    }
    public void ObscuredCheatDetect()
    {
        _injectDetected = true;

        UI_Messagebox _messagebox = Managers.UI.ShowPopupUI<UI_Messagebox>();
        _messagebox.mode = UI_Messagebox.Mode.OK;
        _messagebox.Text = "SpeedCheating 감지되었습니다.";
        _messagebox.OK += () => {
            ForceExit();
        };

        Invoke("ForceExit", 5.0f);
    }
    public void SpeedHackDetect()
    {
        _injectDetected = true;

        UI_Messagebox _messagebox = Managers.UI.ShowPopupUI<UI_Messagebox>();
        _messagebox.mode = UI_Messagebox.Mode.OK;
        _messagebox.Text = "SpeedCheating 감지되었습니다.";
        _messagebox.OK += () => {
            ForceExit();
        };

        Invoke("ForceExit", 5.0f);
    }

    public void RootingDetect()
    {
        _injectDetected = true;

        UI_Messagebox _messagebox = Managers.UI.ShowPopupUI<UI_Messagebox>();
        _messagebox.mode = UI_Messagebox.Mode.OK;
        _messagebox.Text = "루팅된 기기에서는 실행할 수 없습니다.";
        _messagebox.OK += () => {
            ForceExit();
        };

        Invoke("ForceExit", 5.0f);
    }

    bool IsRootedDevice()
    {

#if ENABLE_LOG
        return false;
#endif


        bool isRoot = false;
        if (Application.platform == RuntimePlatform.Android)
        {
            if (File.Exists("/system/su"))
                isRoot = true;
            if (File.Exists("/system/bin/su"))
                isRoot = true;
            if (File.Exists("/system/xbin/su"))
                isRoot = true;
            if (File.Exists("/system/sbin/su"))
                isRoot = true;
            if (File.Exists("/system/app/SuperUser.apk"))
                isRoot = true;
            if (File.Exists("/data/data/com.noshufou.android.su"))
                isRoot = true;
            if (File.Exists("/sbin/su"))
                isRoot = true;
        }
        return isRoot;
    }

    private void Update()
    {
        if (_injectDetected == true)
            return;

        _time += Time.deltaTime;
        if(_time >= 2.0)
        {
            if(IsRootedDevice())
            {
                RootingDetect();
            }
            _time = 0.0f;

        }
    }


    void ForceExit()
    {
        Application.Quit();
    }
}
