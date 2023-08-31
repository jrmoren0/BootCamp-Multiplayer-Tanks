using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using NetworkEvent = Unity.Networking.Transport.NetworkEvent;

using TMPro;

public class GameNetworkManager : MonoBehaviour
{
    [SerializeField] private int _maxConnections = 6;  


    [SerializeField]private TMP_Text _statusText;
    [SerializeField] private TMP_InputField _ipAddress;
    [SerializeField] private UnityTransport _transport;

    [SerializeField] GameObject _hostButton;
    [SerializeField] GameObject _ClientButton;


    [SerializeField] private TMP_InputField _joinCodeText;

    [SerializeField] private TMP_Text _playerIDText;



    private string _playerID;

    private bool _clientAuthenticated = false;

    private string _joinCode;



    private async void Start()
    {
        await AuthenticatePlayer();
    }


    async Task AuthenticatePlayer() {

        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            _playerID = AuthenticationService.Instance.PlayerId;

            _clientAuthenticated = true;
            _playerIDText.text = "Player ID: " + _playerID;

            Debug.Log("Client Authentication Worked! " + _playerID);

        }
        catch( Exception e )
        {
            Debug.Log(e);

            }
    }


    public async Task<RelayServerData> AllocateRelayServerAndGetCode(int maxConnections, string region = null)
    {
        Allocation allocation;

        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
        }
        catch(Exception e)
        {
            Debug.Log("Relay allocation request failed");
            throw;
        }

        Debug.Log(allocation.ConnectionData[0]);
        Debug.Log(allocation.ConnectionData[1]);
        Debug.Log(allocation.AllocationId);

        try
        {
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }catch(Exception e)
        {
            Debug.Log("Unable to create join code");
            throw;
        }


        return new RelayServerData(allocation, "dtls");
    }


    public async Task<RelayServerData> JoinRelayServerWithCode(string joinCode)
    {
        JoinAllocation allocation;

        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log("Relay Allocation Join request failed");
            throw;
        }

        return new RelayServerData(allocation, "dtls");
    }

    IEnumerator ConfigureGetCodeAndJoinHost()
    {
        //Run Allocation and get code
        var allocateAndGetCode = AllocateRelayServerAndGetCode(_maxConnections);


        while (!allocateAndGetCode.IsCompleted)
        {
            yield return null;
        }

        if (allocateAndGetCode.IsFaulted)
        {
            Debug.Log("Connot Start the server due to an exception");
            yield break;
        }

        var relayServerdData = allocateAndGetCode.Result;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerdData);
        NetworkManager.Singleton.StartHost();


        _joinCodeText.gameObject.SetActive(true);
        _joinCodeText.text = _joinCode;

    }

    public void JoinAsHost()
    {
        if (!_clientAuthenticated)
        {
            Debug.Log("Client is not Authenticated,please try again");
           return;
        }


        StartCoroutine(ConfigureGetCodeAndJoinHost());
        ///NetworkManager.Singleton.StartHost();
        _statusText.text = "Joined as Host";
        Debug.Log("Joinnned");

        _hostButton.SetActive(false);
        _ClientButton.gameObject.SetActive(false);
        _joinCodeText.gameObject.SetActive(false);


    }

    IEnumerator ConfigureUseCodeJoinClient(string joinCode)
    {
        var joinAllocationFromCode = JoinRelayServerWithCode(joinCode);

        while (!joinAllocationFromCode.IsCompleted)
        {
            yield return null;
        }

        if(joinAllocationFromCode.IsFaulted)
        {
            Debug.Log("Connot Start the server due to an exception");
            yield break;
        }

        var relayServerdData = joinAllocationFromCode.Result;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerdData);
        NetworkManager.Singleton.StartClient();



        _statusText.text = "Joined as Client";
        Debug.Log("Joinnned");

    }




    public void JoinAsClient()
    {
        if (!_clientAuthenticated)
        {
            Debug.Log("Client is not Authenticated. Please try again ");
            return;
        }

        if(_joinCodeText.text.Length <= 0)
        {
            Debug.Log("Enter proper joinCode Pls");
     
        }

        StartCoroutine(ConfigureUseCodeJoinClient(_joinCodeText.text));



     
      
    }

    public void JoinAsServer()
    {
        _transport.ConnectionData.Address = _ipAddress.text.Replace(" ", " ");
        NetworkManager.Singleton.StartServer();
        _statusText.text = "Joined as Server";
    }

}
