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
    private bool startDespawn = false;

	private int projectileDamage;
	private int currentProjSpeed = 1;

	[SerializeField]
	private float projectileLifetime;

	[SerializeField]
	private LayerMask IgnoreLayer;

	private Timer lifeTime;
    private void Start()
	{
		lifeTime = GameManager.Instance.TimerManager.GenerateTimers(gameObject);
		lifeTime.SetTime(projectileLifetime);
	}
	public void Initialise(Vector2 dir, Vector3 Rtransform, int Scale)
    {
        rb.excludeLayers += IgnoreLayer;
        rb.velocity = dir.normalized * projectileSpeed;
		transform.right = Rtransform;
		projectileSize = Scale;
		projectileDamage = Scale;
        transform.localScale *= projectileSize;
    }
	private void Update()
	{
		

		if((rb.velocity.magnitude < 0.1f || lifeTime.IsTimeZero()) && !startDespawn)
		{
			startDespawn = true;
            lifeTime.DeleteTimer();
            Invoke("DespawnProcess", 1.0f);
		}
	}

    private void FixedUpdate()
    {
        if (startReducingSpeed)
        {
            rb.velocity *= reductionSpeed;
        }
    }

    private void DespawnProcess()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log(rb.velocity.magnitude  + " " + rb.velocity);
		int newSpeed;
		if (collision.gameObject.TryGetComponent(out DamageInterface hitTarget))
		{
            lifeTime.SetTime(projectileLifetime);
            startReducingSpeed = true;
            hitTarget.TakeDamage(projectileDamage, projectileSize, out newSpeed);


            if (newSpeed > 0)
            {
                projectileDamage = newSpeed * projectileSize;
                currentProjSpeed = newSpeed;
                rb.velocity = rb.velocity.normalized * newSpeed;
                return;
            }
        }
        rb.velocity = Vector2.Reflect(rb.velocity, collision.contacts[0].normal) * ReflectValue;

    }

    public void Redirect(Vector2 newDir, int Strength)
	{
		currentProjSpeed = Strength - projectileSize;
		if (currentProjSpeed < 0)
			return;
		projectileDamage = currentProjSpeed * projectileSize;
		rb.velocity = newDir.normalized * projectileSpeed * currentProjSpeed;
		lifeTime.SetTime(projectileLifetime);
		rb.excludeLayers -= IgnoreLayer;
	}
}
