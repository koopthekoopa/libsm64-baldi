using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using LibSM64;

public class PlayerScript : SM64InputProvider
{
    float ropeTrigRestrict;

	private void Start()
	{
		if (PlayerPrefs.GetInt("AnalogMove") == 1)
		{
			sensitivityActive = true;
		}
		height = transform.position.y;
		stamina = maxStamina;
		playerRotation = transform.rotation;
		mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
		principalBugFixer = 1;
		flipaturn = 1f;
        marioObj = GetComponent<SM64Mario>();
        ropeTrigRestrict = ropeJumpTrigger.gameObject.transform.position.y;
        collidedRopeTrigger = false;
	}
	private void Update()
	{
		//transform.position = new Vector3(transform.position.x, height, transform.position.z);
        ropeJumpTrigger.gameObject.transform.position = new Vector3(ropeJumpTrigger.gameObject.transform.position.x, ropeTrigRestrict, ropeJumpTrigger.gameObject.transform.position.z);
		MouseMove();
		PlayerMove();
		StaminaCheck();
		GuiltCheck();
		/*if (cc.velocity.magnitude > 0f)
		{
			gc.LockMouse();
		}
        // mario needs to move
		if (jumpRope & (transform.position - frozenPosition).magnitude >= 1f) // If the player moves, deactivate the jumprope minigame
		{
			DeactivateJumpRope();
		}*/
        if (jumpRope && SM64HasMetalCap())
		{
			DeactivateJumpRope();
            playtime.Disappoint();
		}
        if (jumpRope && (Input.GetKey(KeyCode.K) && gc.debugMode))
		{
			DeactivateJumpRope();
            playtime.Disappoint();
		}
        if (hugging && SM64HasMetalCap())
		{
            firstPrizeScript.GoCrazy();
		}
		if (sweepingFailsave > 0f)
		{
			sweepingFailsave -= Time.deltaTime;
		}
		else
		{
			sweeping = false;
			hugging = false;
		}
        
        if (Input.GetKey(KeyCode.P) && this.gc.debugMode)
        {
            Debug.Log("I am guilty...");
            this.ResetGuilt("running", 0.1f);
        }
	}
	private void MouseMove()
	{
		playerRotation.eulerAngles = new Vector3(playerRotation.eulerAngles.x, playerRotation.eulerAngles.y, /*fliparoo*/ 0.0f);
		playerRotation.eulerAngles = playerRotation.eulerAngles + Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity * Time.timeScale * flipaturn;
		transform.rotation = playerRotation;
	}
	private void PlayerMove()
	{
		/*Vector3 movement = Vector3.zero;
		Vector3 lateralMovement = Vector3.zero;
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveForward)) movement = transform.forward;
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveBackward)) movement = -transform.forward;
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveLeft)) lateralMovement = -transform.right;
		if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveRight)) lateralMovement = transform.right;
		if (stamina > 0f)
		{
			if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Punch))
			{
				playerSpeed = runSpeed;
				sensitivity = 1f;
				if (cc.velocity.magnitude > 0.1f & !hugging & !sweeping)
				{
					ResetGuilt("running", 0.1f);
				}
			}
			else
			{
				playerSpeed = walkSpeed;
				if (sensitivityActive)
				{
					sensitivity = Mathf.Clamp((movement + lateralMovement).magnitude, 0f, 1f);
				}
				else
				{
					sensitivity = 1f;
				}
			}
		}
		else
		{
			playerSpeed = walkSpeed;
			if (sensitivityActive)
			{
				sensitivity = Mathf.Clamp((lateralMovement + movement).magnitude, 0f, 1f);
			}
			else
			{
				sensitivity = 1f;
			}
		}
		playerSpeed *= Time.deltaTime;
		moveDirection = (movement + lateralMovement).normalized * playerSpeed * sensitivity;
		if (!(!jumpRope & !sweeping & !hugging))
		{
			if (sweeping && !bootsActive)
			{
				moveDirection = gottaSweep.velocity * Time.deltaTime + moveDirection * 0.3f;
			}
			else if (hugging && !bootsActive)
			{
				moveDirection = (firstPrize.velocity * 1.2f * Time.deltaTime + (new Vector3(firstPrizeTransform.position.x, height, firstPrizeTransform.position.z) + new Vector3((float)Mathf.RoundToInt(firstPrizeTransform.forward.x), 0f, (float)Mathf.RoundToInt(firstPrizeTransform.forward.z)) * 3f - transform.position)) * (float)principalBugFixer;
			}
			else if (jumpRope)
			{
				moveDirection = new Vector3(0f, 0f, 0f);
			}
		}
		cc.Move(moveDirection);*/
        
        if (marioObj.IsLongJumping())
        {
            ResetGuilt("running", 0.045f);
        }
        
        if (sweeping && (!bootsActive || SM64HasMetalCap()) && gottaSweep.gameObject.GetComponent<SweepScript>().active)
        {
            moveDirection = gottaSweep.velocity * Time.deltaTime + moveDirection * 0.55f;
            marioObj.MovePosition(new Vector3(moveDirection.x, moveDirection.y, moveDirection.z));
            SM64ResetSpeed();
            SM64SetAction(SM64ActionType.ACT_GRABBED);
        }
        else if (hugging && (!bootsActive || SM64HasMetalCap()))
        {
            moveDirection = (firstPrize.velocity * 1.75f * Time.deltaTime + (new Vector3(firstPrizeTransform.position.x, height, firstPrizeTransform.position.z) + new Vector3((float)Mathf.RoundToInt(firstPrizeTransform.forward.x), 0f, (float)Mathf.RoundToInt(firstPrizeTransform.forward.z)) * 3f - base.transform.position)) * (float)principalBugFixer;
            marioObj.MovePosition(new Vector3(moveDirection.x, moveDirection.y, moveDirection.z));
            SM64SetAction(SM64ActionType.ACT_GRABBED);
        }
        
        if (!hugging && !sweeping)
        {
            SM64SetAction(SM64ActionType.ACT_IDLE);
        }
	}
	private void StaminaCheck()
	{
		/*if (cc.velocity.magnitude > 0.1f)
		{
			if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Punch) & stamina > 0f)
			{
				stamina -= staminaRate * Time.deltaTime;
			}
			if (stamina < 0f & stamina > -5f)
			{
				stamina = -5f;
			}
		}
		else if (stamina < maxStamina)
		{
			stamina += staminaRate * Time.deltaTime;
		}
		staminaBar.value = stamina / maxStamina * 100f;*/
	}

