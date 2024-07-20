using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private int attackCount;

    [Header("Animation")]
    private readonly int hashMove = Animator.StringToHash("Move");
    private readonly int hashAttack1 = Animator.StringToHash("Attack1");
    private readonly int hashAttack2 = Animator.StringToHash("Attack2");
    private readonly int hashAttack3 = Animator.StringToHash("Attack3");

    [Header("Attack")]
    private bool isAttack = false;
    [SerializeField] private Transform attackBoxPos;

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

        if (Input.GetKeyDown(KeyCode.X))
        {
            Attack();
        }
    }

    /// <summary>
    /// 움직임
    /// </summary>
    /// <param name="x">좌우 값</param>
    private void Move(float x)
    {
        if (isAttack) return;  // 공격 중일 때는 움직이지 않음

        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
        if (x != 0)
        {
            animator.SetBool(hashMove, true);
        }
        else
        {
            animator.SetBool(hashMove, false);
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
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    private void Attack()
    {
        if (isAttack)
        {
            return;
        }

        if (attackCount == 0)
        {
            animator.SetTrigger(hashAttack1);
            attackCount++;
            OnAttackBox();
        }
        else if (attackCount == 1)
        {
            animator.SetTrigger(hashAttack2);
            attackCount++;
            OnAttackBox();
        }
        else if (attackCount == 2)
        {
            animator.SetTrigger(hashAttack3);
            attackCount++;
            attackCount = 0;
            OnAttackBox();
        }
        StartCoroutine(isAttacking());
    }

    private void OnAttackBox()
    {
        Collider2D[] curEnemy = Physics2D.OverlapBoxAll(attackBoxPos.position, new Vector2(1, 1), 0);
        if (curEnemy != null)
        {
            foreach (Collider2D enemy in curEnemy)
            {
                enemy.GetComponent<Enemy>()?.TakeDamage(attackPower);
            }
        }
    }

    IEnumerator isAttacking()
    {
        isAttack = true;

        yield return new WaitForSeconds(0.2f);

        isAttack = false;
    }
}
