using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public Controller leftController;
    public Controller rightController;

    [Space(10)]
    public GameObject ruler;

    private void Start()
    {
        leftController.controllerManager = rightController.controllerManager = this;

        ruler = Instantiate(ruler);
        ruler.SetActive(false);
    }
}