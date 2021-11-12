///MAIN MOVEMENT SYSTEM OF POTENTIAL CRISIS
///WRITTEN BY: MIUS
///DATE:       07.11.21


using UnityEngine;

public class movementSystem2 : MonoBehaviour
{

    [SerializeField] CharacterController charController;                                    //main Character Controller
    [SerializeField] Transform charCamera;                                                  //the location and rotation of the camera

    [Space(20)]                                                                             //Add an space of 20px in the inspector, just for it to look better

    /* Speed Controller */

    [SerializeField] float moveSpeed = 0f;                                                  //current moving Speed
    [SerializeField] float minMoveSpeed = 3f;                                               //basis speed
    [SerializeField] float maxMoveSpeed = 15f;                                              //Maximun speed archiavable while moving
    [SerializeField] float speedGain = 0.1f;                                                //the speed at wich you gain speed while moving

    [Space(20)]                                                                             //Add an space of 20px in the inspector, just for it to look better

    /* Jump Controller */

    [SerializeField] float jumpForce = 15f;                                                 //Jump force used to calculate how hard the player jump in the Y Coordinate
    [SerializeField] float LongJumpImpulse = 20f;                                           //Long Jump Impulse use to calculate how hard the player jump in the X or Z Coordinate
    [SerializeField] float airVelocity = 5f;                                                //how fast the player is able to move while not grounded

    [Space(10)]
    /* jump rays */
    [SerializeField] float jumpDirectionRayLenght = 0.25f;                                  //the length of the ray to check if the player collided towards something in the jump direction
    [SerializeField] float jumpDirectionRayOffset = 0.1f;                                   //offset towards center of character of the rays
    [SerializeField] float jumpDirectionRayOpening = 2f;                                    //take the number of grads you want to use and divide by 45

    /*private values of jump */
    private Vector3 jumpDirection;                                                          //Stores the actually Jump Direction to execute later
    private bool jump;                                                                      //is the character jumping?
    private float currentAirVelocity;                                                       //the Real CurrentVelocityOnAir

    [Space(20)]

    /* Gravity Controller */

    [SerializeField] float gravity = -9.81f;                                                //the main gravity value
    [SerializeField] float GravityAdderOnAir = 0.5f;                                        //the constantly added gravity so the player always go down faster.
    private float CurrentGravityForce = -9.81f;                                             //the GRAVITY force towards the center mass.

    [Space(20)]                                                                             //Add an space of 20px in the inspector, just for it to look better

    /* rotation Controller */

    [SerializeField] float CharTurnSmoothTime = 0.1f;                                       //rotation smoothness of the character
    private float turnSmoothVelocity;                                                       //the stored value used in the function to calculate the rotation smoothnes of the character

    [Space(20)]                                                                             //Add an space of 20px in the inspector, just for it to look better

    /* grounCheck Controller */

    [SerializeField] Transform charGroundCheck;                                             //the location and rotation of the point where the ground check is created
    [SerializeField] float charGroundDistance = 0.4f;                                       //the radius of the ground check
    [SerializeField] LayerMask groundMask;                                                  //the layers/layer at wich the groundcheck returns true
    [SerializeField] bool isGrounded;                                                       //is the player grounded?
    private Vector3 NotGroundedDirectionMoving;                                             //the Direction Moving while Not Grounded

    [Space(20)]                                                                             //Add an space of 20px in the inspector, just for it to look better

    /* Slope Checker */

    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;

    [Space(20)]                                                                             //Add an space of 20px in the inspector, just for it to look better

    /* movement Vectors */

    private Vector3 direction;                                                              //main movement of the character is not camera based  (NOT IN USE)
    private Vector3 moveDir;                                                                //main movement of the character is camera based      (IN USE)
    private Vector3 Velocity;                                                               //Store the velocity of the Y axis of the character   (let us add gravity or make him jump)


    /* start void, mainly for initializing some not need for loop things */
    private void Start()
    {
        /* lock cursor and make it invisible */

        Cursor.lockState = CursorLockMode.Locked;                                           //define cursor lockstate to locked
        Cursor.visible = false;                                                             //define cursor as invisible

        /* load charController, Camera and charGroundCheck on load */

        charController = this.GetComponent<CharacterController>();                          //get of the current GameObject the CharacterController Component
        charCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;              //search in the whole Project for the GameObject with the tag MainCamera, after that , get the transform out of it.
        charGroundCheck = GameObject.FindGameObjectWithTag("GroundCheck").transform;        //search in the whole Project for the GameObject with the tag GroundCheck, after that , get the transform out of it.

        /* define groundMask as ground mainly */

        groundMask = LayerMask.GetMask("ground");                                           //LayerMask should get the mask "ground" and select it as groundMask
    }

