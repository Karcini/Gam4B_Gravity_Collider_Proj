using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
	public GameObject spherePrefab;
	public int amountOfSpheres = 2;
	public float highestMass = 10;
	public float lowestMass = 1;
	public float highestVelocity = 10;
	public float lowestVelocity = 1;
	public float absorbMassRate = 1;
	
	private float screenSize = 10f;
	//If you want to increase game size, increase gameScale
		//2.5 is a nice number
	private float gameScale = 1f;

	private GravitySphere []newSpheres;
	
	void Start()
	{
		screenSize *= gameScale;
		highestVelocity *= gameScale;
	}

	public void SetScene()
	{
		//button is pressed
		for(int i =0; i<amountOfSpheres; i++)
		{
			float x = Random.Range(-screenSize,screenSize);
			float y = Random.Range(-screenSize,screenSize);
			float z = Random.Range(-screenSize,screenSize);
			GameObject newSphere = Instantiate(spherePrefab, new Vector3 (x,y,z), Quaternion.identity);
			newSphere.GetComponent<GravitySphere>().GetSphereSettings(highestMass, lowestMass, highestVelocity, lowestVelocity, absorbMassRate);
			newSphere.GetComponent<GravitySphere>().RandomizeSphere();
		}
		
		//make other objects aware that there are new spheres
		//RecallSpheres();
	}
	/*
	private void RecallSpheres()
	{
		newSpheres = FindObjectsOfType<CreateNewSpheres>();
		foreach (CreateNewSpheres other in newSpheres)
		{
			other.gameObject.newSpheresCreated = true;
		}
	}
	*/
	/*
	public void SetText(string text)
	{
		Text buttonText = transform.Find("Text").GetComponent<Text>();
		buttonText.text = text;
	}
	
	//Increase and Decrease Amount of Spheres
	public void IncreaseSpheres()
	{
		amountOfSpheres += 1;
		ChangeAmountOfSpheresText();
	}
	public void DecreaseSpheres()
	{
		amountOfSpheres -= 1;
		ChangeAmountOfSpheresText();
	}
	public void ChangeAmountOfSpheresText()
	{
		//Text labelText = transform.Find("NumberOfSpheres").GetComponent<Text>();
		Debug.Log(amountOfSpheres.ToString());
		//string label = amountOfSpheres.ToString();
		//labelText = label;
	}
	*/
}
