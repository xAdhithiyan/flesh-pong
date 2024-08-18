using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using KevinCastejon.MoreAttributes;

public enum playerState
{
    idle,
    moving,
    dashing,
    hit
}
public class PlayerController : MonoBehaviour, PCMInterface
{
    private enum coolDownTimers : int
    {
        dashCastCD,
        dashCD
    }

    [field: Header("Core variables")]
    [field: SerializeField]
    public Rigidbody2D rb { get; private set; }
    [SerializeField]
    private CapsuleCollider2D col2D;
    [SerializeField]
    private CircleCollider2D circCol2D;
    [field: SerializeField]
    public PlayerComponentManager PCM { get; set; }

    [Header("Speed Stats")]

    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    [ReadOnly]
    private float currentMaxSpeed;

    [Header("Dash Stats")]

    [SerializeField]
    private float dashCDTimer;
    [SerializeField]
    private float dashDistance;
    [SerializeField]
    private float dashDuration;
    [SerializeField]
    private int dashCharges;
    [SerializeField, Tooltip("Time before all charges refresh")]
    private float dashRechargeRate;
    [SerializeField]
    [ReadOnly]
    private int currentDashCharges;

    [field: Header("Transforms")]
    [field: SerializeField]
    public Transform AbilityCentre { get; private set; }

    [Header("Input Buffer")]

    [SerializeField]
    private int bufferDuration;
    private int currentBufferDuration;

    [Header("Others")]

    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private LayerMask terrainLayer;
    [SerializeField, ReadOnly, HideOnPlay(true)]
    private Transform cursorPos;

    [field: Header("Debug Values")]

    [field: SerializeField, ReadOnly]
    public Vector2 direction { get; private set; }
    [field: SerializeField, ReadOnly]
    public Vector2 lastDirection { get; private set; }
    [field: SerializeField, ReadOnly]
    public playerState CurrentState { get; private set; }
    [SerializeField]
    [ReadOnly]
    private playerState bufferedState;
    private Vector2 rawPos;
    public Vector2 mousePos { get; private set; }

    private float drag;


    private Coroutine dashCoroutine;
    private Timer timers;
    [SerializeField, ReadOnly]
    private bool isDashing;
    [SerializeField, ReadOnly]
    private bool isMoving;

