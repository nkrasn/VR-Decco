using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruler : MonoBehaviour
{
    public GameObject leftSphere, rightSphere;
    public TextMesh text;
    public float offset = 0.1f;

    private float distanceMeters;
    private GameObject leftController, rightController, playerHead;
    private LineRenderer line;

    private void Start()
    {
        leftController = GameObject.Find("Controller (left)");
        rightController = GameObject.Find("Controller (right)");
        playerHead = GameObject.Find("Camera (eye)");
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        leftSphere.transform.position = leftController.transform.position + leftController.transform.forward * offset;
        rightSphere.transform.position = rightController.transform.position + rightController.transform.forward * offset;
        line.SetPosition(0, leftSphere.transform.position);
        line.SetPosition(1, rightSphere.transform.position);
        distanceMeters = (leftSphere.transform.position - rightSphere.transform.position).magnitude;

        // Make the text position the center of the line
        text.transform.position = Vector3.Lerp(leftSphere.transform.position, rightSphere.transform.position, 0.5f);
        text.transform.LookAt(playerHead.transform.position);

        text.text = inches() + "\n" + feetInches() + "\n" + meters();
    }

    public string inches()
    {
        float inches = distanceMeters / 0.0254f;
        return inches.ToString("0.0") + "\"";
    }

    public string feetInches()
    {
        float inches = distanceMeters / 0.0254f;
        int feet = (int)Mathf.Floor(inches / 12f);
        inches -= feet * 12f;
        return feet.ToString() + "' " + inches.ToString("0.0") + "\"";
    }

    public string meters()
    {
        return distanceMeters.ToString("0.00") + " meters";
    }
}
