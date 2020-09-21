using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Car))]
public class EnvironmentSoundController : MonoBehaviour
{
    public Car car;
    AudioSource audioSrc; 
    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject != car.gameObject)
        {
            return;
        }
        if (audioSrc.isPlaying)
        {
            return;
        }
        audioSrc.Play();
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject != car.gameObject)
        {
            return;
        }
        audioSrc.Pause();
    }
}
