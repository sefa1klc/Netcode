using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : Singleton<RelayManager>
{
    public string _joincode;
    [SerializeField] private string _enviroment = "production";

    [SerializeField] private int _maxConnections = 4;

    public bool _isRelayEnabled => _transport != null && _transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

    public UnityTransport _transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    public async Task<RelayHostData> SetupRelay()
    {
        InitializationOptions _options = new InitializationOptions().SetEnvironmentName(_enviroment);  

        await UnityServices.InitializeAsync(_options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation _allocation = await Relay.Instance.CreateAllocationAsync(_maxConnections);

        RelayHostData _relayhostdata = new RelayHostData()
        {
            Key = _allocation.Key,
            Port = (ushort)_allocation.RelayServer.Port,
            AllocationID = _allocation.AllocationId,
            AllocationIDBytes = _allocation.AllocationIdBytes,
            IPv4Address = _allocation.RelayServer.IpV4,
            ConnectionsData = _allocation.ConnectionData
        };

        _relayhostdata.JoinCode = await Relay.Instance.GetJoinCodeAsync(_relayhostdata.AllocationID);

        _transport.SetRelayServerData(_relayhostdata.IPv4Address,_relayhostdata.Port, _relayhostdata.AllocationIDBytes,
            _relayhostdata.Key,_relayhostdata.ConnectionsData);

        _joincode =  _relayhostdata.JoinCode;

        return _relayhostdata;
    }

    public async Task<RelayJoinData> JoinRelay(string joinCode)
    {
        InitializationOptions _options = new InitializationOptions().SetEnvironmentName(_enviroment);

        await UnityServices.InitializeAsync(_options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        JoinAllocation _joinallocation = await Relay.Instance.JoinAllocationAsync(joinCode);

        RelayJoinData _relayJoindata = new RelayJoinData()
        {
            Key = _joinallocation.Key,
            Port = (ushort)_joinallocation.RelayServer.Port,
            AllocationID = _joinallocation.AllocationId,
            AllocationIDBytes = _joinallocation.AllocationIdBytes,
            IPv4Address = _joinallocation.RelayServer.IpV4,
            ConnectionsData = _joinallocation.ConnectionData,
            HostConnectionsData = _joinallocation.HostConnectionData,
            JoinCode = joinCode
        };

        _transport.SetRelayServerData(_relayJoindata.IPv4Address, _relayJoindata.Port, _relayJoindata.AllocationIDBytes,
            _relayJoindata.Key, _relayJoindata.ConnectionsData, _relayJoindata.HostConnectionsData);

        return _relayJoindata;
    }
}