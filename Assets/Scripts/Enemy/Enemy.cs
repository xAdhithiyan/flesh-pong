using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using UnityEngine.InputSystem.LowLevel;

public class Enemy : MonoBehaviour
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
	private EnemyAttack _enemyAttackPrefab;
	private EnemyAttack _singleEnemyAttack;

	[Header("Attack")]
	[SerializeField]
	private float waitForNextAttack;

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
	}

	private void Update()
	{
		checkIfInCamera();
		checkCamera();
		movement();
	}

	private void movement()
	{
		playerPositon = GameObject.FindWithTag(Tags.T_Player).transform;
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
			_singleEnemyAttack = Instantiate(_enemyAttackPrefab, transform.position + towardsPlayer, Quaternion.identity);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, checkRadius);
	}
}
