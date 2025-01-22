using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baddie : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float damagethreshold = 0.2f;
    [SerializeField] private GameObject baddieDeathParticle;
    [SerializeField] private AudioClip baddieDeathSound;
 
    private float currenthealth;

    private void Awake()
    {
        currenthealth = maxHealth;
    }

    public void DamageBaddie(float damageAmount)
    {
        currenthealth -= damageAmount;

        if (currenthealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameManager.instance.RemoveBaddie(this);
        
        Instantiate(baddieDeathParticle , transform.position, Quaternion.identity);

        AudioSource.PlayClipAtPoint(baddieDeathSound, transform.position);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactvelocity = collision.relativeVelocity.magnitude;

        if (impactvelocity > damagethreshold)
        {
            DamageBaddie(impactvelocity);
        }
    }
}
