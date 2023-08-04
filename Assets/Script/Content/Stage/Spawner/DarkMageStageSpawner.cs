using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "DarkMageStageSpawner", menuName = "Spawner/DarkMageStage")]
public class DarkMageStageSpawner : Spawner
{
    [SerializeField]
    List<GameObject> _mosterPrefabs;

    [SerializeField]
    UnityEngine.Vector3 _spawnPos;
    public override BigInteger GetMonsterHP(int stage)
    {
        return -1;
     
    }
    public GameObject GetMonsterObject()
    {
        int index = UnityEngine.Random.Range(0, _mosterPrefabs.Count);
        return _mosterPrefabs[index];
    }

    public override System.Numerics.BigInteger GetClearGold(int stage)
    {
        return -1;
    }
    public override void DoSpawn()
    {
        GameObject _monster = GetMonsterObject();
        GameObject _copymonster = Managers.Game.Spawn(Define.WorldObject.Monster ,_monster);
        //_copymonster.transform.position = new UnityEngine.Vector3 { x = 2.34f, y = -2.06f, z = 0.0f };
        _copymonster.transform.position = _spawnPos;


        MonsterController _controller = Util.GetOrAddComponent<MonsterController>(_copymonster);
        _controller.SetHp(Define.MonsterType.Dongeon,-1);
        _controller.MonsterType = Define.MonsterType.Dongeon;
    }

    public override UnityEngine.Vector3 GetSpawnPosition(int i)
    {
        return _spawnPos;
    }

#if UNITY_EDITOR
    [ContextMenu("FindMonsterPrefab")]
    private void FindMonsterPrefabs()
    {
        List<GameObject> _findfrefabs = new List<GameObject>();

        


        string[] guids = AssetDatabase.FindAssets("Monster", new[] { "Assets/Stage/DarkMageStage/Monster" });

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var monsterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            //Debug.Log(monsterPrefab.name.Replace("Stage", "").Split('-').First());


            _mosterPrefabs.Add(monsterPrefab);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

    


#endif


}
