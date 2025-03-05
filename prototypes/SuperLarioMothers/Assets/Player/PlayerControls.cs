using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;


public class PlayerControls : MonoBehaviour
{

    /*
     * THINGS I HAVE RIGHT NOW:
     * 
     * - Dash (cooldown on floor, one time when air)
     * - Double jump
     * - Jump Buffer
     * - Coyote Time
     * - Cam follows when you land (and some other conditions, its in cameraFollow.cs
     * - if you jump right at the end of a dash, youll do a super jump. so, you can use this to reach further areas. its like mario dive yfeel me
     * 
     * THINGS I WANT:
     * - DONE - make the super jump be possible when you press right before dash ends. u can use the dash timer.
     * - clean this code up. single responsibility principle and allat
     * - DONE - if you jump during a dash, it cancels the dash. this is so you cant just spam the super jump
     * - DONE - make the super jump rise more than normal but move slower yfeel me
     * - DONE make something under the guy so you can see where youd land
     * - DONE - make a trail that lets u know when the dash jump thing is
     */


    Vector2 moveInput;
    InputAction jumpAction;
    InputAction dash;

    public TrailRenderer dashTrail;

    public CharacterController cc;
    public Transform cameraTransform;
    public cameraFollow cameraFollow;
    public GameObject shadowObj;
    public float shadowDistance = 10f;

    bool isDashJumpRunning = false;

    public int coins = 0;

    public bool isMoving = false;

    float timeH = 0f;
    public bool recoverH = true;

    public float moveSpeed = 13f;
    float jumpVelocity;

    float yVelocity = 0;
    float gravity;

    //if you press jump before u land, it'll make u jump when u touch ground
    float fallingTime = 0;

    float maxJumpTime = .60f;
    float maxJumpHeight = 3.0f;
    bool calcFallTime = false;
    float otherfalltime = 0f;

    int jumpCount = 0;

    //dash
    float dashAmount = 16;
    float dashVelocity = 0;
    float dashTimer = 0;
    float dashLength = .3f;
    int dashCount = 0;
    int groundDashCount = 0;
    bool isDashing = false;
    bool canDash = true;
    int maxDashes = 1;
    int maxJumps = 1;

    bool movementLocked = false;
    bool doDashJump = false;
    bool doingDashJump = false;
    float moveSpeedCoefficient = 0.8f;
    float gravityCoefficient = 1f;
    float jumpVelocityCoefficient = 0.7f;

    bool dashPickedUp = false;

    GameObject movingPlatform;
    Vector3 previousMovingPlatformPosition;

    Vector3 currentCheckpoint;

    bool dead = false;

    private void OnEnable()
    {
    }


    // Start is called before the first frame update
    void Start()
    {

        dashTrail.emitting = false;

        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        jumpVelocity = (2 * maxJumpHeight) / timeToApex;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        //SwitchCamera(CameraStyle.Open);

        jumpAction = InputSystem.actions.FindAction("Jump");
        dash = InputSystem.actions.FindAction("Dash");


    }

