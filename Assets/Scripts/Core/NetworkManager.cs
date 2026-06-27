using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

//通信に関する処理をひとまとめにした司令塔クラス
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private NetworkRunner _runner;
    public string CurrentRoomId { get; private set; }

    //プレイヤーリストが更新されたときにUIへ知らせるイベント
    public event Action<List<PlayerRef>> OnPlayerListUpdated;

    //人数の変化を検知するための記憶用変数
    private int _lastPlayerCount = 0;

    //現在部屋にいるプレイヤーのリストを返す関数
    public List<PlayerRef> GetCurrentPlayers()
    {
        if(_runner != null && _runner.IsRunning)
        {
            return _runner.ActivePlayers.ToList();
        }
        return new List<PlayerRef>();
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        //通信中のみ、部屋の人数に変化がないかを監視する
        if (_runner != null && _runner.IsRunning)
        {
            //現在の接続人数を取得
            int currentPlayerCount = _runner.ActivePlayers.Count();

            //前回の人数と違っていたら（誰かが入ってきたor抜けた）
            if (currentPlayerCount != _lastPlayerCount)
            {
                _lastPlayerCount = currentPlayerCount;

                //プレイヤーのリストを作成して、UIにイベントを飛ばす
                List<PlayerRef> players = _runner.ActivePlayers.ToList();
                OnPlayerListUpdated?.Invoke(players);
            }
        }
    }

    public async Task<string> CreateRoomHost()
    {
        _runner = gameObject.GetComponent<NetworkRunner>();
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
        }
        _runner.ProvideInput = true;

        string generatedRoomId = GenerateRandomRoomId(6);

        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = generatedRoomId,
            PlayerCount = 6
        });

        if (result.Ok)
        {
            CurrentRoomId = generatedRoomId;
            Debug.Log($"ルーム作成成功。RoomID：{generatedRoomId}");
            return generatedRoomId;
        }
        else
        {
            Debug.LogError($"ルーム作成失敗：{result.ShutdownReason}");
            return null;
        }
    }

    public async Task<bool> JoinRoomClient(string inputRoomId)
    {
        _runner = gameObject.GetComponent<NetworkRunner>();
        if (_runner == null) { _runner = gameObject.AddComponent<NetworkRunner>(); }
        _runner.ProvideInput = true;

        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = inputRoomId
        });

        if (result.Ok)
        {
            CurrentRoomId = inputRoomId;
            Debug.Log($"ルーム{inputRoomId}に参加成功");
            return true;
        }
        else
        {
            Debug.LogError($"ルーム参加失敗：{result.ShutdownReason}");
            return false;
        }
    }
    private string GenerateRandomRoomId(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string result = "";
        for(int i = 0; i < length; i++)
        {
            result += chars[UnityEngine.Random.Range(0, chars.Length)];
        }
        return result;
    }
}