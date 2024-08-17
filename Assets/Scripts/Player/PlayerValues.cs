using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using KevinCastejon.MoreAttributes;

public class PlayerValues : MonoBehaviour, PCMInterface
{
    // Start is called before the first frame update
    [field: SerializeField]
    public PlayerComponentManager PCM {  get; set; }
    [field: SerializeField]
    public int StartingHealth;
    [SerializeField, ReadOnly]
    private int CurrentHealth;

    void Start()
    {
        CurrentHealth = StartingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateCamForward();
    }

    public void UpdateHealth(int damageToTake)
    {
        CurrentHealth -= damageToTake;
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
            //death stuff
        }
    }
}
