using UnityEngine;
using LibSM64;

public class NoInputProvider : SM64InputProvider
{
    public GameObject cameraObject = null;

    public override Vector3 GetCameraLookDirection()
    {
        return cameraObject.transform.forward;
    }

    public override Vector2 GetJoystickAxes()
    {
        return Vector2.zero;
    }

    public override bool GetButtonHeld( Button button )
    {
        return false;
    }
}