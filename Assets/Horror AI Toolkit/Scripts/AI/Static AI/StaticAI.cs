using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAI : MonoBehaviour
{
    Player player;

    public bool CanTakeDamage = false;
    public float attackDamage = 10.0f;
    float attackDelayDuration = 3.0f;
    float attackDelayTimer = 0.0f;

    float maxHealth = 10.0f;
    float currentHealth = 0.0f;

    float deathTimer = 0.0f;
    float deathTimerDuration = 2.0f;
    bool isDead = false;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        attackDelayTimer += Time.deltaTime;

        if(isDead)
        {
            Die();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && !other.isTrigger)
        {
            Debug.Log("timer " + attackDelayTimer.ToString());
            if(attackDelayTimer >= attackDelayDuration)
            {
                Debug.Log("attack player");
                other.gameObject.GetComponent<Player>().LoseHealth(attackDamage);
                attackDelayTimer = 0.0f;
            }
        }
    }

    public void TakeDamage(float healthLoss)
    {
        if (CanTakeDamage)
        {
            currentHealth -= healthLoss;
            if (currentHealth <= 0)
            {
                isDead = true;
            }
        }
    }

    void Die()
    {
        transform.position = transform.position - (transform.up / 60);

        deathTimer += Time.deltaTime;
        if(deathTimer >= deathTimerDuration)
        {
            gameObject.SetActive(false);
        }
    }
}
