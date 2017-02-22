using UnityEngine;
using System.Collections;

public class ViveInput
{
    public ViveTouchpad touchpad { get; private set; }
    private SteamVR_Controller.Device _device;
    public SteamVR_Controller.Device device
    {
        get { return _device; }
        set
        {
            _device = value;
            if(touchpad == null)
                touchpad = new ViveTouchpad();
            touchpad.device = value;
        }
    }

    public bool triggerPressed { get; private set; }
    public bool triggerHeld { get; private set; }
    public bool triggerReleased { get; private set; }

    public bool touchpadPressed { get; private set; }
    public bool touchpadHeld { get; private set; }
    public bool touchpadReleased { get; private set; }

    public bool gripPressed { get; private set; }
    public bool gripHeld { get; private set; }
    public bool gripReleased { get; private set; }

    public bool menuPressed { get; private set; }

    public ViveInput()
    {
        device = null;
        touchpad = new ViveTouchpad();
    }
    
    public void updateInput()
    {
        triggerPressed = device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
        triggerHeld = device.GetPress(SteamVR_Controller.ButtonMask.Trigger);
        triggerReleased = device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger);

        touchpad.update();
        touchpadPressed = device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad);
        touchpadHeld = device.GetPress(SteamVR_Controller.ButtonMask.Touchpad);
        touchpadReleased = device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad);

        gripPressed = device.GetPressDown(SteamVR_Controller.ButtonMask.Grip);
        gripHeld = device.GetPress(SteamVR_Controller.ButtonMask.Grip);
        gripReleased = device.GetPressUp(SteamVR_Controller.ButtonMask.Grip);

        menuPressed = device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu);
    }
}