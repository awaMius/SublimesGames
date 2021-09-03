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
    [SerializeField] float animspeedfactor;
    [SerializeField] float maxSpeed;
    public Animator PlayerAnimator;

    [SerializeField] Vector3 oldDirection;

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

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {


            if(direction.x != 0f | direction.z != 0f) oldDirection = direction;



            if (Input.GetKey(KeyCode.LeftShift) && maxSpeed != WalkSpeed) maxSpeed = runningSpeed; else maxSpeed = TrotSpeed;


            if (Input.GetKey(KeyCode.LeftAlt)) { 
                animspeedfactor = speed / 12;
                maxSpeed = WalkSpeed;
            }
            else animspeedfactor = speed / runningSpeed;



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
            animspeedfactor = speed / runningSpeed;
            if (speed > maxSpeed) speed = speed - RunBlending;
            PlayerAnimator.SetFloat("speed", animspeedfactor);
            maxSpeed = 0f;

            //SPEED LOWER



            float targetAngle = Mathf.Atan2(oldDirection.x, oldDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnsmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);


             Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
             controller.Move(moveDir.normalized * speed * Time.deltaTime);



        }
        controller.Move(velocity * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
    }
}
