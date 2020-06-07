using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public InputController InputController { get; private set; }

    private void Awake()
    {
        if (Instance != null) 
        {
            throw new System.Exception("Someone has already created the GameManager instance");
        }
        Instance = this;

        InputController = GetComponentInChildren<InputController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
