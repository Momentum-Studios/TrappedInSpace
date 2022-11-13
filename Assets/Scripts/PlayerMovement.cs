
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed; // used to set Running velocity
    [SerializeField] private float gravity; // used for Jumping
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float horizontalInput;


    private void Awake()
    {
        //Grab reference from rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }


    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        //Walking
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

    
        //Flipping player when going left or right on the horizontal axis
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1,1,1);



        if (Input.GetKey(KeyCode.Space) && isGrounded())
            Jump();



        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

    }


    private void Jump() {

        body.velocity = new Vector2(body.velocity.x, gravity);
        anim.SetTrigger("jump");

    }


    private bool isGrounded() 
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size,0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
            
    }


    public bool canAttack()
    {

        return horizontalInput == 0 && isGrounded();

    }

}
