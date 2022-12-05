/**
 * file: PlayerHealth.cs
 * studio: Momentum Studios
 * authors: Daniel Rodriguez, Daniel Nam, Justin Kim, Gregorius avip
 * class: CS 4700 - Game Development
 * 
 * Assignment: Program 4
 * Date last modified: 11/28/2022
 * 
 * Purpose: Gives players health that can be given, taken or reset when taking damage or picking up health boosts. Any other health interaction 
 * 
 */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : Health
{
    private Animator anim;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable;

    [Header("Audio Source")]
    [SerializeField] private AudioSource deathSoundEffect;
    [SerializeField] private AudioSource damageSoundEffect;
    [SerializeField] private AudioSource addHealthSoundEffect;


    public Image HealthSlider;

    // setup script by getting components
    // and setting current health
    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    // test if health bar works by pressing E,
    // you should see health decrease
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            TakeDamage(1);
    }

    // handles taking damage, playing sounds for damage
    // and the corresponding animation, and gives player invulnerability
    public override void TakeDamage(float _damage)
    {
        
        if (invulnerable) return;
        base.TakeDamage(_damage);
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
                //Deactivate player movement and attack abilities
                foreach (Behaviour component in components)
                    component.enabled = false;

                deathSoundEffect.Play();
                anim.SetBool("grounded", true);
                anim.SetTrigger("die");

                dead = true;
            }
        }
    }

    // handles adding health and playing sound
    public override void AddHealth(float _value)
    {
        addHealthSoundEffect.Play();
        base.AddHealth(_value);
        UpdateHP();
    }

    // subroutine to handle invulnerability
    private IEnumerator Invunerability()
    {
        invulnerable = true;
        // ignore other layers
        Physics2D.IgnoreLayerCollision(10, 11, true);
        // flash the sprite
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        // stop being invulnerable
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }

    // deactivate the game object (player)
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    //Respawn
    public void Respawn()
    {
        dead = false;
        AddHealth(startingHealth); 
        anim.ResetTrigger("die");
        anim.Play("Idle");
        //UpdateHP();
        StartCoroutine(Invunerability());

        //Activate all attached component classes
        foreach (Behaviour component in components)
            component.enabled = true;
    }

    // update the health in the health slider
    private void UpdateHP()
    {
        HealthSlider.fillAmount = ((100 / startingHealth) * (currentHealth)) / 100;
    } 

    // check for death by entering a "DeadCollision"
    private void OnTriggerEnter2D(Collider2D c)
    {
        //Make the player take 3 damage when they fell to death traps
        if (c.tag == "DeadCollision"){
            TakeDamage(3);
        }
    }
}