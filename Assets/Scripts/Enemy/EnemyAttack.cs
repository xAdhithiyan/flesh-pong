using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	[Header("Core Variables")]
	[SerializeField]
	private Rigidbody2D rb;

	[Header("Projectile")]
	[SerializeField]
	private float projectileSpeed;
	[SerializeField]
	private float reductionSpeed;
	[SerializeField]
	private float ReflectValue;

	private GameObject player;
	private GameObject enemy;
	private bool startReducingSpeed = false;

	private void Start()
	{
		player = GameObject.FindWithTag(Tags.T_Player);
		enemy = GameObject.FindWithTag(Tags.T_Enemy);

		rb.velocity = (player.GetComponent<Transform>().position - transform.position).normalized * projectileSpeed;
		transform.up = enemy.GetComponent<Enemy>().towardsPlayer;
	}
	private void Update()
	{
		if (startReducingSpeed)
		{
			rb.velocity *= reductionSpeed;
		}

		if(rb.velocity.magnitude < 0.1f)
		{
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		rb.velocity = Vector2.Reflect(rb.velocity, collision.contacts[0].normal) * ReflectValue;
		startReducingSpeed = true;
	}
}
