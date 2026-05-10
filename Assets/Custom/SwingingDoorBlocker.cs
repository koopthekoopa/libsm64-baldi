using UnityEngine;
using LibSM64;

public class SwingingDoorBlocker : MonoBehaviour
{
    SM64DynamicTerrain terrain;
	GameControllerScript gc;

    Vector3 blockPos;
    Vector3 unblockPos;
    
    void Start()
    {
        gc = FindObjectOfType<GameControllerScript>();
        terrain = GetComponent<SM64DynamicTerrain>();
        
        blockPos = transform.position;
        unblockPos = new Vector3(-5000, -5000, -5000);
    }
    
    public void ActivateBlocker()
    {
        terrain.SetPosition(blockPos);
    }
    
    public void DeactivateBlocker()
    {
        terrain.SetPosition(unblockPos);
    }
}