// # System
using System.Collections;
using System.Collections.Generic;
using TMPro;

// # Unity
using UnityEngine;
using UnityEngine.UIElements;

public class Boss1 : MonoBehaviour
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
    [SerializeField] public float attackPower;

    [Header("Color")]
    private SpriteRenderer spriteRenderer;
    private Color hitColor = Color.black;

    [Header("LayerMask")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Animation")]
    private readonly int hashMove = Animator.StringToHash("Move");
    private readonly int hashAttack1 = Animator.StringToHash("Attack1");
    private readonly int hashAttack2 = Animator.StringToHash("Attack2");
    private readonly int hashAttack3 = Animator.StringToHash("Attack3");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashDie = Animator.StringToHash("Die");

    [Header("Attack")]
    private bool isAttack = false;
    [SerializeField] private Transform[] attackPos;
    [SerializeField] private Vector2[] attackBox;
    [SerializeField] private int attackCount = 0;
    [SerializeField] private bool canAttack = true;

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

    private void OnEnable()
    {
        curHp = maxHp;
        //StageManager.instance.monsters.Add(gameObject);
    }
    private void Update()
    {
        if (!isDie && GameManager.instance.curGameState != curGameState.selectItem)
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
        if (isDie)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos[0].position, attackBox[0]);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(attackPos[1].position, attackBox[1]);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPos[2].position, attackBox[2]);
    }

    private void Move()
    {
        if (playerCollider != null && !isAttack)
        {
            // 플레이어와의 거리를 계산
            float distance = Vector2.Distance(playerCollider.transform.position, transform.position);
            if (distance < 6f)
            {
                //Debug.Log(distance);
                rb.velocity = Vector2.zero;
                Attack();
            }
            // 플레이어가 몬스터의 왼쪽에 있는지 오른쪽에 있는지 확인
            else if (playerCollider.transform.position.x < transform.position.x)
            {
                // 플레이어가 왼쪽에 있으면 왼쪽으로 이동
                rb.velocity = new Vector2(followMoveSpeed * -1, rb.velocity.y);
                //transform.Translate(Vector2.right * followMoveSpeed * -1 * Time.deltaTime);
                transform.localScale = new Vector3(10f * -1, 10f, 10f);
                animator.SetBool(hashMove, true);
            }
            else if (playerCollider.transform.position.x > transform.position.x)
            {
                // 플레이어가 오른쪽에 있으면 오른쪽으로 이동
                rb.velocity = new Vector2(followMoveSpeed, rb.velocity.y);
                //transform.Translate(Vector2.right * followMoveSpeed * 1 * Time.deltaTime);

                transform.localScale = new Vector3(10f, 10f, 10f);
                animator.SetBool(hashMove, true);
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
        if (!isAttack && canAttack)
        {
            Debug.Log("공격준비!!");
            animator.SetBool(hashMove, false);
            isAttack = true;
            if (attackCount == 0)
            {
                animator.SetTrigger(hashAttack1);
                StartCoroutine(Co_waitTime());
            }
            if (attackCount == 1)
            {
                animator.SetTrigger(hashAttack2);
                StartCoroutine(Co_waitTime());
            }
            if (attackCount == 2)
            {
                animator.SetTrigger(hashAttack3);
                StartCoroutine(Co_waitTime());
                StartCoroutine(Co_LongwaitTime());
                canAttack = false;
            }
        }
    }
    public void HitAttack()
    {
        Collider2D[] player = Physics2D.OverlapBoxAll(attackPos[attackCount].position, attackBox[attackCount], playerLayer);
        AudioManager.instance.PlaySfx("BossSlash");
        if (attackCount <= 2)
        {
            attackCount++;
            if (attackCount >= 3)
            {
                attackCount = 0;
            }
        }
        foreach (Collider2D curPlayer in player)
        {
            curPlayer.GetComponent<Player>()?.TakeDamage(attackPower);
        }
    }

    IEnumerator Co_waitTime()
    {
        isAttack = true;
        yield return new WaitForSeconds(1.2f);
        isAttack = false;
    }

    IEnumerator Co_LongwaitTime()
    {
        yield return new WaitForSeconds(4f);
        canAttack = true;
    }

    public void TakeDamage(float damage, bool isSkill)
    {
        if (!isDie)
        {
            curHp -= damage;
            if (!isAttack)
            {
                animator.SetTrigger(hashHit);
            }
            Debug.Log("몬스터의 현재 체력 : " + curHp);
            StartCoroutine(Co_ChangeHitColor());
            TextMeshPro text = Instantiate(hitText, new Vector2(transform.position.x, transform.position.y + 5), Quaternion.identity);
            text.text = damage.ToString("F2");
            if (curHp <= 0)
            {
                Debug.Log("죽었다");
                if (!isSkill)
                {
                    AudioManager.instance.PlaySfx("SlashHit");
                }
                StageManager.instance.aliveMonster.Remove(gameObject);
                Die();
            }
            else
            {
                CameraManager.instance.CameraShake(5, 0.2f);
                if (!isSkill)
                {
                    AudioManager.instance.PlaySfx("SlashHit");
                    Instantiate(hitEffect, new Vector2(transform.position.x, transform.position.y - 2f), Quaternion.identity);
                }
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
        yield return new WaitForSeconds(1.5f);
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
