using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Component")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Stat")]
    [SerializeField] private float maxHp;
    [SerializeField] private float curHp;
    [SerializeField] private float attackPower;
    [SerializeField] private float attackSpeed;
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
    [SerializeField] private Vector2 attackBoxSize;

    [Header("Color")]
    private Color hitColor = Color.black;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        curHp = maxHp;
    }

    private void Update()
    {
        InputFunction();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackBoxPos.position, attackBoxSize);
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
            transform.localScale = new Vector3(1.5f * x, 1.5f, 1.5f);
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
        StartCoroutine(Co_IsAttacking());
    }

    private void OnAttackBox()
    {
        Collider2D[] curEnemy = Physics2D.OverlapBoxAll(attackBoxPos.position, attackBoxSize, 0);
        if (curEnemy != null)
        {
            foreach (Collider2D enemy in curEnemy)
            {
                enemy.GetComponent<Enemy>()?.TakeDamage(attackPower);
            }
        }
    }

    IEnumerator Co_IsAttacking()
    {
        isAttack = true;

        yield return new WaitForSeconds(attackSpeed);

        isAttack = false;
    }

    public void TakeDamage(float Damage)
    {
        curHp -= Damage;
        if (curHp < 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Co_HitChageColor());
        }
    }

    IEnumerator Co_HitChageColor()
    {
        for (int i = 0; i < 1; i++)
        {
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }

    private void Die()
    {

    }
}
