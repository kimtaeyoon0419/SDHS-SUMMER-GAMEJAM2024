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
    [SerializeField] public List<GameObject> aliveMonster;

    [Header("Portal")]
    [SerializeField] private Portal curStagePortal;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI monsterCountText;

    [Header("LastStage")]
    private int lastStage;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitStage("Tutorial");
    }

    private void InitStage(string stageName)
    {
        int index = 0;
        int stageCount = 0;
        if(aliveMonster.Count > 0)
        {
            aliveMonster.Clear();
        }

        foreach (var stage in Stages) // ���� �������� �˻�
        {
            if (stageName == stage.stageName)
            {
                playerObject.transform.position = stage.spawnTransform.position;
                foreach (Transform monsterSpawnPos in stage.spawnPos) // ��� ���� ������
                {
                    index = Random.Range(0, stage.monsters.Count);
                    GameObject enemy = Instantiate(stage.monsters[index].monster, monsterSpawnPos.transform.position, Quaternion.identity);
                    aliveMonster.Add(enemy);
                    Debug.Log("��������!");
                }
                CameraManager.instance.ChageCam(stageCount);
                curStagePortal = stage.stagePortal;
                break;
            }
            else
            {
                Debug.Log("�̸��� �ٸ��ϴ�!");
            }
            stageCount++;
        }
    }

    private void Update()
    {
        if (aliveMonster.Count <= 0)
        {
            monsterCountText.text = "��Ż��!";
            if (curStagePortal != null)
            {
                curStagePortal.isOpen = true;
                Debug.Log("��Ż����");
            }
        }
        else
        {
            monsterCountText.text = aliveMonster.Count.ToString();
        }
    }

    public void StageChageTrigger()
    {
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
