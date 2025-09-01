using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObj : MonoBehaviour
{
    private Vector3 lastPosition;
    public float rotationSpeed = 10f;
    public GameObject Target1;
    public GameObject Target2;
    public GameObject Target3;
    
    void Start()
    {
        

        // Save the initial position
        lastPosition = transform.position;
       // this.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
    }

    void Update()
    {
        // Calculate the direction the object has moved since the last frame
        Vector3 movementDirection = transform.position - lastPosition;
        this.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
        // Check which direction is increasing and rotate accordingly
        if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.y) && Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z))
        {
            Target1.SetActive(true);
            Target2.SetActive(false);
            Target3.SetActive(false);
            // Moving mainly along the X-axis
            if (movementDirection.x > 0)
            {
                RotateTowards(Vector3.right);  // Moving right
            }
            else
            {
                RotateTowards(Vector3.left);   // Moving left
            }
        }
        else if (Mathf.Abs(movementDirection.y) > Mathf.Abs(movementDirection.x) && Mathf.Abs(movementDirection.y) > Mathf.Abs(movementDirection.z))
        {
            Target1.SetActive(false);
            Target2.SetActive(true);
            Target3.SetActive(false);
            // Moving mainly along the Y-axis
            if (movementDirection.y > 0)
            {
                RotateTowards(Vector3.up);    // Moving up
            }
            else
            {
                RotateTowards(Vector3.down);  // Moving down
            }
        }
        else if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.y))
        {
            Target1.SetActive(false);
            Target2.SetActive(false);
            Target3.SetActive(true);
            // Moving mainly along the Z-axis
            if (movementDirection.z > 0)
            {
                RotateTowards(Vector3.forward);  // Moving forward
            }
            else
            {
                RotateTowards(Vector3.back);     // Moving backward
            }
        }

        // Update the last position for the next frame
        lastPosition = transform.position;
    }

    void RotateTowards(Vector3 direction)
    {
        // Smoothly rotate the object towards the given direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}