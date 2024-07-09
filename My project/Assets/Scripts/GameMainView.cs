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

    public GameObject enemy;

    public GameObject curScoreObj;

    public GameObject hisScoreObj;

    private Camera mainCamera;

    private GameObject selectedObject;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        CreateEnemy();
        CreateRoll();  //for test
        setBlockNum();
        updateBlockColor();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            DetectAttack();
        }
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
                GameUtils.rollsArr.Add(newRoll);
                newRoll.transform.position = blockTransform.position;
            }
        }
    }

    //回合开始时随机创建敌人
    public void CreateEnemy()
    {
        //先写死 测试
        GameObject newEnemy = Instantiate(enemy);
        GameUtils.enemysArr.Add(newEnemy);
        newEnemy.GetComponent<Enemy>().row = Random.Range(4, 6);
        newEnemy.GetComponent<Enemy>().col = Random.Range(0, 5);
        GameUtils.posArr.Add(new List<int> { newEnemy.GetComponent<Enemy>().row, newEnemy.GetComponent<Enemy>().col });  //for test
        newEnemy.transform.position = chessBoard.transform.Find("block_" + newEnemy.GetComponent<Enemy>().row.ToString()
        + newEnemy.GetComponent<Enemy>().col.ToString()).position;
    }

    //更新方块颜色
    public void updateBlockColor()
    {
        if (GameUtils.rollsArr.Count == 0)
        {
            return;
        }
        for (int i = 0; i < GameUtils.rollsArr.Count; i++)
        {
            GameUtils.rollType type = GameUtils.rollsArr[i].GetComponent<RollController>().type;
            int row = GameUtils.rollsArr[i].GetComponent<RollController>().row;
            int col = GameUtils.rollsArr[i].GetComponent<RollController>().col;
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
        if (GameUtils.rollsArr.Count == 0)
        {
            return;
        }
        for (int i = 0; i < GameUtils.rollsArr.Count; i++)
        {
            GameUtils.rollType type = GameUtils.rollsArr[i].GetComponent<RollController>().type;
            int row = GameUtils.rollsArr[i].GetComponent<RollController>().row;
            int col = GameUtils.rollsArr[i].GetComponent<RollController>().col;
            int num = GameUtils.rollsArr[i].GetComponent<RollController>().num;
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
                Transform blockTransform = chessBoard.transform.Find("block_" + i.ToString() + j.ToString());
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
        }
    }

    //检测是否按下攻击键
    public void DetectAttack()
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

    private void setBlockColorFalse()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Transform blockTransform = chessBoard.transform.Find("block_" + i.ToString() + j.ToString());
                blockTransform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private void PlayFirstRound()
    {

    }

    //回合结束时销毁除存储骰子外所有骰子
    public void DestroyRoll()
    {
        for (int i = 0; i < GameUtils.rollsArr.Count; i++)
        {
            Destroy(GameUtils.rollsArr[i]);
            GameUtils.rollsArr.RemoveAt(i);
            i--;
        }
    }

    //
    private void PlayAttack()
    {
        for (int i = 0; i < GameUtils.enemysArr.Count; i++)
        {
            GameUtils.enemysArr[i].GetComponent<Enemy>().TakeDamage();
        }
        delPosArr();
        DestroyRoll();
        GameUtils.delBlockNumArr();
        PlayAIRound();
    }


    //回合结束时销毁pos数组中所有骰子和敌人的索引
    public void delPosArr()
    {
        for (int i = 0; i < GameUtils.rollsArr.Count; i++)
        {
            int row = GameUtils.rollsArr[i].GetComponent<RollController>().row;
            int col = GameUtils.rollsArr[i].GetComponent<RollController>().col;
            GameUtils.RemovePair(row, col);
        }
        for (int i = 0; i < GameUtils.enemysArr.Count; i++)
        {
            int row = GameUtils.enemysArr[i].GetComponent<Enemy>().row;
            int col = GameUtils.enemysArr[i].GetComponent<Enemy>().col;
            GameUtils.RemovePair(row, col);
        }
    }

    //轮到AI的回合
    public void PlayAIRound()
    {
        for (int i = 0; i < GameUtils.enemysArr.Count; i++)
        {
            GameUtils.enemysArr[i].GetComponent<Enemy>().Move();
        }
        NextRound();
    }

    //进行下一个回合
    public void NextRound()
    {
        CreateRoll();  //for test
        CreateEnemy();
        setBlockNum();
        setBlockColorFalse();
        updateBlockColor();
    }

    public void AddScore()
    {
        int score = int.Parse(curScoreObj.GetComponent<TextMeshPro>().text);
        score += 15;
        curScoreObj.GetComponent<TextMeshPro>().text = score.ToString();
    }

    public void UpdateHisScore()
    {
        int curScore = int.Parse(curScoreObj.GetComponent<TextMeshPro>().text);
        int hisScore = int.Parse(hisScoreObj.GetComponent<TextMeshPro>().text);
        if (curScore > hisScore)
        {
            hisScoreObj.GetComponent<TextMeshPro>().text = curScore.ToString();
        }
    }
}