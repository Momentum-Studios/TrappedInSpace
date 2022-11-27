using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Health : MonoBehaviour
{

    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable;

    [Header("Death Sound")]
    [SerializeField] private AudioSource deathSoundEffect;
    [SerializeField] private AudioSource damageSoundEffect;
    [SerializeField] private AudioSource addHealthSoundEffect;


    public Image HealthSlider;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        //Test if the health bar works by pressing E, you should see health disapear
        if (Input.GetKeyDown(KeyCode.E))
            TakeDamage(1);
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        UpdateHP();

        if (currentHealth > 0)
        {
            damageSoundEffect.Play();
            anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
 
        }
        else
        {
            if (!dead)
            {
                //Deactivate all attached component classes
                foreach (Behaviour component in components)
                    component.enabled = false;

                deathSoundEffect.Play();
                anim.SetBool("grounded", true);
                anim.SetTrigger("die");

                dead = true;
            }
        }
    }
    public void AddHealth(float _value)
    {
        addHealthSoundEffect.Play();
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
        UpdateHP();
    }
    private IEnumerator Invunerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void UpdateHP(){
        HealthSlider.fillAmount=((100/startingHealth) * (currentHealth))/100;
    }
}