using UnityEngine;

public class KeyLockBound : MonoBehaviour
{
	private void OnEnter(Collider other)
	{
		if (other.tag == "SwingingDoor")
        {
            player.keyLockHitboxTrigger = true;
        }
	}
	private void OnExit(Collider other)
	{
		if (other.tag == "SwingingDoor")
        {
            player.keyLockHitboxTrigger = false;
        }
	}
    
	private void OnTriggerEnter(Collider other)
	{
        OnEnter(other);
	}
	private void OnTriggerExit(Collider other)
	{
        OnExit(other);
	}
	private void OnCollisionEnter(Collision other)
	{
        OnEnter(other.collider);
	}
	private void OnCollisionExit(Collision other)
	{
        OnExit(other.collider);
	}

	public PlayerScript player;
}
