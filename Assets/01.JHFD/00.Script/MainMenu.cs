using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private bool isOptionPanel;
    [SerializeField] private GameObject CreditPanel;
    [SerializeField] private bool isCreditPanel;
    [SerializeField] private GameObject StartPanel;
    [SerializeField] private bool isStartPanel;

    [Header("Fade")]
    [SerializeField] private GameObject fadePanel;

    [Header("Volum")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // Start is called before the first frame update
    void Start()
    {
        StartFadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickStart()
    {
        
    }

    public void OnClickOption()
    {
        isOptionPanel = !isOptionPanel;
        optionPanel.SetActive(isOptionPanel);
    }


    public void OnClickCredit()
    {
        isCreditPanel = !isCreditPanel;
        CreditPanel.SetActive(isCreditPanel);
    }
    public void OnClilckQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else 
        Application.Quit();
#endif
    }

    public void StartFadeIn()
    {
        StartCoroutine(Co_FadeIn());
    }

    public void StartFadeOut(string nextScene)
    {
        StartCoroutine (Co_FadeOut(nextScene));
    }

    public void MusicVolum()
    {
        AudioManager.instance.MusicVolume(musicSlider.value);
    }
    public void SfxVolum()
    {
        AudioManager.instance.SFXVolume(sfxSlider.value);
    }

    IEnumerator Co_FadeIn()
    {
        Debug.Log("페이드 인 시작");
        fadePanel.SetActive(true);
        Image image = fadePanel.GetComponent<Image>();
        Color tempColor = image.color;
        tempColor.a = 1;
        image.color = tempColor;
        while (image.color.a > 0)
        {
            tempColor.a -= Time.deltaTime;
            image.color = tempColor;
            if (tempColor.a <= 0f) tempColor.a = 0f;
            yield return null;
        }
        image.color = tempColor;
        fadePanel.SetActive(false);
    }

    IEnumerator Co_FadeOut(string nextScene)
    {
        Debug.Log("페이드 아웃 시작");
        fadePanel.SetActive(true);
        Image image = fadePanel.GetComponent<Image>();
        Color tempColor = image.color;
        tempColor.a = 0;
        image.color = tempColor;
        while (image.color.a < 1)
        {
            yield return null;
            tempColor.a += Time.deltaTime;
            image.color = tempColor;
            if (tempColor.a >= 1f) tempColor.a = 1f;
        }
        image.color = tempColor;

        if(!string.IsNullOrEmpty(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
