using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField] public AudioManager AudioManager { get; private set; }
    [field: SerializeField] public TimerManager TimerManager { get; private set; }
    public PlayerInputMap playerInputMap { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        playerInputMap = new PlayerInputMap();
    }

    public void EnablePlayer()
    {
        playerInputMap.Player.Enable();
    }
    public void DisablePlayer()
    {
        playerInputMap.Player.Disable();
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
