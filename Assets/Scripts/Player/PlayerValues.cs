using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using KevinCastejon.MoreAttributes;

public class PlayerValues : MonoBehaviour, PCMInterface, DamageInterface
{
    // Start is called before the first frame update
    [field: SerializeField]
    public PlayerComponentManager PCM {  get; set; }
    [field: SerializeField]
    public int StartingHealth;
    [SerializeField, ReadOnly]
    private int CurrentHealth;
    [SerializeField]
    private int CurrentScale;

    void Start()
    {
        CurrentHealth = StartingHealth;
        CurrentScale = 1;
    }

    public int GetCurrentScale()
    {
        return CurrentScale;
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

    public void TakeDamage(int damage, int speed, out int newSpeed)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 0)
        {
            newSpeed = speed - Mathf.FloorToInt(-CurrentHealth / speed);
        }
        else
        {
            newSpeed = 0;
        }
    }
}
