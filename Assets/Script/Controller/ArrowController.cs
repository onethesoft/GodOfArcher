using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("ArrawController");
        
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.GetComponent<MonsterController>() != null)
        {
            collision.gameObject.GetComponent<MonsterController>().OnHit(10000000000000, Define.DamageType.Normal);
            Managers.Resource.Destroy(gameObject);
        }

    }
}
