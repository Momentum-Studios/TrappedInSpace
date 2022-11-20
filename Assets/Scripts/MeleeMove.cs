using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMove : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float distanceBetween;
    public Animator animator;

    private float distance;
    void Start()
    {
        
    }

    
    void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Vector3 scale = transform.localScale;
        if(player.transform.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x) * -1;
        }    
        else
        {
            scale.x = Mathf.Abs(scale.x);
        }

        
        transform.localScale = scale;

        if(distance < distanceBetween)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(Vector3.forward);
        }
        animator.SetFloat("Distance", distance);

        if(distance < 1.5)
        {
            speed = 0;
            animator.SetBool("Alive", false);
        }    
    }
}
