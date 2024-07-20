// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed;
    public float alphaSpeed;
    public TextMeshPro text;
    Color alpha;

    IEnumerator Start()
    {
        text = GetComponent<TextMeshPro>();
        alpha = text.color;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
        StopAllCoroutines();
    }

    void LateUpdate()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));    
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }

}
