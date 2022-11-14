
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    [SerializeField] private float attackCoolDown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] blasterShots;


    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        //Get references
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && cooldownTimer > attackCoolDown && playerMovement.canAttack() )
            Attack();

        cooldownTimer += Time.deltaTime;
    }

    private void Attack() {

        cooldownTimer = 0;

        //object pooling for  blaster
        blasterShots[FindBlaster()].transform.position = firePoint.position;
        blasterShots[FindBlaster()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    
    }
    private int FindBlaster() 
    {
        for (int i = 0; i < blasterShots.Length; i++) 
        { 
            if(!blasterShots[i].activeInHierarchy)
                return i;
            
        }
        return 0;
    }

}
