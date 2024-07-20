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

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI monsterCountText;

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
                break;
            }
            else
            {
                Debug.Log("�̸��� �ٸ��ϴ�!");
            }
        }
    }

    private void Update()
    {
        if (Stages.Count <= 0)
        {
            monsterCountText.text = "��� ���͸� óġ�߽��ϴ�!";
        }
        else
        {
            monsterCountText.text = aliveMonster.Count.ToString();
        }
    }
}
