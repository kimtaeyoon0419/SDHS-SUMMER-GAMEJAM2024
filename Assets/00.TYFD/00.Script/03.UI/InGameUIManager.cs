using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class SelectBtns
{
    public string btnName;
    public GameObject btnObj;
}

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager instance;

    [Header("FadePnl")]
    [SerializeField] private GameObject fadePanel;

    [Header("SelectBtn")]
    [SerializeField] private List<SelectBtns> selectButtons;
    [SerializeField] private Transform[] selectBtnPos;
    [SerializeField] private TextMeshProUGUI statUptext;

    [Header("GameOver")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI stageScoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private GameObject returnBtn;

    private List<GameObject> activeButtons = new List<GameObject>();
    bool isGameOver;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartFadeIn();
    }

    private void Update()
    {
        if (GameManager.instance.curGameState == curGameState.gameOver && !isGameOver)
        {
            isGameOver = true;
            StartCoroutine(Co_GameOverPanelAnim());
        }
    }

    IEnumerator Co_GameOverPanelAnim()
    {
        gameOverPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        gameOverText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        stageScoreText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Co_SetStageScore());
        returnBtn.SetActive(true);
    }


    public void StartFadeIn()
    {
        StartCoroutine(Co_FadeIn());
    }

    public void StartFadeOut(bool ChageStage)
    {
        StartCoroutine(Co_FadeOut(ChageStage));
    }

    public void UIClickSound()
    {
        AudioManager.instance.PlaySfx("ClickSound");
    }

    private void OnSelectBtn()
    {
        List<int> selectedIndices = new List<int>();
        int index;

        // Shuffle the selectButtons to avoid picking the same button twice
        List<SelectBtns> shuffledButtons = new List<SelectBtns>(selectButtons);
        shuffledButtons.Shuffle();

        activeButtons.Clear();

        for (int i = 0; i < 3; i++)
        {
            // Avoid selecting the same button twice
            do
            {
                index = Random.Range(0, shuffledButtons.Count);
            } while (selectedIndices.Contains(index));

            selectedIndices.Add(index);

            // Skip skillLock button if already unlocked
            if (shuffledButtons[index].btnName == "SkillLock" && !StageManager.instance.playerObject.GetComponent<Player>().skillLock)
            {
                i--;
                continue;
            }

            shuffledButtons[index].btnObj.transform.position = selectBtnPos[i].position;
            shuffledButtons[index].btnObj.SetActive(true);
            activeButtons.Add(shuffledButtons[index].btnObj);
        }
    }

    public void ReturnMain()
    {
        StartCoroutine(Co_ReturnMain());
    }

    private IEnumerator Co_SetStageScore()
    {
        for(int i = 0; i <= StageManager.instance.curstage; i++)
        {
            stageScoreText.text = "도달한 스테이지 : " + i.ToString();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator Co_ReturnMain()
    {
        yield return StartCoroutine(Co_FadeOut(false));
        SceneManager.LoadScene("JHScene");
    }

    public void SeletBtnActive(int btnNum)
    {
        float hp;
        float damage;

        switch (btnNum)
        {
            case 0:
                hp = Random.Range(5, 10);
                statUptext.text = "체력이 " + hp.ToString() + " 증가했습니다";
                StageManager.instance.playerObject.GetComponent<Player>().StatUp(hp, 0);
                statUptext.gameObject.SetActive(true);

                break;
            case 1:
                damage = Random.Range(2, 5);
                statUptext.text = "공격력이 " + damage.ToString() + " 증가했습니다";
                StageManager.instance.playerObject.GetComponent<Player>().StatUp(0, damage);
                statUptext.gameObject.SetActive(true);

                break;
            case 2:
                hp = Random.Range(2, 5);
                damage = Random.Range(1, 3);
                statUptext.text = "공격력이 " + damage.ToString() + ", 체력이 " + hp.ToString() + " 증가했습니다";
                statUptext.gameObject.SetActive(true);

                StageManager.instance.playerObject.GetComponent<Player>().StatUp(hp, damage);
                break;
            case 3:
                statUptext.text = "스킬이 해금되었습니다";
                statUptext.gameObject.SetActive(true);
                StageManager.instance.playerObject.GetComponent<Player>().skillLock = false;
                StageManager.instance.playerObject.GetComponent<Player>().LockImage.gameObject.SetActive(false);
                break;
            case 4:
                statUptext.text = "부활 기회가 1회 증가했습니다";
                statUptext.gameObject.SetActive(true);
                StageManager.instance.playerObject.GetComponent<Player>().recoveryCount++;
                break;
            case 5:
                damage = Random.Range(1, 3);
                float curMaxHp = StageManager.instance.playerObject.GetComponent<Player>().maxHp;
                hp =  curMaxHp / damage;
                StageManager.instance.playerObject.GetComponent<Player>().CurHp += hp;
                statUptext.text = "체력을 " + hp.ToString() + " 만큼 회복했습니다";
                break;
        }

        // Deactivate all buttons after one is selected
        foreach (GameObject btn in activeButtons)
        {
            btn.SetActive(false);
        }
        
        GameManager.instance.curGameState = curGameState.fightStage;
        activeButtons.Clear();
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

    IEnumerator Co_FadeOut(bool ChageStage)
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

        if (ChageStage)
        {
            StageManager.instance.StageChageTrigger();
            GameManager.instance.curGameState = curGameState.selectItem;
            OnSelectBtn();
        }
    }
}

// Helper method to shuffle a list
public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
