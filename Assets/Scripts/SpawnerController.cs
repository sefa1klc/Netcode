using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnerController : NetworkSingleton<SpawnerController>
{
    [SerializeField] private GameObject _poolingPrefab;
    [SerializeField] private int  _maxObjectCount;

    private void Awake()
    {
        NetworkObjectPool.Instance.InitializePool();
    }

    public void SpawnObject()
    {
        if (!IsServer) return;

        for (int i = 0; i < _maxObjectCount; i++) 
        {
            GameObject prefab = NetworkObjectPool.Instance.GetNetworkObject(_poolingPrefab).gameObject;

            prefab.transform.position = new Vector3(Random.Range(-10, 10), 10.0f, Random.Range(-10, 10));

            //spawn for client
            prefab.GetComponent<NetworkObject>().Spawn(); 
            //pool Instantiation
        }
    }
}
