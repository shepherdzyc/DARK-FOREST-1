using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class RollController : MonoBehaviour
{
    public GameObject chessBoard;

    public GameObject storageBoard;

    private GameObject selectedObject;

    public int row;
    public int col;
    public int num;

    public GameUtils.rollType type;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
                    if (hit.collider != null)
                    {
                        selectedObject = hit.collider.gameObject;
                    }
                    break;

                case TouchPhase.Moved:
                    if (selectedObject != null)
                    {
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
                        worldPos.z = selectedObject.transform.position.z;
                        Vector3 blockPos = getBoardPos(selectedObject.transform.position, worldPos);
                        selectedObject.transform.position = blockPos;
                    }
                    break;

                case TouchPhase.Ended:
                    selectedObject = null;
                    break;
            }
        }
    }
    private Vector2 getBoardPos(Vector3 beginPos, Vector3 worldPos)
    {
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                bool alreadyChosen = GameUtils.findPos(x, y);
                string blockName = "block_" + x.ToString() + y.ToString();
                Transform blockTransform = chessBoard.transform.Find(blockName);
                if (blockTransform != null && !alreadyChosen && Vector2.Distance(worldPos, blockTransform.position) <= 40)
                {
                    GameUtils.updatePos(selectedObject.GetComponent<RollController>().row, selectedObject.GetComponent<RollController>().col, x, y);
                    moveBlockFalse(selectedObject.GetComponent<RollController>().row, selectedObject.GetComponent<RollController>().col);
                    selectedObject.GetComponent<RollController>().row = x;
                    selectedObject.GetComponent<RollController>().col = y;
                    for (int i = 0; i < GameUtils.blockNumArr.GetLength(0); i++)
                    {
                        for (int j = 0; j < GameUtils.blockNumArr.GetLength(1); j++)
                        {
                            GameUtils.blockNumArr[i, j] = 0;
                        }
                    }
                    chessBoard.GetComponent<GameMainView>().setBlockNum();
                    return blockTransform.position;
                }
            }
        }
        return beginPos;
        // return storageRoll(beginPos, worldPos);
    }

    //将骰子存储起来
    private Vector2 storageRoll(Vector3 beginPos, Vector3 worldPos)
    {
        if (Vector2.Distance(worldPos, storageBoard.transform.position) <= 40)
        {
            return storageBoard.transform.position;
        }
        else
        {
            return beginPos;
        }
    }

    //移动时更新背景颜色和数字为不可见
    public void moveBlockFalse(int row, int col)
    {
        if (selectedObject.GetComponent<RollController>().type == GameUtils.rollType.rowType)
        {
            for (int j = 0; j < 6; j++)
            {
                string blockName = "block_" + j.ToString() + col.ToString();
                Transform blockTransform = chessBoard.transform.Find(blockName);
                blockTransform.GetChild(0).gameObject.SetActive(false);
                blockTransform.GetChild(1).gameObject.SetActive(false);
            }
        }
        else if (selectedObject.GetComponent<RollController>().type == GameUtils.rollType.colType)
        {
            for (int j = 0; j < 5; j++)
            {
                string blockName = "block_" + row.ToString() + j.ToString();
                Transform blockTransform = chessBoard.transform.Find(blockName);
                blockTransform.GetChild(0).gameObject.SetActive(false);
                blockTransform.GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            Transform blockTransform = chessBoard.transform.Find("block_" + row.ToString() + col.ToString());
            blockTransform.GetChild(0).gameObject.SetActive(false);
            blockTransform.GetChild(1).gameObject.SetActive(false);
            if (row - 1 >= 0)
            {
                Transform blockTransform1 = chessBoard.transform.Find("block_" + (row - 1).ToString() + col.ToString());
                blockTransform1.GetChild(0).gameObject.SetActive(false);
                blockTransform1.GetChild(1).gameObject.SetActive(false);
            }
            if (row + 1 <= 5)
            {
                Transform blockTransform2 = chessBoard.transform.Find("block_" + (row + 1).ToString() + col.ToString());
                blockTransform2.GetChild(0).gameObject.SetActive(false);
                blockTransform2.GetChild(1).gameObject.SetActive(false);
            }
            if (col - 1 >= 0)
            {
                Transform blockTransform3 = chessBoard.transform.Find("block_" + row.ToString() + (col - 1).ToString());
                blockTransform3.GetChild(0).gameObject.SetActive(false);
                blockTransform3.GetChild(1).gameObject.SetActive(false);
            }
            if (col + 1 <= 4)
            {
                Transform blockTransform4 = chessBoard.transform.Find("block_" + row.ToString() + (col + 1).ToString());
                blockTransform4.GetChild(0).gameObject.SetActive(false);
                blockTransform4.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
}