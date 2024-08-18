using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField] public AudioManager AudioManager { get; private set; }
    [field: SerializeField] public TimerManager TimerManager { get; private set; }
    [field: SerializeField] public EnemyManager EnemyManager { get; private set; }
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

    public static void UpdateScripts()
    {
        string assetPath = "Assets/Prefabs/GameManager.prefab";

        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(assetPath);

        contentsRoot.GetComponentInChildren<GameManager>().SetValues();

        PrefabUtility.SaveAsPrefabAsset(contentsRoot, assetPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
    }

    public void SetValues()
    {
        AudioManager = GetComponentInChildren<AudioManager>();
        TimerManager = GetComponentInChildren<TimerManager>();
        EnemyManager = GetComponentInChildren<EnemyManager>();
    }
}
