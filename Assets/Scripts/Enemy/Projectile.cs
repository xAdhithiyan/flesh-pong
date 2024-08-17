using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
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
	[SerializeField]
	private int projectileSize;
	private bool startReducingSpeed = false;


	private void Start()
	{
	}
	public void Initialise(Vector2 dir, Vector3 Rtransform, int Scale)
	{
		rb.velocity = dir.normalized * projectileSpeed;
		transform.right = Rtransform;
		projectileSize = Scale;
        transform.localScale = Vector3.one * projectileSize;
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

	public void Redirect(Vector2 newDir, int Strength)
	{
		rb.velocity = newDir.normalized * projectileSpeed * (Strength - projectileSize);
	}
}