    /* Input System in normal Update because better Detection */
    private void Update()
    {

        /* Jump System */

        if (Input.GetKeyDown(KeyCode.Space) && jump && isGrounded)
        {
            Vector3 JumpDirection = transform.forward;                                                                                        //take the forward direction of the player and set it as the LongJumpDiretion;

            float realJumpForce = Mathf.Sqrt(jumpForce * -2 * CurrentGravityForce);
            float realLongJumpForce = Mathf.Sqrt(jumpForce * -1 * CurrentGravityForce);

            if (Input.GetKey(KeyCode.LeftShift) && moveSpeed >= maxMoveSpeed / 2)
            {
                currentAirVelocity = airVelocity * 4;
                jumpDirection = new Vector3(JumpDirection.x * LongJumpImpulse, realLongJumpForce, JumpDirection.z * LongJumpImpulse);         //set the jump direction (long jump)                                                                                                                     //make sure the player only jump once
            }
            else
            {
                currentAirVelocity = airVelocity;
                jumpDirection = new Vector3(JumpDirection.x * (moveSpeed / 2), realJumpForce, JumpDirection.z * (moveSpeed / 2));            //set the jump direction (normal jump)                                                                                                           //make sure the player only jump once
            }

            Velocity += jumpDirection;                                                                                                      //add the jump direction to the player direction making the character do the jump
            jump = false;                                                                                                                   //set the state of jump to it is jumping
        }

    }


    /* update void, mainly for aplying the movement  */

    private void FixedUpdate()
    {

        Debug.DrawRay(transform.position, new Vector3(Velocity.x, 0, Velocity.z) * jumpDirectionRayLenght, Color.red);
        Debug.DrawRay(transform.position + new Vector3(jumpDirectionRayOffset, 0, jumpDirectionRayOffset), new Vector3(Velocity.x, 0, Velocity.z) * jumpDirectionRayLenght + new Vector3(jumpDirectionRayOpening, 0, jumpDirectionRayOpening), Color.red);
        Debug.DrawRay(transform.position - new Vector3(jumpDirectionRayOffset, 0, jumpDirectionRayOffset), new Vector3(Velocity.x, 0, Velocity.z) * jumpDirectionRayLenght + new Vector3(-jumpDirectionRayOpening, 0, -jumpDirectionRayOpening), Color.red);

        /* check if grounded */

        isGrounded = Physics.CheckSphere(charGroundCheck.position, charGroundDistance, groundMask);                                         //Create a sphere at the marked transform to check if grounded

        /* if grounded reset gravity force */

        if (isGrounded && Velocity.y < 0)
        {
            Velocity.y = -2f;                                                                                                               //If player is grounded do velocity.y to -2
            Velocity.x = 0f;
            Velocity.z = 0f;


            jumpDirection = new Vector3(0, 0, 0);                                                                                           //If player is grounded do airDirection to 0
            jump = true;                                                                                                                    //If player is grounded reset jump state
            CurrentGravityForce = gravity;                                                                                                  //reset of gravity force
            NotGroundedDirectionMoving = new Vector3(0, 0, 0);
        }
        else if (!isGrounded)
        {
            if (jump)
            {
                if(NotGroundedDirectionMoving == new Vector3(0,0,0))
                 NotGroundedDirectionMoving = moveDir;                                                                              //the the direction on wich the player was moving

                charController.Move(NotGroundedDirectionMoving.normalized * moveSpeed * Time.deltaTime);                                                         //if the player jump out of a sledge without jumping save the speed and apply it until it hits the ground again 
            }
            CurrentGravityForce -= GravityAdderOnAir;                                                                               //apply the gravity adder on air 
        }

        /* define Horizontal and Vertical Axis */

        float horizontal = Input.GetAxisRaw("Horizontal");                                                                                  //get horizontal axis as an input
        float vertical = Input.GetAxisRaw("Vertical");                                                                                      //get vertical axis as an input

        /* check if input being added */

        direction = new Vector3(horizontal, 0f, vertical).normalized;                                                                       //Simple Movement "NO CAMERA BASE MOVEMENT"

        /* if input is being received move character */

        if (direction.magnitude >= 0.1f)
        {

            /* actually move the player */

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + charCamera.eulerAngles.y;                           //Simple character rotation "WITHOUT SMOOTH ROTATION"

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, CharTurnSmoothTime);          //Character Rotation WITH SMOOTH ROTATION
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;                                                              //Character Movement Camera Based.

            /* check if char grounded or not */

            if (isGrounded)
            {
                /* fix speed if lower than minimun */


                if (moveSpeed < minMoveSpeed)
                {
                    moveSpeed = minMoveSpeed + 0.1f;                                                                                            //check if character move speed is starting lower than the min Move Speed
                }

                /* increase speed over time */

                if (moveSpeed >= minMoveSpeed && moveSpeed < maxMoveSpeed)
                {
                    moveSpeed = moveSpeed + speedGain;                                                                                          //if the player speed is lower than the maximun, add the gain speed util it hits the maximum
                }

                transform.rotation = Quaternion.Euler(0f, angle, 0f);                                                                       //DO THE ROTATION USING THE ANGLE SELECTED FROM BEFORE 

                charController.Move(moveDir.normalized * moveSpeed * Time.deltaTime);                                                       //Do the player to execute the Character Movement selected from before (grounded)
            }
            else
            {
                Velocity += (moveDir.normalized * airVelocity * Time.deltaTime);                                                            //Do the player to execute the Character Movement selected from before (on air)
            }
        }

