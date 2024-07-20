// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;
using TMPro;

public class SelectText : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 2.0f; // ���̵� �ƿ��� �ɸ��� �ð� (��)

    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        // ���� ���� �ٽ� 1�� ����
        if (textComponent != null)
        {
            Color color = textComponent.color;
            textComponent.color = new Color(color.r, color.g, color.b, 1f);
        }
        // �ڷ�ƾ ����
        StartCoroutine(StartFadeOut());
    }

    private IEnumerator StartFadeOut()
    {
        if (textComponent != null)
        {
            Color originalColor = textComponent.color;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            gameObject.SetActive(false);
        }
    }
}
