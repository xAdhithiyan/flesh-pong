using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using UnityEngine.InputSystem.LowLevel;
using System.Xml.Schema;
using System.Runtime.CompilerServices;
using System.IO.IsolatedStorage;
using System;

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
	[SerializeField]
	private Meat meatPrefab;

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
	[SerializeField,ReadOnly]
	private int meatToDrop;

	[Header("Health")]
	[SerializeField]

	[ReadOnly]
	public Vector3 towardsPlayer;

	private bool inCameraForAttack = false;

	private Timer attackTimer;

	private enum enemyState
	{
		idle,
		moving,
		attacked,
		dying
	}
	private enemyState currentState;

	private void Start()
	{
		attackTimer = GameManager.Instance.TimerManager.GenerateTimers(gameObject);
		attackTimer.SetTime(waitForNextAttack);
		currentState = enemyState.idle;

		playerPositon = GameObject.FindWithTag(Tags.T_Player).transform;

		if (currentHealth == 0)
			currentHealth = Scale * 5;

        if (meatToDrop == 0)
			meatToDrop = currentHealth;
	}

	public void Initialise(int scale, int projSize)
	{
		Scale = scale;
		ProjectileSize = projSize;
		currentHealth = scale * 5;
		meatToDrop = currentHealth;
	}

	private void Update()
	{
		checkIfInCamera();
		checkCamera();
		movement();
		ShootProjectile();
        Animate();
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
		if (currentState == enemyState.dying)
			return;
		Collider2D player = Physics2D.OverlapCircle(transform.position, checkRadius, playerLayerMask);
		if (player != null)
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
		if (GeometryUtility.TestPlanesAABB(planes, bound))
		{
			inCameraForAttack = true;
		}
		else
		{
			inCameraForAttack = false;
		}
	}
	private bool attacked = false;
	private void ShootProjectile()
	{
		if (!attackTimer.IsTimeZero())
			return;
		if (inCameraForAttack && currentState == enemyState.idle)
		{
			Projectile spawned = Instantiate(_enemyAttackPrefab, transform.position + towardsPlayer, Quaternion.identity);
			spawned.Initialise(towardsPlayer, towardsPlayer, ((Scale - 1) * 5) + ProjectileSize);
			attacked = true;
            attackTimer.SetTime(waitForNextAttack);
        }
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, checkRadius);
	}

	public void TakeDamage(int damage, int speed, out int newSpeed)
	{
		currentHealth -= damage;
		if (currentHealth <= 0)
		{
			if (currentHealth < 0)
				newSpeed = speed - Mathf.CeilToInt(-currentHealth / speed);
			else
				newSpeed = 0;
			Death();
			return;
		}
		else
		{
			newSpeed = 0;
		}
	}

	public void Death()
	{
		GameManager.Instance.EnemyManager.RemoveEnemy();
		MeatSpawn();
		currentState = enemyState.dying;
		Invoke("DestroySelf", 0.1f);
	}

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void MeatSpawn()
	{
		Vector3 offset = Vector3.zero;

		// currentHealth = meat dropped
		for (int i = 0; i < meatToDrop; i++)
		{
			Meat spawned = Instantiate(meatPrefab, transform.position + offset, Quaternion.identity);
			spawned.Initialize();
			offset += Vector3.right;
		}
		Debug.Log("ran");
	}

	#region Anim
	[SerializeField]
	private Animator anim;

    public static int GetDirection(Vector2 originVector, Vector2 dirVector, bool fourMode = true)
    {
        float signedAngle = Vector2.SignedAngle(originVector, dirVector);
        float angle = signedAngle > 0 ? signedAngle : 360f + signedAngle;
        return Mathf.RoundToInt((Mathf.Floor(angle / (fourMode ? 90f : 45f)) + (angle / (fourMode ? 90f : 45f) % 1 > 0.5 ? 1 : 0)) % (fourMode ? 4 : 8));
    }

	private void Animate()
	{
		anim.SetFloat("Dir", GetDirection(Vector2.right, (Vector2)towardsPlayer));


        if (currentState == enemyState.dying)
        {
            anim.Play("Dying");
        }
        else if (attacked)
		{
			anim.Play("Attacked");
			if (attackTimer.GetTime() < waitForNextAttack * 0.4f)
				attacked = false;
		}
		else
		{
			anim.Play("Moving");
		}
	}
    #endregion
}
