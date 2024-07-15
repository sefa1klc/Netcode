using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTag : MonoBehaviour
{

    private Transform _cameraTransform;

    void Start()
    {
        if(UnityEngine.Camera.main.transform != null)
            _cameraTransform = UnityEngine.Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward, _cameraTransform.rotation * Vector3.up);

    }
}
