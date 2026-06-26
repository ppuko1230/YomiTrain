using System;
using System.Collections;
using UnityEngine;

public enum GameState
{
    Title,              // �^�C�g�����
    RoomSelect,         // ���[���I��
    RoomLobby,          // �����o�[�ҋ@
    RoleSelection,      // �e���߁i3�b�̎������o�j
    GenreSelection,     // �W����������
    InGame,             // �Q�[����
    Result              // �Q�[���I���i�E���E���ʔ��\�j
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
        Debug.Log($"�yState�ύX�z���݂̏�Ԃ� [{nextState}] �ɂȂ�܂����B");

        OnStateChanged?.Invoke(nextState);
    }
}