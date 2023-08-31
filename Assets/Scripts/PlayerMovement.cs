using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    private float _tankSpeed = 10;

    [SerializeField]
    private float _tankTurnSpeed = 10;



    private Rigidbody tankRB;


    private float _horizontal;

    private float _vertical;


    // Called when joining the network
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        tankRB = GetComponent<Rigidbody>();


       // gameObject.transform.position = new Vector3(0, 3, 0);

      


    }
   

    // Update is called once per frame
    void Update()
    {

        if (!IsOwner) return;



        if (IsServer && IsLocalPlayer)
        {
            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");
        }
        else
        {
            MovementServerRPC(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

    }



    private void FixedUpdate()
    {
       /// if (!IsOwner) return;

        tankRB.velocity = tankRB.transform.forward * _tankSpeed * _vertical;
        tankRB.rotation = Quaternion.Euler(transform.eulerAngles + transform.up * _horizontal * _tankTurnSpeed);

    }

    [ServerRpc]
   public void MovementServerRPC(float horizontal,float vertical)
    {
        _horizontal = horizontal;
        _vertical = vertical;
    }
}
