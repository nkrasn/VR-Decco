using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditSphere
{
    private Controller controller;

    public Vector3 sphereOffset { get; private set; }
    private float _diameter;
    public float diameter
    {
        get
        {
            return _diameter;
        }
        set
        {
            _diameter = value;
            sphere.transform.localScale = Vector3.one * _diameter;
        }
    }
    public GameObject sphere { get; private set; }

    public EditSphere(Controller controller, Vector3 sphereOffset)
    {
        this.controller = controller;
        this.sphereOffset = sphereOffset;

        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/IdleEditSphere");
        sphere.transform.parent = controller.transform;
        sphere.transform.localPosition = new Vector3(0f, 0f, 0.1f);
        diameter = 0.1f;
        GameObject.Destroy(sphere.GetComponent<SphereCollider>());

        hideSphere(true);
    }

    public void hideSphere(bool hide)
    {
        sphere.SetActive(!hide);
    }

    public void setMaterial(string mat)
    {
        switch(mat)
        {
            case "hit":
                sphere.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/HitEditSphere");
                break;
            case "idle":
                sphere.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/IdleEditSphere");
                break;
        }
    }
}