    static GameObject TriggerObj = null;

    public static GameObject GetTriggered()
    {
        return TriggerObj;
    }

	private void OnTriggerEnter(Collider other)
	{
        TriggerObj = other.gameObject;

		if (other.transform.name == "Playtime" & !jumpRope & playtime.playCool <= 0f)
		{
            SM64ResetSpeed();
			ActivateJumpRope();
		}
	}
	public IEnumerator KeepTheHudOff()
	{
		while (gameOver)
		{
			hud.enabled = false;
			jumpRopeScreen.SetActive(false);
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}
	private void OnTriggerStay(Collider other)
	{
        if (other.transform.name == "Baldi" & (!gc.debugMode && !SM64HasMetalCap() && !gameOver))
		{
			gameOver = true;
			RenderSettings.skybox = blackSky; //Sets the skybox black
			StartCoroutine(KeepTheHudOff()); //Hides the Hud
		}
		else if (other.transform.name == "Gotta Sweep")
		{
			sweeping = true;
			sweepingFailsave = 1f;
		}
		else if (other.transform.name == "1st Prize" & firstPrize.velocity.magnitude > 5f)
		{
			hugging = true;
			sweepingFailsave = 1f;
		}
        
        if (other.name == ropeJumpTrigger.name)
        {
            collidedRopeTrigger = true;
        }
	}
	private void OnTriggerExit(Collider other)
	{
        TriggerObj = null;
		if (other.transform.name == "Office Trigger")
		{
			ResetGuilt("escape", door.lockTime);
		}
		else if (other.transform.name == "Gotta Sweep")
		{
			sweeping = false;
		}
		else if (other.transform.name == "1st Prize")
		{
			hugging = false;
		}
        
        if (other.name == ropeJumpTrigger.name)
        {
            collidedRopeTrigger = false;
        }
	}
	public void ResetGuilt(string type, float amount)
	{
		if (amount >= guilt)
		{
			guilt = amount;
			guiltType = type;
		}
	}
	private void GuiltCheck()
	{
		if (guilt > 0f)
		{
			guilt -= Time.deltaTime;
		}
	}
	public void ActivateJumpRope()
	{
		jumpRopeScreen.SetActive(true);
		jumpRope = true;
		frozenPosition = transform.position;
	}
	public void DeactivateJumpRope()
	{
		jumpRopeScreen.SetActive(false);
		jumpRope = false;
	}
	public void ActivateBoots()
	{
		bootsActive = true;
		StartCoroutine(BootTimer());
	}
	private IEnumerator BootTimer()
	{
		float time = 15f;
		while (time > 0f)
		{
			time -= Time.deltaTime;
			yield return null;
		}
		bootsActive = false;
		yield break;
	}
	public GameControllerScript gc;
	public BaldiScript baldi;
	public DoorScript door;
	public PlaytimeScript playtime;
	public bool gameOver;
	public bool jumpRope;
	public bool sweeping;
	public bool hugging;
	public bool bootsActive;
	public int principalBugFixer;
	public float sweepingFailsave;
	public float fliparoo;
	public float flipaturn;
	private Quaternion playerRotation;
	public Vector3 frozenPosition;
	private bool sensitivityActive;
	private float sensitivity;
	public float mouseSensitivity;
	public float walkSpeed;
	public float runSpeed;
	public float slowSpeed;
	public float maxStamina;
	public float staminaRate;
	public float guilt;
	public float initGuilt;
	private Vector3 moveDirection;
	private float playerSpeed;
	public float stamina;
	public CharacterController cc;
	public NavMeshAgent gottaSweep;
	public NavMeshAgent firstPrize;
	[SerializeField] public FirstPrizeScript firstPrizeScript;
	public Transform firstPrizeTransform;
	public Slider staminaBar;
	public float db;
	public string guiltType;
	public GameObject jumpRopeScreen;
	public float height;
	public Material blackSky;
	public Canvas hud;
    public SM64Mario marioObj;
    public bool collidedRopeTrigger;
    public Collider ropeJumpTrigger;
    public bool keyLockHitboxTrigger;

    /** SM64 INPUT **/

    public void SM64Teleport(Vector3 position)
    {
        marioObj.TeleportTo(position);
    }

    public void SM64ResetSpeed()
    {
        marioObj.ResetSpeed();
    }

    public Quaternion SM64Rotation()
    {
        return marioObj.GetRotation();
    }

    public bool SM64HasMetalCap()
    {
        return marioObj.HasMetalCap();
    }

    public bool SM64IsAttacking()
    {
        return marioObj.IsAttacking();
    }

    SM64ActionType lastSet = SM64ActionType.ACT_IDLE;
    public void SM64SetAction(SM64ActionType type, bool force = false)
    {
        if (lastSet != type || force)
        {
            lastSet = type;
            marioObj.SetAction(lastSet);
        }
    }
    
    [SerializeField] GameObject cameraObject;
    
    public override Vector3 GetCameraLookDirection()
    {
        return cameraObject.transform.forward;
    }
    
    static float CalcHorizontalAxis()
    {
        if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveLeft))
        {
            return -1.0f;
        }
        else if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveRight))
        {
            return 1.0f;
        }
        else
        {
            return 0.0f;
        }
    }
    
    static float CalcVerticalAxis()
    {
        if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveBackward))
        {
            return -1.0f;
        }
        else if (Singleton<InputManager>.Instance.GetActionKey(InputAction.MoveForward))
        {
            return 1.0f;
        }
        else
        {
            return 0.0f;
        }
    }
    
    // HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH what fucking ever
    static float DumbLimitFix()
    {
        bool up = CalcVerticalAxis() > 0;
        bool down = CalcVerticalAxis() < 0;
        bool right = CalcHorizontalAxis() > 0;
        bool left = CalcHorizontalAxis() < 0;
        return ((up || down) && (left || right)) ? 0.75f : 1.0f;
    }
 
    public override Vector2 GetJoystickAxes()
    {
        if (jumpRope || (hugging || (sweeping && gottaSweep.gameObject.GetComponent<SweepScript>().active) && (!bootsActive || SM64HasMetalCap())))
        {
            return Vector2.zero;
        }
        return new Vector2(CalcHorizontalAxis() * DumbLimitFix(),
                           CalcVerticalAxis() * DumbLimitFix());
    }

    public override bool GetButtonHeld(Button button)
    {
        switch (button)
        {
            case SM64InputProvider.Button.Kick: return Singleton<InputManager>.Instance.GetActionKey(InputAction.Punch) /*&& !(hugging || sweeping)*/;
            case SM64InputProvider.Button.Jump: return Singleton<InputManager>.Instance.GetActionKey(InputAction.Jump)  /*&& !(hugging || sweeping)*/;
            case SM64InputProvider.Button.Stomp: return Singleton<InputManager>.Instance.GetActionKey(InputAction.Crouch)  /*&& !(hugging || sweeping)*/;
        }
        return false;
    }
}
