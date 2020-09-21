using System.Collections;
﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Transform centerOfMass;

    public float motorTorque = 80f;

    int _currentGear = 0;
    float _gearRatio = 0;

    public int Gear
    {
        get 
        {
            return _currentGear;
        }
    }

    float _engineRpm;
    public float EngineRpm 
    {
        get 
        {
            return _engineRpm; 
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

    public bool AnyWheelIsSlipping { get; private set; }

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

        UpdateTransmission();

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

        float appliedTorque = Throttle * motorTorque * _gearRatio;
        

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
        

        //print($" {_currentGear}, {_logn_speed}, {appliedTorque}, {_gearRatio}");

        bool anyWheelIsSlipping = false;

        foreach (var wheel in wheels) 
        {
            wheel.SteerAngle = wheel.isLeft ? ackeackermannLeft : ackeackermannRight;
               
            //wheel.SteerAngle = Steer * maxSteer;
            wheel.Torque = appliedTorque;

            wheel.Brake = Brake;

            anyWheelIsSlipping |= wheel.WheelIsSlipping;
        }

        AnyWheelIsSlipping = anyWheelIsSlipping;

        if (anyWheelIsSlipping) 
        {
            _engineRpm = Mathf.Abs(Throttle);
        }
    }

    private float GetRpm(float minSpeed, float maxSpeed) 
    {
        float absSpeed = Mathf.Abs(_logn_speed);
        if (absSpeed <= minSpeed) 
        {
            return Throttle;
        }

        if (absSpeed >= maxSpeed) {
            return Throttle;
        }

        float val = (absSpeed - minSpeed) / (maxSpeed - minSpeed);

        return val > Mathf.Abs(Throttle) ? Throttle : val;
    }

    private void UpdateTransmission()
    {
        if (_logn_speed >= -40 && _logn_speed < 0 ||
            _logn_speed >= 0 && _logn_speed < 20)
        {
            _currentGear = 1;
            _gearRatio = 4;
            if (_logn_speed >= -40 && _logn_speed < 0) 
            {
                _engineRpm = GetRpm(0,40);
            }
            else 
            {
                _engineRpm = GetRpm(0, 20);
            }
        }
        else if (_logn_speed >= 20 && _logn_speed < 40)
        {
            _currentGear = 2;
            _gearRatio = 2;
            _engineRpm = GetRpm(20, 40);
        }
        else if (_logn_speed >= 40 && _logn_speed < 60)
        {
            _currentGear = 3;
            _gearRatio = 1.2f;
            _engineRpm = GetRpm(40, 60);
        }
        else if (_logn_speed >= 60 && _logn_speed < 90)
        {
            _currentGear = 4;
            _gearRatio = 0.9f;
            _engineRpm = GetRpm(60, 90);
        }
        else
        {
            _currentGear = 0;
            _gearRatio = 0;
            _engineRpm = Throttle;
        }
    }

    private void Update()
    {
    }
}
