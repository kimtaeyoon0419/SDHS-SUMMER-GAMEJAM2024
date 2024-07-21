// # System
using System.Collections;
using System.Collections.Generic;
using TMPro;

// # Unity
using UnityEngine;

[System.Serializable]
public class Monsters
{
    public string monsterName;
    public GameObject monster;
}

[System.Serializable]
public class SpawnMonsterIndex
{
    public string stageName;
    public Portal stagePortal;
    public Transform spawnTransform;
    public List<Transform> spawnPos;
    public List<Monsters> monsters;
}

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    [Header("Player")]
    [SerializeField] public GameObject playerObject;

    [Header("Monster")]
    [SerializeField] public List<SpawnMonsterIndex> Stages = new List<SpawnMonsterIndex>();
    [SerializeField] public SpawnMonsterIndex bossStage;
    [SerializeField] public List<GameObject> aliveMonster;

    [Header("Portal")]
    [SerializeField] private Portal curStagePortal;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI monsterCountText;

    [Header("LastStage")]
    private int lastStage;

    [Header("StageCount")]
    [SerializeField] public int curstage = 1;
    [SerializeField] private TextMeshProUGUI curStageText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitStage("Tutorial");
        AudioManager.instance.PlayMusic("StageMusic1");
    }

    private void InitStage(string stageName)
    {
        int index = 0;
        int stageCount = 0;

        if (aliveMonster.Count > 0)
        {
            aliveMonster.Clear();
        }
        else
        {
            foreach (var stage in Stages) // 현재 스테이지 검사
            {
                if (stageName == stage.stageName)
                {
                    playerObject.transform.position = stage.spawnTransform.position;
                    foreach (Transform monsterSpawnPos in stage.spawnPos) // 모든 스폰 포지션
                    {
                        index = Random.Range(0, stage.monsters.Count);
                        GameObject enemy = Instantiate(stage.monsters[index].monster, monsterSpawnPos.transform.position, Quaternion.identity);
                        aliveMonster.Add(enemy);
                        Debug.Log("스폰성공!");
                    }
                    CameraManager.instance.ChageCam(stageCount);
                    curStagePortal = stage.stagePortal;
                    curstage++;
                    curStageText.text = curstage.ToString() + " Stage";
                    break;
                }
                else
                {
                    Debug.Log("이름이 다릅니다!");
                }
                stageCount++;
            }
        }
    }

    private void Update()
    {
        if (aliveMonster.Count <= 0)
        {
            monsterCountText.text = "포탈로!";
            if (curStagePortal != null)
            {
                curStagePortal.isOpen = true;
                Debug.Log("포탈오픈");
            }
        }
        else
        {
            monsterCountText.text = aliveMonster.Count.ToString();
        }
    }

    public void StageChageTrigger()
    {
        curStagePortal.isOpen = false;
        if(curstage % 5 == 0)
        {
            InitStage("BossStage");
        }
        int index = Random.Range(0, Stages.Count - 1);
        while(index != lastStage)
        {
            index = Random.Range(0, Stages.Count - 1);
        }
        lastStage = index;

        switch (index)
        {
            case 0:
                InitStage("Stage1");
                break;
            case 1:
                InitStage("Stage2");
                break;
            case 2:
                InitStage("Stage3");
                break;
        }
    }
}
