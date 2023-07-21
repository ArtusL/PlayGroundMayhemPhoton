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

	Rigidbody rb;

	PhotonView PV;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();
		roleManager = GetComponent<PlayerRoleManager>();
	}

	void Start()
	{
		if (!PV.IsMine)
		{
			Destroy(GetComponentInChildren<Camera>().gameObject);
			Destroy(rb);
		}
	}

	void Update()
	{
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
		moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);

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
}



//using Photon.Pun;
//using Photon.Realtime;
//using System.Collections;
//using UnityEngine;

//public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
//{
//    public IPowerUp currentPowerUp;

//    public SeekerLogic seekerLogic;

//    public float mouseSensitivity = 100f;
//    private float xRotation = 0f;

//    public float speed = 3.0f;
//    public float sprintSpeed = 5.0f;

//    public float maxStaminaHider = 5f;
//    public float maxStaminaSeeker = 7f;
//    private bool isRecoveringStamina = false;

//    private float staminaRechargeDelay = 3f;
//    public float currentStamina;
//    private bool isSprinting = false;
//    private float maxStamina;
//    private Coroutine staminaRecoveryCoroutine;

//    public bool isSpeedBoosted = false;
//    public bool hasPowerup = false;
//    public float powerupBoost = 0f;
//    public float powerupDuration = 0f;
//    public bool isSeeker;

//    public bool canMove = true;

//    void Start()
//    {
//        seekerLogic = GetComponent<SeekerLogic>();

//        maxStamina = maxStaminaHider;
//        currentStamina = maxStamina;
//    }

//    public void OnPhotonInstantiate(PhotonMessageInfo info)
//    {
//        PlayerUI playerUI = FindObjectOfType<PlayerUI>();
//        playerUI.playerController = this;
//    }

//    void FixedUpdate()
//    {
//        if (photonView.IsMine && !seekerLogic.isStunned && canMove)
//        {
//            HandleMovement();
//            HandleSprinting();
//            HandleMouseLook();
//        }

//    }

//    public void SpeedBoost(float increase, float duration)
//    {
//        photonView.RPC("RpcSpeedBoost", RpcTarget.AllBuffered, increase, duration);
//    }

//    [PunRPC]
//    void RpcSpeedBoost(float increase, float duration)
//    {
//        StartCoroutine(SpeedBoostCoroutine(increase, duration));
//    }

//    void Update()
//    {
//        GetComponent<Renderer>().material.color = seekerLogic.isSeeker ? Color.red : Color.blue;

//        if (Input.GetKeyDown(KeyCode.Q) && currentPowerUp != null)
//        {
//            currentPowerUp.UsePowerUp(this);
//            hasPowerup = false;
//            currentPowerUp = null;
//        }
//    }

//    public void ActivatePowerUp()
//    {
//        if (currentPowerUp != null)
//        {
//            currentPowerUp.UsePowerUp(this);
//            currentPowerUp = null;
//        }
//    }
//    private void HandleMovement()
//    {
//        float moveHorizontal = Input.GetAxis("Horizontal");
//        float moveVertical = Input.GetAxis("Vertical");

//        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
//        float moveSpeed = isSprinting ? sprintSpeed : speed;

//        transform.position = transform.position + transform.TransformDirection(movement) * moveSpeed * Time.deltaTime;
//    }

//    private void HandleMouseLook()
//    {
//        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

//        transform.Rotate(Vector3.up * mouseX);
//    }

//    private void HandleSprinting()
//    {
//        bool isMoving = (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0);

//        if (isMoving && Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
//        {
//            if (staminaRecoveryCoroutine != null)
//            {
//                StopCoroutine(staminaRecoveryCoroutine);
//                isRecoveringStamina = false;
//            }

//            isSprinting = true;
//            Debug.Log("player has started sprinting");
//        }

//        if ((!isMoving || !Input.GetKey(KeyCode.LeftShift) || currentStamina <= 0) && isSprinting)
//        {
//            isSprinting = false;
//            if (!isRecoveringStamina)
//            {
//                staminaRecoveryCoroutine = StartCoroutine(RecoverStaminaAfterDelay(staminaRechargeDelay));
//                Debug.Log("player has stopped sprinting");
//            }
//        }

//        if (isSprinting && isMoving)
//        {
//            currentStamina -= Time.fixedDeltaTime;
//        }

//        if (currentStamina <= 0)
//        {
//            currentStamina = 0;
//        }

//        if (currentStamina > maxStamina)
//        {
//            currentStamina = maxStamina;
//        }
//    }


//    private IEnumerator RecoverStaminaAfterDelay(float delay)
//    {
//        isRecoveringStamina = true;

//        yield return new WaitForSeconds(delay);

//        while (currentStamina < maxStamina)
//        {
//            currentStamina += Time.fixedDeltaTime;
//            yield return null;
//        }

//        isRecoveringStamina = false;
//    }

//    public IEnumerator SpeedBoostCoroutine(float increase, float duration) 
//    {
//        float originalSpeed = speed;
//        float originalSprintSpeed = sprintSpeed;
//        speed += increase;
//        sprintSpeed += increase;

//        yield return new WaitForSeconds(duration);

//        speed = originalSpeed;
//        sprintSpeed = originalSprintSpeed;
//    }
//    public void ChangeRole(bool isSeeker)
//    {
//        this.isSeeker = isSeeker;
//        maxStamina = isSeeker ? maxStaminaSeeker : maxStaminaHider;
//    }

//    public void SetStamina(float stamina)
//    {
//        currentStamina = stamina;
//    }

//    public void RestoreStamina()
//    {
//        currentStamina = maxStamina;
//        Debug.Log("Stamina restored to maximum");
//    }
//    public float GetMaxStamina()
//    {
//        return maxStamina;
//    }

//    public void ApplyPowerup(IPowerUp powerup)
//    {
//        hasPowerup = true;
//        currentPowerUp = powerup;
//        Debug.Log("In PlayerController: hasPowerup = " + hasPowerup);
//        Debug.Log("In PlayerController: currentPowerUp = " + currentPowerUp.GetPowerUpName());
//    }

//    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//    {
//        if (stream.IsWriting)
//        {
//            // We own this player: send the others our data
//            stream.SendNext(isSeeker);
//        }
//        else
//        {
//            // Network player, receive data
//            this.isSeeker = (bool)stream.ReceiveNext();
//        }
//    }
//}
