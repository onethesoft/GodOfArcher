using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager 
{
    

    Dictionary<Define.Sound, string> _sounds;
    Dictionary<Define.Option, string> _options;

    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    float[] _audioVolume = new float[(int)Define.Sound.MaxCount];

    public float BGM 
    { 
        get
        {
            return float.Parse(_sounds[Define.Sound.Bgm]);
        }
        set
        {
            _sounds[Define.Sound.Bgm] = value.ToString();
            if (_audioSources != null)
                if (_audioSources[(int)Define.Sound.Bgm] != null)
                    if(_audioSources[(int)Define.Sound.Bgm].isPlaying)
                        _audioSources[(int)Define.Sound.Bgm].volume = float.Parse(_sounds[Define.Sound.Bgm]);
        }
    }

    public float SoundEffect
    {
        get
        {
            return float.Parse(_sounds[Define.Sound.Effect]);
        }
        set
        {
            _sounds[Define.Sound.Effect] = value.ToString();
            if (_audioSources != null)
                if (_audioSources[(int)Define.Sound.Effect] != null)
                    if (_audioSources[(int)Define.Sound.Effect].isPlaying)
                        _audioSources[(int)Define.Sound.Effect].volume = float.Parse(_sounds[Define.Sound.Effect]);
        }
    }
    public bool ShowDamage
    {
        get
        {
            return bool.Parse(_options[Define.Option.ShowDamage]);
        }
        set
        {
            _options[Define.Option.ShowDamage] = value.ToString();
        }
    }
    public bool ShowEffect
    {
        get
        {
            return bool.Parse(_options[Define.Option.ShowEffect]);
        }
        set
        {
            _options[Define.Option.ShowEffect] = value.ToString();
        }
    }
    public bool EnableShake
    {
        get
        {
            return bool.Parse(_options[Define.Option.ShowShake]);
        }
        set
        {
            _options[Define.Option.ShowShake] = value.ToString();
        }
    }

    public void Init()
    {
        _sounds = new Dictionary<Define.Sound, string>();
        _options = new Dictionary<Define.Option, string>();

        GameObject root = GameObject.Find("@Sound");
        if(root == null)
        {
            root = new GameObject { name = "@Sound" };
            UnityEngine.Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }
            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }

        Load();
        


    }
    public void Load()
    {
        for(int i=0; i < (int)Define.Sound.MaxCount;i++)
        {
            Define.Sound _key = (Define.Sound)i;
            if (PlayerPrefs.HasKey(_key.ToString()))
                _sounds.Add(_key, PlayerPrefs.GetString(_key.ToString()));
            else
                _sounds.Add(_key, ((float)(1.0f)).ToString());
        }

        for (int i = 0; i < (int)Define.Option.MaxCount; i++)
        {
            Define.Option _key = (Define.Option)i;
            if (PlayerPrefs.HasKey(_key.ToString()))
                _options.Add(_key, PlayerPrefs.GetString(_key.ToString()));
            else
                _options.Add(_key, ((bool)true).ToString());
        }

    }
    public void Save()
    {
        for (int i = 0; i < (int)Define.Sound.MaxCount; i++)
        {
            Define.Sound _key = (Define.Sound)i;
            PlayerPrefs.SetString(_key.ToString(), _sounds[_key]);
        }

        for (int i = 0; i < (int)Define.Option.MaxCount; i++)
        {
            Define.Option _key = (Define.Option)i;
            PlayerPrefs.SetString(_key.ToString(), _options[_key]);
        }
        PlayerPrefs.Save();
    }
    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);

        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;
        

        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.volume = BGM;
            audioSource.Play();

        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.volume = SoundEffect;
            audioSource.PlayOneShot(audioClip);
        }
    }

    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == Define.Sound.Bgm)
            audioClip = Managers.Resource.Load<AudioClip>(path);
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");




        return audioClip;
    }





}
