using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Animator anim;
    
    public static float MaxHealth { get; set; }
    public static float CurrentHealth { get; set; }
    public static float Score { get; set; }
    public static bool IsDead { get; private set; }

    [SerializeField] private AudioSource scoreSound;
    [SerializeField] private AudioSource hurtSound;
    [SerializeField] private AudioSource deathSound;

    [SerializeField] private GameObject currentRespawn;
    [SerializeField] private GameObject endgame;
    [SerializeField] private GameObject ingame;

    void Awake()
    {
        IsDead = false;
        anim = GetComponent<Animator>();
        anim.SetBool("noBlood", false);
    }

    void FixedUpdate()
    {
        
    }

    private void TakeDamage(float damage)
    {
        
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);

        if (CurrentHealth < 1)
        {
            StartCoroutine(Die());
        }
        else
        {
            anim.SetTrigger("Hurt");
            transform.position = currentRespawn.transform.position;
        }
    }

    public void HurtSound()
    {
        hurtSound.Play();
    }

    private IEnumerator Die()
    {
        anim.SetTrigger("Death");        
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        IsDead = true;
        yield return new WaitForSeconds(0.5f);

        ShowMenu("GAME OVER");
    }

    public void DeadSound()
    {
        deathSound.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Death")
        {
            TakeDamage(3f);
        }
        else if (collision.gameObject.tag == "Damage")
        {
            TakeDamage(1f);
        }
        else if (collision.gameObject.tag == "Projectile")
        {
            TakeDamage(1f);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Scores")
        {
            Score += 10;
            scoreSound.Play();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "EndOfLevel")
        {
            if (SceneManager.GetActiveScene().buildIndex < 2)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            else
            {
                ShowMenu("GAME FINISHED");
                IsDead = true;
            }
        }
    }

    public void ShowMenu(string txt)
    { 
        endgame.SetActive(true);
        ingame.SetActive(false);

        endgame.transform.Find("Communication").Find("Text").GetComponent<TextMeshProUGUI>().text = txt;
        endgame.transform.Find("Score").Find("Text").GetComponent<TextMeshProUGUI>().text = $"Score: {Score}";       
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
