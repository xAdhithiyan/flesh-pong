using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour, PCMInterface
{
    // Start is called before the first frame update

    private PlayerInputMap.PlayerActions playerActions;
    [field: SerializeField]
    public PlayerComponentManager PCM { get; set; }
    void Awake()
    {
        playerActions = GameManager.Instance.playerInputMap.Player;
    }
    private void OnEnable()
    {
        GameManager.Instance.EnablePlayer();
        playerActions.Move.performed += PCM.controller.SetDirection;
        playerActions.Move.canceled += PCM.controller.SetDirection;
        playerActions.LookMouse.performed += PCM.controller.MousePosition;
        playerActions.Dash.performed += PCM.controller.BufferDash;
    }

    private void OnDisable()
    {
        playerActions.Move.performed -= PCM.controller.SetDirection;
        playerActions.Move.canceled -= PCM.controller.SetDirection;
        playerActions.LookMouse.performed -= PCM.controller.MousePosition;
        playerActions.Dash.performed -= PCM.controller.BufferDash;
        GameManager.Instance.DisablePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
