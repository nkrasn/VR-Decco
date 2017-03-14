using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEditIdle : State
{
    public StateEditIdle(Controller controller) : base(controller) { }

    public override void enter()
    {

    }

    public override void update()
    {
        // Find a grabbable within range of the edit sphere
        Collider[] cols = Physics.OverlapSphere(c.editSphere.sphere.transform.position, c.editSphere.sphere.transform.localScale.x / 2);
        Collider col = null;
        foreach(Collider c in cols)
        {
            if(c.GetComponent<Grabbable>() != null)
            {
                col = c;
                break;
            }
        }

        // Change the color of the edit sphere
        if(col != null)
            c.editSphere.setMaterial("hit");
        else
            c.editSphere.setMaterial("idle");

        // Grab the vertices
        if(c.input.triggerHeld && col != null)
        {
            Mesh grabbedMesh = col.GetComponent<MeshFilter>().sharedMesh;
            List<int> grabbedVertices = new List<int>();

            // Find the vertices which need to be moved
            for(int i = 0; i < grabbedMesh.vertexCount; i++)
            {
                float dist = (col.transform.TransformPoint(grabbedMesh.vertices[i]) - c.editSphere.sphere.transform.position).magnitude;
                if(dist <= c.editSphere.diameter / 2f)
                {
                    grabbedVertices.Add(i);
                }
            }
            c.stateEditGrabbing.meshObject = col.gameObject;
            c.stateEditGrabbing.vertIndices = grabbedVertices.ToArray();
            
            c.currentState = c.stateEditGrabbing;
            return;
        }

        // Go back to idle mode
        if(c.input.menuPressed)
        {
            c.teleporter.disabledController = null;
            c.hideLaserPointer(false);
            c.editSphere.hideSphere(true);
            c.currentState = c.stateIdle;
            return;
        }
    }

    public override void exit()
    {
        
    }
}