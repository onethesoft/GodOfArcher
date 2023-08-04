using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        Managers.Resource.Destroy(gameObject);
    }
}
