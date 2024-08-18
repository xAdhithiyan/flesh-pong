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

    private Timer chargeTime;
    [SerializeField, ReadOnly]
    private ChargeState chargeState = ChargeState.fast;
    [SerializeField]
    private List<Projectile> Attacks = new List<Projectile>();
    [SerializeField]
    private Transform hammerScale;

    #region input

    public void AttackHeld(InputAction.CallbackContext context)
    {
        chargeTime.SetTime(chargeRate[0]);
    }

    public void AttackReleased(InputAction.CallbackContext context)
    {
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
        if(Attacks.Count > 0)
        {
            foreach (Projectile attack in Attacks)
            {
                attack.Redirect(attack.transform.position - transform.position,((int)chargeState + 1)*PCM.values.GetCurrentScale());
            }
        }
        chargeTime.ResetSpecificToZero();
        chargeState = ChargeState.fast;
    }
    private void currentChargeStage(object sender, EventArgs e)
    {
        if (chargeState != ChargeState.tooHeavy)
        {
            chargeTime.SetTime(chargeRate[(int)chargeState]);
            chargeState++;
            hammerScale.localScale = Vector3.one * (((int)chargeState * 0.25f) + 1);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Projectile projectile))
        {
            Attacks.Add(projectile);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Projectile>(out Projectile projectile))
        {
            Attacks.Remove(projectile);
        }
    }
}
