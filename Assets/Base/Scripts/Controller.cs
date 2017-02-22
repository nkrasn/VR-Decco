using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public SteamVR_TrackedObject.EIndex deviceID;
    public float rotateSensitivity = 20f;

    private SteamVR_TrackedObject trackedObj;
    public SteamVR_Controller.Device device { get; private set; }
    private LaserPointer laserPointer;
    private TeleportVive teleporter;

    private Grabbable _item;
    public Grabbable item { get; set; }
    private FixedJoint joint;

    private bool insideGrabbable;

    private ViveInput input;

    // States
    private enum State
    {
        Idle, Grabbing,
        Teleporting
    }
    private bool switchedStates;
    private State _currentState;
    private State currentState
    {
        get { return _currentState; }
        set
        {
            switchedStates = true;
            _currentState = value;
            Debug.Log(_currentState);
        }
    }

    void Start()
    {
        // Initialize variables
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        laserPointer = new LaserPointer(this, 100f);
        teleporter = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TeleportVive>();
        
        insideGrabbable = false;

        input = new ViveInput();

        switchedStates = false;

        // Hand needs a rigidbody for joints
        Rigidbody r = gameObject.AddComponent<Rigidbody>();
        r.angularDrag = 0f;
        r.useGravity = false;
        r.isKinematic = true;

        joint = gameObject.AddComponent<FixedJoint>();

        // Give the hand a sphere trigger
        SphereCollider sc = gameObject.AddComponent<SphereCollider>();
        sc.radius = 0.03f;
        sc.isTrigger = true;
	}


    private void OnTriggerEnter(Collider other)
    {
        Grabbable otherItem = other.gameObject.GetComponent<Grabbable>();

        if(otherItem != null)
        {
            if(currentState == State.Idle || (currentState == State.Grabbing && otherItem == item && laserPointer.currentDistance == 0f))
            {
                insideGrabbable = true;
                item = other.gameObject.GetComponent<Grabbable>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Grabbable otherItem = other.gameObject.GetComponent<Grabbable>();

        if(otherItem != null)
        {
            insideGrabbable = false;
            if(currentState != State.Grabbing)
                item = null;
        }
    }


    // Enable/disable the colliders for the item
    private void setCollision(bool choice)
    {
        foreach(Collider col in item.GetComponents<Collider>())
        {
            if(!col.isTrigger)
                col.enabled = choice;
        }
    }

    // Activate/deactive the laser pointer
    private void hideLaserPointer(bool hide)
    {
        if(hide)
        {
            laserPointer.hideHitSphere(true);
            laserPointer.enabled = false;
        }
        else
        {
            laserPointer.enabled = true;
            laserPointer.hideHitSphere(false);
        }
    }

    private void grab(bool asHand)
    {
        // Release the other hand if it's grabbing
        if(item.grabbingHand != this)
        {
            if(item.grabbingHand != null)
                item.grabbingHand.release();
            item.grabbingHand = this;
        }

        // Grab it
        item.GetComponent<Rigidbody>().freezeRotation = !asHand;
        joint.connectedBody = item.GetComponent<Rigidbody>();
        setCollision(false);

        currentState = State.Grabbing;
    }

    public void release()
    {
        if(item == null)
            return;
        item.grabbingHand = null;

        // Release it
        item.GetComponent<Rigidbody>().freezeRotation = false;
        joint.connectedBody = null;
        setCollision(true);

        // Fling the item appropriately
        Rigidbody itemRb = item.GetComponent<Rigidbody>();
        float flingMultiplier = laserPointer.enabled ? Mathf.Max(laserPointer.currentDistance * device.angularVelocity.magnitude / 2, 1f) : 1f;

        itemRb.velocity = device.velocity * flingMultiplier;
        itemRb.angularVelocity = device.angularVelocity;

        currentState = State.Idle;
    }



    void Update()
    {
        if(input == null)
            input = new ViveInput();

        // Don't proceed unless the controller is set up
        if(deviceID != trackedObj.index || !trackedObj.isValid)
        {
            deviceID = trackedObj.index;
            device = SteamVR_Controller.Input((int) trackedObj.index);
            input.device = device;
            return;
        }

        input.updateInput();
        
        // Do appropriate behavior for the current state
        switch(currentState)
        {
            case State.Idle:
                // Executed when you first enter the state
                if(switchedStates)
                {
                    hideLaserPointer(false);
                    if(teleporter.disabledController == trackedObj)
                        teleporter.disabledController = null;
                    switchedStates = false;
                }

                // Unable to grab with hands, see if pointer hit something
                if(!insideGrabbable)
                {
                    RaycastHit hit;
                    item = null;
                    if(Physics.Raycast(transform.position, transform.forward, out hit, laserPointer.maxDistance))
                    {
                        laserPointer.currentDistance = hit.distance;
                        laserPointer.setMaterial("hit");

                        item = hit.transform.gameObject.GetComponent<Grabbable>();

                        // Get the position of the item origin relative to the hit point (for item rotation)
                        if(item != null)
                        {
                            laserPointer.itemOffset = hit.point - item.transform.position;
                        }
                    }
                    // Nothing hit, laser distance = max
                    if(item == null)
                    {
                        laserPointer.currentDistance = laserPointer.maxDistance;
                        laserPointer.setMaterial("idle");
                    }
                }
                else
                {
                    laserPointer.currentDistance = 0f;
                }

                // Duplicate the item (temporary)
                if(item != null && input.gripPressed)
                {
                    GameObject dupe = Instantiate(item.gameObject);
                    Vector3 newPos = dupe.transform.position;
                    newPos.y += dupe.GetComponent<BoxCollider>().bounds.extents.y * 2f + 0.25f;
                    dupe.transform.position = newPos;
                }

                // Grab the item
                if(item != null && input.triggerPressed)
                {
                    grab(insideGrabbable);
                    break;
                }
                
                // Teleporting, don't grab or do anything else
                if(teleporter.disabledController == null && input.touchpadHeld)
                {
                    currentState = State.Teleporting;
                    break;
                }

                break;
            case State.Grabbing:
                if(switchedStates)
                {
                    teleporter.disabledController = trackedObj;
                    laserPointer.setMaterial("grab");
                    switchedStates = false;
                }

                // Holding the item with lasers
                if(!insideGrabbable)
                {
                    // Keep item frozen when not gripping
                    item.GetComponent<Rigidbody>().freezeRotation = !input.touchpadHeld;
                    
                    // Scrolling
                    if(input.touchpad.getScrollY() != 0f && Mathf.Abs(input.touchpad.getScrollY()) > Mathf.Abs(input.touchpad.getScrollX()))
                    {
                        joint.connectedBody = null;
                        
                        // Resize the pointer and keep track of the size change
                        float prevPointerDistance = laserPointer.currentDistance;
                        laserPointer.currentDistance += Mathf.Sign(input.touchpad.getScrollY()) * Mathf.Pow(input.touchpad.getScrollY() * 3, 2);
                        //Mathf.Lerp(0, input.touchpad.getScrollY(), 0.5f);
                        float currPointerDistance = laserPointer.currentDistance;

                        // Move the item by the amount the pointer was resized
                        Vector3 shiftVector = transform.forward * (currPointerDistance - prevPointerDistance);

                        item.transform.position += shiftVector;
                        joint.connectedBody = item.GetComponent<Rigidbody>();
                    }

                    // Rotating
                    if(input.touchpad.getScrollX() != 0f && Mathf.Abs(input.touchpad.getScrollY()) <= Mathf.Abs(input.touchpad.getScrollX()))
                    {
                        Vector3 rotatePoint = transform.position + transform.forward * laserPointer.currentDistance;
                        joint.connectedBody = null;

                        item.transform.RotateAround(rotatePoint, Vector3.up, -input.touchpad.getScrollX() * rotateSensitivity);

                        joint.connectedBody = item.GetComponent<Rigidbody>();
                    }

                    // When ungripped, get the new item origin relative to hit point
                    if(input.touchpadReleased)
                    {
                        RaycastHit hit;
                        bool hitSomething = false;
                        if(Physics.Raycast(transform.position, transform.forward, out hit, laserPointer.maxDistance))
                        {
                            hitSomething = true;
                            if(hit.transform.gameObject.GetComponent<Grabbable>() == item)
                            {
                                laserPointer.currentDistance = hit.distance;
                                laserPointer.itemOffset = hit.point - item.transform.position;
                            }
                        }
                        if(hitSomething && hit.transform.gameObject.GetComponent<Grabbable>() != item)
                        {
                            laserPointer.currentDistance = 0f;
                            laserPointer.itemOffset = transform.position - item.transform.position;
                        }
                    }

                    // No grip = item rotation relative to world
                    if(!input.touchpadHeld)
                    {
                        // Make the item's rotation stay relative to the world
                        joint.connectedBody = null;
                        item.transform.position = transform.position + transform.forward * laserPointer.currentDistance - laserPointer.itemOffset;
                        joint.connectedBody = item.GetComponent<Rigidbody>();
                    }
                }

                if(!input.triggerHeld)
                {
                    release();
                    break;
                }

                break;
            case State.Teleporting:
                if(switchedStates)
                {
                    hideLaserPointer(true);
                    switchedStates = false;
                }

                if(!input.touchpadHeld)
                {
                    currentState = State.Idle;
                    break;
                }
                break;
        }
    }
}