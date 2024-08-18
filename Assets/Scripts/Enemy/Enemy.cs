using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using UnityEngine.InputSystem.LowLevel;

public class Enemy : MonoBehaviour, DamageInterface
{
	[Header("Core Variables")]
	[SerializeField]
	private Transform playerPositon;
	[SerializeField]
	private Rigidbody2D rb;

	[Header("Movement")]
	[SerializeField]
	private float moveSpeed;
	[SerializeField]
	private float checkRadius;
	[SerializeField]
	private LayerMask playerLayerMask;

	[Header("Prefabs")]
	[SerializeField]
	private Projectile _enemyAttackPrefab;

	[Header("Attack")]
	[SerializeField]
	private float waitForNextAttack;

	[Header("Stats")]
	[SerializeField]
	private int Scale = 1;
	[SerializeField]
	private int ProjectileSize = 1;
	[SerializeField, ReadOnly]
	private int currentHealth = 5;


	[ReadOnly]
	public Vector3 towardsPlayer;

	private bool inCameraForAttack = false;

	private enum enemyState
	{
		idle,
		moving,
	}
	private enemyState currentState;

	private void Start()
	{
	  StartCoroutine(ShootProjectiles());
		currentState = enemyState.idle;

      playerPositon = GameObject.FindWithTag(Tags.T_Player).transform;
    }

	public void Initialise(int scale,int projSize)
	{
		Scale = scale;
		ProjectileSize = projSize;
		currentHealth = scale * 5;
	}

	private void Update()
	{
		checkIfInCamera();
		checkCamera();
		movement();
	}

	private void movement()
	{
		towardsPlayer = (playerPositon.position - transform.position).normalized;

		if (currentState == enemyState.moving)
		{
			rb.velocity = towardsPlayer * moveSpeed;
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
	}
	private void checkCamera()
	{
		Collider2D player = Physics2D.OverlapCircle(transform.position, checkRadius, playerLayerMask);
		if(player != null)
		{
			currentState = enemyState.idle;
		}
		else
		{
			currentState = enemyState.moving;
		}
	}
	private void checkIfInCamera()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		Bounds bound = new Bounds(transform.position, Vector3.zero);
		if(GeometryUtility.TestPlanesAABB(planes, bound))
		{
			inCameraForAttack = true;
		}
		else
		{
			inCameraForAttack = false;
		}
	}

	private IEnumerator ShootProjectiles() { 
		while(true) {
			ShootProjectile();
			yield return new WaitForSeconds(waitForNextAttack);
		}
	}

	private void ShootProjectile()
	{
		if (inCameraForAttack)
		{
			Projectile spawned = Instantiate(_enemyAttackPrefab, transform.position + towardsPlayer, Quaternion.identity);
			spawned.Initialise(towardsPlayer, towardsPlayer, ((Scale-1)*5) + ProjectileSize);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, checkRadius);
	}

    public void TakeDamage(int damage, int speed, out int newSpeed)
    {
		currentHealth -= damage;
		if (currentHealth < 0)
		{
            newSpeed = speed - Mathf.FloorToInt(-currentHealth / speed);
			GameManager.Instance.EnemyManager.RemoveEnemy();
			Destroy(gameObject);
        }
		else
		{
			newSpeed = 0;
		}
    }
}
