using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Vector3 = UnityEngine.Vector3;

[CreateAssetMenu(fileName ="MainStageSpawner",menuName ="Spawner/MainStage")]
public class MainStageSpawner : Spawner
{
    [SerializeField]
    MonsterStatCalculator _monsterHPGenerator;

    [SerializeField]
    List<IntervalUnit<GameObject>> _mosterPrefabs;

    [SerializeField]
    List<IntervalUnit<List<Vector3>>> _pos;

    UnityEngine.Vector3 boss = new UnityEngine.Vector3 { x = 0.5f, y = 0.5f };

    UnityEngine.Vector3 normal = new UnityEngine.Vector3 { x = 0.4f, y = 0.4f };

    public GameObject GetMonsterObject(int targetStage, Transform parent = null)
    {
        foreach (IntervalUnit<GameObject> _inverval in _mosterPrefabs)
        {
            if (_inverval.IsContains(targetStage))
            {
                return _inverval.Unit;
            }
        }
        return _mosterPrefabs[0].Unit;
    }

    public override BigInteger GetMonsterHP(int stage)
    {
        return _monsterHPGenerator.GetHP(stage + 5000);
    }

    public override BigInteger GetClearGold(int stage)
    {
        /*
        IntervalUnit<List<Vector3>> _posItem = _pos.Where(x => x.IsContains(Managers.Game.Stage)).FirstOrDefault();

        int BossCount = _posItem.Unit.Count / 3;
        int NormalCount = _posItem.Unit.Count - BossCount;
        BigInteger temp = GetMonsterHP(stage)* (BigInteger)(NormalCount) + GetMonsterHP(stage)* 5* (BigInteger)(BossCount);
        return temp;
        */
        return GetMonsterHP(stage);
    }

    public override void DoSpawn()
    {
        IntervalUnit<List<Vector3>> _posItem = _pos.Where(x => x.IsContains(Managers.Game.Stage)).FirstOrDefault();
        if (_posItem != null )
        {
            GameObject _monster = GetMonsterObject(Managers.Game.Stage);
           
            int generateCount = 0;

            foreach(Vector3 _monsterPos in _posItem.Unit)
            {
                GameObject _copymonster = Managers.Game.Spawn(Define.WorldObject.Monster, _monster, Managers.Scene.CurrentScene.transform);
                
                _copymonster.transform.position = _monsterPos;


                MonsterController _controller = Util.GetOrAddComponent<MonsterController>(_copymonster);
              

                if (generateCount % 3 == 2)
                {
                    _copymonster.transform.localScale = boss;

                   
                    _controller.SetHp(Define.MonsterType.Boss, _monsterHPGenerator.GetHP(Managers.Game.Stage + 5000) * 5);
                 



                }
                else
                {
                    
                    _copymonster.transform.localScale = normal;
                    _controller.SetHp(Define.MonsterType.Normal,_monsterHPGenerator.GetHP(Managers.Game.Stage + 5000));
                   

                    //Debug.Log(_monsterHPGenerator.GetHP(Managers.Game.Stage + 5000));
                    
                }

                generateCount++;
            }

#if UNITY_EDITOR
          // (Managers.Scene.CurrentScene.GetSceneUI() as UI_Main).SetMonsterHPText(Util.GetBigIntegerUnit(_monsterHPGenerator.GetHP(Managers.Game.Stage + 5000)));
#endif

        }
        
       
        
       
    }

    public override UnityEngine.Vector3 GetSpawnPosition(int i)
    {
        IntervalUnit<List<Vector3>> _s = _pos.Where(x => x.min <= Managers.Game.Stage && Managers.Game.Stage >= x.max).First();

        if (i < _s.Unit.Count)
        {
            return _s.Unit[i];
        }
        else
            return _s.Unit.First();

    }

#if UNITY_EDITOR
    [ContextMenu("FindMonsterPrefab")]
    private void FindMonsterPrefabs()
    {
        List<GameObject> _findfrefabs = new List<GameObject>();

        _mosterPrefabs = new List<IntervalUnit<GameObject>>();


        string[] guids = AssetDatabase.FindAssets("Stage", new[] { "Assets/Stage/MainStage/Monster"});

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var monsterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            //Debug.Log(monsterPrefab.name.Replace("Stage", "").Split('-').First());

            int min = int.Parse(monsterPrefab.name.Replace("Stage", "").Split('-').First());
            int max = int.Parse(monsterPrefab.name.Replace("Stage", "").Split('-').Last());

            _mosterPrefabs.Add(new IntervalUnit<GameObject> { min = min , max = max , Unit = monsterPrefab });

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

    [ContextMenu("FindMonsterPosition")]
    private void FindMonsterPosistions()
    {
        _pos = new List<IntervalUnit<List<Vector3>>>();

        // 1 ~ 100000
        _pos.Add(new IntervalUnit<List<Vector3>> 
        { 
            min = 1, 
            max = 99999, 
            Unit = new List<Vector3> { 
            new Vector3(6.3f, 0.791f, 0.0f) , new Vector3(6.3f, -0.809f, 0.0f), new Vector3(7.8f, 1.6f, 0.0f), new Vector3(7.8f, -0.009f, 0.0f), new Vector3(7.8f, -1.609f, 0.0f)
        } });

        // 100000 ~ 200000
        _pos.Add(new IntervalUnit<List<Vector3>>
        {
            min = 100000,
            max = 199999,
            Unit = new List<Vector3> {
            new Vector3(3.27f, 0.791f, 0.0f) , new Vector3(3.27f, -0.809f, 0.0f), new Vector3(4.77f, 1.6f, 0.0f), new Vector3(4.77f, -0.009f, 0.0f), new Vector3(4.77f, -1.609f, 0.0f),
            new Vector3(6.3f, 0.791f, 0.0f) , new Vector3(6.3f, -0.809f, 0.0f), new Vector3(7.8f, 1.6f, 0.0f), new Vector3(7.8f, -0.009f, 0.0f), new Vector3(7.8f, -1.609f, 0.0f)
        }
        });

        _pos.Add(new IntervalUnit<List<Vector3>>
        {
            min = 200000,
            max = 299999,
            Unit = new List<Vector3> {
            new Vector3(0.14f, 0.791f, 0.0f) , new Vector3(0.14f, -0.809f, 0.0f), new Vector3(1.64f, 1.6f, 0.0f), new Vector3(1.64f, -0.009f, 0.0f), new Vector3(1.64f, -1.609f, 0.0f),
            new Vector3(3.27f, 0.791f, 0.0f) , new Vector3(3.27f, -0.809f, 0.0f), new Vector3(4.77f, 1.6f, 0.0f), new Vector3(4.77f, -0.009f, 0.0f), new Vector3(4.77f, -1.609f, 0.0f),
            new Vector3(6.3f, 0.791f, 0.0f) , new Vector3(6.3f, -0.809f, 0.0f), new Vector3(7.8f, 1.6f, 0.0f), new Vector3(7.8f, -0.009f, 0.0f), new Vector3(7.8f, -1.609f, 0.0f)
        }
        });
    }

    




#endif
}
