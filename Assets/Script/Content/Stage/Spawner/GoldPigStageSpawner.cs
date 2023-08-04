using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


[CreateAssetMenu(fileName = "GoldPigStageSpawner", menuName = "Spawner/GoldPigStage")]
public class GoldPigStageSpawner : Spawner
{
    [SerializeField]
    MonsterStatCalculator _monsterHPGenerator;

    [SerializeField]
    UnityEngine.Vector3 _pos;

    [SerializeField]
    GameObject _mosterPrefab;
    public override BigInteger GetMonsterHP(int stage)
    {
        return _monsterHPGenerator.GetHP(stage + 5000) * 5;
    }
    public override System.Numerics.BigInteger GetClearGold(int stage)
    {
        return GetMonsterHP(stage);
    }
    public GameObject GetMonsterObject()
    {
        return _mosterPrefab;
    }
    public override void DoSpawn()
    {
        GameObject _monster = GetMonsterObject();
        GameObject _copymonster = Managers.Game.Spawn(Define.WorldObject.Monster, _monster); ;
        //_copymonster.transform.position = new UnityEngine.Vector3 { x = 5.34f, y = -1.06f, z = 0.0f };
        _copymonster.transform.position = _pos;

        MonsterController _controller = Util.GetOrAddComponent<MonsterController>(_copymonster);
        _controller.SetHp(Define.MonsterType.Dongeon,_monsterHPGenerator.GetHP(Managers.Game.Stage + 5000) * 5);
        _controller.MonsterType = Define.MonsterType.Dongeon;

    }

    public override UnityEngine.Vector3 GetSpawnPosition(int i)
    {
        return _pos;
    }
}
