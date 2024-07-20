// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public enum curGameState
{
    title,
    fightStage,
    selectItem,
    gameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public curGameState curGameState;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
