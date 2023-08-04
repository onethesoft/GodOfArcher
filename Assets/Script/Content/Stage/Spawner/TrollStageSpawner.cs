using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[CreateAssetMenu(fileName = "TrollgStageSpawner", menuName = "Spawner/TrollStage")]
public class TrollStageSpawner : Spawner
{
    [SerializeField]
    List<GameObject> _mosterPrefabs;

    [SerializeField]
    MonsterStatCalculator _monsterHPGenerator;

    [SerializeField]
    UnityEngine.Vector3 _pos;

    public override BigInteger GetMonsterHP(int targetStage)
    {
        return _monsterHPGenerator.GetHP(targetStage);
    }
    public override System.Numerics.BigInteger GetClearGold(int stage)
    {
        return -1;
    }
    public GameObject GetMonsterObject(int targetStage, Transform parent = null)
    {
        int prefabIndex = targetStage - 1;
        prefabIndex %= _mosterPrefabs.Count;
        return _mosterPrefabs[prefabIndex];
    }
    public override void DoSpawn()
    {
        GameObject _copymonster = Managers.Game.Spawn(Define.WorldObject.Monster ,GetMonsterObject(Managers.Game.TrollStage + 1));
        //_copymonster.transform.position = new UnityEngine.Vector3 { x = 5.34f, y = -1.06f, z = 0.0f };
        _copymonster.transform.position = _pos;

        MonsterController _controller = Util.GetOrAddComponent<MonsterController>(_copymonster);
        _controller.SetHp(Define.MonsterType.Dongeon , GetMonsterHP(Managers.Game.TrollStage + 1));
        _controller.MonsterType = Define.MonsterType.Dongeon;

        (Managers.Scene.CurrentScene.GetSceneUI() as UI_Troll).SetMonsterHPText(Util.GetBigIntegerUnit(_monsterHPGenerator.GetHP(Managers.Game.TrollStage + 1)));
        //throw new System.NotImplementedException();
    }

    public override UnityEngine.Vector3 GetSpawnPosition(int i)
    {
        return _pos;
    }
}
