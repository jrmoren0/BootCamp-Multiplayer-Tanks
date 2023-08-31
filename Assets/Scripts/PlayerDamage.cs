using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerDamage : NetworkBehaviour 
{
   public void GetDamage()
    {
        Debug.Log(OwnerClientId + " is hit");
       GameManager.instance.ResetPlayerPosition(NetworkObject, OwnerClientId);



    }
}
