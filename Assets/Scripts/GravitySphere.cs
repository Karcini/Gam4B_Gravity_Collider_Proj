using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySphere : MonoBehaviour
{
	//variables you can change
	private float MassHigh = 0;
	private float MassLow = 0;
	private float VelocityHigh = 0;
	private float VelocityLow = 0;
	private float MassAbsorbRate = 0;
	
	private Vector3 locScale;
	private float originalMass;
	
	private Rigidbody body;
	[SerializeField]
	private Vector3 vec;
	[SerializeField]
	private float gravity;
	[SerializeField]
	private bool useUnityGravity = false;
	[SerializeField]
	private static GravitySphere []spheres;

	[SerializeField]
	private Vector3 forceVector;
	
	[SerializeField]
	private float distance; 
	[SerializeField]
	private float force; 
	//Decrease force Cap if you get tunneling, anything under -10 diminishes pull effect
	public float forceCap;

	private Renderer renderer;
	
	//BUGS
		// Program becomes very slow if no collisions are made, resulting in many gravity spheres with force vectors
		
	void Start()
	{
		GravitySphere.spheres = FindObjectsOfType<GravitySphere>();
		body = GetComponent<Rigidbody>();
		renderer = GetComponent<Renderer>();
		renderer.materials[0].color = new Color(Random.value, Random.value, Random.value);
		if (useUnityGravity)
			gravity = Physics.gravity.y;
		
		forceCap = -20;
		locScale = transform.localScale;
		originalMass = body.mass;
	}
	
    void FixedUpdate()
    {
			
		foreach (GravitySphere other in GravitySphere.spheres)
		{
			if (other != null)
			{
				//check if this is the same object as other
				if (this.gameObject != other.gameObject)
				{
					float myDist = ObjectDistance(this, other);
					if (myDist > 0)
					{
						//Debug.Log(myDist);
						forceVector = GravitationalPull(other);
						vec += forceVector;
						vec *= VelocitySizeSlowdown();
						vec *= Time.fixedDeltaTime;
						transform.Translate(vec);
					}
					else
					{
						//The objects have collided
						//Debug.Log("Objects collided");
						ResetVectors();
						Absorb(other);
						DestroyOnZeroMass(other);
					}
				}
			}
		}
    }
	
	public void GetSphereSettings(float highMass, float lowMass, float highVel, float lowVel, float absorb)
	{
		MassHigh = highMass;
		MassLow = lowMass;
		VelocityHigh = highVel;
		VelocityLow = lowVel;
		MassAbsorbRate = absorb;
	}
	
	//Randomize Sphere Assets----
	public void RandomizeSphere()
	{
		CreateMass();
		CreateVelocity();
	}
	//Randomize Mass, Scale value by Mass
	private void CreateMass()
	{
		float scale = Random.Range(MassHigh, MassLow);
		OverrideMass(scale);
	}
	private void OverrideMass(float newMass )
	{
		if(body == null)
			body = GetComponent<Rigidbody>();
		body.mass = newMass;
	}
	//Creates a Vector3 of Random Velocity
	private void CreateVelocity()
	{
		vec = new Vector3(Random.Range(-1,2), Random.Range(-1,2), Random.Range(-1,2)).normalized;
		float velScale = Random.Range(VelocityLow, VelocityHigh);
		vec *= velScale;
	}
	//Reset Vectors
	private void ResetVectors()
	{
		vec = new Vector3 (0,0,0);
		forceVector = new Vector3 (0,0,0);
	}
	//Slowdown Velocity by Size
	private float VelocitySizeSlowdown()
	{
		float massScale = originalMass/body.mass;
		return massScale;
	}
	
	//Handle Gravitational Pulls ----
		//F = G ((m1 m2) / distance^2)
			//we have gravity m1 and m2
			//distance can be found using a method that is given 2 vectors and removes their radius to return a "fixed" distance that approaches 0 as they collide
	private Vector3 GravitationalPull(GravitySphere other)
	{
		float m1 = body.mass;
		float m2 = other.body.mass;
		distance = ObjectDistance(this, other);
		force = gravity *((m1*m2)/(distance*distance));
		//Include this method to Cap Force
		CapForce();
		
		Vector3 gravitationalPull = SubtractVectors(this,other);
		gravitationalPull.Normalize();
		//might need to divide force by 2 because it's technically being done twice 
		gravitationalPull *= force;
		return gravitationalPull;
	}
	private float ObjectDistance(GravitySphere obj1, GravitySphere obj2)
	{
		Vector3 dist = SubtractVectors(obj1,obj2);
		float distance = (Mathf.Abs(dist.magnitude));
		//get radii of both objects localscale
		Vector3 objectLS1 = obj1.gameObject.transform.localScale;
		float objectRad1 = (Mathf.Abs(objectLS1.x/2));
		Vector3 objectLS2 = obj2.gameObject.transform.localScale;
		float objectRad2 = (Mathf.Abs(objectLS2.x/2));
		//subtract radii of both objects to create a stop on "collision"
		float fixedDistance = distance - objectRad1 - objectRad2;
			//Debug.LogError(distance +" - "+ objectRad1 +" - "+ objectRad2 +" = "+ fixedDistance);
		return fixedDistance;
	}
	private Vector3 SubtractVectors(GravitySphere obj1, GravitySphere obj2)
	{
		Vector3 objectVec1 = obj1.gameObject.transform.position;
		Vector3 objectVec2 = obj2.gameObject.transform.position;
		Vector3 subtractedVector = objectVec1 - objectVec2;
		return subtractedVector;
	}
	private void CapForce()
	{
		if(force < forceCap)
			force = forceCap;
	}
	
	//Checks which mass is larger to call AbsorbMass
	private void Absorb(GravitySphere other)
	{
		//Debug.Log("Absob played");
		float m1 = body.mass;
		float m2 = other.gameObject.GetComponent<Rigidbody>().mass;
		bool otherObjLarger = m2 > m1;
		float rate;
		if (otherObjLarger)
		{
			//get absorb rate of m2
			rate = other.MassAbsorbRate;
			AbsorbMass(other, this, rate);
		}
		else
		{
			rate = MassAbsorbRate;
			AbsorbMass(this, other, rate);
		}
	}
	private void AbsorbMass(GravitySphere bigObj, GravitySphere smallObj, float rate)
	{
		//Debug.Log("AbsobMass played");
		//increase rate by 10%, get bigObj mass, increase temp mass by rate
		float tempMass = bigObj.gameObject.GetComponent<Rigidbody>().mass;
		tempMass += (tempMass*rate/10);
		//increase mass by new temp mass, override local scale by mass, increase absorb rate 10%
		bigObj.gameObject.GetComponent<Rigidbody>().mass = tempMass;
		bigObj.OverrideMass(tempMass);
		bigObj.transform.localScale += locScale * rate/10;

		//decrease rate by 10%, get bigObj mass, decrease temp mass by rate
		tempMass = smallObj.gameObject.GetComponent<Rigidbody>().mass;
		tempMass -= (tempMass*rate/10);
		//decrease mass by new temp mass, override local scale by mass, decrease absorb rate 10%
		smallObj.gameObject.GetComponent<Rigidbody>().mass = tempMass;
		smallObj.OverrideMass(tempMass);
		smallObj.transform.localScale -= locScale * rate/10;
	}
	private void DestroyOnZeroMass(GravitySphere obj)
	{
		if(obj.transform.localScale.x < 0)
		{
			Destroy(obj.gameObject);
			GravitySphere.spheres = FindObjectsOfType<GravitySphere>();
			locScale = transform.localScale;
		}
	}
}
