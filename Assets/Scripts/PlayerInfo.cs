using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class PlayerInfo : NetworkBehaviour 
{
    [SerializeField]
    private TMP_Text _txtPlayerName;



    public NetworkVariable<FixedString64Bytes> _playerName = new NetworkVariable<FixedString64Bytes>(
    new FixedString64Bytes("Player Name"),
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _playerName.OnValueChanged += OnNameChanged;

        _txtPlayerName.SetText(_playerName.Value.ToString());
        gameObject.name = "Player_" + _playerName.Value.ToString();

        if (IsLocalPlayer)
        {
            GameManager.instance.SetLocalPlayer(NetworkObject);
        }

        GameManager.instance.OnplayerJoined(NetworkObject);


    }




    public void SetName(string name)
    {
        _playerName.Value = new FixedString64Bytes(name);
    }


    void OnNameChanged(FixedString64Bytes prevVal, FixedString64Bytes currentVal)
    {

        if(currentVal != prevVal)
        {
            _txtPlayerName.SetText(currentVal.Value);
            //Added Set Name
            GameManager.instance.SetPlayerName(NetworkObject, currentVal.Value.ToString());

        }

    }
  
}
