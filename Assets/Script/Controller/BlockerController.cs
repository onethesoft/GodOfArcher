using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerController : MonoBehaviour
{
    [SerializeField]
    UI_ButtonBlocker Blocker;

    public void Block()
    {
        Blocker.StartBlocker();
    }
}
