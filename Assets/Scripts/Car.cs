using System.Collections;
﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Transform centerOfMass;

    public float motorTorque = 80f;

    static float[] _transmissionRatios = new float[] { 4, 2, 1.2f, 0.9f, 0.0f };

    int _currentGear = 0;

    public int Gear
    {
        get 
        {
            return _currentGear;
        }
    }

    public float maxSteer = 20f;

    public float Throttle { get; set; }
    public float Steer { get; set; }

    public bool Brake { get; set; }

    float _logn_speed = 0;
    public float LongSpeed { 
        get 
        {
            return _logn_speed;
        }  
    }

    private Rigidbody _rigidbody;

    private Wheel[] wheels;

    public float wheelBase;
    public float halfTrack;
    public float turnRadius;

    private float ackeackermannPositiveKoef;
    private float ackeackermannNegativeKoef;

    private float ackeackermannLeft;
    private float ackeackermannRight;

    private Quaternion _initialRotation;
    private Vector3 _initialPostion;

    private void Start()
    {
        wheels = GetComponentsInChildren<Wheel>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = centerOfMass.localPosition;

        _initialRotation = gameObject.transform.rotation;
        _initialPostion = gameObject.transform.position;

        ackeackermannPositiveKoef = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + halfTrack));
        ackeackermannNegativeKoef = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - halfTrack));
    }

    public void ResetCar()
    {
        gameObject.transform.rotation = _initialRotation;
        gameObject.transform.position = _initialPostion;
        _rigidbody.velocity = new Vector3();
    }

    private void FixedUpdate()
    {
        _logn_speed = Vector3.Dot(gameObject.transform.forward.normalized, _rigidbody.velocity.normalized) * _rigidbody.velocity.magnitude * 3.6f;

        _currentGear = CurrentGear();

        Steer = GameManager.Instance.InputController.SteerInput;
        Throttle = GameManager.Instance.InputController.ThrottleInput;

        Brake = GameManager.Instance.InputController.ParkingBrake;

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

        float appliedTorque = Throttle * motorTorque * GearRatio(_currentGear);
        

        if (Mathf.Sign(Throttle) * Mathf.Sign(_logn_speed) < 0)
        {
            //deceleration
            if (Mathf.Abs(_logn_speed) > 15)
            {
                Throttle = 0;
                appliedTorque = 0;
                Brake = true;
            }
        }
        

        print($" {_currentGear}, {_logn_speed}, {appliedTorque}, {GearRatio(_currentGear)}");

        foreach (var wheel in wheels) 
        {
            wheel.SteerAngle = wheel.isLeft ? ackeackermannLeft : ackeackermannRight;
               
            //wheel.SteerAngle = Steer * maxSteer;
            wheel.Torque = appliedTorque;

            wheel.Brake = Brake;
        }
    }

    private int CurrentGear()
    {
        int gear = 0;
        if (_logn_speed >= -40 && _logn_speed <20)
            gear = 0;
        else if (_logn_speed >= 20 && _logn_speed < 40)
            gear = 1;
        else if (_logn_speed >= 40 && _logn_speed < 60)
            gear = 2;
        else if (_logn_speed >= 60 && _logn_speed < 90)
            gear = 3;
        else
            gear = 4;

        return gear;
    }

    private float GearRatio(int gear)
    {
        return _transmissionRatios[gear];
    }

    private void Update()
    {
    }
}
