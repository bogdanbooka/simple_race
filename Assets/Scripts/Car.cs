using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Camera followingCamera;
    private Quaternion _cameraRot;
    private Vector3 _cameraPos;

    public Transform centerOfMass;

    public float motorTorque =  300f;
    public float maxSteer = 20f;

    public float Throttle { get; set; }
    public float Steer { get; set; }

    private Rigidbody _rigidbody;

    private Wheel[] wheels;

    public float wheelBase;
    public float halfTrack;
    public float turnRadius;

    private float ackeackermannPositiveKoef;
    private float ackeackermannNegativeKoef;

    private float ackeackermannLeft;
    private float ackeackermannRight;

    private void Start()
    {
        wheels = GetComponentsInChildren<Wheel>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = centerOfMass.localPosition;

        _cameraRot = followingCamera.transform.rotation;
        _cameraPos = followingCamera.transform.localPosition;

        ackeackermannPositiveKoef = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + halfTrack));
        ackeackermannNegativeKoef = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - halfTrack));
    }

    private void FixedUpdate()
    {
        Steer = GameManager.Instance.InputController.SteerInput;
        Throttle = GameManager.Instance.InputController.ThrottleInput;

        if (Steer > 0)
        {
            ackeackermannLeft = ackeackermannPositiveKoef * Steer;
            ackeackermannRight = ackeackermannNegativeKoef * Steer;
        }
        else if (Steer < 0)
        {
            ackeackermannLeft = ackeackermannNegativeKoef * Steer;
            ackeackermannRight = ackeackermannPositiveKoef * Steer;
        }
        else 
        {
            ackeackermannLeft = 0f;
            ackeackermannRight = 0f;
        }

        foreach (var wheel in wheels) 
        {
            wheel.SteerAngle = wheel.isLeft ? ackeackermannLeft : ackeackermannRight;
               
            //wheel.SteerAngle = Steer * maxSteer;
            wheel.Torque = Throttle * motorTorque;
        }
    }


    private void Update()
    {
        var pos = Vector3.zero;
        var rot = Quaternion.identity;

        followingCamera.transform.rotation = _cameraRot;
        followingCamera.transform.position = gameObject.transform.position + _cameraPos;
    }
}
