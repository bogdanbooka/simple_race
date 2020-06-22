using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolowingCameraController : MonoBehaviour
{
    public float distanceToObject = 10;
    public float heightAboveObject = 3.5f;

    public float cameraDistancePointHeight = 2.5f;

    public Camera followingCamera;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    private void UpdateCameraRotation()
    {
        var gameObjectPos = gameObject.transform.position;
        gameObjectPos.y += cameraDistancePointHeight;
        var cameraPos = followingCamera.transform.position;

        var gameObjectToCameraVector = gameObjectPos - cameraPos;

        followingCamera.transform.rotation = Quaternion.LookRotation(gameObjectToCameraVector.normalized);
    }

    private void UpdateCameraPosition() 
    {
        var gameObjectPos = gameObject.transform.position;
        gameObjectPos.y = 0;

        var cameraPos = followingCamera.transform.position;
        cameraPos.y = 0;


        var gameObjectToCameraVector = gameObjectPos - cameraPos;
        var curDistance = gameObjectToCameraVector.magnitude;

        var followingCameraPosition = followingCamera.transform.position + gameObjectToCameraVector.normalized * (curDistance - distanceToObject);
        followingCameraPosition.y = gameObject.transform.position.y + heightAboveObject;

        followingCamera.transform.position = followingCameraPosition;
    }


}
