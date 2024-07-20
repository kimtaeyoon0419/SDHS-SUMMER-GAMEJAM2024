// # System
using System.Collections;
using System.Collections.Generic;
using TMPro;

// # Unity
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Component")]
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Player")]
    [SerializeField] private Collider2D playerCollider;

    [Header("Stat")]
    [SerializeField] private float maxHp;
    [SerializeField] private float curHp;
    [SerializeField] private float followMoveSpeed;
    [SerializeField] private float attackPower;

    [Header("Color")]
    private SpriteRenderer spriteRenderer;
    private Color hitColor = Color.black;

    [Header("LayerMask")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Animation")]
    private readonly int hashMove = Animator.StringToHash("Move");
    private readonly int hashAttack = Animator.StringToHash("Attack");
    private readonly int hashDie = Animator.StringToHash("Die");

    [Header("Attack")]
    private bool isAttack = false;
    [SerializeField] private Transform attackPos;
    [SerializeField] private Vector2 attackBox;

    [Header("HitEffect")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private TextMeshPro hitText;

    [Header("Die")]
    private bool isDie;

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
        if (!isDie)
        {
            Collider2D detectedPlayer = Physics2D.OverlapCircle(transform.position, 10f, playerLayer);
            if (detectedPlayer != null)
            {
                playerCollider = detectedPlayer;
            }
            else
            {
                playerCollider = null;
            }
            Move();
        }
        if(isDie)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 3f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, attackBox);
    }

    private void Move()
    {
        if (playerCollider != null)
        {
            if (playerCollider != null)
            {
                // 플레이어와의 거리를 계산
                float distance = Vector2.Distance(playerCollider.transform.position, transform.position);

                // 플레이어가 몬스터의 왼쪽에 있는지 오른쪽에 있는지 확인
                if (playerCollider.transform.position.x < transform.position.x)
                {
                    // 플레이어가 왼쪽에 있으면 왼쪽으로 이동
                    rb.velocity = new Vector2(followMoveSpeed * -1, rb.velocity.y);
                    transform.localScale = new Vector3(1.5f * -1, 1.5f, 1.5f);
                    animator.SetBool(hashMove, true);
                }
                else if (playerCollider.transform.position.x > transform.position.x)
                {
                    // 플레이어가 오른쪽에 있으면 오른쪽으로 이동
                    rb.velocity = new Vector2(followMoveSpeed, rb.velocity.y);
                    transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    animator.SetBool(hashMove, true);
                }

                // 플레이어와의 거리가 일정 이하일 때 공격
                if (distance < 2f)
                {
                    Debug.Log(distance);
                    rb.velocity = Vector2.zero;
                    Attack();
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.SetBool(hashMove, false);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool(hashMove, false);
        }
    }

    private void Attack()
    {
        if (!isAttack)
        {
            animator.SetBool(hashMove, false);
            animator.SetTrigger(hashAttack);
            Collider2D[] player = Physics2D.OverlapBoxAll(attackPos.position, attackBox, playerLayer);
            foreach (Collider2D curPlayer in player)
            {
                curPlayer.GetComponent<Player>()?.TakeDamage(attackPower);
            }
            StartCoroutine(Co_waitTime());
        }
    }

    IEnumerator Co_waitTime()
    {
        isAttack = true;
        yield return new WaitForSeconds(0.5f);
        isAttack = false;
    }

    public void TakeDamage(float damage)
    {
        if (!isDie)
        {
            curHp -= damage;
            Debug.Log("몬스터의 현재 체력 : " + curHp);
            StartCoroutine(Co_ChangeHitColor());
            if (curHp <= 0)
            {
                Debug.Log("죽었다");
                Die();
            }
            else
            {
                CameraManager.instance.CameraShake(5, 0.2f);
                Instantiate(hitEffect, transform.position, Quaternion.identity);
                TextMeshPro text = Instantiate(hitText, new Vector2(transform.position.x, transform.position.y + 5f), Quaternion.identity);
                text.text = damage.ToString();
            }
        }
    }

    private void Die()
    {
        // 죽는 애니메이션 실행
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        isDie = true;
        animator.SetTrigger(hashDie);
        StartCoroutine(Co_Destroy());
    }

    IEnumerator Co_Destroy()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    IEnumerator Co_ChangeHitColor()
    {
        for (int i = 0; i < 1; i++)
        {
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }
}
