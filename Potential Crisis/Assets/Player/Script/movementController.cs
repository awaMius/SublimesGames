///////////////////////////////////////////////////////////
/// Main Movement System for Potential Crisis
/// Script Created by:  Mius
/// Date:               07-11-21
//////////////////////////////////////////////////////////

using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]                                       //Add Rigibody and collider if not in gameObject 
public class movementController : MonoBehaviour
{

    /* Movement Variables */

    [SerializeField] float moveSpeed = 5, maxMoveSpeed = 10;

    [Space(20)]                                                                                         //add 20px of space line in inspector

    [SerializeField] float jumpForce = 4;

    [Space(20)]                                                                                         //add 20px of space line in inspector

    [SerializeField] float frictionMultiplier = 1, bounciness = 0;

    [Space(20)]                                                                                         //add 20px of space line in inspector

    [SerializeField] Vector3 Direction;

    [Space(20)]                                                                                         //add 20px of space line in inspector

    Rigidbody rigidbody;
    CapsuleCollider collider;
    PhysicMaterial physicMaterial;

    [Space(20)]                                                                                         //add 20px of space line in inspector

    RaycastHit raycastHitGround;                                                                        //the point where the raycast is hitting
    [SerializeField] Vector3 raycastDirGround;
    [SerializeField] float raycastDistanceGround = 1;
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] bool isGrounded;

    [Space(20)]                                                                                         //add 20px of space line in inspector

    /* define Keys */

    [SerializeField] KeyCode jump;

    [Space(20)]

    /* debug speeds */

    [SerializeField] float RBSpeed;

    // Start is called before the first frame update
    void Start()
    {
        /* Initialize rigidbody and Direction */

        rigidbody = this.GetComponent<Rigidbody>();                                                 // initialize Rigibody
        collider = this.GetComponent<CapsuleCollider>();                                            // initialize Collider
        physicMaterial = collider.material;                                                         // initialize Material

        Direction = new Vector3(0, 0, 0);                                                           // initialize Velocity
        raycastDirGround = new Vector3(0, -1, 0);                                                   // initialize ground direction for ground

    }
    void FixedUpdate()
    {

        RBSpeed = rigidbody.velocity.y;

        /* define Friction */

        physicMaterial.bounciness = bounciness;                                                     // define Bounciness
        physicMaterial.staticFriction = rigidbody.velocity.magnitude * frictionMultiplier;          // define staticfriction
        physicMaterial.dynamicFriction = rigidbody.velocity.magnitude * frictionMultiplier;         // define DynamicFriction

        /* define velocity */

        float x = Input.GetAxisRaw("Horizontal");                                                   // define x
        float z = Input.GetAxisRaw("Vertical");                                                     // define z
        float realMoveSpeed = moveSpeed * 100;


        Direction = new Vector3(x, 0, z).normalized;                              // define direction and normalize it
        rigidbody.AddForce(Direction * realMoveSpeed * Time.deltaTime);                                  // do the movement

        /* limit player velocity */

        if (rigidbody.velocity.magnitude > maxMoveSpeed)
        {
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxMoveSpeed);          // clamp player movement to maxMoveSpeed
        }

        /* check for player grounded */


        //end edit//
        if (Physics.Raycast(transform.position, raycastDirGround, out raycastHitGround, raycastDistanceGround, GroundLayer))
        {
            Debug.DrawRay(transform.position, raycastDirGround * raycastDistanceGround, Color.green);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            Debug.DrawRay(transform.position, raycastDirGround * 10, Color.red);
        }


        /* jump Function */

        if (Input.GetKey(jump) && isGrounded)
        {
            rigidbody.AddForce(new Vector3 (0,jumpForce,0));
        }

        if(!isGrounded)
        {
            rigidbody.AddForce(new Vector3(0,-9.81f,0));
        }

    }


}
