using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateEditGrabbing : State
{
    public GameObject meshObject;
    public Mesh mesh;
    public int[] vertIndices; // Indices for vertices to adjust

    private Vector3[] vertOffsets; // Offsets from sphere position and vertices

    public StateEditGrabbing(Controller controller) : base(controller) { }

    public override void enter()
    {
        // meshObject must be set before changing the current state to StateEditGrabbing
        mesh = meshObject.GetComponent<MeshFilter>().sharedMesh;


        vertOffsets = new Vector3[mesh.vertices.Length];
        Vector3 spherePos = meshObject.transform.InverseTransformPoint(c.editSphere.sphere.transform.position);

        foreach(int i in vertIndices)
            vertOffsets[i] = mesh.vertices[i] - spherePos;
    }

    public override void update()
    {
        Vector3[] vs = mesh.vertices;
        Vector3 spherePos = meshObject.transform.InverseTransformPoint(c.editSphere.sphere.transform.position);

        // Move the vertices
        foreach(int i in vertIndices)
        {
            Debug.Log(vertOffsets.Length + " " + i);
            vs[i] = vertOffsets[i] + spherePos + vertOffsets[i];
        }
        mesh.vertices = vs;

        // Readjust the collider of the item
        meshObject.GetComponent<Grabbable>().recalculateBounds();

        // Release the vertices
        if(!c.input.triggerHeld)
        {
            c.currentState = c.stateEditIdle;
            return;
        }
    }

    public override void exit()
    {
        mesh = null;
        vertIndices = null;
    }
}