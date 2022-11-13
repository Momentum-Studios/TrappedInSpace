
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    

    private void Awake()
    {
        //Grab reference from rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
      
        float horizontalInput = Input.GetAxis("Horizontal");

        //Walking
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

    
        //Flipping player when going left or right on the horizontal axis
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1,1,1);



        if (Input.GetKey(KeyCode.Space) && grounded)
            Jump();



        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", grounded);

    }

    private void Jump() {

        body.velocity = new Vector2(body.velocity.x, gravity);
        anim.SetTrigger("jump");
        grounded = false;

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Ground")
            grounded = true;

    }



}
