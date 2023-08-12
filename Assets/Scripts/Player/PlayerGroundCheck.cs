using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.3f;
    Collider playerCollider;

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        playerCollider = GetComponentInParent<Collider>();
    }

    void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, groundCheckRadius, groundLayer);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != playerCollider)
            {
                playerController.SetGroundedState(true);
                return;
            }
        }
        playerController.SetGroundedState(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
}

