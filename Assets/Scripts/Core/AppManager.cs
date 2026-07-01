using System;
using System.Collections;
using UnityEngine;

public enum GameState
{
    Title,
    RoomSelect,
    RoomLobby,
    RoleSelection,
    GenreSelection,
    InGame,
    Result
}

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }

    public event Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ChangeState(GameState.Title);
    }
    public void ChangeState(GameState nextState)
    {
        CurrentState = nextState;
        Debug.Log($"ルームStateが [{nextState}] に変わりました");

        OnStateChanged?.Invoke(nextState);
    }
}