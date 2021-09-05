using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    //movement + animations
    public CharacterController controller;
    public float turnSmoothTime = 0.1f;
    float TurnsmoothVelocity;
    public Transform cam;
    public float gravity = 9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;


         //Anim Controller
         public float WalkSpeed = 2f, TrotSpeed = 4f, runningSpeed = 6f, speed = 0f;
         public float RunBlending = 0.1f, stopBlendin = 0.3f;
         public Animator PlayerAnimator;

         [SerializeField] float animspeedfactor;
         [SerializeField] float maxSpeed;
         [SerializeField] bool updateDirection, FirstDirectionInput, isGrounded;
         [SerializeField] float directionCheckerDelay = 0f;

         [SerializeField] Vector3 oldDirection, direction, velocity;

    //Energy Controller

    public float maxEnergy = 10f, currentEnergy, EnergyLostPerRunningFrame = 0.1f, EnergyHideBlend = 0.01f, EnergyShowBlend = 0.05f;
    public Slider EnergyBar;
    public CanvasGroup EnergyGroup;


    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        if (Input.GetKey(KeyCode.Escape)) Application.Quit();
    }
    void FixedUpdate()
    {

        if(speed < 0)
        {
            speed = 0;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        //energyController
        if (currentEnergy < maxEnergy && !Input.GetKey(KeyCode.LeftShift))
            currentEnergy = currentEnergy + EnergyLostPerRunningFrame;

        if (currentEnergy > maxEnergy) { 
            currentEnergy = maxEnergy;
        }

        if(currentEnergy == maxEnergy) if (EnergyGroup.alpha > 0f)
                EnergyGroup.alpha = EnergyGroup.alpha - EnergyHideBlend;


        EnergyBar.maxValue = maxEnergy;
        EnergyBar.value = currentEnergy;


        //player controller


        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            if (direction.x != 0f & !updateDirection & FirstDirectionInput | direction.z != 0f & !updateDirection & FirstDirectionInput)
            {
                Invoke("UpdateDirection", directionCheckerDelay);
                updateDirection = true;
            }

            if (!FirstDirectionInput)
            {
                Invoke("UpdateDirection", 0);
                FirstDirectionInput = true;
            }

            if (speed > TrotSpeed + 0.1f && currentEnergy > 0f) { 
                currentEnergy = currentEnergy - EnergyLostPerRunningFrame;
                EnergyGroup.alpha = EnergyGroup.alpha + EnergyShowBlend;
            }



            if (Input.GetKey(KeyCode.LeftShift) && maxSpeed != WalkSpeed && currentEnergy > 0f)
            {
                maxSpeed = runningSpeed;
            }
            else
            {
                maxSpeed = TrotSpeed;
            }

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
            if (speed > maxSpeed) speed = speed - stopBlendin;
            PlayerAnimator.SetFloat("speed", animspeedfactor);
            maxSpeed = 0f;

            //SPEED LOWER

            if (speed <= 0)
            {
                FirstDirectionInput = false;
                oldDirection = new Vector3(0, 0, 0);
            }
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
