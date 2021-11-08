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

    [SerializeField] float jumpForce = 10f;                                                 //Jump force used to calculate how hard the player jump in the Y Coordinate
    [SerializeField] float LongJumpImpulse = 10f;                                           //Long Jump Impulse use to calculate how hard the player jump in the X or Z Coordinate
    private Vector3 jumpDirection;                                                          //Stores the actually Jump Direction to execute later

    [Space(20)]

    /* Gravity Controller */

    [SerializeField] float gravity = -9.81f;                                                //the GRAVITY!

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

    /* some Important Vector3 */

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
    /* update void, mainly for aplying the movement  */

    private void FixedUpdate()
    {

        /* check if grounded */

        isGrounded = Physics.CheckSphere(charGroundCheck.position, charGroundDistance, groundMask);                                         //Create a sphere at the marked transform to check if grounded

            /* if grounded reset gravity force */

            if(isGrounded && Velocity.y < 0)
            {
                Velocity.y = -2f;                                                                                                           //If player is grounded do velocity.y to -2
                Velocity.x = 0f;                                                                                                            //If player is grounded do velocity.x to 0
                Velocity.z = 0f;                                                                                                            //If player is grounded do velocity.z to 0
        }

        /* define Horizontal and Vertical Axis */

        float horizontal = Input.GetAxisRaw("Horizontal");                                                                                  //get horizontal axis as an input
        float vertical = Input.GetAxisRaw("Vertical");                                                                                      //get vertical axis as an input

        /* check if input being added */

        direction = new Vector3(horizontal, 0f, vertical).normalized;                                                                       //Simple Movement "NO CAMERA BASE MOVEMENT"

        /* if input is being received move character */

        if (direction.magnitude >= 0.1f && isGrounded)
        {

            /* fix speed if lower than minimun */

            if(moveSpeed < minMoveSpeed )
            {
                moveSpeed = minMoveSpeed + 0.1f;                                                                                            //check if character move speed is starting lower than the min Move Speed
            }

            /* increase speed over time */

            if(moveSpeed >= minMoveSpeed && moveSpeed < maxMoveSpeed)
            {
                moveSpeed = moveSpeed + speedGain;                                                                                          //if the player speed is lower than the maximun, add the gain speed util it hits the maximum
            }

            /* actually move the player */

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + charCamera.eulerAngles.y;                           //Simple character rotation "WITHOUT SMOOTH ROTATION"

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, CharTurnSmoothTime);          //Character Rotation WITH SMOOTH ROTATION

            transform.rotation = Quaternion.Euler(0f, angle, 0f);                                                                           //DO THE ROTATION USING THE ANGLE SELECTED FROM BEFORE 

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;                                                              //Character Movement Camera Based.

            charController.Move(moveDir.normalized * moveSpeed * Time.deltaTime);                                                           //Do the player to execute the Character Movement selected from before
        }

        /* check if there is non input */

        if (direction.magnitude == 0)
        {
            if (moveSpeed > minMoveSpeed)
            {
                moveSpeed = moveSpeed - speedGain;                                                                                          //if the player speed is higher than the maximun, remove the gain speed util it hits the maximum
            }
        }


        /* Jump System */

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Vector3 airDirection = moveDir.normalized;

            jumpDirection = new Vector3(airDirection.x * moveSpeed, jumpForce, airDirection.z * moveSpeed);

            Velocity += jumpDirection;
        }



        /* gravity creation and application */

        Velocity.y += gravity * Time.deltaTime;                                                                                             //Create the gravity

        charController.Move(Velocity * Time.deltaTime);                                                                                     //Add the gravity to our Character

    }
}
