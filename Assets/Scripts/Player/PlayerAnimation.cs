using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour, PCMInterface
{
    [field: SerializeField]
    public PlayerComponentManager PCM { get; set; }

    [SerializeField]
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PCM.controller.CurrentState == playerState.moving)
        {
            anim.Play("Moving");
            anim.SetFloat("moveX", PCM.controller.lastDirection.x);
            anim.SetFloat("moveY", PCM.controller.lastDirection.y);
        }
        else if (PCM.controller.CurrentState == playerState.idle)
        {
            anim.Play("Idle");
            anim.SetFloat("moveX", PCM.controller.lastDirection.x);
            anim.SetFloat("moveY", PCM.controller.lastDirection.y);
        }
    }
}
