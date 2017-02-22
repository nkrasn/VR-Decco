using UnityEngine;
using System.Collections;

public class LaserPointer
{
    private Controller controller;

    public GameObject laser, laserHolder;
    private Material idleMaterial, hitMaterial, grabMaterial;

    // Represents where the item is relative to the pointer's hit location
    public Vector3 itemOffset { get; set; }

    public float maxDistance;
    private float _currentDistance;
    public float currentDistance
    {
        get
        {
            return _currentDistance;
        }
        set
        {
            _currentDistance = Mathf.Max(0f, value);
            laser.transform.localScale = new Vector3(laserThickness, laserThickness, _currentDistance);
            laser.transform.localPosition = Vector3.forward * (_currentDistance / 2);

            hitSphere.transform.localPosition = Vector3.forward * (_currentDistance);
        }
    }
    private bool _enabled;
    public bool enabled
    {
        get
        {
            return _enabled;
        }
        set
        {
            _enabled = value;
            laser.GetComponent<Renderer>().enabled = _enabled;
        }
    }

    private float laserThickness;
    private GameObject hitSphere;

    public LaserPointer(Controller controller, float maxDistance)
    {
        this.controller = controller;

        this.maxDistance = maxDistance;
        laserThickness = 0.002f;
        idleMaterial = Resources.Load<Material>("Materials/IdleLaser");
        hitMaterial = Resources.Load<Material>("Materials/HitLaser");
        grabMaterial = Resources.Load<Material>("Materials/GrabLaser");

        itemOffset = Vector3.zero;

        // Parent for the laser cube
        laserHolder = new GameObject();
        laserHolder.transform.parent = this.controller.transform;
        laserHolder.transform.localPosition = Vector3.zero;

        // Hit sphere (where the laser hits)
        hitSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitSphere.transform.parent = laserHolder.transform;
        hitSphere.transform.localScale = Vector3.one * 0.01f;
        GameObject.Destroy(hitSphere.GetComponent<SphereCollider>());

        // Create the laser
        laser = GameObject.CreatePrimitive(PrimitiveType.Cube);
        laser.transform.parent = laserHolder.transform;
        currentDistance = maxDistance;
        laser.GetComponent<Collider>().enabled = false;
        setMaterial("idle");
        laser.GetComponent<Renderer>().enabled = enabled;

        enabled = true;
    }

    public void setMaterial(string m)
    {
        switch(m)
        {
            case "idle":
                laser.GetComponent<Renderer>().material = idleMaterial;
                break;
            case "hit":
                laser.GetComponent<Renderer>().material = hitMaterial;
                break;
            case "grab":
                laser.GetComponent<Renderer>().material = grabMaterial;
                break;
        }
    }

    public void hideHitSphere(bool choice)
    {
        hitSphere.SetActive(!choice);
    }
}