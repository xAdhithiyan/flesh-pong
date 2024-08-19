using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;

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

	public void Awake()
	{
		meatDecayTimer = GameManager.Instance.TimerManager.GenerateTimers(gameObject);
	}
	public void Initialize()
	{
		meatDecayTimer.SetTime(UnityEngine.Random.Range(1, maxMeatLifeTime));
		animator.SetTrigger("fall");
	}
	private void Update()
	{
		if(meatDecayTimer.IsTimeZero())
		{
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (hasCollided) return;

		if ((1 << collision.gameObject.layer) == playerLayerMask)
		{
			hasCollided = true;

			// increase meat value
			GameObject.FindWithTag(Tags.T_Player).GetComponent<PlayerValues>().IncreaseMeat();
			Destroy(gameObject);
		}
	}
}
