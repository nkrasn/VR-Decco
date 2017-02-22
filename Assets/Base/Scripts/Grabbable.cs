using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour
{
    public CollisionShape collisionShape;
    public PhysicMaterial physicMaterial = null;
    public Vector3 customCenter = Vector3.zero;
    public bool useCustomCenter = false;

    public Controller grabbingHand { get; set; }

    public enum CollisionShape
    {
        Box, Sphere, ConvexMesh
    }

    private BoxCollider solidBox;
    private BoxCollider triggerBox;

    void Start()
    {
        grabbingHand = null;

        switch(collisionShape)
        {
            case CollisionShape.Box:
                solidBox = gameObject.AddComponent<BoxCollider>();
                triggerBox = gameObject.AddComponent<BoxCollider>();
                solidBox.isTrigger = false;
                triggerBox.isTrigger = true;

                if(physicMaterial != null)
                    solidBox.material = physicMaterial;

                // Calculate the size of the boxes
                solidBox.size = GetComponent<MeshFilter>().sharedMesh.bounds.extents * 2f;
                foreach(Transform t in transform)
                {
                    Mesh mesh = t.gameObject.GetComponent<MeshFilter>().sharedMesh;
                    Vector3 meshBounds = mesh.bounds.extents; // Mesh bounds is half while BoxCollider size is not for some reason
                    Vector3 meshCenter = mesh.bounds.center;

                    float meshMaxX = meshCenter.x + meshBounds.x;
                    float meshMinX = meshCenter.x - meshBounds.x;
                    float meshMaxY = meshCenter.y + meshBounds.y;
                    float meshMinY = meshCenter.y - meshBounds.y;
                    float meshMaxZ = meshCenter.z + meshBounds.z;
                    float meshMinZ = meshCenter.z - meshBounds.z;

                    float boxMaxX = solidBox.center.x + solidBox.size.x / 2f;
                    float boxMinX = solidBox.center.x - solidBox.size.x / 2f;
                    float boxMaxY = solidBox.center.y + solidBox.size.y / 2f;
                    float boxMinY = solidBox.center.y - solidBox.size.y / 2f;
                    float boxMaxZ = solidBox.center.z + solidBox.size.z / 2f;
                    float boxMinZ = solidBox.center.z - solidBox.size.z / 2f;

                    if(meshMaxX > boxMaxX)
                    {
                        Vector3 adjust = Vector3.right * (meshMaxX - boxMaxX);
                        solidBox.center = solidBox.center + adjust / 2f;
                        solidBox.size = solidBox.size + adjust;
                    }
                    if(meshMinX < boxMinX)
                    {
                        Vector3 adjust = Vector3.right * (boxMinX - meshMinX);
                        solidBox.center = solidBox.center - adjust / 2f;
                        solidBox.size = solidBox.size + adjust;
                    }

                    if(meshMaxY > boxMaxY)
                    {
                        Vector3 adjust = Vector3.up * (meshMaxY - boxMaxY);
                        solidBox.center = solidBox.center + adjust / 2f;
                        solidBox.size = solidBox.size + adjust;
                    }
                    if(meshMinY < boxMinY)
                    {
                        Vector3 adjust = Vector3.up * (boxMinY - meshMinY);
                        solidBox.center = solidBox.center - adjust / 2f;
                        solidBox.size = solidBox.size + adjust;
                    }

                    if(meshMaxZ > boxMaxZ)
                    {
                        Vector3 adjust = Vector3.forward * (meshMaxZ - boxMaxZ);
                        solidBox.center = solidBox.center + adjust / 2f;
                        solidBox.size = solidBox.size + adjust;
                    }
                    if(meshMinZ < boxMinZ)
                    {
                        Vector3 adjust = Vector3.forward * (boxMinZ - meshMinZ);
                        solidBox.center = solidBox.center - adjust / 2f;
                        solidBox.size = solidBox.size + adjust;
                    }
                }

                if(useCustomCenter)
                    solidBox.center = customCenter;
                triggerBox.center = solidBox.center;
                triggerBox.size = solidBox.size;
                break;
            case CollisionShape.Sphere:
                
                break;
        }

        // Make sure the item is in the item hierarchy
        if(transform.parent == null || transform.parent.tag != "ItemHierarchy")
        {
            transform.SetParent(GameObject.FindGameObjectWithTag("ItemHierarchy").transform);
        }
	}

    void Update()
    {

    }
}