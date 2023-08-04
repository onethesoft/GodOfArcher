using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetController : BaseController
{
    public override void Init()
    {
        base.Init();
        type = Define.WorldObject.Pet;
        State = Define.State.Idle;
    }
    private void OnDestroy()
    {
        Destroy(GetComponent<Poolable>());
    }


}
