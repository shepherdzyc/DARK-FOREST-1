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

    //抽象方法，子类必须实现
    public abstract void Attack();

    //虚方法，子类可以选择重写
    public virtual void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }

    //创建敌人
    public virtual void Spawn()
    {

    }

    //敌人死亡时的处理，销毁游戏对象、播放死亡动画等
    public virtual void Die()
    {
        Destroy(gameObject);
    }

    //敌人向前移动
    public virtual void Move()
    {

    }
}
