// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class HitEffectDestroy : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}
