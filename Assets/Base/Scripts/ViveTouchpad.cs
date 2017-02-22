using UnityEngine;
using System.Collections;

public class ViveTouchpad
{
    public SteamVR_Controller.Device device;
    private Vector2 prevPos, currPos;

    public ViveTouchpad()
    {
        device = null;

        prevPos = Vector2.zero;
        currPos = Vector2.zero;
    }

    public void update()
    {
        if(device == null)
            return;

        if(device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad)) // At initial touch, all values = touch location
        {
            prevPos.y = currPos.y = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y;
            prevPos.x = currPos.x = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
        }
        if(device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad)) // No touch, all values = 0
        {
            prevPos.y = currPos.y = 0f;
            prevPos.x = currPos.x = 0f;
        }
        prevPos.y = currPos.y;
        prevPos.x = currPos.x;
        currPos.y = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y;
        currPos.x = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
    }

    public float getX() { return currPos.x; }
    public float getY() { return currPos.y; }
    public float getScrollY()
    {
        return currPos.y - prevPos.y;
    }
    public float getScrollX()
    {
        return currPos.x - prevPos.x;
    }

    public float getAngle()
    {
        if((prevPos == Vector2.zero && currPos == Vector2.zero) || currPos.magnitude < 0.25f)
            return 0f;

        float sign = Vector3.Cross(prevPos, currPos).z < 0f ? -1f : 1f;
        return Vector2.Angle(prevPos, currPos) * sign;
    }
}
