// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class Portal : MonoBehaviour
{
    Animator animator;
    private readonly int hashPortalOpen = Animator.StringToHash("PortalOpen");
    public bool isOpen;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(isOpen)
        {
            animator.SetBool(hashPortalOpen, true);
        }
    }
}
