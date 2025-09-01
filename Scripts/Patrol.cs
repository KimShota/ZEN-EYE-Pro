using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;


public class Patrol : MonoBehaviour
{
    public Animator animator;
    public Transform patrolPoint;
    

    public float movementSpeed;
    public float rotationSpeed;
    

    void Start()
    {
        animator.SetBool("Moving", false);
    }

    void Update()
    {
        // Keep Y position unchanged and look only in XZ plane
        Vector3 targetPosition = new Vector3(patrolPoint.position.x, transform.position.y, patrolPoint.position.z);

        // Smoothly rotate toward the patrol point without changing Y rotation
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Move toward patrol point on XZ only
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        // Update animator
        if (transform.position != targetPosition)
        {
            animator.SetBool("Moving", true);
            
        }
        else
        {
            animator.SetBool("Moving", false);
        }
        
      
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "TriggerArea")
        {
            Debug.Log("Entered TriggerArea!");
            animator.SetTrigger("Bow");
            
            animator.SetBool("DanceTrigger", true);
            
        }

        if (other.gameObject.name == "OrderTrigger")
        {
            Debug.Log("Entered TriggerArea!");
            animator.SetTrigger("Bow");

            animator.SetBool("OrderTrigger", true);
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "TriggerArea")
        {
            Debug.Log("Exited TriggerArea!");
            animator.SetBool("DanceTrigger", false);
        }   
        if (other.gameObject.name == "OrderTrigger")
        {
            Debug.Log("Exited TriggerArea!");
            animator.SetBool("OrderTrigger", false);
            
        }

    }
}