    private LayerMask initialLayer;
    #region Unity Function
    void Awake()
    {
        lastDirection = Vector2.up;

    }
    public void Start()
    {
        timers = GameManager.Instance.TimerManager.GenerateTimers(typeof(coolDownTimers), gameObject);
        timers.times[(int)coolDownTimers.dashCD].OnTimeIsZero += DashResetter;
        currentMaxSpeed = maxSpeed;
        currentDashCharges = dashCharges;
        drag = rb.drag;
        initialLayer = circCol2D.excludeLayers;
    }
    #region Updates
    // Update is called once per frame
    void Update()
    {
        StateDecider();
        ExecuteInput();
        UpdateMousePos();
        AimPoint();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #endregion

    #endregion

    #region GetInputs
    public void SetDirection(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>().normalized;
        if (direction != Vector2.zero)
            lastDirection = context.ReadValue<Vector2>();
    }


    public void Interact(InputAction.CallbackContext context)
    {
        //GameManager.Instance.CallInteraction();
    }
    public void MousePosition(InputAction.CallbackContext context)
    {
        rawPos = context.ReadValue<Vector2>();
    }

    private void UpdateMousePos()
    {
        mousePos = Camera.main.ScreenToWorldPoint(rawPos);
    }


    public void BufferDash(InputAction.CallbackContext context)
    {
        BufferInput(playerState.dashing);
    }
    #endregion


    #region Ability 

    private void AimPoint()
    {
        Vector2 vectorToTarget;
        if (mousePos != Vector2.zero)
            vectorToTarget = mousePos - (Vector2)AbilityCentre.position;
        else
            vectorToTarget = lastDirection;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        AbilityCentre.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    #endregion

    #region Input Buffering
    private void ExecuteInput()
    {
        switch ((int)bufferedState)
        {
            case (int)playerState.dashing:               
                Dash();
                break;

        }
        if (currentBufferDuration > 0)
            currentBufferDuration--;
        if (currentBufferDuration == 0)
            RemoveBufferInput();
    }

    public void RemoveBufferInput()
    {
        bufferedState = playerState.idle;
    }

    private void BufferInput(playerState input)
    {
        bufferedState = input;
        currentBufferDuration = bufferDuration;
    }
    #endregion

    #region Movement

    private void Move()
    {
        isMoving = false;
        rb.drag = drag;
        playerState[] allowed = { playerState.idle, playerState.moving };

        if (!CheckStates(allowed))
        {
            //rb.velocity = Vector2.zero;
            return;
        }
        if (rb.velocity.magnitude <= maxSpeed && !direction.Equals(Vector2.zero))
        {
            rb.drag = 0;
            isMoving = true;
            rb.velocity += direction * acceleration * rb.mass;
            if ((rb.velocity + direction * acceleration * rb.mass).magnitude > currentMaxSpeed)
            {
                rb.velocity = rb.velocity.normalized * currentMaxSpeed;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
    #endregion

    #region Dash
    private void Dash()
    {        
        if (!timers.IsTimeZero((int)coolDownTimers.dashCastCD) || currentDashCharges < 1)
            return;
        RemoveBufferInput();
        isDashing = true;
        currentDashCharges--;
        timers.SetTime(dashCDTimer + dashDuration, (int)coolDownTimers.dashCastCD);

        //GameManager.Instance.AudioManager.PlaySound(AudioRef.Dash, false, 0.65f);
        BeginDash(dashDistance, dashDuration, direction);
    }

    public void Dash(float distance, float dur, Vector2 dir, Color color, float blend)
    {
        col2D.excludeLayers += enemyLayer;
        circCol2D.excludeLayers += enemyLayer;
        dashCoroutine = StartCoroutine(StartDashing(distance, dur, dir));
    }

    private void BeginDash(float distance, float dur, Vector2 dir)
    {
        col2D.excludeLayers += enemyLayer;
        circCol2D.excludeLayers += enemyLayer;
        dashCoroutine = StartCoroutine(StartDashing(distance, dur, dir));
    }

    private IEnumerator StartDashing(float distance, float dur, Vector2 dir)
    {
        float startTime = Time.time;
        Vector2 endPos = (Vector2)transform.position + (dir * distance);
        Vector2 startPos = transform.position;
        Vector2 dashDirection = endPos - startPos;
        for (float timer = 0; timer < dur; timer += Time.deltaTime)
        {

            float ratio = (Time.time - startTime) / dur;
            //float cubic = Mathf.Sin((ratio * Mathf.PI) * 0.5f);
            Vector2 nextPosition = Vector2.Lerp(startPos, endPos, ratio);
            if (Physics2D.CircleCast(circCol2D.offset + (Vector2)transform.position, circCol2D.radius, dashDirection, Vector2.Distance(transform.position, nextPosition), terrainLayer))
            {
                break;
            }
            if (Vector2.Distance(transform.position, endPos) > 0.1f)
            {
                transform.position = nextPosition;
            }
            else
            {
                transform.position = endPos;
            }
            yield return null;
        }
        StopDash();
    }

    private void DashResetter(object sender, EventArgs e)
    {
        currentDashCharges = dashCharges;
    }

    private void StopDash()
    {
        timers.SetTime(dashRechargeRate, (int)coolDownTimers.dashCD);
        if (dashCoroutine != null)
        {
            dashCoroutine = null;
        }
        col2D.excludeLayers = 0;
        circCol2D.excludeLayers = initialLayer;
        isDashing = false;
    }

    private void StopDash(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
        StopDash();
    }
    #endregion

    #region Setter
    #endregion
    #region Utility
    private void StateDecider()
    {
        if (isDashing)
        {
            CurrentState = playerState.dashing;
        }
        else if (isMoving)
        {
            CurrentState = playerState.moving;
        }
        else
        {
            CurrentState = playerState.idle;
        }
    }


    /// <summary>
    /// returns true is the current state is any of the allowedstates
    /// </summary>
    /// <param name="allowedStates"></param>
    /// <returns></returns>
    public bool CheckStates(playerState[] allowedStates)
    {
        bool allowed = false;
        foreach (playerState action in allowedStates)
        {
            if (action == CurrentState)
                allowed = true;
        }
        return allowed;
    }
    #endregion
}
