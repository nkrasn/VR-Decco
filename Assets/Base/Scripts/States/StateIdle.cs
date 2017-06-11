using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : State
{

    public StateIdle(Controller controller) : base(controller) { }

    public override void enter()
    {
        c.hideLaserPointer(false);
        if(c.teleporter.disabledController == c.trackedObj)
            c.teleporter.disabledController = null;
    }

    public override void update()
    {
        // Unable to grab with hands, see if pointer hit something
        if(!c.insideGrabbable)
        {
            RaycastHit hit;
            c.item = null;
            if(Physics.Raycast(c.transform.position, c.transform.forward, out hit, c.laserPointer.maxDistance))
            {
                c.laserPointer.currentDistance = hit.distance;
                c.laserPointer.setMaterial("hit");

                c.item = hit.transform.gameObject.GetComponent<Grabbable>();

                // Get the position of the item origin relative to the hit point (for item rotation)
                if(c.item != null)
                {
                    c.laserPointer.itemOffset = hit.point - c.item.transform.position;
                }
            }
            // Nothing hit, laser distance = max
            if(c.item == null)
            {
                c.laserPointer.currentDistance = c.laserPointer.maxDistance;
                c.laserPointer.setMaterial("idle");
            }
        }
        else
        {
            c.laserPointer.currentDistance = 0f;
        }

        // Duplicate the item (temporary)
        if(c.item != null && c.input.gripPressed)
        {
            GameObject dupe = GameObject.Instantiate(c.item.gameObject);
            Vector3 newPos = dupe.transform.position;
            newPos.y += dupe.GetComponent<BoxCollider>().bounds.extents.y * 2f + 0.25f;
            dupe.transform.position = newPos;
        }

        // Grab the item
        if(c.item != null && c.input.triggerPressed)
        {
            c.grab(c.insideGrabbable);
            return;
        }

        // Teleporting, don't grab or do anything else
        if(c.teleporter.disabledController == null && c.input.touchpadHeld)
        {
            c.currentState = c.stateTeleporting;
            return;
        }

        /*
        // Edit mode for meshes
        // Will figure out later, possibly using compute shaders
        if(c.input.menuPressed)
        {
            c.teleporter.disabledController = c.trackedObj;
            c.hideLaserPointer(true);
            c.editSphere.hideSphere(false);
            c.currentState = c.stateEditIdle;
            return;
        }
        */

        // Ruler
        if(c.input.menuPressed)
        {
            c.controllerManager.ruler.SetActive(!c.controllerManager.ruler.activeSelf);
        }
    }

    public override void exit()
    {

    }
}
