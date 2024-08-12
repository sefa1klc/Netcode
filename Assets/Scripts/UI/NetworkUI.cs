using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class NetworkUI : MonoBehaviour
{

    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private TMP_InputField _joinCodeInput;
    [SerializeField] private TextMeshProUGUI _Joincode;

    private bool _hasServerStarted;

    private void Awake()
    {
        _Joincode.enabled = false;

        hostBtn.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance._isRelayEnabled)
            {
                await RelayManager.Instance.SetupRelay();
                _Joincode.enabled = true;
                _Joincode.text = RelayManager.Instance._joincode.ToString();
            }
            NetworkManager.Singleton.StartHost();
        });

        clientBtn.onClick.AddListener( async() =>
        {
            if (RelayManager.Instance._isRelayEnabled && !string.IsNullOrEmpty(_joinCodeInput.text))
            {
                await RelayManager.Instance.JoinRelay(_joinCodeInput.text);
            }

            NetworkManager.Singleton.StartClient();
        });

        //NetworkManager.Singleton.OnServerStarted += () =>
        //{
        //    _hasServerStarted = true;
        //};

        

    }

    
}
