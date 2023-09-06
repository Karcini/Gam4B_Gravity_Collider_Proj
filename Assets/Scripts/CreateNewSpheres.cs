using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNewSpheres : MonoBehaviour
{
	private float MassHigh = 0;
	private float MassLow = 0;
	private float VelocityHigh = 0;
	private float VelocityLow = 0;
	private float MassAbsorbRate = 0;
	//rate to consume mass
	
	private Rigidbody body;
	[SerializeField]
	private Vector3 vec;
	
	private Vector3 gravity;
	[SerializeField]
	private CreateNewSpheres []newSpheres;
	[SerializeField]
	private Vector3 forceVector;
	
	[SerializeField]
	private float distance; 
	[SerializeField]
	private float force; 
	//Decrease force Cap if you get tunneling, anything under -10 diminishes pull effect
	public float forceCap;
	
	//public bool newSpheresCreated = false;
	//BUGS
		//The first object created has no force vector
			//There is a single frame when force is calculated, where m2 = 0 making force = 9.81
				//This means there is a single frame where obj2 does not exist therefore the Force Vector is 0,0,0
				//Even though both objects are pulling onto one another correctly, force Vector3 and other values of the 1st object in the scene is stuck at defaults.
					//FIXED: decided it didn't visibly affect the results of program
					
		//Objects currently pull exponentially quickly when near other objects
			//As distance -> 0, Force -> infiniti
				//Results in tunneling, force being = to 0 when distance <= 0 would not fix due to the fact when force IS calculated F = very large values due to small values of distance.
				//Force Vector needs a cap dictated by it's scalar "force"
					//cap force value should be low enough to prevent tunneling but high enough to produce a satisfying "strong pull" effect when near an object
					//FIXED: -20 produces a satisfying pull effect, increase to make it look more "realistic".  Worse computers might get tunneling with high cap values.
					
		//Both If and Else in the forEach method is occurring
			//8 Instances of foreach occurs
				//You need to check if THIS is the same object as other and if it is SKIP it so that the Distance is not 0
				//create variables to hold memory instead of changing the direct values
				//FIXED: I think
				
		//Absorb mass seemingly consumes the entire objects mass in a single frame, not instantly destroying the smaller object results in infinite values.
			//Prof says setting the object to INTERPOLATION, or adding a COLLIDER might help with the tunnelling issues
		
	void Start()
	{
		gravity = Physics.gravity;
		forceCap = -20;
		//newSpheresCreated = true;
		//needs to be in update function or old spheres wont calculate new spheres
		//newSpheres = FindObjectsOfType<CreateNewSpheres>();
	}
	
    void Update()
    {
		newSpheres = FindObjectsOfType<CreateNewSpheres>();
		foreach (CreateNewSpheres other in newSpheres)
		{	
			//check if this is the same object as other
			if (this.gameObject != other.gameObject)
			{
				float myDist = ObjectDistance(this,other);
				if(myDist > 0)
				{
					Debug.Log(myDist);
					forceVector = GravitationalPull(other);
					transform.Translate((vec+forceVector) * Time.deltaTime);
				}
				else
				{
					//The objects have collided and the two objects in question distribute mass
					Debug.Log("Objects collided");
					Absorb(other);
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
	private void OverrideMass(float value)
	{
		body = GetComponent<Rigidbody>();
		body.mass = value;
		this.transform.localScale *= value;
	}
	//Creates a Vector3 of Random Velocity
	private void CreateVelocity()
	{
		vec = new Vector3(Random.Range(-1,2), Random.Range(-1,2), Random.Range(-1,2)).normalized;
		float velScale = Random.Range(VelocityLow, VelocityHigh);
		vec *= velScale;
	}
	
	//Handle Gravitational Pulls ----
		//F = G ((m1 m2) / distance^2)
			//we have gravity m1 and m2
			//distance can be found using a method that is given 2 vectors and removes their radius to return a "fixed" distance that approaches 0 as they collide
	private Vector3 GravitationalPull(CreateNewSpheres other)
	{
		float m1 = body.mass;
		float m2 = other.gameObject.GetComponent<Rigidbody>().mass;
		distance = ObjectDistance(this, other);
		force = gravity.y *((m1*m2)/(distance*distance));
		//Include this method to Cap Force
		//CapForce();
		
		Vector3 gravitationalPull = SubtractVectors(this,other);
		gravitationalPull.Normalize();
		////might need to divide force by 2 because it's technically being done twice 
		gravitationalPull *= force;
		return gravitationalPull;
	}
	private float ObjectDistance(CreateNewSpheres obj1, CreateNewSpheres obj2)
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
	private Vector3 SubtractVectors(CreateNewSpheres obj1, CreateNewSpheres obj2)
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
	
	//Checks which mass is larger and distributes Mass
	private void Absorb(CreateNewSpheres other)
	{
		Debug.Log("Absob played");
		float m1 = body.mass;
		float m2 = other.gameObject.GetComponent<Rigidbody>().mass;
		bool otherObjLarger = m2 > m1;
		float rate;
		if (otherObjLarger)
		{
			//get absorb rate of m2
			rate = other.MassAbsorbRate;
			AbsorbMass(other, this, rate);
			Destroy(this.gameObject);
		}
		else
		{
			rate = MassAbsorbRate;
			AbsorbMass(this, other, rate);
			Destroy(other.gameObject);
		}
	}
	private void AbsorbMass(CreateNewSpheres bigObj, CreateNewSpheres smallObj, float rate)
	{
		Debug.Log("AbsobMass played");
		//increase rate by 10%, get bigObj mass, increase temp mass by rate
		float newRate = rate + (rate/10);
		float tempMass = bigObj.gameObject.GetComponent<Rigidbody>().mass;
		tempMass += (tempMass*rate);
		//increase mass by new temp mass, override local scale by mass, increase absorb rate 10%
		bigObj.gameObject.GetComponent<Rigidbody>().mass = tempMass;
		bigObj.OverrideMass(tempMass);
		//bigObj.MassAbsorbRate = newRate;
		
		//decrease rate by 10%, get bigObj mass, decrease temp mass by rate
		newRate = rate - (rate/10);
		tempMass = smallObj.gameObject.GetComponent<Rigidbody>().mass;
		tempMass -= (tempMass*rate);
		//decrease mass by new temp mass, override local scale by mass, decrease absorb rate 10%
		smallObj.gameObject.GetComponent<Rigidbody>().mass = tempMass;
		smallObj.OverrideMass(tempMass);
		//smallObj.MassAbsorbRate = newRate;
	}
}
