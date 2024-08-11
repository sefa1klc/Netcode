using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utiliy_DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float _timeUntilDestroyed = 5f;

    private void Awake()
    {
        Destroy(gameObject,_timeUntilDestroyed);
    }
}
