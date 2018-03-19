// MoveTo.cs
using UnityEngine;
using System.Collections;

public class MoveTo : MonoBehaviour
{

    public Waypoint target;
    public Vector3 goal;
    UnityEngine.AI.NavMeshAgent agent;
    private Waypoint tempTarget;
    private bool isJumping;
    private Rigidbody rb;

    void Start()
    {
        isJumping = false;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        agent.destination = goal;
        tempTarget = target;
        agent.enabled = false;
    }

    void Update()
    {
        CheckObstacle();
        CheckJump();

        if (target != null)
        {
            goal = target.position;
        }
        if (agent.enabled)
        {
            agent.destination = goal;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Waypoint"))
        {
            if (target != null)
            {
                target = target.target;
                tempTarget = target;
            }
        }
    }

    private void CheckObstacle()
    {
        if(Physics.Raycast(transform.position, transform.forward, 1f))
        {
            if(target != null)
            {
                tempTarget = target;
                target = null;
                goal = transform.position;
            }
        }
        else
        {
            target = tempTarget;
        }
    }

    private void CheckJump()
    {
        bool rayCastHit = Physics.Raycast(transform.position + Vector3.up * 0.5f + transform.forward * 1f, Vector3.down, 1.10f);

        Debug.DrawRay(transform.position + Vector3.up * 0.5f + transform.forward * 1f, Vector3.down * 1.10f, Color.red);

        Vector3 planarDirectionToGoal = goal - transform.position;
        Vector3 planarGoal = goal;
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
                rb.AddForce((Vector3.up) * 300); //make 300 a variable jump height
                Debug.Log("jump");
            }

        }
        // pre jump start
        else if(rayCastHit)
        {
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
        Vector3 planarDirectionToGoal = new Vector3(goal.x, transform.position.y, goal.z) - transform.position;
        Quaternion fullRotation = Quaternion.LookRotation(planarDirectionToGoal, Vector3.up);
        rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, fullRotation, 400f * Time.deltaTime);
    }
}