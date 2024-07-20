using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private bool isOptionPanel;
    [SerializeField] private GameObject CreditPanel;
    [SerializeField] private bool isCreditPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickStart()
    {
        Debug.Log("�����ϱ�");
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
}
