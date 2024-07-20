// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] private GameObject playerObj;

    private void Update()
    {
        transform.position = playerObj.transform.position;
    }
}