    // Update is called once per frame
    void Update()
    {
        PlaceShadowUnder();

        recoverH = true;
        timeH -= Time.deltaTime;

        bool dashedThisTurn = false;


        moveInput = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        float hAxis = moveInput.x;
        float vAxis = moveInput.y;

        if (dashTimer <= 0.1f && isDashing && dashTimer > 0 && dashPickedUp) {
            if (!isDashJumpRunning)
            {
                isDashJumpRunning = true;
                StartCoroutine(dashJumpWindow());
                movementLocked = false;
            }
        }

        if (dashTimer == 0 /*|| (isDashing && Input.GetKeyUp(KeyCode.LeftShift))*/)
        {
            isDashing = false;
            dashVelocity = 0;
            if (!dashPickedUp) {
                dashTrail.material.color = Color.white;
                dashTrail.emitting = false;
            }
        }
        if (dash.WasPressedThisFrame() && !isDashing && dashCount < maxDashes && groundDashCount < maxDashes && canDash)
        {
            if (maxDashes != 2)
            {
                dashedThisTurn = true;
            }
            else if (groundDashCount == 1)
            {
                dashedThisTurn = true;
            }
            dashTrail.emitting = true;
            movementLocked = true;
            isDashing = true;
            dashVelocity = dashAmount;
            yVelocity = 0;
            dashTimer = dashLength;
            if (cc.isGrounded)
            {
                groundDashCount++;
            }
            else
            {
                dashCount++;
            }
        }
        dashTimer -= Time.deltaTime;
        dashTimer = Mathf.Clamp(dashTimer, 0, 10000);

        if (!cc.isGrounded)
        {

            //Debug.Log("in the air for some reason");

            // *** If we are in here, we are IN THE AIR ***

            otherfalltime += Time.deltaTime;
            if (jumpAction.WasPressedThisFrame() && !doDashJump && jumpCount < maxJumps)
            {
                if (isDashing) {
                    dashTimer = 0;
                    if (!doingDashJump && !doDashJump && !isDashJumpRunning)
                    {
                        dashTrail.material.color = Color.white;
                        dashTrail.emitting = false;
                    }
                }
                cameraFollow.jumpedInAir();
                yVelocity = jumpVelocity;
                jumpCount++;
            }


            //coyote time
            if (otherfalltime < .25f && !isDashing && jumpCount == 0 && !doDashJump && (jumpAction.WasPressedThisFrame()))
            {
                yVelocity = jumpVelocity;
                jumpCount++;
            }

            //start jump buffer
            if (jumpAction.WasPressedThisFrame() && !doDashJump && yVelocity < 0.0f && !isDashing)  
            {

                calcFallTime = true;
            }

            if (calcFallTime)
            {

                fallingTime += Time.deltaTime;

            }

            if (!isDashing)
            {
                if (yVelocity > 0.0f)
                {
                    yVelocity += gravity * Time.deltaTime;
                }
                else if (yVelocity <= 0.0f)
                {
                    yVelocity += gravity * 2.0f * Time.deltaTime;
                }
            }

            //if (Input.GetKeyUp(KeyCode.Space) && yVelocity > 0) { yVelocity = 0.0f; }


        }
        else if (cc.isGrounded)
        {
            //RidOfShadow();

            finishDashJump();
            //if (!isDashing && isDashJumpRunning) {
            //    dashTrail.material.color = Color.white;
            //    //dashTrail.emitting = false;
            //}

            otherfalltime = 0f;
            dashCount = 0;


            yVelocity = -2;

            cameraFollow.landed();

            jumpCount = 0;

            //this is to add a delay when trying to dash on the ground
            if (dashedThisTurn)
            {
                StartCoroutine(DisableDash());
            }


            if ((fallingTime < .2f) && calcFallTime)
            {
                cameraFollow.jumped();
                jumpCount++;
                yVelocity = jumpVelocity;
            }
            calcFallTime = false;
            fallingTime = 0;

            // Jump!

            if (jumpAction.WasPressedThisFrame() && !doDashJump)
            {
                if (isDashing)
                {
                    dashTimer = 0;
                    if (!doingDashJump && !doDashJump && !isDashJumpRunning) {
                        dashTrail.material.color = Color.white;
                        dashTrail.emitting = false;
                    }
                }
                cameraFollow.jumped();
                groundDashCount = 0;
                jumpCount++;
                yVelocity = jumpVelocity;
            }
            

        }

        DashJump();




        Vector3 amountToMove = new Vector3(hAxis, 0, vAxis) * moveSpeed;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        Vector3 forwardRelative = amountToMove.z * camForward;
        Vector3 rightRelative = amountToMove.x * camRight;

        Vector3 moveDir = forwardRelative + rightRelative;

        amountToMove = new Vector3(moveDir.x, 0, moveDir.z);

        if (amountToMove != Vector3.zero)
        {
            amountToMove += amountToMove.normalized * dashVelocity;
        }
        else
        {
            amountToMove += transform.forward * dashVelocity;
        }
        if (!isDashing)
        {
            amountToMove.y += yVelocity;


        }

        amountToMove *= Time.deltaTime;

        isMoving = amountToMove != Vector3.zero;



        //animator.SetBool("IsRunning", hAxis != 0 || vAxis != 0);
        //animator.SetBool("IsIdle", hAxis == 0 && vAxis == 0);
        //bool a = animator.GetBool("IsIdle");
        //bool b = animator.GetBool("IsRunning");

        bool shouldRotate = true;
        if (movingPlatform != null)
        {
            if (amountToMove.x == 0 && amountToMove.z == 0)
            {
                shouldRotate = false;
            }
            Vector3 amountPlatformMoved = movingPlatform.transform.position - previousMovingPlatformPosition;
            amountToMove += amountPlatformMoved;
            previousMovingPlatformPosition = movingPlatform.transform.position;
        }







        if (true /*!movementLocked*/)
        {
            cc.Move(amountToMove);

            if (dead)
            {
                cc.enabled = false;
                transform.position = currentCheckpoint;
                dead = false;
                cc.enabled = true;
            }

        }


        Vector3 rotate = amountToMove;
        rotate.y = 0;
        if (rotate != Vector3.zero && shouldRotate)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotate.normalized), 5f * Time.deltaTime);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MovingPlatform"))
        {
            movingPlatform = other.gameObject;
            previousMovingPlatformPosition = movingPlatform.transform.position;
        }
        if (other.CompareTag("DashPickup"))
        {
            dashPickedUp = true;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("JumpPickup"))
        {
            maxJumps = 2;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Kill"))
        {
            die();
        }
        if (other.CompareTag("Collectible")) {
            coins += 1;
            Destroy(other.gameObject);
        
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MovingPlatform"))
        {
            movingPlatform = null;
        }
    }

    void finishDashJump() {
        if (doingDashJump)
        {
            moveSpeed = moveSpeed / moveSpeedCoefficient;
            doingDashJump = false;
            gravity = gravity * gravityCoefficient;
            jumpVelocity *= jumpVelocityCoefficient;
            dashTrail.material.color = Color.white;
            dashTrail.emitting = false;
        }
    }

    void DashJump() {
        if (doDashJump)
        {
            cameraFollow.jumpedInAir();
            jumpVelocity = jumpVelocity / jumpVelocityCoefficient;
            yVelocity = jumpVelocity;
            movementLocked = true;
            moveSpeed = moveSpeed * moveSpeedCoefficient;
            doDashJump = false;
            doingDashJump = true;
            gravity = gravity / gravityCoefficient;
            dashCount = maxDashes;
        }
    }


    IEnumerator dashJumpWindow() {
        float timeElapsed = 0f;
        dashTrail.material.color = Color.red;
        while (timeElapsed < 0.2f)
        {
            if (jumpAction.WasPressedThisFrame())
            {

                doDashJump = true;
                dashTimer = 0;
                dashVelocity = 0f;
                isDashJumpRunning = false;
                yield break; 
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        dashTrail.material.color = Color.white;
        dashTrail.emitting = false;
        isDashJumpRunning = false;
    }
  
    IEnumerator DisableDash()
    {
        canDash = false;
        yield return new WaitForSeconds(0.5f);
        canDash = true;
        groundDashCount = 0;
    }

    void PlaceShadowUnder()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shadowDistance))
        {
            shadowObj.gameObject.SetActive(true);


            float distance = Vector3.Distance(transform.position, hit.point);
            float sizePercentage = 1 - (distance / shadowDistance);
            shadowObj.transform.localScale = new Vector3(sizePercentage, sizePercentage, sizePercentage);
            shadowObj.transform.position = hit.point;
        }
        else {
            shadowObj.gameObject.SetActive(false);
        }
    }

    void RidOfShadow() { 
        shadowObj.gameObject.SetActive(false);
    }

    public void setCheckpoint(Vector3 point) {
        currentCheckpoint = point;
    }

    public void die() {
        dead = true;
    }



    //void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if (hit.gameObject.CompareTag("MovingPlatform"))
    //    {
    //        transform.SetParent(hit.transform);
    //    }
    //    else
    //    {
    //        transform.SetParent(null);
    //    }
    //}

}