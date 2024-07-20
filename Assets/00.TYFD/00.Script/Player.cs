// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Component")]
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Stat")]
    [SerializeField] private float maxHp;
    [SerializeField] private float curHp;
    [SerializeField] private float attackPower;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float curAttackSpeed;
    [SerializeField] private float moveSpeed;

    [Header("Animation")]
    private readonly int hashMove = Animator.StringToHash("Move");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        InputFunction();
    }

    private void InputFunction()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        Move(hor);
        Flip(hor);
    }

    /// <summary>
    /// 움직임
    /// </summary>
    /// <param name="x">좌우 값</param>
    private void Move(float x)
    {
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
        if (x != 0)
        {
            animator.SetBool(hashMove, true);
        }
        else
        {
            animator.SetBool(hashMove,false);
        }
    }
    /// <summary>
    /// 방향전환
    /// </summary>
    /// <param name="x">방향 값</param>
    private void Flip(float x)
    {
        if (x != 0)
        {
            if (x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180 * x, 0);
            }
            else if(x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
