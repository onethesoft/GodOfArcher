using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerGroup : MonoBehaviour
{
    BlockerController [] blockers;
    // Start is called before the first frame update
    void Start()
    {
        blockers = GetComponentsInChildren<BlockerController>();
    }

    public void Block()
    {
        foreach (BlockerController blocker in blockers)
            blocker.Block();
    }

   
}
