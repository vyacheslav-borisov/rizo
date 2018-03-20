using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamHelper : MonoBehaviour
{
    public float MovementSpeed = 20.0f;
    [Range(1.0f, 100.0f)]
    public float HorizontalSensevity = 50.0f;
    [Range(1.0f, 100.0f)]
    public float VerticalSensevity = 50.0f;

    private Camera _camera;

	void Start ()
    {
        _camera = GetComponent<Camera>();		
	}
	
	void Update ()
    {
        if (Input.GetMouseButton(0))
        {
            float xAxis = Input.GetAxis("Mouse X");
            float yAxis = Input.GetAxis("Mouse Y");

            Vector3 angles = transform.localEulerAngles;
            angles.x += -yAxis * Time.deltaTime * VerticalSensevity;
            angles.y += xAxis * Time.deltaTime * HorizontalSensevity;
            angles.z = 0.0f;
            transform.localEulerAngles = angles;
        }

        if(Input.GetMouseButton(2))
        {
            float xAxis = Input.GetAxis("Mouse X");
            float yAxis = Input.GetAxis("Mouse Y");

            Vector3 localUP = transform.InverseTransformDirection(transform.up);
            Vector3 localRight = transform.InverseTransformDirection(transform.right);
            Vector3 trans = Vector3.zero;

            trans += localUP * yAxis * Time.deltaTime * VerticalSensevity;
            trans += localRight * xAxis * Time.deltaTime * HorizontalSensevity;

            transform.Translate(trans);
        }

        Vector3 translation = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            //forward
            Vector3 direction = transform.InverseTransformDirection(transform.forward);
            translation += direction * Time.deltaTime * MovementSpeed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            //backward
            Vector3 direction = transform.InverseTransformDirection(-transform.forward);
            translation += direction * Time.deltaTime * MovementSpeed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            //left
            Vector3 direction = transform.InverseTransformDirection(-transform.right);
            translation += direction * Time.deltaTime * MovementSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            //right
            Vector3 direction = transform.InverseTransformDirection(transform.right);
            translation += direction * Time.deltaTime * MovementSpeed;
        }

        transform.Translate(translation);                
    }
}
