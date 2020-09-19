using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Car))]
[RequireComponent(typeof(AudioSource))]
public class CarEngine : MonoBehaviour
{
    Car car;
    AudioSource audioSrc;
    [SerializeField]
    float PitchModifier = 1;
    // Start is called before the first frame update
    void Start()
    {
        car = GetComponent<Car>();
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float soundPitch = Mathf.Abs(car.Throttle) * PitchModifier + 0.5f;

        if (car.Brake)
        {
            soundPitch = 0.5f;
        }
        else if (car.LongSpeed < 0)
        {
            soundPitch = Mathf.Abs(car.Throttle) * PitchModifier + 0.5f;
        }
        else {
            soundPitch = Mathf.Abs(car.Throttle) * PitchModifier + 0.5f;
        }


        audioSrc.pitch = soundPitch;

    }
}
