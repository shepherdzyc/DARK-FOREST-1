using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance; // 单例

    public GameObject[] enemyPrefab;  //用数组存储不同的敌人预设
    public int poolSize = 10;
    private Dictionary<int, Queue<GameObject>> enemyPools = new Dictionary<int, Queue<GameObject>>(); // 创建一个字典，键是敌人类型，值是对应的对象池

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < enemyPrefab.Length; i++)
        {
            //为每一种敌人创建一个新的队列，作为其对象池
            Queue<GameObject> enemyPool = new Queue<GameObject>();
            for (int j = 0; j < poolSize; j++)
            {
                //在每个对象池中初始化一定数量的敌人
                GameObject enemy = Instantiate(enemyPrefab[i]);
                enemy.SetActive(false);
                enemyPool.Enqueue(enemy);
            }
            enemyPools[i] = enemyPool;  //将对象池添加到字典中
        }
    }

    public GameObject GetEnemy(int level)
    {
        if (enemyPools[level].Count == 0)
        {
            // 如果对象池空了，可以考虑动态创建新对象
            GameObject enemy = Instantiate(enemyPrefab[level]);
            enemy.SetActive(false);
            return enemy;
        }
        else
        {
            GameObject enemy = enemyPools[level].Dequeue();
            enemy.SetActive(true);
            return enemy;
        }
    }

    public void ReturnEnemy(GameObject enemy, int level)
    {
        enemy.SetActive(false);
        enemyPools[level].Enqueue(enemy);
    }
}
