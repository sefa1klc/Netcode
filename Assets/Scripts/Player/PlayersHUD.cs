using System;

using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayersHUD : NetworkBehaviour
{
    private NetworkVariable<NetworkString> _playersName = new NetworkVariable<NetworkString>();

    private bool _overlaySet = false;
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _playersName.Value = $"Player {OwnerClientId}";
        }
    }


    private void Update()
    {
        if (!_overlaySet && !string.IsNullOrEmpty(_playersName.Value))
        {
            SetOverlay();
        }

    }


    public void SetOverlay()
    {
        var localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        localPlayerOverlay.text = _playersName.Value;
    }

}

public struct NetworkString : INetworkSerializable
{
    private FixedString32Bytes info;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref info);
    }

    public override string ToString()
    {
        return info.ToString();
    }

    public static implicit operator string(NetworkString str) => str.ToString();
    public static implicit operator NetworkString(string str) => new NetworkString() { info = new FixedString32Bytes(str) };
}

