using UnityEngine;
using LibSM64;

public class ExampleInputProvider : SM64InputProvider
{
    public GameObject cameraObject = null;

    public override Vector3 GetCameraLookDirection()
    {
        return cameraObject.transform.forward;
    }

    public override Vector2 GetJoystickAxes()
    {
        return new Vector2( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") );
    }

    public override bool GetButtonHeld( Button button )
    {
        switch( button )
        {
            case SM64InputProvider.Button.Jump:  return Input.GetKey(KeyCode.X);
            case SM64InputProvider.Button.Kick:  return Input.GetKey(KeyCode.C);
            case SM64InputProvider.Button.Stomp: return Input.GetKey(KeyCode.Z);
        }
        return false;
    }
}