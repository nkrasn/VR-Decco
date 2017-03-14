using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public SteamVR_TrackedObject.EIndex deviceID;
    public float rotateSensitivity = 20f;

    public SteamVR_TrackedObject trackedObj { get; private set; }
    public SteamVR_Controller.Device device { get; private set; }
    public LaserPointer laserPointer { get; private set; }
    public EditSphere editSphere { get; private set; }
    public TeleportVive teleporter { get; private set; }

    private Grabbable _item;
    public Grabbable item { get; set; }
    public FixedJoint joint { get; private set; }

    public bool insideGrabbable { get; private set; }

    public ViveInput input { get; private set; }

    // States
    public StateIdle stateIdle { get; private set; }
    public StateGrabbing stateGrabbing { get; private set; }
    public StateTeleporting stateTeleporting { get; private set; }
    public StateEditIdle stateEditIdle { get; private set; }
    public StateEditGrabbing stateEditGrabbing { get; private set; }

    private State _currentState;
    public State currentState
    {
        get { return _currentState; }
        set
        {
            if(_currentState != null)
                _currentState.exit();
            _currentState = value;
            _currentState.enter();
            Debug.Log(_currentState);
        }
    }

    void Start()
    {
        // Initialize variables
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        laserPointer = new LaserPointer(this, 100f);
        editSphere = new EditSphere(this, new Vector3(0f, -0.05f, 0.1f));
        teleporter = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TeleportVive>();
        
        insideGrabbable = false;

        input = new ViveInput();

        // States
        stateIdle = new StateIdle(this);
        stateGrabbing = new StateGrabbing(this);
        stateTeleporting = new StateTeleporting(this);
        stateEditIdle = new StateEditIdle(this);
        stateEditGrabbing = new StateEditGrabbing(this);
        currentState = stateIdle;

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

    void Update()
    {
        if(input == null)
            input = new ViveInput();
        if(currentState == null)
            currentState = stateIdle;

        // Don't proceed unless the controller is set up
        if(deviceID != trackedObj.index || !trackedObj.isValid)
        {
            deviceID = trackedObj.index;
            device = SteamVR_Controller.Input((int) trackedObj.index);
            input.device = device;
            return;
        }
        
        input.updateInput();
        currentState.update();
    }



    private void OnTriggerEnter(Collider other)
    {
        Grabbable otherItem = other.gameObject.GetComponent<Grabbable>();

        if(otherItem != null)
        {
            if(currentState == stateIdle || (currentState == stateGrabbing && otherItem == item && laserPointer.currentDistance == 0f))
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
            if(currentState != stateGrabbing)
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
    public void hideLaserPointer(bool hide)
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

    public void grab(bool asHand)
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

        currentState = stateGrabbing;
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

        currentState = stateIdle;
    }
}