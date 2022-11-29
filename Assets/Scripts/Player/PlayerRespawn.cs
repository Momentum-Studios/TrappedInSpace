using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{

    [SerializeField] private AudioSource checkpointSound;

    private Transform currentCheckpoint;
    private PlayerHealth playerHealth;

    private Animator anim;

    private void Awake()
    {
        anim= GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void Respawn()
    {
        playerHealth.Respawn(); //Restore player health and reset animation
        transform.position = currentCheckpoint.position; //Move player to checkpoint location

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "CheckPoint")
        {
            anim.Play("IdleFloppy");
            checkpointSound.Play();
            currentCheckpoint = collision.transform;
            collision.GetComponent<Collider2D>().enabled = false;
          
        }
    }
}