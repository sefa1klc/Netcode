using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WorldAIManager : Singleton<WorldAIManager>
{
    [Header("Debug")]
    [SerializeField] bool _deSpawnCharacter;
    [SerializeField] bool _reSpawnCharacter;

    [Header("Character")]
    [SerializeField] GameObject[] _aiCharacters;
    [SerializeField] List<GameObject> _spawnedCharacters;

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }
    }

    private void Update()
    {
        if(_reSpawnCharacter)
        {
            _reSpawnCharacter = false;
            SpawnAllCharacters();
        }
        if (_deSpawnCharacter)
        {
            _deSpawnCharacter = false;
            DeSpawnAllCharacters();
        }


    }

    private void OnServerStarted()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            StartCoroutine(WaitForSceneToLoadThenSpawnCharacter());
        }
    }

    private IEnumerator WaitForSceneToLoadThenSpawnCharacter()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
        SpawnAllCharacters();
    }
    
    private void SpawnAllCharacters()
    {
        foreach (var character in _aiCharacters)
        {
            Debug.Log(character);
            GameObject instantiatedCharacter = Instantiate(character);
            instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
            _spawnedCharacters.Add(instantiatedCharacter);
        }
    }

    private void DeSpawnAllCharacters()
    {
        foreach (var character in _spawnedCharacters)
        {
            character.GetComponent<NetworkObject>().Despawn();
        }
    }

    private void DisableSpawnAllCharacters()
    {

    }
}
