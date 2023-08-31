using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class SimpleChat : NetworkBehaviour
{
    [SerializeField]
    private TMP_Text _chatText;

    [SerializeField]
    private TMP_InputField _chatInput;


    public void SendChat()
    {
        if(IsServer)
        {
            ChatClientRPC(NetworkManager.Singleton.LocalClientId + _chatInput.text);
        }else if (IsClient)
        {
           ChatServerRPC(NetworkManager.Singleton.LocalClientId + _chatInput.text);
        }
    }

   [ServerRpc(RequireOwnership = false)]
   public void ChatServerRPC(string message)
    {
        if (!IsHost)
         _chatText.text += "\n" + message;

        ChatClientRPC(message);
    }

    [ClientRpc]
    public void ChatClientRPC(string message)
    {
        _chatText.text += "\n" + message;
    }


}
