// MoveTo.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveTo : MonoBehaviour
{

    public Queue<Vector3> Targets = new Queue<Vector3>();
    private Vector3 CurrentGoal;
    bool Obstacle;


    UnityEngine.AI.NavMeshAgent agent;
    private Waypoint tempTarget;
    private bool isJumping;
    private Rigidbody rb;

    void Start()
    {
        Targets.Enqueue(new Vector3(-20.6f, 1.083333f, 14.05f));
        Targets.Enqueue(new Vector3(18.4f, 1.083333f, 14.05f));

        isJumping = false;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.enabled = false;

        rb = GetComponent<Rigidbody>();
        
    }

    void Update()
    {
        Debug.Log("current target: " + CurrentGoal);
        Debug.Log("Distance: " + Vector3.Distance(rb.transform.position, CurrentGoal));
        CheckJump();
        CheckObstacle();
        UpdateWaypoints();
       
    }

    private void UpdateWaypoints()
    {
        if (!Obstacle)
        {
            if (Vector3.Distance(rb.transform.position, CurrentGoal) < 0.2f && Targets.Count > 0)
            {
                Targets.Dequeue();
            }
            if (Targets.Count > 0)
            {
                CurrentGoal = Targets.Peek();
            }
        }

        if (agent.enabled)
        {
            agent.destination = CurrentGoal;
        }
    }

    private void CheckObstacle()
    {
        if(Physics.Raycast(transform.position, transform.forward, 1f))
        {
            Obstacle = true;
            CurrentGoal = transform.position;
        }
        else
        {
            Obstacle = false;
            if (Targets.Count > 0)
            {
                CurrentGoal = Targets.Peek();
            }
        }
    }

    private void CheckJump()
    {
        bool rayCastHit = Physics.Raycast(transform.position + Vector3.up * 0.5f + transform.forward * 1f, Vector3.down, 1.10f);

        Debug.DrawRay(transform.position + Vector3.up * 0.5f + transform.forward * 1f, Vector3.down * 1.10f, Color.red);

        Vector3 planarDirectionToGoal = CurrentGoal - transform.position;
        Vector3 planarGoal = CurrentGoal;
        planarGoal.y = transform.position.y;
        planarDirectionToGoal.y = 0;
        planarDirectionToGoal.Normalize();

        // jump start
        if (!rayCastHit && !isJumping)
        {
            transform.LookAt(planarGoal);
            isJumping = true;
            agent.enabled = false;

            //if (transform.forward == planarDirectionToGoal)


            if (Physics.Raycast(transform.position + Vector3.up * 0.5f + transform.forward, Vector3.down))
            {
                Debug.DrawRay(transform.position + Vector3.up * 0.5f + transform.forward, Vector3.down * 100.0f, Color.blue, 10.0f);
                Debug.Log("fall");
            }
            else
            {
                rb.velocity = Vector3.zero;
                rb.AddForce((Vector3.up) * 300); //make 300 a variable jump height
                Debug.Log("jump");
            }

        }
        // Not jumping
        else if(rayCastHit)
        {
            //Debug.Log("not moving in jump");
            agent.enabled = true;
            isJumping = false;
        }
        // jumping
        else if(isJumping)
        {
            //Debug.Log("moving in jump");
            transform.position += (transform.forward * 5f * Time.deltaTime); // make 5 a variable horizontal air speed
            RotateToGoal();
        }

    }

    private void RotateToGoal()
    {
        Vector3 planarDirectionToGoal = new Vector3(CurrentGoal.x, transform.position.y, CurrentGoal.z) - transform.position;
        Quaternion fullRotation = Quaternion.LookRotation(planarDirectionToGoal, Vector3.up);
        rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, fullRotation, 400f * Time.deltaTime);
    }
}