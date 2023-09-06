using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple Camera with X,Y movement and FOV zoom in
public class CameraZoom : MonoBehaviour
{
	private Camera cam;
	
	private float verticalInput;
	private float horizontalInput;
	[SerializeField]
	private float mouseWheelInput;
	[SerializeField]
	private float speed = 20;
	private float zoom;
	[SerializeField]
	private float zoomSpeed = 15;
	[SerializeField]
	private float zoomLerpSpeed = 10;

	void Start()
	{
		cam = Camera.main;
		zoom = cam.fieldOfView;
	}
	
    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
		horizontalInput = Input.GetAxis("Horizontal");
		mouseWheelInput = Input.GetAxis("Mouse ScrollWheel");
		
		transform.Translate(Vector3.up * verticalInput * speed * Time.deltaTime);
		transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);
		
		zoom = zoom - (mouseWheelInput * zoomSpeed);
		zoom = Mathf.Clamp(zoom, 1f, 100f);
		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom, Time.deltaTime * zoomLerpSpeed);
    }
}
