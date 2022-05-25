using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GameObject music; 

    void Start()
    {
    }

    public void PlayGame()
    {
        Player.MaxHealth = 3;
        Player.Score = 0;

        Player.CurrentHealth = Player.MaxHealth;    

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
