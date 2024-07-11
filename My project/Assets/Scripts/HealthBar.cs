using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public int hp;

    public Slider slider;

    public int currentHp;

    private void Start()
    {
        currentHp = hp;
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
    }
}
