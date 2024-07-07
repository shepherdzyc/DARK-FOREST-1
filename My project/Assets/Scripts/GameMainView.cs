using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;

public class GameMainView : MonoBehaviour
{
    public GameObject chessBoard;

    public GameObject[] rolls;

    private Camera mainCamera;

    private GameObject[] createdRollArr;

    void Awake()
    {
        mainCamera = Camera.main;
        createdRollArr = new GameObject[3];
    }

    void Start()
    {
        CreateRoll();  //for test
    }

    void Update()
    {
        setBlockBg(true);
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

    }

    //回合结束时除存储骰子外所有骰子
    public void DestroyRoll()
    {
        for (int i = 0; i < createdRollArr.Length; i++)
        {
            Destroy(createdRollArr[i]);
        }
    }

    public void setBlockBg(bool isRed)
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
                    blockTransform.GetChild(0).gameObject.SetActive(isRed);
                }
            }
            else if (type == GameUtils.rollType.colType)
            {
                for (int j = 0; j < 5; j++)
                {
                    string blockName = "block_" + row.ToString() + j.ToString();
                    Transform blockTransform = chessBoard.transform.Find(blockName);
                    blockTransform.GetChild(0).gameObject.SetActive(isRed);
                }
            }
            else
            {
                Transform blockTransform = chessBoard.transform.Find("block_" + row.ToString() + col.ToString());
                blockTransform.GetChild(0).gameObject.SetActive(isRed);
                if (row - 1 >= 0)
                {
                    Transform blockTransform1 = chessBoard.transform.Find("block_" + (row - 1).ToString() + col.ToString());
                    blockTransform1.GetChild(0).gameObject.SetActive(isRed);
                }
                if (row + 1 <= 5)
                {
                    Transform blockTransform2 = chessBoard.transform.Find("block_" + (row + 1).ToString() + col.ToString());
                    blockTransform2.GetChild(0).gameObject.SetActive(isRed);
                }
                if (col - 1 >= 0)
                {
                    Transform blockTransform3 = chessBoard.transform.Find("block_" + row.ToString() + (col - 1).ToString());
                    blockTransform3.GetChild(0).gameObject.SetActive(isRed);
                }
                if (col + 1 <= 4)
                {
                    Transform blockTransform4 = chessBoard.transform.Find("block_" + row.ToString() + (col + 1).ToString());
                    blockTransform4.GetChild(0).gameObject.SetActive(isRed);
                }
            }
        }
    }
}