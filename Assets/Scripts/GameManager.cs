using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{



    #region Singleton

    public static GameManager instance;


    void Singleton()
    {
        if (instance != null && instance != this)
        {

            Destroy(instance);

        }
        instance = this;
    }

    [SerializeField] private TMP_InputField _PlayerNameField;

    private NetworkObject _localPlayer;



    public NetworkVariable<short> _state = new NetworkVariable<short>(

        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);



    Dictionary<ulong, string> _playerNames = new Dictionary<ulong, string>();


    Dictionary<ulong, int> _playerScores = new Dictionary<ulong, int>();



    [SerializeField] private Transform[] _startPositions;



    [Header("UI Elements")]
    [SerializeField] private GameObject _endGameScreen;

    [SerializeField] private TMP_Text _endGameMessage;

    [SerializeField] private TMP_Text _scoreUI;




    public void OnplayerJoined(NetworkObject playerObject)
    {
        playerObject.transform.position = _startPositions[(int)playerObject.OwnerClientId].position;
        _playerScores.Add(playerObject.OwnerClientId, 0);
    }




    #endregion


    private void Awake()
    {
        Singleton();
        if (IsServer)
        {
            _state.Value = 0;
        }
    }

    public void SetLocalPlayer(NetworkObject localPlayer)
    {

        _localPlayer = localPlayer;

        if(_PlayerNameField.text.Length > 0)
        {
            _localPlayer.GetComponent<PlayerInfo>().SetName(_PlayerNameField.text);
        }
        else
        {
            _localPlayer.GetComponent<PlayerInfo>().SetName("Player " + _localPlayer.OwnerClientId);
        }

        _PlayerNameField.gameObject.SetActive(false);
       
    }


    public void SetPlayerName(NetworkObject playerObject, string name)
    {
        if(_playerNames.ContainsKey(playerObject.OwnerClientId))
        {
            _playerNames[playerObject.OwnerClientId] = name;
        }
        else
        {
            _playerNames.Add(playerObject.OwnerClientId, name);
        }

    }

    //If the bullet hit this will be called by the server.(Only the server)

    public void AddScore(ulong playerID)
    {
        if (IsServer)
        {
            _playerScores[playerID]++;
            ShowScoreUI();
            CheckWinner(playerID);
        }
    }


    void CheckWinner(ulong PlayerId)
    {
        if(_playerScores[PlayerId] >= 10)
        {
            EndGame(PlayerId);
        }
    }

   public void EndGame(ulong WinnerID)
    {
        if (IsServer)
        {
            if(WinnerID == NetworkManager.LocalClientId)
            {
                _endGameMessage.text = "YOU WIN!";
            }
            else
            {
                _endGameMessage.text = "You Lose! The Winner is " + _playerNames[WinnerID];
            }

  

            ScoreInfo temp = new ScoreInfo();
            temp.score = _playerScores[WinnerID];
            temp.id = WinnerID;
            temp.name = _playerNames[WinnerID];

            string tempJSON = JsonUtility.ToJson(temp);

            ShowGameEndUIClientRPC(tempJSON);

           
        }


    }



    public void ShowScoreUI()
    {
        _scoreUI.text = "";

        PlayerScores _scores = new PlayerScores();
        _scores._scores = new List<ScoreInfo>();

        foreach (var item in _playerScores)
        {

            ScoreInfo temp = new ScoreInfo();
            temp.score = item.Value;
            temp.id = item.Key;
            temp.name = _playerNames[item.Key];
            _scores._scores.Add(temp);

            _scoreUI.text += item.Key + " " + _playerNames[item.Key] + " " + item.Value;

        }

        //Update the client
        UpdateClientScoreClientRPC(JsonUtility.ToJson(_scores));
    }

    //UpdateClient
    [ClientRpc]
    public void UpdateClientScoreClientRPC(string scoreInfo)
    {
        PlayerScores _scores = JsonUtility.FromJson<PlayerScores>(scoreInfo);
        Debug.Log(_scores._scores);
        _scoreUI.text = "";
        foreach (var item in _scores._scores) 
        {
            _scoreUI.text += item.id + " " + item.name + " " + item.score;
        }
    }


    [ClientRpc]
    public void ShowGameEndUIClientRPC(string winnerInfo)
    {
        _endGameScreen.SetActive(true);
        ScoreInfo info = JsonUtility.FromJson<ScoreInfo>(winnerInfo);


        if(info.id == NetworkManager.LocalClientId)
        {
            _endGameMessage.text = "YOU WIN!";
        }
        else
        {
            _endGameMessage.text = "You Lose! The Winner is " + info.name;
        }


    }

    public void ResetPlayerPosition(NetworkObject playerObject, ulong playerID)
    {
       // playerObject.transform.position = _startPositions[(int)playerID].position;
    }


}
