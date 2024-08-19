using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;

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
	[SerializeField,ReadOnly]
	private int currentProjSpeed = 1;

	[SerializeField]
	private float projectileLifetime;

	[SerializeField]
	private LayerMask IgnoreLayer;

	private Vector2 lastDir;

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
		lastDir = dir;
		transform.right = Rtransform;
		projectileSize = Scale;
		projectileDamage = Scale;
        transform.localScale *= projectileSize;
		transform.localEulerAngles = GetEularAngleToDir(Vector2.down, dir);
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
		
		int newSpeed;
		if (collision.gameObject.TryGetComponent(out DamageInterface hitTarget))
		{
            lifeTime.SetTime(projectileLifetime);
            hitTarget.TakeDamage(projectileDamage, currentProjSpeed, out newSpeed);
			Debug.Log(newSpeed);
            if (newSpeed > 1)
            {
                projectileDamage = newSpeed * projectileSize;
                currentProjSpeed = newSpeed;
                rb.velocity = lastDir.normalized * newSpeed * projectileSpeed;
				return;
            }
			else
			{
                startReducingSpeed = true;
				currentProjSpeed = 1;
            }
            Debug.Log(rb.velocity.magnitude + " " + newSpeed);
        }
        rb.velocity = Vector2.Reflect(rb.velocity, collision.contacts[0].normal).normalized * currentProjSpeed * projectileSpeed;
		lastDir = rb.velocity;
        transform.localEulerAngles = GetEularAngleToDir(Vector2.down, lastDir);
		
	}

    public void Redirect(Vector2 newDir, int Strength)
	{
		currentProjSpeed = Strength - projectileSize + 1;
		if (currentProjSpeed < 1)
			return;
		projectileDamage = currentProjSpeed * projectileSize;
		rb.velocity = newDir.normalized * projectileSpeed * currentProjSpeed;
		lifeTime.SetTime(projectileLifetime);
		rb.excludeLayers -= IgnoreLayer;
		lastDir = rb.velocity;
        transform.localEulerAngles = GetEularAngleToDir(Vector2.down, newDir);
    }

    public static Vector3 GetEularAngleToDir(Vector2 originVector, Vector2 dirVector)
    {
        return new Vector3(0, 0, Vector2.SignedAngle(originVector, dirVector));
    }
}
