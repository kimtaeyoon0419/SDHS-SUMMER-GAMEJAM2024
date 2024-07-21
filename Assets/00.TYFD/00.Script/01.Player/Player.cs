using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [Header("Component")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Stat")]
    [SerializeField] public float maxHp;
    [SerializeField] private float curHp;
    public float CurHp
    {
        get { return curHp; } 
        set
        {
            if (curHp != value)
            {
                hpBar.fillAmount = curHp / maxHp;
            }

            if (curHp > value)
            {
                CameraManager.instance.CameraShake(5, 0.2f);
                StartCoroutine(Co_HitChageColor());
            }

            curHp = value;

            if (curHp > maxHp)
            {
                curHp = maxHp;
            }

            if (curHp < 0)
            {
                curHp = 0;
                Die();
            }
        }
    }
    [SerializeField] private float attackPower;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int attackCount;
    [SerializeField] private float jumpPower;
    private bool isJumping;
    private float hor;
    [SerializeField] public bool skillLock = true;
    [SerializeField] public int recoveryCount = 0;

    [Header("Animation")]
    private readonly int hashMove = Animator.StringToHash("Move");
    private readonly int hashAttack1 = Animator.StringToHash("Attack1");
    private readonly int hashAttack2 = Animator.StringToHash("Attack2");
    private readonly int hashAttack3 = Animator.StringToHash("Attack3");
    private readonly int hashDash = Animator.StringToHash("Dash");
    private readonly int hashIdle = Animator.StringToHash("Idle");

    [Header("Attack")]
    private bool isAttack = false;
    [SerializeField] private Transform attackBoxPos;
    [SerializeField] private Vector2 attackBoxSize;

    [Header("Color")]
    private Color hitColor = Color.black;

    [Header("HPbar")]
    [SerializeField] private Image hpBar;

    [Header("Dash")]
    private bool isDash;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCoolTime;
    [SerializeField] private float curDashCoolTime;
    [SerializeField] private bool dashOn;
    private float originalMoveSpeed;
    private Coroutine dashCoroutine;

    [Header("Skill")]
    [SerializeField] private GameObject skillObj;
    [SerializeField] private float maxSkillCoolTime;
    [SerializeField] private float curSkillCoolTime;

    [Header("Portal")]
    [SerializeField] private bool isPortal;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI portalText;
    [SerializeField] private Image skillBlackPnl;
    [SerializeField] private Transform skillPos;
    [SerializeField] public Image LockImage;
    [SerializeField] private Image dashBlackPnl;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        CurHp = maxHp;
    }

    private void Update()
    {

        if (GameManager.instance.curGameState != curGameState.selectItem)
        {
            InputFunction();
            if(!skillLock && curSkillCoolTime >= 0)
            {
                curSkillCoolTime -= Time.deltaTime;
                skillBlackPnl.fillAmount = curSkillCoolTime / maxSkillCoolTime;
            }

            if (curDashCoolTime >= 0)
            {
                curDashCoolTime -= Time.deltaTime;
                dashBlackPnl.fillAmount = curDashCoolTime / dashCoolTime;
            }
            else
            {
                dashOn = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackBoxPos.position, attackBoxSize);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            float attackPower = 0;
            if(collision.gameObject.GetComponent<Enemy>() != null)
            {
                attackPower = collision.gameObject.GetComponent<Enemy>().attackPower;
            }
            else if (collision.gameObject.GetComponent<Boss1>() != null)
            {
                attackPower = collision.gameObject.GetComponent<Boss1>().attackPower;
            }

            TakeDamage(attackPower);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal"))
        {
            if (collision.GetComponent<Portal>().isOpen)
            {
                portalText.gameObject.SetActive(true);
                isPortal = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal"))
        {
            portalText.gameObject.SetActive(false);
            isPortal = false;
        }
    }

    private void InputFunction()
    {
        float x = Input.GetAxisRaw("Horizontal");
        if (x == 0 && isDash)
            x = hor;

        Move(x);
        Flip(x);

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Attack();
        }
        if (Input.GetKeyDown(KeyCode.Z) && x != 0 && dashOn == false)
        {
            dashCoroutine = StartCoroutine(Co_Dash(x));
        }
        if(Input.GetKeyDown(KeyCode.E) && isPortal)
        {
            InGameUIManager.instance.StartFadeOut(true);
        }
        if (Input.GetKeyDown(KeyCode.F) && curSkillCoolTime <= 0)
        {
            curSkillCoolTime = maxSkillCoolTime;
            Skill();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            maxHp += 100000;
            CurHp += 100000;
        }
    }

    private void Skill()
    {
        if (isAttack)
            return;
        GameObject bolt = Instantiate(skillObj, skillPos.position, Quaternion.identity);
        animator.SetTrigger(hashAttack2);
        bolt.GetComponent<Bolt>().attackPower = attackPower * 1.25f;
        bolt.transform.localScale = -transform.localScale.normalized;
    }

    /// <summary>
    /// 움직임
    /// </summary>
    /// <param name="x">좌우 값</param>
    private void Move(float x)
    {
        if (!isDash)
        {
            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(hor * moveSpeed, rb.velocity.y);
        }
        if (x != 0)
        {
            animator.SetBool(hashMove, true);
        }
        else
        {
            animator.SetBool(hashMove, false);
        }
    }
    IEnumerator Co_Dash(float x)
    {
        hor = x;
        animator.SetTrigger(hashDash);
        isDash = true;
        dashOn = true;
        curDashCoolTime = dashCoolTime;
        originalMoveSpeed = moveSpeed;
        moveSpeed = dashSpeed;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true); // 몬스터와 충돌무시
        yield return new WaitForSeconds(dashTime);
        moveSpeed = originalMoveSpeed;
        isDash = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false); // 몬스터와 충돌무시
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

    private void Jump()
    {
        rb.velocity = Vector2.up * jumpPower;
        isJumping = true;
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
        AudioManager.instance.PlaySfx("Slash");
        StartCoroutine(Co_IsAttacking());
    }

    private void OnAttackBox()
    {
        Collider2D[] curEnemy = Physics2D.OverlapBoxAll(attackBoxPos.position, attackBoxSize, 0);
        if (curEnemy != null)
        {
            foreach (Collider2D enemy in curEnemy)
            {
                enemy.GetComponent<Enemy>()?.TakeDamage(attackPower, false);
                enemy.GetComponent<Boss1>()?.TakeDamage(attackPower, false);
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
        if(isDash)
        {
            return;
        }
        CurHp -= Damage;
    }

    IEnumerator Co_HitChageColor()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true); // 몬스터와 충돌무시
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false); // 몬스터와 충돌무시
    }

    private void Die()
    {
        GameManager.instance.curGameState = curGameState.gameOver;
    }

    public void StatUp(float hp, float damage)
    {
        maxHp += hp;
        CurHp += hp;
        attackPower += damage;
    }
}
