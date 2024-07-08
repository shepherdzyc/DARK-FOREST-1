using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    //基本属性
    public int hp;
    public int damage;

    public int row;

    public int col;

    public GameObject homeBase;

    //敌人的攻击逻辑
    public void Attack()
    {
        if (col == 0)
        {
            homeBase.GetComponent<HomeBase>().hp -= damage;
        }
    }

    //收到玩家攻击
    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }

    //敌人死亡时的处理，销毁游戏对象、播放死亡动画等
    public void Die()
    {
        Destroy(gameObject);
    }

    //敌人回合开始时向前移动
    public void Move()
    {
        if (col > 0)
        {
            col--;
        }
    }
}
