using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField]
    private GameObject _bulletObject;

    [SerializeField]
    private float _shootSpeed;

    [SerializeField]
    private Transform _shootPoint;



    private Rigidbody _tankRB;


    // Start is called before the first frame update

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _tankRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
      

        if (Input.GetButtonDown("Jump")){

            if (IsServer && IsLocalPlayer)
            {
                Shoot(OwnerClientId);
            }

            else if(IsClient && IsLocalPlayer)
            {
                RequestShootServerRPC();
            }

        }
    }


    void Shoot(ulong ownerID)
    {

        GameObject bullet = Instantiate(_bulletObject, _shootPoint.position, _shootPoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<Bullet>().clientID = ownerID; 
        
        bullet.GetComponent<Rigidbody>().AddForce(_tankRB.velocity + bullet.transform.forward * _shootSpeed, ForceMode.VelocityChange);
        Debug.Log("Host Shot");
    }

    [ServerRpc]
    void RequestShootServerRPC(ServerRpcParams serverRpcParams = default)
    {
        Shoot(serverRpcParams.Receive.SenderClientId);

        Debug.Log("Client Shot");
    }

}
