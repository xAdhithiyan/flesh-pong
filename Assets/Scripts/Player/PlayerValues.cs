using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using UnityEngine.UIElements;

public class PlayerValues : MonoBehaviour, PCMInterface, DamageInterface
{
    // Start is called before the first frame update
    [field: SerializeField]
    public PlayerComponentManager PCM {  get; set; }
    [field: SerializeField]
    public int StartingMeat;
    [SerializeField, ReadOnly]
    private int CurrentHealth;
    [SerializeField]
    private int CurrentScale;
    [SerializeField]
    private int TotalMeat;
    [SerializeField]
    private List<int> MeatPerSize = new List<int>();

    void Start()
    {
        CurrentHealth = StartingMeat;
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
        IncreaseScale();
        if (CurrentHealth < 0)
        {
        }
        newSpeed = 0;
    }

    private void IncreaseScale()
    {
        for(int i = 0; i < MeatPerSize.Count; i++)
        {
            if (CurrentHealth <= MeatPerSize[i])
            {
                transform.localScale = Vector3.one * (i + 1);
                CurrentScale = i + 1;
                return;
            }
        }
    }

    public void IncreaseMeat()
    {
        CurrentHealth++;
        IncreaseScale();
    }
}
