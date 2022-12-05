/**
 * file: PlayerRespawn.cs
 * studio: Momentum Studios
 * authors: Daniel Rodriguez, Justin Kim
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/17/2022
 * 
 * purpose: This script acts as the script that handles respawning of players
 */
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{

    [SerializeField] private AudioSource checkpointSound;

    private PlayerHealth playerHealth;
    private GameManager gameManager;

    private Animator anim;

    // setup script by getting components for this script
    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        gameManager = GameManager.getInstance();
    }

    // handle respawning player
    public void Respawn()
    {
        gameManager.reloadLevel();
    }

    // handle entering checkpoints
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.ToLower().StartsWith("checkpoint"))
        {
            checkpointSound.Play();
            gameManager.setCheckPoint(new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z));
            collision.GetComponent<Collider2D>().enabled = false;
        }
    }
}