using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using DG.Tweening;

public class Meat : MonoBehaviour
{
	[Header("Meat Values")]
	private Timer meatDecayTimer;
	[SerializeField]
	private float maxMeatLifeTime;
	[SerializeField]

	[Header("Components")]
	private Animator animator;
	[SerializeField]
	private LayerMask playerLayerMask;

	private bool hasCollided = false;

	private Tween myTween;
	public void Awake()
	{
		meatDecayTimer = GameManager.Instance.TimerManager.GenerateTimers(gameObject);
	}
	public void Initialize(Vector2 endpos)
	{
		myTween = transform.DOMove(endpos + (Vector2)transform.position, 0.5f);
		//meatDecayTimer.SetTime(UnityEngine.Random.Range(1, maxMeatLifeTime));
		animator.SetTrigger("fall");
	}
	private void Update()
	{
		/*if(meatDecayTimer.IsTimeZero())
		{
			Destroy(gameObject);
		}*/
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasCollided) return;

        if ((1 << collision.gameObject.layer) == playerLayerMask)
        {
            hasCollided = true;

            // increase meat value
            GameManager.Instance.PCM.values.IncreaseMeat();
			myTween.Kill();
            Destroy(gameObject);
        }
    }
}
