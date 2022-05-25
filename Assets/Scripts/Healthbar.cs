using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image totalHealthbar;
    [SerializeField] private Image currentHealthbar;

    void Start()
    {
        //totalHealthbar.fillAmount = player.CurrentHealth / 10;
    }

    void FixedUpdate()
    {
        currentHealthbar.fillAmount = Player.CurrentHealth / 10;
    }
}
