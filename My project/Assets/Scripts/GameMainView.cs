using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
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

    private int level = 1;

    void Awake()
    {
        mainCamera = Camera.main;
        chessBoardTransform = chessBoard.transform;  //缓存棋盘的Transform引用
    }

    void Start()
    {
        PlayFirstRound();
    }

    void Update()
    {
        DetectAttack();
    }

    #region 回合内方法
    // 回合开始时创建三个骰子
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
                // newRoll.transform.position = blockTransform.position;
            }
        }
    }

    // 回合开始时创建敌人
    public void CreateEnemy()
    {
        string filePath = Application.dataPath + "/Config/EnemySpawn.csv";
        var result = ReadCSVFile(filePath);
        int[][] type = ParseData(result, 1);
        int[][] hp = ParseData(result, 2);
        int[][] pos = ParseData(result, 3);
        InstantiateEnemies(type, hp, pos);
        level++;
    }

    // 读取CSV文件，返回数据集
    private DataSet ReadCSVFile(string filePath)
    {
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
            {
                return reader.AsDataSet();
            }
        }
    }

    //解析数据集，返回数组
    private int[][] ParseData(DataSet data, int columnIndex)
    {
        return GameUtils.ParseIntArray2D(data.Tables[0].Rows[level][columnIndex].ToString());
    }

    // 根据获取的数据，实例化敌人对象
    private void InstantiateEnemies(int[][] type, int[][] hp, int[][] pos)
    {
        for (int i = 0; i < type.Length; i++)
        {
            for (int j = 0; j < type[0].Length; j++)
            {
                GameObject newEnemy = Instantiate(enemy[0]);  //先用0，之后再用type中的类型
                UpdateEnemyProperties(newEnemy.GetComponent<Enemy>(), new int[] { hp[i][j], hp[i][j + 1] }, pos[i][j], i == 0 ? 4 : 5);
                GameUtils.enemysArr.Add(newEnemy);
                GameUtils.posArr.Add(new List<int> { newEnemy.GetComponent<Enemy>().row, newEnemy.GetComponent<Enemy>().col });
            }
        }
    }

    // 更新敌人属性，包括HP、位置和行数
    private void UpdateEnemyProperties(Enemy enemy, int[] hpRange, int pos, int row)
    {
        enemy.row = row;
        enemy.col = pos;
        enemy.hp = UnityEngine.Random.Range(hpRange[0], hpRange[1]);
        enemy.transform.position = chessBoardTransform.Find("block_" + enemy.row + enemy.col).position;
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

    #endregion

    //回合结束时销毁除存储骰子外所有骰子
    public void DestroyRoll()
    {
        foreach (var roll in GameUtils.rollsArr)
        {
            Destroy(roll);
        }
        GameUtils.rollsArr.Clear();
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

    // 游戏结束后统一清楚棋盘上所有骰子
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

    #region 回合逻辑
    //开始第一个回合
    private void PlayFirstRound()
    {
        CreateEnemy();
        CreateRoll();  //for test
        SetBlockNum();
        SetBlockColor();
        UpdateBlockNum();
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

    #endregion

    #region 分数相关
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

    #endregion
}