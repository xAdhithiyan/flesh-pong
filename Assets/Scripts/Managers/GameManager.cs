using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BuildScene
{
    game,menu
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField] public AudioManager AudioManager { get; private set; }
    [field: SerializeField] public TimerManager TimerManager { get; private set; }
    [field: SerializeField] public EnemyManager EnemyManager { get; private set; }
    [field: SerializeField] public PlayerComponentManager PCM { get; set; }
    public PlayerInputMap playerInputMap { get; private set; }

    public BuildScene scene { get; private set; }

    public CameraManager cameraManager { get; set; }
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
        scene = BuildScene.menu;
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
        AudioManager.PlaySound(AudioRef.Music, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
#if UNITY_EDITOR
    public static void UpdateScripts()
    {
        string assetPath = "Assets/Prefabs/GameManager.prefab";

        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(assetPath);

        contentsRoot.GetComponentInChildren<GameManager>().SetValues();

        PrefabUtility.SaveAsPrefabAsset(contentsRoot, assetPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
    }
#endif
    public void SetValues()
    {
        AudioManager = GetComponentInChildren<AudioManager>();
        TimerManager = GetComponentInChildren<TimerManager>();
        EnemyManager = GetComponentInChildren<EnemyManager>();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
        scene = BuildScene.game;
    }

    public void LoadStart()
    {
        SceneManager.LoadScene(0);
        scene = BuildScene.menu;
    }
    public void ModiyBGM(float Value)
    {
        AudioManager.ModifyBGMVolume(Value);
    }

    public void ModiySFX(float Value)
    {
        AudioManager.ModifySFXVolume(Value);
    }
}
