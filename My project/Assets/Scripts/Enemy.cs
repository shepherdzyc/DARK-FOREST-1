using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ExcelDataReader;
using System.IO;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    //基本属性
    public int hp;

    [SerializeField]
    private int damage;

    public int row;

    public int col;

    public int type;

    public bool isFrozen = false; //增加敌人被冰冻状态

    public int score; // 消灭敌人增加的分数

    public GameObject chessBoard;

    public UnityEngine.UI.Slider slider;

    public GameObject hpObj;

    private Sprite sprite;

    private void Awake()
    {

    }

    private void Start()
    {

    }

    // 初始化被创建出来的敌人
    public void Initialize()
    {
        if (File.Exists(Path.Combine(Application.dataPath, "Resources/Arts/Enemy_" + type.ToString() + ".png")))
        {
            sprite = Resources.Load<Sprite>("Arts/Enemy_" + type.ToString());
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
        else
        {
            Debug.Log("资源不存在");
        }
        UpdateHP();
    }

    // 敌人的攻击逻辑
    public void Attack()
    {
        if (row == 0)
        {
            slider.value -= damage;
        }
    }

    // 敌人生成时的处理
    public void Spawn()
    {

    }


    // 受到玩家攻击
    public void TakeDamage()
    {
        Transform blockTransform = chessBoard.transform.Find("block_" + row.ToString() + col.ToString());
        if (blockTransform != null)
        {
            hp -= int.Parse(blockTransform.GetChild(1).GetComponent<TextMeshPro>().text);
            if (hp <= 0)
            {
                Die();
                return;
            }
            UpdateHP();
        }
    }

    // 更新敌人血量
    public void UpdateHP()
    {
        hpObj.GetComponent<TextMeshPro>().text = hp.ToString();
    }

    // 敌人死亡时的处理，增加游戏得分、销毁游戏对象、播放死亡动画等
    public virtual void Die()
    {
        GameUtils.RemovePosPair(row, col);
        GameUtils.enemysArr.Remove(gameObject);
        SpawnSurroundingEnemies();
        Destroy(gameObject);
        chessBoard.GetComponent<GameMainView>().AddScore();
        chessBoard.GetComponent<GameMainView>().UpdateHisScore();
    }

    // 生成周围新的敌人
    private void SpawnSurroundingEnemies()
    {
        if (type != 3)
        {
            return;
        }
        int[,] directions = new int[,]
        {
            {0,1},{0,-1},{-1,0},{0,0}
        };
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newRow = row + directions[i, 0];
            int newCol = col + directions[i, 1];
            if (!GameUtils.findPos(newRow, newCol))
            {
                // 生成敌人
                GameObject newEnemy = Instantiate(gameObject);
                chessBoard.GetComponent<GameMainView>().UpdateEnemyProperties(newEnemy.GetComponent<Enemy>(), new int[] { 1, 3 }, newCol, newRow, 0);
                newEnemy.GetComponent<Enemy>().Initialize();
                GameUtils.enemysArr.Add(newEnemy);
                GameUtils.posArr.Add(new List<int> { newRow, newCol });
            }
        }
    }

    // 敌人回合开始时向前移动
    public void Move(bool isFirstCreated)
    {
        // 如果被道具冰冻，则敌人无法移动
        if (isFrozen)
        {
            isFrozen = false;
            return;
        }
        if (!isFirstCreated)
        {
            GameUtils.RemovePosPair(row, col);
            if (row > 0)
            {
                row--;
                StartCoroutine(MoveAnim(chessBoard.transform.Find("block_" + row.ToString() + col.ToString()).position));
            }
            GameUtils.AddPosPair(row, col);
        }
        else
        {
            StartCoroutine(MoveAnim(chessBoard.transform.Find("block_" + row.ToString() + col.ToString()).position));
        }
    }

    // 敌人沿x轴移动过程
    IEnumerator MoveAnim(Vector3 target)
    {
        Vector3 startPosition = transform.position;
        float journey = 0f;
        float duration = 1f; // 移动所需时间，可以根据需要调整

        while (journey < duration)
        {
            journey += Time.deltaTime;
            float percent = Mathf.Clamp01(journey / duration);
            transform.position = Vector3.Lerp(startPosition, target, percent);
            yield return null;
        }

        // 确保最后位置精确设置为目标位置
        transform.position = target;
    }
}