using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGizmo : MonoBehaviour
{
    public bool showGizmos = false;
    public Color color = Color.yellow;

    void OnDrawGizmos()
    {
        if (showGizmos) 
        {
            // Draw a sphere at the transform's position
            SphereCollider sphereCollider = GetComponent<SphereCollider>();

            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, sphereCollider.radius);
        }
    }
}
