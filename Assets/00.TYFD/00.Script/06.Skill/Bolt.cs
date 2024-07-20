// # System
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;

// # Unity
using UnityEngine;

public class Bolt : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject hitEffect;

    [SerializeField] public float attackPower;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * -transform.localScale.x * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Ground"))
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
            collision.GetComponent<Enemy>()?.TakeDamage(attackPower, true);
            collision.GetComponent<Boss1>()?.TakeDamage(attackPower, true);
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}
