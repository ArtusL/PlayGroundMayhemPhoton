using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public AudioManager audioManager;
	public PlayerRoleManager roleManager;
	private GameManager gameManager;


	[SerializeField] GameObject cameraHolder;

	[SerializeField] float mouseSensitivity, sprintSpeed, jumpForce, smoothTime;

	 private Animator animator;

	float verticalLookRotation;
	bool grounded;
	Vector3 smoothMoveVelocity;
	Vector3 moveAmount;
	private bool isStunned = false;
	public bool canMove = true;
	private bool canLook = true;
	private bool wasSeeker = false;
	private Vector3 storedVelocity;
	private Vector3 jumpVelocity;

	private bool RunningAnim = false;
	private bool SprintingAnim = false;


	public float baseSpeed = 5f;
	public float baseJumpPower = 10f;

	[SerializeField] float seekerSpeed = 6f;
	[SerializeField] float hiderSpeed = 5f;
	[SerializeField] float seekerSprintSpeed = 8f;
	[SerializeField] float hiderSprintSpeed = 7f;
	private float speed;
	private float jumpPower;

	public PowerUp? storedPowerUp = null;
	private float storedMultiplier = 0f;
	private float storedDuration = 0f;
	public bool IsPowerUpActive { get; private set; } = false;
	public bool HasPowerUp { get; private set; } = false;

	public void PickupPowerUp()
	{
		HasPowerUp = true;
	}

	private float stamina = 100f;
	private float maxStamina = 100f;
	public float Stamina
	{
		get { return stamina; }
	}

	public float MaxStamina
	{
		get { return maxStamina; }
	}
	private float staminaDepletionRate = 20f;
	private float staminaRegenRate = 10f;
	private float regenDelay = 3f;
	private float regenTimer = 0f;

	Rigidbody rb;

	PhotonView PV;
	public enum PowerUp
	{
		None,
		SpeedBoost,
		JumpBoost,
		Stun,
		StaminaRefill
	}
	public PowerUp activePowerUp = PowerUp.None;

	public bool IsLocalPlayer()
	{
		return PV.IsMine;
	}

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();
		roleManager = GetComponent<PlayerRoleManager>();
		audioManager = GetComponent<AudioManager>();
	}

	void Start()
	{
		gameManager = GameManager.Instance;
		if (PV.IsMine)
        {
            animator = transform.Find("Capsule/Model").GetComponent<Animator>();
            //Debug.Log("Animator enabled: " + animator.enabled);
            //Debug.Log("Animation Controller: " + animator.runtimeAnimatorController);
        }
		jumpPower = jumpForce;
		speed = baseSpeed;
		jumpPower = baseJumpPower;
		if (!PV.IsMine)
		{
			Destroy(GetComponentInChildren<Camera>().gameObject);
			Destroy(rb);
		}
	}
	private void ChangeColor(Color color)
	{
		PV.RPC("UpdateColor", RpcTarget.AllBuffered, color.r, color.g, color.b);
	}

	[PunRPC]
	private void UpdateColor(float r, float g, float b)
	{
		Renderer modelRenderer = transform.Find("Capsule/Model/default").GetComponent<Renderer>();
		modelRenderer.material.color = new Color(r, g, b);
	}
	void Update()
	{
		RaycastHit hit;
		float distance = 1f;
		Vector3 dir = new Vector3(0, -1);

		//Debug.Log("Move amount: " + moveAmount.magnitude); 
		

		if (!PV.IsMine)
			return;
		//canMove = gameManager.GameStarted;

		UpdateAnimationState();
		UpdateAudio();

		if (canLook)
		{
			Look();
		}

		if (canMove)
		{
			Move();
			Jump();
		}
		else
		{
			rb.velocity = storedVelocity;
		}

		Renderer modelRenderer = transform.Find("Capsule/Model/default").GetComponent<Renderer>();
		if (PV.IsMine)
		{
			if (roleManager.isSeeker)
			{
				speed = seekerSpeed;
				sprintSpeed = seekerSprintSpeed;
				if (!wasSeeker)
				{
					ChangeColor(Color.red);
					StartCoroutine(Stun(3.0f));
					isStunned = true;
					//Debug.Log($"Player is Seeker. Speed: {speed}, Sprint Speed: {sprintSpeed}");
				}
			}
			else
			{
				speed = hiderSpeed;
				sprintSpeed = hiderSprintSpeed;
				ChangeColor(Color.blue);
				//Debug.Log($"Player is Hider. Speed: {speed}, Sprint Speed: {sprintSpeed}");
			}
			wasSeeker = roleManager.isSeeker;
		}

		if (Input.GetKeyDown(KeyCode.E) && storedPowerUp.HasValue)
		{
			switch (storedPowerUp.Value)
			{
				case PowerUp.SpeedBoost:
					ApplySpeedBoost(storedMultiplier, storedDuration);
					break;
				case PowerUp.JumpBoost:
					ApplyJumpBoost(storedMultiplier, storedDuration);
					break;
				case PowerUp.Stun:
					ApplyStunToOthers(storedDuration);
					break;
				case PowerUp.StaminaRefill:
					RefillStamina();
					break;
				default:
					break;
			}
			storedPowerUp = null;
			HasPowerUp = false;
		}

		if (!Input.GetKey(KeyCode.LeftShift))
		{
			regenTimer += Time.deltaTime;

			if (regenTimer >= regenDelay)
			{
				stamina += staminaRegenRate * Time.deltaTime;
				stamina = Mathf.Clamp(stamina, 0, maxStamina);
			}
		}

		if (Input.GetKeyDown(KeyCode.Q))
		{
			PushOtherPlayer(1000);
		}
		//if (Input.GetKeyDown(KeyCode.Q))
		//{
		//	StartCoroutine(Stun(3.0f));
		//}
	}
	void Look()
	{
		if (canLook)  
		{
			transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

			verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
			verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

			cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
		}
	}
	void UpdateAudio()
	{
		bool isRunning = grounded && Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.LeftShift);
		bool isSprinting = grounded && Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift);

		if (isRunning)
		{
			if (!audioManager.IsPlayingLocalRunSound())
			{
				audioManager.StopLocalSprintSound();
				audioManager.PlayLocalRunSound();
				audioManager.photonView.RPC("PlayRunSound", RpcTarget.Others);

				audioManager.photonView.RPC("StopSprintSound", RpcTarget.Others);
			}
		}
		if (isSprinting)
		{
			if (!audioManager.IsPlayingLocalSprintSound())
			{
				audioManager.PlayLocalSprintSound();
				audioManager.photonView.RPC("PlaySprintSound", RpcTarget.Others);
				audioManager.StopLocalRunningSound();
				audioManager.photonView.RPC("StopRunningSound", RpcTarget.Others);
			}
		}
		
        else if (!isRunning&&!isSprinting)
        {
            if (audioManager.IsPlayingLocalRunSound() || audioManager.IsPlayingLocalSprintSound())
            {
                audioManager.StopLocalRunningSound();
                audioManager.StopLocalSprintSound();
                audioManager.photonView.RPC("StopSprintSound", RpcTarget.Others);
                audioManager.photonView.RPC("StopRunningSound", RpcTarget.Others);
            }
        }
    }


	void UpdateAnimationState()
	{
		
		if (moveAmount.magnitude > 6.5f)
		{
			animator.SetBool("isSprinting", true);
			animator.SetBool("isRunning", false);
		}
		else if (moveAmount.magnitude > 2f)
		{
			animator.SetBool("isSprinting", false);
			animator.SetBool("isRunning", true);
		}
		else if (moveAmount.magnitude < 2f)
		{
			animator.SetBool("isSprinting", false);
			animator.SetBool("isRunning", false);
		}
	}

	void Move()
	{
		//UpdateAnimationState();
		if (!canMove || !grounded)
			return;

		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

		if (Input.GetKey(KeyCode.LeftShift) && stamina > 0 && moveDir.magnitude > 0 && Input.GetKey(KeyCode.W))
		{
			SprintingAnim = true;
			RunningAnim = false;
			moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * sprintSpeed, ref smoothMoveVelocity, smoothTime);
			stamina -= staminaDepletionRate * Time.deltaTime;
			stamina = Mathf.Clamp(stamina, 0, maxStamina);
			regenTimer = 0;
		}
		else
		{
			SprintingAnim = false;
			RunningAnim = true;
			moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * speed, ref smoothMoveVelocity, smoothTime);
		}
	}

	void Jump()
	{
		if (Input.GetKeyDown(KeyCode.Space) && grounded)
		{
			jumpVelocity = moveAmount; 
			Debug.Log("Jump triggered");
			rb.AddForce(transform.up * jumpForce);
		}
	}

	public void SetGroundedState(bool _grounded)
	{
		grounded = _grounded;
        //Debug.Log("Grounded state set: " + grounded);
    }

	void FixedUpdate()
	{
		if (!PV.IsMine)
			return;
		if (canMove && grounded)
		{
			rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
		}
		else if (!grounded)
		{
			rb.MovePosition(rb.position + transform.TransformDirection(jumpVelocity) * Time.fixedDeltaTime);
		}
		else
		{
			rb.velocity = storedVelocity;
		}
	}

	public void StorePowerUp(PowerUp powerUp, float multiplier, float duration)
	{
		if (storedPowerUp == null)
		{
			Debug.Log("Storing powerup: " + powerUp);
			storedPowerUp = powerUp;
			storedMultiplier = multiplier;
			storedDuration = duration;
		}
	}

	public void ApplySpeedBoost(float multiplier, float duration)
	{
		Debug.Log("ApplySpeedBoost with multiplier: " + multiplier + ", duration: " + duration);

		if (!IsPowerUpActive)
		{
			activePowerUp = PowerUp.SpeedBoost;
			StartCoroutine(ApplySpeedBoostCoroutine(multiplier, duration));
		}
	}

	private IEnumerator ApplySpeedBoostCoroutine(float multiplier, float duration)
	{
		Debug.Log("ApplySpeedBoostCoroutine with multiplier: " + multiplier + ", duration: " + duration);
		IsPowerUpActive = true;
		float originalSpeed = speed;
		float originalSprintSpeed = sprintSpeed;
		float originalSeekerSpeed = seekerSpeed;
		float originalSeekerSprintSpeed = seekerSprintSpeed;
		float originalHiderSpeed = hiderSpeed;
		float originalHiderSprintSpeed = hiderSprintSpeed;

		speed *= multiplier;
		sprintSpeed *= multiplier;
		hiderSpeed *= multiplier;
		hiderSprintSpeed *= multiplier;
		seekerSpeed *= multiplier;
		seekerSprintSpeed *= multiplier;

		Debug.Log("Player speed after boost: " + speed + ", Sprint speed after boost: " + sprintSpeed);

		yield return new WaitForSeconds(duration);

		speed = originalSpeed;
		sprintSpeed = originalSprintSpeed;
		hiderSpeed= originalHiderSpeed;
		hiderSprintSpeed = originalHiderSprintSpeed;
		seekerSpeed = originalSeekerSpeed;
		seekerSprintSpeed = originalSeekerSprintSpeed;

		IsPowerUpActive = false;
		activePowerUp = PowerUp.None;
		Debug.Log("Speed boost ended. Player speed is now: " + speed + ", Sprint speed is now: " + sprintSpeed);
	}

	public void ApplyJumpBoost(float multiplier, float duration)
	{
		Debug.Log("ApplyJumpBoost called with multiplier: " + multiplier + ", duration: " + duration);

		if (!IsPowerUpActive)
		{
			activePowerUp = PowerUp.JumpBoost;
			StartCoroutine(ApplyJumpBoostCoroutine(multiplier, duration));
		}
	}

	private IEnumerator ApplyJumpBoostCoroutine(float multiplier, float duration)
	{
		Debug.Log("ApplyJumpBoostCoroutine called with multiplier: " + multiplier + ", duration: " + duration);
		IsPowerUpActive = true;
		jumpForce *= multiplier;

		yield return new WaitForSeconds(duration);

		jumpForce /= multiplier;
		IsPowerUpActive = false;
		activePowerUp = PowerUp.None;
		Debug.Log("Jump boost ended");
	}

	public bool IsStunned()
	{
		return isStunned;
	}

	public IEnumerator Stun(float duration)
	{
		canMove = false;
		canLook = false;
		isStunned = true;

		if (rb != null)
		{
			storedVelocity = rb.velocity;

			storedVelocity.x = 0;
			storedVelocity.z = 0;

			bool wasMovingUpwards = storedVelocity.y > 0;

			rb.useGravity = false;

			rb.velocity = storedVelocity;

			float stunTimer = 0;
			while (stunTimer < duration)
			{
				if (rb != null && wasMovingUpwards)
				{
					rb.velocity += Physics.gravity * Time.deltaTime * 2;
				}

				stunTimer += Time.deltaTime;
				yield return null;
			}

			if (rb != null)
			{
				rb.useGravity = true;
			}
		}

		canMove = true;
		canLook = true;
		isStunned = false;
		//rb.useGravity = true;
	}
	public void ApplyStunToOthers(float duration)
	{
		Debug.Log($"{gameObject.name} is applying stun to others.");

		foreach (var player in GameObject.FindObjectsOfType<PlayerController>())
		{
			if (player != this)
			{
				Debug.Log($"Stunning player {player.gameObject.name}.");
				StartCoroutine(player.Stun(duration));
			}
		}
	}

	[PunRPC]
	public void StunAllExceptUser(int userViewID, float duration)
	{
		Debug.Log($"Received RPC to stun all except view ID {userViewID} for {duration} seconds.");
		if (PV.ViewID != userViewID)
		{
			StartCoroutine(Stun(duration));
		}
	}


	public void RefillStamina()
	{
		Debug.Log("RefillStamina called");
		stamina = maxStamina;
	}

	public void PushOtherPlayer(float pushForce)
	{
		RaycastHit hit;
		if (Physics.Raycast(cameraHolder.transform.position, cameraHolder.transform.forward, out hit, 10f))
		{
			if (hit.collider.CompareTag("Player"))
			{
				PlayerController playerController = hit.collider.GetComponent<PlayerController>();
				if (playerController != null)
				{
					Vector3 direction = hit.collider.transform.position - transform.position;
					direction.Normalize();

					playerController.Push(direction, pushForce);
				}
			}
		}

	}

	public void Push(Vector3 direction, float force)
	{
		rb.AddForce(direction * force);
	}
}



