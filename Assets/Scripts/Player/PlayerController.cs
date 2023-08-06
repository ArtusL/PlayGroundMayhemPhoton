using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public PlayerRoleManager roleManager;

	[SerializeField] GameObject cameraHolder;

	[SerializeField] float mouseSensitivity, sprintSpeed, jumpForce, smoothTime;

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

	public float baseSpeed = 5f;
	public float baseJumpPower = 10f;

	private float speed;
	private float jumpPower;

	public PowerUp? storedPowerUp = null;
	private float storedMultiplier = 0f;
	private float storedDuration = 0f;
	public bool IsPowerUpActive { get; private set; } = false;

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
	}

	void Start()
	{
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
		Debug.Log($"UpdateColor RPC called with r={r}, g={g}, b={b}");
		Renderer modelRenderer = transform.Find("Capsule/Model/default").GetComponent<Renderer>();
		modelRenderer.material.color = new Color(r, g, b);
	}
	void Update()
	{
		RaycastHit hit;
		float distance = 1f;
		Vector3 dir = new Vector3(0, -1);

		if (!PV.IsMine)
			return;

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
			if (roleManager.isSeeker && !wasSeeker)
			{
				ChangeColor(Color.red);
				StartCoroutine(Stun(3.0f));
				isStunned = true;
			}
			else if (!roleManager.isSeeker)
			{
				ChangeColor(Color.blue);
			}
			wasSeeker = roleManager.isSeeker;
		}
		wasSeeker = roleManager.isSeeker;

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
	void Move()
	{
		if (!canMove || !grounded)
			return;

		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

		if (Input.GetKey(KeyCode.LeftShift) && stamina > 0 && moveDir.magnitude > 0)
		{
			moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * sprintSpeed, ref smoothMoveVelocity, smoothTime);
			stamina -= staminaDepletionRate * Time.deltaTime;
			stamina = Mathf.Clamp(stamina, 0, maxStamina);
			regenTimer = 0;
		}
		else
		{
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
		Debug.Log("Grounded state set: " + grounded);
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

		speed *= multiplier;
		sprintSpeed *= multiplier;

		Debug.Log("Player speed after boost: " + speed + ", Sprint speed after boost: " + sprintSpeed);

		yield return new WaitForSeconds(duration);

		speed = originalSpeed;
		sprintSpeed = originalSprintSpeed;

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

		storedVelocity = rb.velocity;

		storedVelocity.x = 0;
		storedVelocity.z = 0;

		bool wasMovingUpwards = storedVelocity.y > 0;

		rb.useGravity = false;

		rb.velocity = storedVelocity;

		float stunTimer = 0;
		while (stunTimer < duration)
		{
			if (wasMovingUpwards)
			{
				rb.velocity += Physics.gravity * Time.deltaTime * 2;
			}

			stunTimer += Time.deltaTime;
			yield return null;
		}

		canMove = true;
		canLook = true;
		isStunned = false;
		rb.useGravity = true;
	}
	public void ApplyStunToOthers(float duration)
	{
		Debug.Log($"{gameObject.name} is applying stun to others.");

		foreach (var player in GameObject.FindObjectsOfType<PlayerController>())
		{
			if (player != this)
			{
				Debug.Log($"Stunning player {player.gameObject.name}.");
				player.Stun(duration);
			}
		}
	}


	[PunRPC]
	public void StunAllExceptUser(int userViewID, float duration)
	{
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
		if (Physics.Raycast(transform.position, transform.forward, out hit, 2f)) 
		{
			PlayerController otherPlayer = hit.transform.GetComponent<PlayerController>();
			if (otherPlayer != null && otherPlayer != this)
			{
				Rigidbody otherPlayerRb = otherPlayer.GetComponent<Rigidbody>();
				if (otherPlayerRb != null)
				{
					otherPlayerRb.AddForce(transform.forward * pushForce, ForceMode.Impulse);
				}
			}
		}
	}
}



