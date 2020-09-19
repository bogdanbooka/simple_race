using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashboardController : MonoBehaviour
{

    public Car car;

    public Text SpeedText;
    public Text GearText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SpeedText.text = car.LongSpeed.ToString("F0");
        GearText.text = car.Gear.ToString();
    }
}
