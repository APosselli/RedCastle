using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    

    private Rigidbody rb;
    [Header("Movement Settings")]
    [Tooltip("The max speed the player can travel.")]
    public float maxVelocity = 10f;
    [Tooltip("The force applied to the player when a directional key is pressed. Affects acceleration.")]
    public float movementForce = 20f;
    [Tooltip("The upwards force applied to the player character when they jump.")]
    public float jumpForce = 250f;
    [Tooltip("The force applied to the player to make them come to a stop more quickly. Affects how quickly the player comes to a stop.")]
    public float brakeForce = 4f;
    [Tooltip("How slow the player must be moving before braking stops.")]
    public float brakingVelocity = 2f;
    [Tooltip("The fraction of the sideways force that will be applied when moving the player character in midair.")]
    public float sidewaysAerialForce = 0.01f;
    [Tooltip("The fraction of the front/back force that will be applied when moving the player character in midair.")]
    public float straightAerialFore = 0.02f;

    private bool canJump = false;
    private bool wallJump = false;
    private bool clinging = false;
    private Vector3 wallJumpVector;
    private Vector3 relativePosition;
    private GameObject clingSurface;
    private Vector3 clingNormal;

    //private bool collidingWithWall;
    //private GameObject collidingWall;

    private float playerColliderHeight;
    private float playerColliderRadius;

    // After a wall jump, the player character will be able to rotate for a brief period of time. As a result, we also disable
    // midair player movement during this time, though this wouldn't be noticable, since the player input force would be almost
    // negligible when compared to the wall jump force.
    private float wallJumpTimer = 0.0f;

    //private Rigidbody childRb;
    private Vector3 oldEuler;
    //private Quaternion oldRotation;

    public GameObject greenModel;

    //nicks variables:
    public bool StopAllMovement;
    public GameObject CameraReference;

    //forces that get set
    private Vector3 previousForce;

    private Vector3 JumpDirection;

    //animation:
    public Animator PlayerAnimator;
    bool JumpForAnimation;
    float AngleDifference; // for running_left and running_right
    float zrotation;

    bool CF_Running;
    bool CF_Jumping;
    bool CF_Mid_Jump;
    bool CF_Fall;
    bool CF_Standing;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerColliderHeight = transform.gameObject.GetComponent<CapsuleCollider>().height;
        playerColliderRadius = transform.gameObject.GetComponent<CapsuleCollider>().radius;
        wallJumpVector = new Vector3();
        oldEuler = rb.rotation.eulerAngles;
        JumpDirection = new Vector3();
        //oldRotation = rb.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Interaction();

        //if(!StopAllMovement)
        Movement();
    }

    void Movement()
    {
        SetCling();

        transform.position = transform.position;
        Vector3 force = new Vector3();

        // Get input for movement on the x, y plane. Apply it to the force.
        if (Input.GetKey(KeyCode.W))
        {
            force += CameraReference.transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            force += -CameraReference.transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            force += -CameraReference.transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            force += CameraReference.transform.right;
        }

        // act like camera is always facing horizontal, to prevent movement towards ground/air
        force = new Vector3(force.x, force.y * 0, force.z).normalized;

        //snap or regular rotation
        Vector3 planarVelocity = rb.velocity;
        planarVelocity.y = 0;
        //snap rotation at move start
        if (previousForce == Vector3.zero && planarVelocity.magnitude <= 0.01f && canJump && force.magnitude > 0f)// || Physics.Raycast(rb.transform.position, rb.transform.forward, playerColliderRadius + 0.1f))
        {
            //RotateTowards(force);
            Debug.Log("snap move");
            transform.LookAt(transform.position + force);
        }
        else //regular rotation
        {
            if (planarVelocity.magnitude > 0.05f && (canJump || wallJumpTimer > 0.0f))
                SetRotation();
        }

        //Debug.Log("force " + force);
        // movement against a wall
        RaycastHit WallMovementCastHit;
        if (Physics.Raycast(rb.transform.position, force, out WallMovementCastHit, playerColliderRadius + .5f))
        {

            force = Vector3.ProjectOnPlane(force, WallMovementCastHit.normal);
            Debug.Log("wall move" + force);
        }

        // Make the player come to a stop more quickly.
        Vector3 playerVelocity = rb.velocity;
        playerVelocity.y = 0;
        
        if (force == Vector3.zero && playerVelocity.magnitude > brakingVelocity && canJump)
        {
            force = -playerVelocity * brakeForce;
        }

        // Normalize the force for movement on the x, y plane, so moving diagonally doesn't make the player faster.
        // Multiply by the movement force we can change how fast the player speeds up.
        force = force.normalized * movementForce;

        // Lessen movement in midair.
        if (!canJump)
        {
            Vector3 forwardForce = Vector3.Dot(force, transform.forward) * transform.forward * straightAerialFore;
            Vector3 sidewaysForce = Vector3.Dot(force, transform.right) * transform.right * sidewaysAerialForce;
            force = forwardForce + sidewaysForce;

            Debug.Log("force 1 " + force);
            float fm = Mathf.Min(force.magnitude, straightAerialFore);
            force = force.normalized * fm;
            Debug.Log("force 2 " + force);

            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0.0f)
                wallJumpTimer = 0.0f;
        }



        if (wallJumpTimer > 0.0f)
        {
            force = Vector3.zero;
        }

        if(clinging)
        {
            rb.velocity = Vector3.zero;
            rb.transform.position = clingSurface.transform.position - relativePosition;
            transform.LookAt(transform.position - clingNormal);
        }

        // Jump Logic:
        JumpForAnimation = false;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump || clinging)
            {
                clinging = false;

                force += Vector3.up * jumpForce;

                JumpForAnimation = true;

                if(clinging)
                {
                    Rigidbody clingRb = clingSurface.GetComponent<Rigidbody>();
                    if(clingRb != null)
                    {
                        rb.velocity = clingRb.velocity;
                    }
                }

                JumpDirection = rb.velocity;
            }
            else if (wallJump)
            {
                force += wallJumpVector;

                wallJumpTimer = 0.4f;

                JumpForAnimation = true;

                JumpDirection = rb.velocity;
            }
        }

        if (StopAllMovement || clinging)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            // Add final force
            rb.AddForce(force);
            // Limit the player speed.
            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
        }

        previousForce = force;



        RaycastHit ModelGroundCastHit;
        if(IsGrounded() && Physics.Raycast(transform.position, -Vector3.up, out ModelGroundCastHit)){

            //GroundNormal = ModelGroundCastHit.normal;
            //Quaternion newRotation = new Quaternion();
            //newRotation.SetLookRotation(transform.forward, ModelGroundCastHit.normal);
            //greenModel.transform.rotation = newRotation;


            //greenModel.transform.LookAt(greenModel.transform.position + transform.forward);
            greenModel.transform.up = ModelGroundCastHit.normal;
            greenModel.transform.RotateAround(greenModel.transform.position, greenModel.transform.up, transform.rotation.eulerAngles.y);


            //GroundZ = greenModel.transform.rotation.eulerAngles.z;
            //Debug.Log("ModelGroundCastHit.normal" + ModelGroundCastHit.normal);
            //Debug.Log("greenModel.transform.up" + greenModel.transform.up);
        }
        //greenModel.transform.rotation = transform.rotation;


        // animation
        if (PlayerAnimator)
        {
            if (JumpForAnimation && !StopAllMovement) //jump
            {
                greenModel.transform.rotation = Quaternion.Euler(greenModel.transform.rotation.eulerAngles.x, greenModel.transform.rotation.eulerAngles.y, 0f);

                if (!PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("jumping")) // && !CF_stand && !P_stand)
                {
                    PlayerAnimator.SetTrigger("Jumping");
                    //Debug.Log("Jumping");
                    CF_Jumping = true;
                }

                CF_Running = false;
                CF_Mid_Jump = false;
                CF_Fall = false;
                CF_Standing = false;
            }
            else if(!IsGrounded() && rb.velocity.y <= 0 && !StopAllMovement) //fall
            {
                if (!wallJump)
                {
                    if (!PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("fall") && !CF_Fall)
                    {
                        PlayerAnimator.SetTrigger("Fall");
                        //Debug.Log("Fall");

                        CF_Mid_Jump = false;
                        CF_Fall = true;
                    }
                }
                else
                {
                    if (!PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("mid_jump") && !CF_Mid_Jump) // wall slide
                    {
                        PlayerAnimator.SetTrigger("Mid_Jump");
                        //Debug.Log("Mid_Jump");

                        CF_Mid_Jump = true;
                        CF_Fall = false;
                    }
                }

                CF_Running = false;
                CF_Jumping = false;
                CF_Standing = false;
            }
            else if (force != Vector3.zero && IsGrounded() && !CF_Jumping && !StopAllMovement) // run
            {


                if (!PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("running") && !CF_Running) // && !CF_stand && !P_stand)
                {
                    PlayerAnimator.SetTrigger("Running");
                    //Debug.Log("Running");
                    CF_Running = true;
                }

                //if(AngleDifference > 0.01f && AngleDifference < 5f || AngleDifference < -5f)
                //{
                //    rb.transform.rotation = Quaternion.Euler(rb.transform.rotation.eulerAngles.x, rb.transform.rotation.eulerAngles.y, 15);
                //}
                //else if (AngleDifference < -0.01f && AngleDifference > -5f || AngleDifference > 5f)
                //{
                //    rb.transform.rotation = Quaternion.Euler(rb.transform.rotation.eulerAngles.x, rb.transform.rotation.eulerAngles.y,  - 15);
                //}
                //else
                //{
                //    rb.transform.rotation = Quaternion.Euler(rb.transform.rotation.eulerAngles.x, rb.transform.rotation.eulerAngles.y, rb.transform.rotation.eulerAngles.z);
                //}

                //float zrotation = 0;
                if (AngleDifference != 0)
                {
                    float newzrotation = Mathf.Min(Mathf.Abs(AngleDifference) * 100, 30) * (AngleDifference / Mathf.Abs(AngleDifference));
                    zrotation = Mathf.Lerp(zrotation, newzrotation, Time.deltaTime * 20);
                } 
                //Debug.Log(zrotation);
                greenModel.transform.rotation = Quaternion.Euler(greenModel.transform.rotation.eulerAngles.x, greenModel.transform.rotation.eulerAngles.y, zrotation);

                CF_Jumping = false;
                CF_Mid_Jump = false;
                CF_Fall = false;
                CF_Standing = false;
            }
            else if (force == Vector3.zero && playerVelocity.magnitude < brakingVelocity && IsGrounded() && !CF_Jumping || StopAllMovement) // stand
            {

                if (!PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("standing") && !CF_Standing)
                {
                    PlayerAnimator.SetTrigger("Standing");
                    //Debug.Log("Standing");
                    CF_Standing = true;
                }

                CF_Running = false;
                CF_Jumping = false;
                CF_Mid_Jump = false;
                CF_Fall = false;
                
            }
        }

        //if (planarVelocity.magnitude <= 0.5f)
        //    greenModel.transform.rotation = Quaternion.RotateTowards(greenModel.transform.rotation, rb.transform.rotation, 100f * Time.deltaTime);

    }

    private void SetRotation()
    {
        Quaternion previousRotation = rb.transform.rotation;
        Vector3 planarVelocity = rb.velocity;
        planarVelocity.y = 0;
        Quaternion fullRotation = Quaternion.LookRotation(planarVelocity, Vector3.up);
        rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, fullRotation, 500f * Time.deltaTime);


        //Animation logic for running_left and running_right
        
        Vector3 PreviousVector = previousRotation * Vector3.forward;
        Vector3 NewVector = rb.transform.rotation * Vector3.forward;

        float PreviousAngle = Mathf.Atan2(PreviousVector.x, PreviousVector.z); //  Mathf.Rad2Deg;
        float NewAngle = Mathf.Atan2(NewVector.x, NewVector.z);

        AngleDifference = Mathf.DeltaAngle(PreviousAngle, NewAngle);

        //Debug.Log("AngleDifference " + AngleDifference);

        //oldRotation = rb.transform.rotation;

    }

    private void RotateTowards(Vector3 direction)
    {
        Quaternion previousRotation = rb.transform.rotation;
        Vector3 planarVelocity = direction;
        planarVelocity.y = 0;
        Quaternion fullRotation = Quaternion.LookRotation(planarVelocity, Vector3.up);
        rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, fullRotation, 250f * Time.deltaTime);


        //Animation logic for running_left and running_right

        Vector3 PreviousVector = previousRotation * Vector3.forward;
        Vector3 NewVector = rb.transform.rotation * Vector3.forward;

        float PreviousAngle = Mathf.Atan2(PreviousVector.x, PreviousVector.z); //  Mathf.Rad2Deg;
        float NewAngle = Mathf.Atan2(NewVector.x, NewVector.z);

        AngleDifference = Mathf.DeltaAngle(PreviousAngle, NewAngle);
    }

    private bool IsGrounded()
    {
        //if(Physics.Raycast(transform.position, Vector3.down, (playerColliderHeight / 2) * 1.05f))
            //Debug.Log("Print Grounded " + Time.time);

        //return Physics.Raycast(transform.position, Vector3.down, (playerColliderHeight / 2) * 1.15f);
        return Physics.Raycast(transform.position, Vector3.down, (playerColliderHeight / 2) * 1.05f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //theres an error if you dont check rb.
        if (rb)
        {
            SetJump(collision);
            //if (!IsGrounded())
            //{
            //    Vector3 newVelocity = rb.velocity;
            //    newVelocity.x = 0;
            //    newVelocity.z = 0;
            //    rb.velocity = newVelocity;
            //}
            //collidingWithWall = true;
            //collidingWall = collision.gameObject;
        }
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (rb)
        {
            SetJump(collision);
            

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (rb)
        {
            canJump = false;
            wallJump = false;
            //collidingWithWall = false;
        }
           
    }

    private void SetJump(Collision collision)
    {
        ContactPoint highestContactPoint = collision.contacts[0];
        //Debug.Log(highestContactPoint);
        foreach (ContactPoint point in collision.contacts)
        {
            if (point.point.y > highestContactPoint.point.y)
            {
                highestContactPoint = point;
            }
        }

        if (highestContactPoint.point.y <= (rb.transform.position.y - ((playerColliderHeight / 2) * 0.9f)) || IsGrounded())
        {
            wallJump = false;
            canJump = true;
            wallJumpTimer = 0.0f;

            }
        else
        {
            wallJump = true;
            canJump = false;

            wallJumpVector = highestContactPoint.normal;
            wallJumpVector.y = 0;

            RaycastHit hit;
            Vector3 playerPlanarVector = transform.forward;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if(hit.transform.gameObject == collision.gameObject)
                {
                    playerPlanarVector = -transform.forward; // if player is actually looking at the wall mirror transform
                }
            }

            playerPlanarVector.y = 0;
            wallJumpVector.Normalize();
            wallJumpVector = playerPlanarVector - 2 * (playerPlanarVector - (Vector3.Dot(playerPlanarVector, wallJumpVector) * wallJumpVector));
            wallJumpVector.Normalize();
            wallJumpVector = wallJumpVector * jumpForce;
            wallJumpVector.y = jumpForce;
        }
    }

    private void SetCling()
    {
        // Replace hard-coded distance
        RaycastHit hit;
        if(rb.velocity.y < 0f && !IsGrounded() && Physics.Raycast(rb.transform.position, rb.transform.forward, out hit, playerColliderRadius + 0.1f) && 
            !Physics.Raycast(rb.transform.position + Vector3.up * (playerColliderHeight + 0.1f), rb.transform.forward, playerColliderRadius + 0.1f))
        {
            
            clingSurface = hit.collider.gameObject;

            if (!clinging)
            {
                relativePosition = clingSurface.transform.position - rb.transform.position;
            }
            clinging = true;
            clingNormal = hit.normal;
        }
    }

    void Interaction()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //BDebug.Log("debug box");
            DrawBoxCastBox(transform.position, new Vector3(.75f, .75f, 0f), transform.rotation, transform.forward, 1.5f, Color.red);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            RaycastHit HitInfo;
            int Layer = 1 << LayerMask.NameToLayer("InteractiveObject");
            //Debug.DrawLine (transform.position, transform.forward * 10f, Color.red, 10f);
            
            if (Physics.BoxCast(transform.position, new Vector3(.75f, .75f, 0f), transform.forward, out HitInfo, transform.rotation, 1.5f, Layer))
            {
                //Debug.Log("Hit object");
                if (HitInfo.transform.GetComponent<InteractiveObjectBase>() != null)
                {

                    HitInfo.transform.GetComponent<InteractiveObjectBase>().StartSwitch(0);
                    if (HitInfo.transform.GetComponent<InteractiveObjectBase>().StopPlayerMovement)
                    {
                        StopAllMovement = true;
                    }
                }
            }
            //if (Physics.Raycast(transform.position, transform.forward, out HitInfo, 1.5f, Layer))
            //{
            //    Debug.Log("Hit object");
            //    if (HitInfo.transform.GetComponent<InteractiveObjectBase>() != null)
            //    {

            //        HitInfo.transform.GetComponent<InteractiveObjectBase>().StartSwitch(0);
            //        if (HitInfo.transform.GetComponent<InteractiveObjectBase>().StopPlayerMovement)
            //        {
            //            StopAllMovement = true;
            //        }
            //    }
            //}
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {

            RaycastHit HitInfo;
            int Layer = 1 << LayerMask.NameToLayer("InteractiveObject");
            //Debug.DrawLine (transform.position, transform.forward * 10f, Color.red, 10f);
            //Debug.Log("attempt raycast");
            if (Physics.BoxCast(transform.position, new Vector3(.75f, .75f, 0f), transform.forward, out HitInfo, transform.rotation,  1.5f, Layer))
            {
                //Debug.Log("Hit object");
                if (HitInfo.transform.GetComponent<InteractiveObjectBase>() != null)
                {

                    HitInfo.transform.GetComponent<InteractiveObjectBase>().StopSwitch(0);

                }
            }
            //if (Physics.Raycast(transform.position, transform.forward, out HitInfo, 10f, Layer))
            //{
            //    //Debug.Log("Hit object");
            //    if (HitInfo.transform.GetComponent<InteractiveObjectBase>() != null)
            //    {

            //        HitInfo.transform.GetComponent<InteractiveObjectBase>().StopSwitch(0);

            //    }
            //}

            StopAllMovement = false;
        }

    }

    public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
    {
        origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
        DrawBox(origin, halfExtents, orientation, color);
    }

    //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
    public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
    {
        direction.Normalize();
        Box bottomBox = new Box(origin, halfExtents, orientation);
        Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

        Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
        Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
        Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
        Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
        Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
        Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
        Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
        Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

        DrawBox(bottomBox, color);
        DrawBox(topBox, color);
    }

    public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
    {
        DrawBox(new Box(origin, halfExtents, orientation), color);
    }
    public static void DrawBox(Box box, Color color)
    {
        Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
        Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
        Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
        Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

        Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
        Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
        Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
        Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

        Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
        Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
        Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
        Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
    }

    public struct Box
    {
        public Vector3 localFrontTopLeft { get; private set; }
        public Vector3 localFrontTopRight { get; private set; }
        public Vector3 localFrontBottomLeft { get; private set; }
        public Vector3 localFrontBottomRight { get; private set; }
        public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
        public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
        public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
        public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

        public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
        public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
        public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
        public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
        public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
        public Vector3 backTopRight { get { return localBackTopRight + origin; } }
        public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
        public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

        public Vector3 origin { get; private set; }

        public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
        {
            Rotate(orientation);
        }
        public Box(Vector3 origin, Vector3 halfExtents)
        {
            this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

            this.origin = origin;
        }


        public void Rotate(Quaternion orientation)
        {
            localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
            localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
            localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
            localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
        }
    }

    //This should work for all cast types
    static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
    {
        return origin + (direction.normalized * hitInfoDistance);
    }

    static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        Vector3 direction = point - pivot;
        return pivot + rotation * direction;
    }

}