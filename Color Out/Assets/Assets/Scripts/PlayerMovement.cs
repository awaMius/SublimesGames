using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;


    public float turnSmoothTime = 0.1f;
    float TurnsmoothVelocity;

    public Transform cam;

    public float gravity = 9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;


    //Anim Controller
    public float speed = 2f;
    public float WalkSpeed = 2f;
    public float TrotSpeed = 4f;
    public float runningSpeed = 6f;
    public float RunBlending = 0.1f;
    public float directionCheckerDelay = 0.2f;
    public Animator PlayerAnimator;

    [SerializeField] float animspeedfactor;
    [SerializeField] float maxSpeed;
    [SerializeField] bool updateDirection;
    [SerializeField] bool FirstDirectionInput;

    [SerializeField] Vector3 oldDirection;
    [SerializeField] Vector3 direction;

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        if (Input.GetKey(KeyCode.Escape)) Application.Quit();
    }
    void FixedUpdate()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            if (direction.x != 0f & !updateDirection & FirstDirectionInput| direction.z != 0f & !updateDirection & FirstDirectionInput)
            {
                Invoke("UpdateDirection", directionCheckerDelay);
                updateDirection = true;
            }

            if (!FirstDirectionInput)
            {
                Invoke("UpdateDirection", 0);
                FirstDirectionInput = true;
            }


            if (Input.GetKey(KeyCode.LeftShift) && maxSpeed != WalkSpeed) maxSpeed = runningSpeed; else maxSpeed = TrotSpeed;


            if (Input.GetKey(KeyCode.LeftAlt)) { 
                maxSpeed = WalkSpeed;
            }
            animspeedfactor = speed;



            if (speed < maxSpeed)
                speed = speed + RunBlending;
            else if (speed > maxSpeed) speed = speed - RunBlending; 
            else if (speed == maxSpeed) speed = maxSpeed;

            PlayerAnimator.SetFloat("speed", animspeedfactor);


            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnsmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);



            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            controller.Move(velocity * Time.deltaTime);

        } else if(direction.magnitude < 0.1f)
        {
            animspeedfactor = speed;
            if (speed > maxSpeed) speed = speed - RunBlending;
            PlayerAnimator.SetFloat("speed", animspeedfactor);
            maxSpeed = 0f;

            //SPEED LOWER

               if(speed <= 0) FirstDirectionInput = false;

            if (speed > 0f)
            {
                float targetAngle = Mathf.Atan2(oldDirection.x, oldDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnsmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);


                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }


        }
        controller.Move(velocity * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
    }



    public void UpdateDirection()
    {
        if (direction.magnitude >= 0.1f)
        {
            oldDirection = direction;
        }
        updateDirection = false;
    }

}
