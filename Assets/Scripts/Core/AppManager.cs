using System;
using System.Collections;
using UnityEngine;

public enum GameState
{
    Title,              // タイトル画面
    RoomSelect,         // ルーム選択
    RoomLobby,          // メンバー待機
    RoleSelection,      // 親決め（3秒の自動演出）
    GenreSelection,     // ジャンル決め
    InGame,             // ゲーム中
    Result              // ゲーム終了（脱落・結果発表）
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
        Debug.Log($"【State変更】現在の状態が [{nextState}] になりました。");

        OnStateChanged?.Invoke(nextState);
    }
}