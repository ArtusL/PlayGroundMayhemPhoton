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
        // Check if player is grounded by casting an overlap sphere
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





//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerGroundCheck : MonoBehaviour
//{
//	PlayerController playerController;
//	public LayerMask groundLayer;
//	public float groundCheckRadius = 0.3f;
//	void Awake()
//	{
//		playerController = GetComponentInParent<PlayerController>();
//	}
//	void Update()
//	{
//		// Check if player is grounded by casting an overlap sphere
//		if (Physics.OverlapSphere(transform.position, groundCheckRadius, groundLayer).Length > 0)
//		{
//			playerController.SetGroundedState(true);
//		}
//		else
//		{
//			playerController.SetGroundedState(false);
//		}
//	}
//	void OnTriggerEnter(Collider other)
//	{
//		if (other.gameObject == playerController.gameObject)
//			return;

//		playerController.SetGroundedState(true);
//	}

//	void OnTriggerExit(Collider other)
//	{
//		if (other.gameObject == playerController.gameObject)
//			return;

//		playerController.SetGroundedState(false);
//	}

//	void OnTriggerStay(Collider other)
//	{
//		if (other.gameObject == playerController.gameObject)
//			return;

//		playerController.SetGroundedState(true);
//	}
//}