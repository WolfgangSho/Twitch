﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (UnityEngine.AI.NavMeshAgent))]
[RequireComponent (typeof (Animator))]


public class AnimationNavigationMatch : MonoBehaviour
{
    Animator anim;
    UnityEngine.AI.NavMeshAgent agent;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    void Start()
    {
        anim = GetComponent<Animator> ();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
        // Don’t update position automatically
        agent.updatePosition = false;
    }
    
    void Update ()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot (transform.right, worldDeltaPosition);
        float dy = Vector3.Dot (transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2 (dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime/0.15f);
        smoothDeltaPosition = Vector2.Lerp (smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
        {
            velocity = smoothDeltaPosition / Time.deltaTime;

        }

        if(agent.remainingDistance > agent.radius)
        {
            anim.SetFloat("Speed_f", velocity.magnitude);
            anim.SetBool("Static_b",false);

           // Debug.Log(velocity.magnitude);
        }
        else
        {
            anim.SetBool("Static_b",true);
        }

    //    GetComponent<LookAt>().lookAtTargetPosition = agent.steeringTarget + transform.forward;
    }

    void OnAnimatorMove ()
    {
        // Update position to agent position
        transform.position = agent.nextPosition;
    }
}
