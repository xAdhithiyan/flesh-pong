using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Audio : MonoBehaviour
{
    [SerializeField]
    public Slider music;

    public void ModiyBGM()
    {
        GameManager.Instance.AudioManager.ModifyBGMVolume(music.value);
    }
    [SerializeField]
    public Slider sfx;

    public void ModiySFX()
    {
        GameManager.Instance.AudioManager.ModifySFXVolume(music.value);
    }
    public void Retry()
    {
        GameManager.Instance.LoadGame();
    }
}
