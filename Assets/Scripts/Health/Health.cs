
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    
    [Header("Health")]
    [SerializeField] private float StartingHealth;

    public float currentHealth { get; private set; }
    private bool dead;
    private Animator anim;
    public Image HealthSlider;
    
    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private float numberofFlashes;

    private SpriteRenderer spriteRend;


    private void Awake()
    {
        currentHealth = StartingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }


    public void TakeDamage(float _damage) 
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, StartingHealth);
        UpdateHP();
        if (currentHealth > 0)
        {

            anim.SetTrigger("hurt");
            StartCoroutine(Invulnerability());

        }
        else
        {
            if (!dead) 
            {
                anim.SetTrigger("die");
                GetComponent<PlayerMovement>().enabled= false;
                
                dead = true;

            }
           
        }
    }

    private void Update()
    {
        //Test if the health bar works by pressing E, you should see health disapear
        if( Input.GetKeyDown(KeyCode.E)) 
            TakeDamage(1); 
    }

    public void AddHealth(float _value)
    { 
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, StartingHealth);
        UpdateHP();
    }

    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true); //need to know the layers of enemies and players

        for (int i = 0; i < numberofFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f); //change to red when invulnerable
            yield return new WaitForSeconds(iFramesDuration / (numberofFlashes) * 2);
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberofFlashes) * 2);

        }
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }

    public void UpdateHP(){
        HealthSlider.fillAmount = ((100/StartingHealth) * currentHealth) / 100;
    }
}
