/**
 * file: HealthCollectible.cs
 * studio: Momentum Studios
 * authors: Daniel Rodriguez, Daniel Nam, Justin Kim
 * class: CS 4700 - Game Development
 * 
 * Assignment: Program 4
 * Date last modified: 11/28/2022
 * 
 * Purpose: Gives players health that can be given, taken or reset when taking damage or picking up health boosts
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

    public override void AddHealth(float _value)
    {
        addHealthSoundEffect.Play();
        base.AddHealth(_value);
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

    private void UpdateHP(){
        HealthSlider.fillAmount=((100/startingHealth) * (currentHealth))/100;
    }

    private void OnTriggerEnter2D(Collider2D c){
        //Make the player take 10 damage when they fell to death traps
        if (c.tag == "DeadCollision"){
            TakeDamage(10);
        }
    }
}