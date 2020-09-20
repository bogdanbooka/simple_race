﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSkid : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    SkidmarksController skidmarksController;


    WheelCollider wheelCollider;
	Wheel wheel;
    WheelHit wheelHitInfo;

    const float SKID_FX_SPEED = 5f; // Min side slip speed in m/s to start showing a skid
    const float MAX_SKID_INTENSITY = 20.0f; // m/s where skid opacity is at full intensity
    const float WHEEL_SLIP_MULTIPLIER = 1.0f; // For wheelspin. Adjust how much skids show
    int lastSkid = -1; // Array index for the skidmarks controller. Index of last skidmark piece this wheel used
    float lastFixedUpdateTime;

	protected void Awake()
    {
        wheelCollider = GetComponent<WheelCollider>();
		wheel = GetComponent<Wheel>();
		lastFixedUpdateTime = Time.time;
    }

	protected void LateUpdate()
	{
		if (wheelCollider.GetGroundHit(out wheelHitInfo))
		{
			// Check sideways speed

			// Gives velocity with +z being the car's forward axis
			Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
			float skidTotal = Mathf.Abs(localVelocity.x);

			// Check wheel spin as well

			float wheelAngularVelocity = wheelCollider.radius * ((2 * Mathf.PI * wheelCollider.rpm) / 60);
			float carForwardVel = Vector3.Dot(rb.velocity, transform.forward);
			float wheelSpin = Mathf.Abs(carForwardVel - wheelAngularVelocity) * WHEEL_SLIP_MULTIPLIER;


			wheel.WheelIsSlipping = wheelAngularVelocity - carForwardVel > 10;

			//Debug.Log($"[fwd {carForwardVel}] - [bckwd {wheelAngularVelocity}] = [{wheelSpin}]");

			// NOTE: This extra line should not be needed and you can take it out if you have decent wheel physics
			// The built-in Unity demo car is actually skidding its wheels the ENTIRE time you're accelerating,
			// so this fades out the wheelspin-based skid as speed increases to make it look almost OK
			//wheelSpin = Mathf.Max(0, wheelSpin * (10 - Mathf.Abs(carForwardVel)));

			skidTotal += wheelSpin;

			// Skid if we should
			if (skidTotal >= SKID_FX_SPEED)
			{
				float intensity = Mathf.Clamp01(skidTotal / MAX_SKID_INTENSITY);
				//Debug.Log($"[skidTotal = {skidTotal}] [intensity {intensity}]");
				
				// Account for further movement since the last FixedUpdate
				Vector3 skidPoint = wheelHitInfo.point + (rb.velocity * (Time.time - lastFixedUpdateTime));
				lastSkid = skidmarksController.AddSkidMark(skidPoint, wheelHitInfo.normal, intensity, lastSkid);
			}
			else
			{
				lastSkid = -1;
			}
		}
		else
		{
			lastSkid = -1;

			wheel.WheelIsSlipping = true;
		}
	}

	protected void FixedUpdate()
    {
        lastFixedUpdateTime = Time.time;
    }
}
