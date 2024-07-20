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
    [SerializeField] private GameObject playerObject;

    [Header("Monster")]
    [SerializeField] public List<SpawnMonsterIndex> Stages = new List<SpawnMonsterIndex>();
    [SerializeField] public List<GameObject> aliveMonster;

    [Header("Portal")]
    [SerializeField] private Portal curStagePortal;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI monsterCountText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitStage("Stage1");
    }

    private void InitStage(string stageName)
    {
        int index = 0;
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
                curStagePortal = stage.stagePortal;
                break;
            }
            else
            {
                Debug.Log("이름이 다릅니다!");
            }
        }
    }

    private void Update()
    {
        if (aliveMonster.Count <= 0)
        {
            monsterCountText.text = "모든 몬스터를 처치했습니다!";
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
}
