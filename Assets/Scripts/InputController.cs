﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public string inputSteerAxis = "Horizontal";
    public string inputThrottleAxis = "Vertical";


    public float ThrottleInput { get; private set; }
    public float SteerInput { get; private set; }

    public bool ParkingBrake { get; private set; }



    // Update is called once per frame
    void FixedUpdate()
    {
        SteerInput = Input.GetAxis(inputSteerAxis);
        ThrottleInput = Input.GetAxis(inputThrottleAxis);

        ParkingBrake = Input.GetKey(KeyCode.Space);
    }
}
