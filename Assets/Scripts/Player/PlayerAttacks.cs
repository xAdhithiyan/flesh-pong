using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KevinCastejon.MoreAttributes;
using DG.Tweening;
using static Unity.Collections.AllocatorManager;

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
    [SerializeField]
    private float attackLag;
    [SerializeField]
    private Animator anim;

    private bool isCharging;

    [Header("Shaders")]
    [SerializeField]
    private float flashDuration;
    [SerializeField]
    private SpriteRenderer rend;
    private MaterialPropertyBlock block;

    #region input

    public void AttackHeld(InputAction.CallbackContext context)
    {
        if (!chargeTime.IsTimeZero(1))
            return;
        chargeTime.SetTime(chargeRate[0]);
        TweenHammerSize();
        isCharging = true;
    }

    private Sequence mySequence;
    public void AttackReleased(InputAction.CallbackContext context)
    {
        if(!isCharging) return;
        isCharging = false;
        ReleaseAttack();
        PCM.controller.SetCurrentSpeed(1);
        if(hammerTween != null)
        {
            hammerTween.Kill();
        }
    }

    #endregion
    // Start is called before the first frame update
    void Start()
    {

        block = new MaterialPropertyBlock();
        chargeTime = GameManager.Instance.TimerManager.GenerateTimers(gameObject,2);
        chargeTime.times[0].OnTimeIsZero += currentChargeStage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ReleaseAttack()
    {
        anim.Play("hammer");
        if(Attacks.Count > 0)
        {
            GameManager.Instance.AudioManager.PlaySound(AudioRef.Deflect);
            for(int i = 0; i < Attacks.Count; i++)
            {
                if (i == 0)
                {
                    Attacks[i].Redirect(PCM.controller.mousePos - (Vector2)transform.position, ((int)chargeState + 1) * PCM.values.GetCurrentScale());
                }
                else
                {
                    Attacks[i].Redirect(Attacks[i].transform.position - transform.position, ((int)chargeState + 1) * PCM.values.GetCurrentScale());
                }
            }
        }

        GameManager.Instance.AudioManager.PlaySound(AudioRef.Hit);
        chargeTime.ResetSpecificToZero();
        chargeTime.SetTime(attackLag, 1);
        chargeState = ChargeState.fast;
    }

    private void ResetScale()
    {
        hammerScale.localScale = Vector3.one;
    }
    private void currentChargeStage(object sender, EventArgs e)
    {
        if (chargeState != ChargeState.tooHeavy)
        {
            chargeTime.SetTime(chargeRate[(int)chargeState]);
            chargeState++;
            PCM.controller.SetCurrentSpeed((int)chargeState);
            TweenHammerSize();
            StartCoroutine(DamageFlash());
            //hammerScale.localScale = Vector3.one * (((int)chargeState * 0.4f) + 1);
        }
    }
    private Tween hammerTween;
    private void TweenHammerSize()
    {
        if ((int)chargeState >= chargeRate.Count)
            return;
        hammerTween = hammerScale.DOScale(((((int)chargeState + 1) * 0.4f) + 1), chargeRate[(int)chargeState]).SetEase(Ease.InCubic, 3f);
    }
    private IEnumerator DamageFlash()
    {
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / flashDuration));
            block.SetFloat("_FlashAmount", currentFlashAmount);
            block.SetTexture("_MainTex", rend.sprite.texture);
            rend.SetPropertyBlock(block);
            yield return null;
        }
        block.SetTexture("_MainTex", rend.sprite.texture);
        block.SetFloat("_FlashAmount", 0);
        rend.SetPropertyBlock(block);
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
