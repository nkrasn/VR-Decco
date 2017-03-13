using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateGrabbing : State
{
    public StateGrabbing(Controller controller) : base(controller) { }

    public override void enter()
    {
        c.teleporter.disabledController = c.trackedObj;
        c.laserPointer.setMaterial("grab");
    }

    public override void update()
    {
        // Holding the item with lasers
        if(!c.insideGrabbable)
        {
            // Keep item frozen when not gripping
            c.item.GetComponent<Rigidbody>().freezeRotation = !c.input.touchpadHeld;

            // Scrolling
            if(c.input.touchpad.getScrollY() != 0f && Mathf.Abs(c.input.touchpad.getScrollY()) > Mathf.Abs(c.input.touchpad.getScrollX()))
            {
                c.joint.connectedBody = null;

                // Resize the pointer and keep track of the size change
                float prevPointerDistance = c.laserPointer.currentDistance;
                c.laserPointer.currentDistance += Mathf.Sign(c.input.touchpad.getScrollY()) * Mathf.Pow(c.input.touchpad.getScrollY() * 3, 2);
                //Mathf.Lerp(0, input.touchpad.getScrollY(), 0.5f);
                float currPointerDistance = c.laserPointer.currentDistance;

                // Move the item by the amount the pointer was resized
                Vector3 shiftVector = c.transform.forward * (currPointerDistance - prevPointerDistance);

                c.item.transform.position += shiftVector;
                c.joint.connectedBody = c.item.GetComponent<Rigidbody>();
            }

            // Rotating
            if(c.input.touchpad.getScrollX() != 0f && Mathf.Abs(c.input.touchpad.getScrollY()) <= Mathf.Abs(c.input.touchpad.getScrollX()))
            {
                Vector3 rotatePoint = c.transform.position + c.transform.forward * c.laserPointer.currentDistance;
                c.joint.connectedBody = null;

                c.item.transform.RotateAround(rotatePoint, Vector3.up, -c.input.touchpad.getScrollX() * c.rotateSensitivity);

                c.joint.connectedBody = c.item.GetComponent<Rigidbody>();
            }

            // When ungripped, get the new item origin relative to hit point
            if(c.input.touchpadReleased)
            {
                RaycastHit hit;
                bool hitSomething = false;
                if(Physics.Raycast(c.transform.position, c.transform.forward, out hit, c.laserPointer.maxDistance))
                {
                    hitSomething = true;
                    if(hit.transform.gameObject.GetComponent<Grabbable>() == c.item)
                    {
                        c.laserPointer.currentDistance = hit.distance;
                        c.laserPointer.itemOffset = hit.point - c.item.transform.position;
                    }
                }
                if(hitSomething && hit.transform.gameObject.GetComponent<Grabbable>() != c.item)
                {
                    c.laserPointer.currentDistance = 0f;
                    c.laserPointer.itemOffset = c.transform.position - c.item.transform.position;
                }
            }

            // No grip = item rotation relative to world
            if(!c.input.touchpadHeld)
            {
                // Make the item's rotation stay relative to the world
                c.joint.connectedBody = null;
                c.item.transform.position = c.transform.position + c.transform.forward * c.laserPointer.currentDistance - c.laserPointer.itemOffset;
                c.joint.connectedBody = c.item.GetComponent<Rigidbody>();
            }
        }

        if(!c.input.triggerHeld)
        {
            c.release();
        }
    }

    public override void exit()
    {
        
    }
}