using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolowingCameraController : MonoBehaviour
{
    public Camera followingCamera;
    private Quaternion _cameraRot;
    private Vector3 _cameraPos;

    // Start is called before the first frame update
    void Start()
    {
        _cameraRot = followingCamera.transform.rotation;
        _cameraPos = followingCamera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        followingCamera.transform.rotation = _cameraRot;
        followingCamera.transform.position = gameObject.transform.position + _cameraPos;
    }
}