        /* check if there is non input */

        if (direction.magnitude == 0)
        {
            if (moveSpeed > minMoveSpeed)
            {
                moveSpeed = moveSpeed - speedGain;                                                                                          //if the player speed is higher than the maximun, remove the gain speed util it hits the maximum
            }
        }



        /* gravity creation and application */

        Velocity.y += CurrentGravityForce * Time.deltaTime;                                                                                 //Create the gravity

        charController.Move(Velocity * Time.deltaTime);



        //Add the gravity to our Character

        /* if on slope add force to fix it */

        if (direction.magnitude > 0 && onSlope())
            charController.Move(Vector3.down * charController.height / 2 * slopeForce * Time.deltaTime);                                    //add an constant force to character when it is in a slope

    }

    /* check if player is on slope */

    private bool onSlope()
    {
        slopeForce = -gravity;

        if (!jump)
            return false;                                                                                                                   //if player is jumping it is not in a slope

        RaycastHit hit;                                                                                                                     //stored raycast hit point

        if (Physics.Raycast(transform.position, Vector3.down, out hit, charController.height / 2 * slopeForceRayLength))                    //send a ray downwards of player
            if (hit.normal != Vector3.up)                                                                                                   //check if the hit point of the ray is at 90grad
                return true;                                                                                                                //if it isnt , the player is on a slope
        return false;                                                                                                                       //if it is , player is not on a slope
    }

    /* check if character is colliding with groundmask */

    // ON PROGRES STILL BUGGY 

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if ((groundMask.value & (1 << hit.transform.gameObject.layer)) > 0)
        {
            RaycastHit hitDirectionJump;

            /*  execute the rays and the collision test */

            if (Physics.Raycast(transform.position, new Vector3(Velocity.x, 0, Velocity.z), out hitDirectionJump, charController.height / 2 * jumpDirectionRayLenght)                                            //check for a hit in the jump direction forwards 
                && Physics.Raycast(transform.position + new Vector3(jumpDirectionRayOffset, 0, jumpDirectionRayOffset), new Vector3(Velocity.x + jumpDirectionRayOpening, 0, Velocity.z + jumpDirectionRayOpening), out hitDirectionJump, charController.height / 2 * jumpDirectionRayLenght)                 //check for a hit in the jump direction diagonally +45
                && Physics.Raycast(transform.position - new Vector3(jumpDirectionRayOffset, 0, jumpDirectionRayOffset), new Vector3(Velocity.x - jumpDirectionRayOpening, 0, Velocity.z - jumpDirectionRayOpening), out hitDirectionJump, charController.height / 2 * jumpDirectionRayLenght)
                && !jump && !onSlope())               //check for a hit in the jump direction diagonally -45
            {
                Velocity.x = 0;                                                                                                                     //IF CHARACTER IS COLLING RESET VELOCITY X & Z
                Velocity.z = 0;                                                                                                                     //IF CHARACTER IS COLLIDING RESET VELOCITY X & Z
            }
        }
        else
        {
            Debug.Log("Not in Layermask");
        }
    }

}
