using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using UnityEngine.UIElements;
using DG.Tweening;

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
    private List<int> MeatPerSize = new List<int>();
    [Header("Shaders")]
    [SerializeField]
    private float flashDuration;
    [SerializeField]
    private SpriteRenderer rend;
    private MaterialPropertyBlock block;
    [SerializeField]
    private int healthToAdd;

    public GameObject GameOverUI;

    public TMP_Text text;

    void Start()
    {
        block = new MaterialPropertyBlock();
        CurrentHealth = StartingMeat;
        CurrentScale = 1;

        text.text = CurrentHealth.ToString();
    }

    public int GetCurrentScale()
    {
        return CurrentScale;
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateCamForward();
        if(healthToAdd != 0)
        {
            TakeDamage(healthToAdd, 0, out int b);
            healthToAdd = 0;
        }
    }

    
    public bool isDead = false;
    public void TakeDamage(int damage, int speed, out int newSpeed)
    {
        CurrentHealth -= damage;
        text.text = CurrentHealth.ToString();
        IncreaseScale();
        StartCoroutine(DamageFlash());
        if (CurrentHealth < 0)
        {
            GameManager.Instance.DisablePlayer();
            GameOverUI.SetActive(true);
            isDead = true;
            GameManager.Instance.AudioManager.PlaySound(AudioRef.PDeath);
        }
        newSpeed = 0;
    }

    private void IncreaseScale()
    {
        Debug.Log("running");
        for(int i = MeatPerSize.Count; i > 0; i--)
        {
            Debug.Log(i);
            if (CurrentHealth >= MeatPerSize[i-1])
            {
                if (CurrentScale == i)
                    return;
                TweenSize(i);
                CurrentScale = i;
                PCM.controller.SetMaxSpeed(CurrentScale);
                return; 
            }
        }
    }
    private void TweenSize(int scale)
    {
        Debug.Log("tweening " + scale);
        GameManager.Instance.cameraManager.ScaleCamera(scale);
        DOVirtual.Float(transform.localScale.x, ((scale - 1) * 0.5f + 1), 0.3f, (x) =>
        {
            transform.localScale = Vector3.one * x;
        });
        //transform.DOScale(Vector3.one * ((scale - 1) * 0.5f + 1), 0.3f);
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

    public void IncreaseMeat()
    {
        CurrentHealth++;
        text.text = CurrentHealth.ToString();
        IncreaseScale();
    }
}
