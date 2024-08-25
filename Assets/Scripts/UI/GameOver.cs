using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public void Retry()
    {
        GameManager.Instance.LoadGame();
    }

    public void Menu()
    {
        GameManager.Instance.LoadStart();
    }
}
