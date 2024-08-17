using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KevinCastejon.MoreAttributes;

public class PlayerAttacks : MonoBehaviour, PCMInterface
{
    private enum ChargeState : int
    {
        fast,
        medium,
        heavy,
        superHeavy,
        tooHeavy
    }
    [field: SerializeField]
    public PlayerComponentManager PCM { get ; set; }
    [SerializeField]
    [Tooltip("Time in seconds before each hammer size stage")]
    private List<float> chargeRate;

    private bool Charging;

    private Timer chargeTime;
    [SerializeField, ReadOnly]
    private ChargeState chargeState = ChargeState.fast;

    #region input

    public void AttackHeld(InputAction.CallbackContext context)
    {
        Charging = true;
        chargeTime.SetTime(chargeRate[0]);
    }

    public void AttackReleased(InputAction.CallbackContext context)
    {
        Charging = false;
        ReleaseAttack();
    }

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        chargeTime = GameManager.Instance.TimerManager.GenerateTimers(gameObject);
        chargeTime.times[0].OnTimeIsZero += currentChargeStage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ReleaseAttack()
    {

        chargeTime.ResetSpecificToZero();
        chargeState = ChargeState.fast;
    }
    private void currentChargeStage(object sender, EventArgs e)
    {
        if (chargeState != ChargeState.tooHeavy)
        {
            chargeTime.SetTime(chargeRate[(int)chargeState]);
            chargeState++;
        }
    }
}
