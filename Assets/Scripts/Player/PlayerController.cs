using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	public PlayerRoleManager roleManager;

	[SerializeField] GameObject cameraHolder;

	[SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

	float verticalLookRotation;
	bool grounded;
	Vector3 smoothMoveVelocity;
	Vector3 moveAmount;

	private float baseSpeed = 10f;
	private float baseJumpPower = 10f;

	private float speed;
	private float jumpPower;

	public PowerUp? storedPowerUp = null;
	private float storedMultiplier = 0f;
	private float storedDuration = 0f;
	public bool IsPowerUpActive { get; private set; } = false;

	Rigidbody rb;

	PhotonView PV;
	public enum PowerUp
	{
		None,
		SpeedBoost,
		JumpBoost
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

	void Update()
	{
		RaycastHit hit;
		float distance = 1f;
		Vector3 dir = new Vector3(0, -1);

		grounded = Physics.Raycast(transform.position, dir, out hit, distance);

		if (!PV.IsMine)
			return;

		Look();
		Move();
		Jump();

		if (roleManager.isSeeker)
		{
			GetComponentInChildren<Renderer>().material.color = Color.red;
		}
		else
		{
			GetComponentInChildren<Renderer>().material.color = Color.blue;
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
				default:
					break;
			}
			storedPowerUp = null;
		}
	}
	void Look()
	{
		transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

		verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

		cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
	}

	void Move()
	{
		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed), ref smoothMoveVelocity, smoothTime);
	}

	void Jump()
	{
		if (Input.GetKeyDown(KeyCode.Space) && grounded)
		{
			rb.AddForce(transform.up * jumpForce);
		}
	}

	public void SetGroundedState(bool _grounded)
	{
		grounded = _grounded;
	}

	void FixedUpdate()
	{
		if (!PV.IsMine)
			return;

		rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}

	public void StorePowerUp(PowerUp powerUp, float multiplier, float duration)
	{
		if (storedPowerUp == null)
		{
			Debug.Log("Storing power-up: " + powerUp);
			storedPowerUp = powerUp;
			storedMultiplier = multiplier;
			storedDuration = duration;
		}
	}

	public void ApplySpeedBoost(float multiplier, float duration)
	{
		Debug.Log("ApplySpeedBoost called with multiplier: " + multiplier + ", duration: " + duration);

		if (!IsPowerUpActive)
		{
			activePowerUp = PowerUp.SpeedBoost;
			StartCoroutine(ApplySpeedBoostCoroutine(multiplier, duration));
		}
	}

	private IEnumerator ApplySpeedBoostCoroutine(float multiplier, float duration)
	{
		Debug.Log("ApplySpeedBoostCoroutine called with multiplier: " + multiplier + ", duration: " + duration);
		IsPowerUpActive = true;
		speed *= multiplier;

		Debug.Log("Player speed after boost: " + speed);

		yield return new WaitForSeconds(duration);

		speed = baseSpeed;
		IsPowerUpActive = false;
		activePowerUp = PowerUp.None;
		Debug.Log("Speed boost ended. Player speed is now: " + speed);
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
}



