using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour 
{

    public ulong clientID;

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            PlayerDamage other = collision.gameObject.GetComponent<PlayerDamage>();

            if(other != null && clientID != other.OwnerClientId)
            {
                other.GetDamage();
                GameManager.instance.AddScore(clientID);
                Debug.Log(clientID + " hit " + other.OwnerClientId);
                Destroy(gameObject);
            }
        }
        
    }
}
