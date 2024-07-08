using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;

public class GameMainView : MonoBehaviour
{
    public GameObject chessBoard;

    public GameObject[] rolls;

    public GameObject[] enemysArr;

    private Camera mainCamera;

    private GameObject selectedObject;

    private GameObject[] createdRollArr;

    private GameObject[] createdEnemyArr;

    private bool setNumOnce = true;

    void Awake()
    {
        mainCamera = Camera.main;
        createdRollArr = new GameObject[3];
        createdEnemyArr = new GameObject[10];
    }

    void Start()
    {
        CreateRoll();  //for test
        // CreateEnemy();
        setBlockNum();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

        }
        updateBlockColor();
        updateBlockNum();
    }

    //回合开始时创建三个骰子
    public void CreateRoll()
    {
        int[] randomNumArr = GameUtils.CreateRandomNum();
        int[][] randomPosArr = GameUtils.CreateRandomPos();
        for (int i = 0; i < 3; i++)
        {
            int x = randomPosArr[i][0];
            int y = randomPosArr[i][1];
            GameUtils.rollType type = GameUtils.CreateRandomType();
            string blockName = "block_" + x.ToString() + y.ToString();
            Transform blockTransform = chessBoard.transform.Find(blockName);
            if (blockTransform == null)
            {
                return;
            }
            else
            {
                GameObject newRoll = Instantiate(rolls[randomNumArr[i] - 1]);
                newRoll.GetComponent<RollController>().row = x;
                newRoll.GetComponent<RollController>().col = y;
                newRoll.GetComponent<RollController>().num = randomNumArr[i];
                newRoll.GetComponent<RollController>().type = type;
                createdRollArr[i] = newRoll;
                newRoll.transform.position = blockTransform.position;
            }
        }
    }

    //回合开始时随机创建敌人
    public void CreateEnemy()
    {
        //先写死 测试
        GameObject enemy = Instantiate(enemysArr[0]);
        createdEnemyArr[0] = enemy;
        enemy.transform.position = chessBoard.transform.Find("block_52").position;
        enemy.GetComponent<Enemy>().row = 5;
        enemy.GetComponent<Enemy>().col = 2;
    }

    //回合结束时除存储骰子外所有骰子
    public void DestroyRoll()
    {
        for (int i = 0; i < createdRollArr.Length; i++)
        {
            Destroy(createdRollArr[i]);
        }
    }

    //更新方块颜色
    public void updateBlockColor()
    {
        if (createdRollArr.Length == 0)
        {
            return;
        }
        for (int i = 0; i < createdRollArr.Length; i++)
        {
            GameUtils.rollType type = createdRollArr[i].GetComponent<RollController>().type;
            int row = createdRollArr[i].GetComponent<RollController>().row;
            int col = createdRollArr[i].GetComponent<RollController>().col;
            if (type == GameUtils.rollType.rowType)
            {
                for (int j = 0; j < 6; j++)
                {
                    string blockName = "block_" + j.ToString() + col.ToString();
                    Transform blockTransform = chessBoard.transform.Find(blockName);
                    blockTransform.GetChild(0).gameObject.SetActive(true);
                }
            }
            else if (type == GameUtils.rollType.colType)
            {
                for (int j = 0; j < 5; j++)
                {
                    string blockName = "block_" + row.ToString() + j.ToString();
                    Transform blockTransform = chessBoard.transform.Find(blockName);
                    blockTransform.GetChild(0).gameObject.SetActive(true);
                }
            }
            else
            {
                Transform blockTransform = chessBoard.transform.Find("block_" + row.ToString() + col.ToString());
                blockTransform.GetChild(0).gameObject.SetActive(true);
                if (row - 1 >= 0)
                {
                    Transform blockTransform1 = chessBoard.transform.Find("block_" + (row - 1).ToString() + col.ToString());
                    blockTransform1.GetChild(0).gameObject.SetActive(true);
                }
                if (row + 1 <= 5)
                {
                    Transform blockTransform2 = chessBoard.transform.Find("block_" + (row + 1).ToString() + col.ToString());
                    blockTransform2.GetChild(0).gameObject.SetActive(true);
                }
                if (col - 1 >= 0)
                {
                    Transform blockTransform3 = chessBoard.transform.Find("block_" + row.ToString() + (col - 1).ToString());
                    blockTransform3.GetChild(0).gameObject.SetActive(true);
                }
                if (col + 1 <= 4)
                {
                    Transform blockTransform4 = chessBoard.transform.Find("block_" + row.ToString() + (col + 1).ToString());
                    blockTransform4.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }

    public void setBlockNum()
    {
        if (createdRollArr.Length == 0)
        {
            return;
        }
        for (int i = 0; i < createdRollArr.Length; i++)
        {
            GameUtils.rollType type = createdRollArr[i].GetComponent<RollController>().type;
            int row = createdRollArr[i].GetComponent<RollController>().row;
            int col = createdRollArr[i].GetComponent<RollController>().col;
            int num = createdRollArr[i].GetComponent<RollController>().num;
            if (type == GameUtils.rollType.rowType)
            {
                for (int j = 0; j < 6; j++)
                {
                    GameUtils.blockNumArr[j, col] += num;
                }
            }
            else if (type == GameUtils.rollType.colType)
            {
                for (int j = 0; j < 5; j++)
                {
                    GameUtils.blockNumArr[row, j] += num;
                }
            }
            else
            {
                GameUtils.blockNumArr[row, col] += num;
                if (row - 1 >= 0)
                {
                    GameUtils.blockNumArr[row - 1, col] += num;
                }
                if (row + 1 <= 5)
                {
                    GameUtils.blockNumArr[row + 1, col] += num;
                }
                if (col - 1 >= 0)
                {
                    GameUtils.blockNumArr[row, col - 1] += num;
                }
                if (col + 1 <= 4)
                {
                    GameUtils.blockNumArr[row, col + 1] += num;
                }
            }
        }
    }

    //更新方块数值
    public void updateBlockNum()
    {
        for (int i = 0; i < GameUtils.blockNumArr.GetLength(0); i++)
        {
            for (int j = 0; j < GameUtils.blockNumArr.GetLength(1); j++)
            {
                if (GameUtils.blockNumArr[i, j] != 0)
                {
                    Transform blockTransform = chessBoard.transform.Find("block_" + i.ToString() + j.ToString());
                    blockTransform.GetChild(1).gameObject.SetActive(true);
                    blockTransform.GetChild(1).GetComponent<TextMeshPro>().text = GameUtils.blockNumArr[i, j].ToString();
                }
            }
        }
    }

    //检测是否按下攻击键
    public void DetectAttack()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
                if (hit.collider != null)
                {
                    selectedObject = hit.collider.gameObject;
                    PlayAttack();
                }
            }
        }
    }

    //
    public void PlayAttack()
    {

    }

    //轮到AI的回合
    public void PlayAIRound()
    {
        for (int i = 0; i < enemysArr.Length; i++)
        {
            enemysArr[i].GetComponent<Enemy>().Move();
        }
    }
}