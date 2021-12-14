using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isRunningHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        Debug.Log(animator);
    }

    // Update is called once per frame
    void Update()
    {
        bool isrunning = animator.GetBool(isRunningHash);
        bool isWalking = animator.GetBool(isWalkingHash);
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");

        // if player presses w key
        if (!isWalking && forwardPressed)
        {
            // then set the isWalking boolean to be true
            animator.SetBool(isWalkingHash, true);
        }

        // is player is not pressing w key
        if (isWalking && !forwardPressed)
        {
            // then set the isWalking boolean to be false
            animator.SetBool(isWalkingHash, false);
        }

        // if player is walking and not running and presses left shift
        if(!isrunning && (forwardPressed && runPressed))
        {
            // then set the isRunning boolean to be true
            animator.SetBool(isRunningHash, true);
        }

        // if player is running stops running or stops walking
        if(isrunning && (!forwardPressed || !runPressed))
        {
            // then set the isRunning boolean to be false
            animator.SetBool(isRunningHash, false);
        }
    }

}
