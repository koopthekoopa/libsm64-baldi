using UnityEngine;
using LibSM64;

public class NotebookScript : MonoBehaviour
{
    bool touched;

	private void Start()
	{
		up = true;
        touched = false;
	}
	private void Update()
	{
		if (gc.mode == "endless")
		{
			if (respawnTime > 0f)
			{
				if ((transform.position - player.position).magnitude > 60f)
				{
					respawnTime -= Time.deltaTime;
				}
			}
			else if (!up)
			{
				transform.position = new Vector3(transform.position.x, 4f, transform.position.z);
				up = true;
				audioDevice.Play();
			}
		}
		if (/*Singleton<InputManager>.Instance.GetActionKey(InputAction.Interact) && */Time.timeScale != 0f)
		{
			/*Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit) && (raycastHit.transform.tag == "Notebook" & Vector3.Distance(player.position, transform.position) < openingDistance))*/
			if ((touched & Vector3.Distance(player.position, transform.position) < openingDistance))
			{
                touched = false;
				transform.position = new Vector3(transform.position.x, -20f, transform.position.z);
				up = false;
				respawnTime = 120f;
				gc.CollectNotebook();
				GameObject gameObject = Instantiate<GameObject>(learningGame);
				gameObject.GetComponent<MathGameScript>().gc = gc;
				gameObject.GetComponent<MathGameScript>().baldiScript = bsc;
				gameObject.GetComponent<MathGameScript>().playerPosition = player.position;
			}
		}
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Player")
        {
            other.gameObject.GetComponent<PlayerScript>().SM64SetAction(SM64ActionType.ACT_IDLE);
            other.gameObject.GetComponent<PlayerScript>().marioObj.SetAction(SM64ActionType.ACT_IDLE);
            other.gameObject.GetComponent<PlayerScript>().SM64Teleport(transform.position);
            touched = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "Player")
        {
            touched = false;
        }
    }

	public float openingDistance;
	public GameControllerScript gc;
	public BaldiScript bsc;
	public float respawnTime;
	public bool up;
	public Transform player;
	public GameObject learningGame;
	public AudioSource audioDevice;
}
