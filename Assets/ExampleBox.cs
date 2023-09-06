using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleBox : MonoBehaviour
{
    public Collider collider;  //GetComponent() in Start()
    //public Collider other_collider;
    //public Transform testPoint;
	public ExampleBox []allBoxes;

	private Vector3 pt1, pt2, pt3, pt4, pt5, pt6, pt7, pt8;         //corners of the collider. Should be counted clockwise, bottom first. 5 above 1.
	private Color lineColor = Color.green;

	//returns true if there is an overlap in every axis
		//AABB Volume Bounds are true on collision if they overlap on every axis
    bool isPointInsideAABB(Vector3 point, Bounds box)
    {
        return ((point.x >= box.min.x && point.x <= box.max.x) &&
                (point.y >= box.min.y && point.y <= box.max.y) &&
                (point.z >= box.min.z && point.z <= box.max.z));
    }

    bool OverlapTest(Bounds a, Bounds b)
    {
        return ((a.min.x <= b.max.x && a.max.x >= b.min.x) &&
                (a.min.y <= b.max.y && a.max.y >= b.min.y) &&
                (a.min.z <= b.max.z && a.max.z >= b.min.z));
    }
	
	private void Start()
	{
		allBoxes = FindObjectsOfType<ExampleBox>();
		collider = GetComponent<Collider>();
	}
	
    private void Update()
    {
		foreach (ExampleBox other in allBoxes)
		{
			//we use this and not other.gameObject because this detects all colliders within the root of an object
			if (other.transform.root != collider.transform.root)
			{
				if (OverlapTest(collider.bounds, other.collider.bounds))
				{
					lineColor = Color.red;
					break;
				}
			}
			else
			{
				lineColor = Color.green;
			}
		}
        //getting coords of each boxcollider corner
        pt1 = collider.bounds.min;
        pt2 = collider.bounds.max;
        pt3 = new Vector3(pt1.x, pt1.y, pt2.z);
        pt4 = new Vector3(pt1.x, pt2.y, pt1.z);
        pt5 = new Vector3(pt2.x, pt1.y, pt1.z);
        pt6 = new Vector3(pt1.x, pt2.y, pt2.z);
        pt7 = new Vector3(pt2.x, pt1.y, pt2.z);
        pt8 = new Vector3(pt2.x, pt2.y, pt1.z);

        Debug.DrawLine(pt1, pt3, lineColor);
        Debug.DrawLine(pt1, pt4, lineColor);
        Debug.DrawLine(pt1, pt5, lineColor);

        Debug.DrawLine(pt2, pt6, lineColor);
        Debug.DrawLine(pt2, pt7, lineColor);
        Debug.DrawLine(pt2, pt8, lineColor);


        Debug.DrawLine(pt3, pt6, lineColor);
        Debug.DrawLine(pt3, pt7, lineColor);

        Debug.DrawLine(pt4, pt6, lineColor);
        Debug.DrawLine(pt4, pt8, lineColor);

        Debug.DrawLine(pt5, pt8, lineColor);
        Debug.DrawLine(pt5, pt7, lineColor);
    }
}
