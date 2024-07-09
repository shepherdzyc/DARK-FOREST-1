using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //基本属性
    public int hp;

    public int damage;

    public int row;

    public int col;

    // public GameObject homeBase;

    public GameObject chessBoard;

    public void Start()
    {

    }

    //敌人的攻击逻辑
    public void Attack()
    {
        // if (col == 0)
        // {
        //     homeBase.GetComponent<HomeBase>().hp -= damage;
        // }
    }

    //受到玩家攻击
    public void TakeDamage()
    {
        Transform blockTransform = chessBoard.transform.Find("block_" + row.ToString() + col.ToString());
        hp -= int.Parse(blockTransform.GetChild(1).GetComponent<TextMeshPro>().text);
        if (hp <= 0)
        {
            Die();
        }
    }

    public void Spawn()
    {

    }

    //敌人死亡时的处理，增加游戏得分、销毁游戏对象、播放死亡动画等
    public void Die()
    {
        GameUtils.RemovePair(row, col);
        GameUtils.enemysArr.Remove(gameObject);
        Destroy(gameObject);
        chessBoard.GetComponent<GameMainView>().AddScore();
        chessBoard.GetComponent<GameMainView>().UpdateHisScore();
    }

    //敌人回合开始时向前移动
    public void Move()
    {
        if (row > 0)
        {
            row--;
            transform.position = chessBoard.transform.Find("block_" + row.ToString() + col.ToString()).position;
        }
    }
}
