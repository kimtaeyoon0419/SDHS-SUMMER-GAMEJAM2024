// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] private float maxHp;
    [SerializeField] private float curHp;

    [Header("Color")]
    private SpriteRenderer spriteRenderer;
    private Color hitColor = Color.black;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        curHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        curHp -= damage;
        Debug.Log("몬스터의 현재 체력 : " + curHp);
        StartCoroutine(Co_ChangeHitColor());
        if(curHp < 0)
        {
            Debug.Log("죽었다");
        }
    }

    IEnumerator Co_ChangeHitColor()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }
}
