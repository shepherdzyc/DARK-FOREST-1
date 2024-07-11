using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameMainView : MonoBehaviour
{
    public GameObject chessBoard;

    public GameObject[] rolls;

    public GameObject[] enemy;

    public GameObject curScoreObj;

    public GameObject hisScoreObj;

    private Camera mainCamera;

    private GameObject selectedObject;

    private Transform chessBoardTransform;

    void Awake()
    {
        mainCamera = Camera.main;
        chessBoardTransform = chessBoard.transform;  //缓存棋盘的Transform引用
    }

    void Start()
    {
        CreateEnemy();
        CreateRoll();  //for test
        SetBlockNum();
        SetBlockColor();
        UpdateBlockNum();
    }

    void Update()
    {
        DetectAttack();
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
            GameUtils.RollType type = GameUtils.CreateRandomType();
            string blockName = "block_" + x.ToString() + y.ToString();
            Transform blockTransform = chessBoardTransform.Find(blockName);
            if (blockTransform == null)
            {
                Debug.LogError("Block transform not found: " + blockName);
                return;
            }
            else
            {
                GameObject newRoll = Instantiate(rolls[randomNumArr[i] - 1]);
                newRoll.GetComponent<RollController>().row = x;
                newRoll.GetComponent<RollController>().col = y;
                newRoll.GetComponent<RollController>().num = randomNumArr[i];
                newRoll.GetComponent<RollController>().type = type;
                GameUtils.rollsArr.Add(newRoll);
                newRoll.transform.position = blockTransform.position;
            }
        }
    }

    //回合开始时随机创建敌人
    public void CreateEnemy()
    {
        //先写死 测试
        GameObject newEnemy = Instantiate(enemy[0]);
        GameUtils.enemysArr.Add(newEnemy);
        newEnemy.GetComponent<Enemy>().row = Random.Range(4, 6);
        newEnemy.GetComponent<Enemy>().col = Random.Range(0, 5);
        GameUtils.posArr.Add(new List<int> { newEnemy.GetComponent<Enemy>().row, newEnemy.GetComponent<Enemy>().col });  //for test
        newEnemy.transform.position = chessBoardTransform.Find("block_" + newEnemy.GetComponent<Enemy>().row.ToString()
        + newEnemy.GetComponent<Enemy>().col.ToString()).position;
    }

    // 更新方块颜色方法
    public void SetBlockColor()
    {
        if (GameUtils.rollsArr.Count == 0)
        {
            return;
        }

        foreach (var roll in GameUtils.rollsArr)
        {
            GameUtils.RollType type = roll.GetComponent<RollController>().type;
            int row = roll.GetComponent<RollController>().row;
            int col = roll.GetComponent<RollController>().col;

            UpdateBlockBasedOnType(type, row, col, 0, true, false);
        }
    }

    // 设置块的数字方法
    public void SetBlockNum()
    {
        if (GameUtils.rollsArr.Count == 0)
        {
            return;
        }

        foreach (var roll in GameUtils.rollsArr)
        {
            GameUtils.RollType type = roll.GetComponent<RollController>().type;
            int row = roll.GetComponent<RollController>().row;
            int col = roll.GetComponent<RollController>().col;
            int num = roll.GetComponent<RollController>().num;

            UpdateBlockBasedOnType(type, row, col, num, false, true);
        }
    }

    //更新方块数值
    public void UpdateBlockNum()
    {
        for (int i = 0; i < GameUtils.blockNumArr.GetLength(0); i++)
        {
            for (int j = 0; j < GameUtils.blockNumArr.GetLength(1); j++)
            {
                Transform blockTransform = chessBoardTransform.Find("block_" + i.ToString() + j.ToString());
                if (blockTransform != null)
                {
                    if (GameUtils.blockNumArr[i, j] != 0)
                    {
                        blockTransform.GetChild(1).gameObject.SetActive(true);
                        blockTransform.GetChild(1).GetComponent<TextMeshPro>().text = GameUtils.blockNumArr[i, j].ToString();
                    }
                    else
                    {
                        blockTransform.GetChild(1).GetComponent<TextMeshPro>().text = 0.ToString();
                        blockTransform.GetChild(1).gameObject.SetActive(false);
                    }
                }
                else
                {
                    Debug.LogError("Error!");
                }
            }
        }
    }

    // 提取出的共用方法，用于更新块的颜色或数字
    public void UpdateBlockBasedOnType(GameUtils.RollType type, int row, int col, int num, bool updateColor, bool updateNumber)
    {
        if (type == GameUtils.RollType.rowType)
        {
            for (int j = 0; j < 6; j++)
            {
                UpdateBlock(j, col, num, updateColor, updateNumber);
            }
        }
        else if (type == GameUtils.RollType.colType)
        {
            for (int j = 0; j < 5; j++)
            {
                UpdateBlock(row, j, num, updateColor, updateNumber);
            }
        }
        else
        {
            UpdateBlock(row, col, num, updateColor, updateNumber);
            if (row - 1 >= 0)
            {
                UpdateBlock(row - 1, col, num, updateColor, updateNumber);
            }
            if (row + 1 <= 5)
            {
                UpdateBlock(row + 1, col, num, updateColor, updateNumber);
            }
            if (col - 1 >= 0)
            {
                UpdateBlock(row, col - 1, num, updateColor, updateNumber);
            }
            if (col + 1 <= 4)
            {
                UpdateBlock(row, col + 1, num, updateColor, updateNumber);
            }
        }
    }

    //共用方法，根据参数更新方块状态
    private void UpdateBlock(int row, int col, int num, bool updateColor, bool updateNumber)
    {
        Transform blockTransform = chessBoardTransform.Find("block_" + row.ToString() + col.ToString());
        if (blockTransform == null)
        {
            Debug.LogError("未找到方块的 Transform: block_" + row.ToString() + col.ToString());
            return;
        }
        blockTransform.GetChild(0).gameObject.SetActive(updateColor);
        if (updateNumber)
        {
            int currentNum = GameUtils.blockNumArr[row, col] + num;
            GameUtils.blockNumArr[row, col] = currentNum;
            TextMeshPro textMeshPro = blockTransform.GetChild(1).GetComponent<TextMeshPro>();
            textMeshPro.text = currentNum.ToString();
            textMeshPro.gameObject.SetActive(currentNum != 0);
        }
        else
        {
            blockTransform.GetChild(1).gameObject.SetActive(false);
        }
    }

    //检测是否按下攻击键
    public void DetectAttack()
    {
        //在移动端上运行
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(touch.position), Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject.tag == "Attack")
                {
                    selectedObject = hit.collider.gameObject;
                    PlayAttack();
                }
            }
        }
        //在PC,Editor或web上
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.tag == "Attack")
            {
                selectedObject = hit.collider.gameObject;
                PlayAttack();
            }
        }
    }

    //回合开始时取消所有方块颜色
    private void SetBlockColorFalse()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Transform blockTransform = chessBoardTransform.Find("block_" + i.ToString() + j.ToString());
                blockTransform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    // private void PlayFirstRound()
    // {

    // }

    //回合结束时销毁除存储骰子外所有骰子
    public void DestroyRoll()
    {
        foreach (var roll in GameUtils.rollsArr)
        {
            Destroy(roll);
        }
        GameUtils.rollsArr.Clear();
    }

    //玩家点击攻击
    private void PlayAttack()
    {
        for (int i = 0; i < GameUtils.enemysArr.Count; i++)
        {
            GameUtils.enemysArr[i].GetComponent<Enemy>().TakeDamage();
        }
        DelPosArr();
        DestroyRoll();
        GameUtils.delBlockNumArr();
        PlayAIRound();
    }


    //回合结束时销毁pos数组中所有骰子的索引
    public void DelPosArr()
    {
        for (int i = 0; i < GameUtils.rollsArr.Count; i++)
        {
            int row = GameUtils.rollsArr[i].GetComponent<RollController>().row;
            int col = GameUtils.rollsArr[i].GetComponent<RollController>().col;
            GameUtils.RemovePair(row, col);
        }
    }

    //轮到AI的回合
    public void PlayAIRound()
    {
        for (int i = 0; i < GameUtils.enemysArr.Count; i++)
        {
            GameUtils.enemysArr[i].GetComponent<Enemy>().Attack();
            GameUtils.enemysArr[i].GetComponent<Enemy>().Move();
        }
        NextRound();
    }

    //进行下一个回合
    public void NextRound()
    {
        CreateEnemy();  //先创建敌人数组，防止随后创建的骰子位置和敌人重复
        CreateRoll();
        SetBlockNum();
        SetBlockColorFalse();
        SetBlockColor();
        UpdateBlockNum();
    }

    //消灭敌人后增加分数
    public void AddScore()
    {
        int score = int.Parse(curScoreObj.GetComponent<TextMeshPro>().text);
        score += 15;
        curScoreObj.GetComponent<TextMeshPro>().text = score.ToString();
    }

    //更新历史总得分
    public void UpdateHisScore()
    {
        int curScore = int.Parse(curScoreObj.GetComponent<TextMeshPro>().text);
        int hisScore = int.Parse(hisScoreObj.GetComponent<TextMeshPro>().text);
        if (curScore > hisScore)
        {
            hisScoreObj.GetComponent<TextMeshPro>().text = curScore.ToString();
        }
    }

    // Other variables and methods remain unchanged
    public void ClearBlock(int row, int col)
    {
        Transform blockTransform = chessBoardTransform.Find("block_" + row + "_" + col);
        if (blockTransform == null)
        {
            Debug.LogError("未找到方块的 Transform: block_" + row + "_" + col);
            return;
        }

        // Clear color and number visibility
        blockTransform.GetChild(0).gameObject.SetActive(false);
        blockTransform.GetChild(1).gameObject.SetActive(false);

        // Clear blockNumArr based on row and col
        GameUtils.blockNumArr[row, col] = 0;
    }
}