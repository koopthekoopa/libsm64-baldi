using UnityEngine;

public class PickupScript : MonoBehaviour
{
    private bool triggered = false;
    private GameObject hit = null;
    public GameObject spriteForMetalCap = null;
    
	private void Update()
	{
        bool metalCapAvailable = false;
        if (gameObject.name == "Pickup_MetalCap")
        {
            if (gc.notebooks >= 4 && gc.notebooks < 7)
            {
                metalCapAvailable = true;
            }
            spriteForMetalCap.SetActive(metalCapAvailable);
        }

		if (/*Singleton<InputManager>.Instance.GetActionKeyDown(InputAction.Interact) &&*/ Time.timeScale != 0f)
		{
			/*Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit))*/
            if (triggered)
			{
                //Debug.Log(gameObject.name);
				if (gameObject.name == "Pickup_EnergyFlavoredZestyBar"/* & Vector3.Distance(player.position, transform.position) < 10f*/)
				{
					transform.gameObject.SetActive(false);
					gc.CollectItem(1);
				}
				else if (gameObject.name == "Pickup_YellowDoorLock"/* & Vector3.Distance(player.position, transform.position) < 10f*/)
				{
					transform.gameObject.SetActive(false);
					gc.CollectItem(2);
				}
				else if (gameObject.name == "Pickup_Key"/* & Vector3.Distance(player.position, transform.position) < 10f*/)
				{
					transform.gameObject.SetActive(false);
					gc.CollectItem(3);
				}
				else if (gameObject.name == "Pickup_BSODA"/* & Vector3.Distance(player.position, transform.position) < 10f*/)
				{
					transform.gameObject.SetActive(false);
					gc.CollectItem(4);
				}
				else if (gameObject.name == "Pickup_Quarter"/* & Vector3.Distance(player.position, transform.position) < 10f*/)
				{
					transform.gameObject.SetActive(false);
					gc.CollectItem(5);
				}
				else if (gameObject.name == "Pickup_Tape"/* & Vector3.Distance(player.position, transform.position) < 10f*/)
				{
					transform.gameObject.SetActive(false);
					gc.CollectItem(6);
				}
				else if (gameObject.name == "Pickup_AlarmClock"/* & Vector3.Distance(player.position, transform.position) < 10f*/)
				{
					transform.gameObject.SetActive(false);
					gc.CollectItem(7);
				}
				else if (gameObject.name == "Pickup_WD-3D"/* & Vector3.Distance(player.position, transform.position) < 10f*/)
				{
					transform.gameObject.SetActive(false);
					gc.CollectItem(8);
				}
				else if (gameObject.name == "Pickup_SafetyScissors"/* & Vector3.Distance(player.position, transform.position) < 10f*/)
				{
					transform.gameObject.SetActive(false);
					gc.CollectItem(9);
				}
				else if (gameObject.name == "Pickup_BigBoots"/* & Vector3.Distance(player.position, transform.position) < 10f*/)
				{
					transform.gameObject.SetActive(false);
					gc.CollectItem(10);
				}
				else if (gameObject.name == "Pickup_Teleporter"/* & Vector3.Distance(player.position, transform.position) < 10f*/)
				{
					transform.gameObject.SetActive(false);
					gc.CollectItem(11);
				}
				else if (gameObject.name == "Pickup_MetalCap"/* & Vector3.Distance(player.position, transform.position) < 10f*/ && metalCapAvailable)
				{
					transform.gameObject.SetActive(false);
					gc.MetalCap();
				}
			}
		}
	}
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            triggered = true;
            hit = other.gameObject;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            triggered = false;
            hit = null;
        }
    }

	public GameControllerScript gc;
	public Transform player;
}
