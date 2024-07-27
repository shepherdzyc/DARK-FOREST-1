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

    public GameUtils.RollType type;

    private bool isStorage = false;

    [SerializeField]
    private float speed = 500f;  // 初始速度

    private bool isMoving = false;  // 用于跟踪骰子是否正在移动

    void Start()
    {
        transform.position = new Vector3(-500, 0, 0);
        StartParabolaMove();
    }

    private void FixedUpdate()
    {
    }

    private void Update()
    {
        if (isMoving)
        {
            return;  // 如果骰子正在移动，禁止拖动
        }

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
                        if (Vector2.Distance(worldPos, storageBoard.transform.position) <= 40 && !isStorage)
                        {
                            storageRoll();
                            selectedObject.transform.position = storageBoard.transform.position;
                        }
                        else if (selectedObject.transform.position == storageBoard.transform.position && isStorage)
                        {
                            UseStorageRoll(worldPos);
                        }
                        else
                        {
                            selectedObject.transform.position = getBoardPos(selectedObject.transform.position, worldPos);
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    if (selectedObject != null)
                    {
                        // 移动结束时更新游戏主视图的方块状态
                        selectedObject = null;
                    }
                    break;
            }
        }

        // 如果是在PC,Editor或web上,使用鼠标事件处理
        // if (Input.GetMouseButtonDown(0))
        // {
        //     HandleTouchEvents(Input.mousePosition, TouchPhase.Began);
        // }
        // else if (Input.GetMouseButtonUp(0))
        // {
        //     HandleTouchEvents(Input.mousePosition, TouchPhase.Ended);
        // }
        // else if (Input.GetMouseButton(0))
        // {
        //     HandleTouchEvents(Input.mousePosition, TouchPhase.Moved);
        // }
    }

    // private void HandleTouchEvents(Vector3 position, TouchPhase phase)
    // {
    //     switch (phase)
    //     {
    //         case TouchPhase.Began:
    //             RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position), Vector2.zero);
    //             if (hit.collider != null)
    //             {
    //                 selectedObject = hit.collider.gameObject;
    //             }
    //             break;

    //         case TouchPhase.Moved:
    //             if (selectedObject != null)
    //             {
    //                 Vector3 worldPos = Camera.main.ScreenToWorldPoint(position);
    //                 worldPos.z = selectedObject.transform.position.z;
    //                 if (Vector2.Distance(worldPos, storageBoard.transform.position) <= 40 && !isStorage)
    //                 {
    //                     storageRoll();
    //                     selectedObject.transform.position = storageBoard.transform.position;
    //                 }
    //                 else if (selectedObject.transform.position == storageBoard.transform.position && isStorage)
    //                 {
    //                     UseStorageRoll(worldPos);
    //                 }
    //                 else
    //                 {
    //                     selectedObject.transform.position = getBoardPos(selectedObject.transform.position, worldPos);
    //                 }
    //             }
    //             break;

    //         case TouchPhase.Ended:
    //             if (selectedObject != null)
    //             {
    //                 // 移动结束时更新游戏主视图的方块状态
    //                 selectedObject = null;
    //             }
    //             break;
    //     }
    // }

    // //骰子移动时的判断
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
                    GameUtils.delBlockNumArr();
                    chessBoard.GetComponent<GameMainView>().SetBlockNum();
                    chessBoard.GetComponent<GameMainView>().SetBlockColor();
                    chessBoard.GetComponent<GameMainView>().UpdateBlockNum();
                    return blockTransform.position;
                }
            }
        }
        return beginPos;
    }

    //存储棋盘上骰子
    private void storageRoll()
    {
        int x = selectedObject.GetComponent<RollController>().row;
        int y = selectedObject.GetComponent<RollController>().col;
        moveBlockFalse(selectedObject.GetComponent<RollController>().row, selectedObject.GetComponent<RollController>().col);
        GameUtils.rollsArr.Remove(selectedObject);
        GameUtils.RemovePosPair(x, y);
        GameUtils.delBlockNumArr();
        chessBoard.GetComponent<GameMainView>().SetBlockNum();
        chessBoard.GetComponent<GameMainView>().SetBlockColor();
        chessBoard.GetComponent<GameMainView>().UpdateBlockNum();
        isStorage = true;
    }

    //使用已经存储起来的骰子
    private void UseStorageRoll(Vector3 worldPos)
    {
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                string blockName = "block_" + x.ToString() + y.ToString();
                Transform blockTransform = chessBoard.transform.Find(blockName);
                if (blockTransform != null && Vector2.Distance(worldPos, blockTransform.position) <= 40)
                {
                    bool alreadyChosen = GameUtils.findPos(x, y);
                    if (alreadyChosen)
                    {
                        return;
                    }
                    else
                    {
                        selectedObject.transform.position = blockTransform.position;
                        GameUtils.rollsArr.Add(selectedObject);
                        selectedObject.GetComponent<RollController>().row = x;
                        selectedObject.GetComponent<RollController>().col = y;
                        GameUtils.AddPosPair(x, y);
                        GameUtils.updatePos(selectedObject.GetComponent<RollController>().row, selectedObject.GetComponent<RollController>().col, x, y);
                        GameUtils.delBlockNumArr();
                        chessBoard.GetComponent<GameMainView>().SetBlockNum();
                        chessBoard.GetComponent<GameMainView>().SetBlockColor();
                        chessBoard.GetComponent<GameMainView>().UpdateBlockNum();
                        isStorage = false;
                        return;
                    }
                }
            }
        }
    }
    //移动时更新背景颜色和数字为不可见
    public void moveBlockFalse(int row, int col)
    {
        if (selectedObject.GetComponent<RollController>().type == GameUtils.RollType.rowType)
        {
            for (int j = 0; j < 6; j++)
            {
                string blockName = "block_" + j.ToString() + col.ToString();
                Transform blockTransform = chessBoard.transform.Find(blockName);
                blockTransform.GetChild(0).gameObject.SetActive(false);
                blockTransform.GetChild(1).gameObject.SetActive(false);
            }
        }
        else if (selectedObject.GetComponent<RollController>().type == GameUtils.RollType.colType)
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

    //使骰子能够沿着曲线进行运动
    public void StartParabolaMove()
    {
        string blockName = "block_" + row.ToString() + col.ToString();
        Transform blockTransform = chessBoard.transform.Find(blockName);
        Vector2 start = transform.position;
        Vector2 end = blockTransform.position;
        float journeyLength = Vector2.Distance(start, end);

        isMoving = true;  // 开始移动时设置为true
        StartCoroutine(MoveAlongParabola(start, end, journeyLength));
    }

    private IEnumerator MoveAlongParabola(Vector2 start, Vector2 end, float journeyLength)
    {
        float time = 0f;
        float totalTime = journeyLength / speed; // 总移动时间

        while (time < totalTime)
        {
            float fracJourney = time / totalTime;
            time += Time.deltaTime;

            float height = Mathf.Sin(fracJourney * Mathf.PI) * journeyLength / 3;
            transform.position = Vector2.Lerp(start, end, fracJourney) + Vector2.up * height;

            yield return null;
        }

        transform.position = end; // 确保物体移动到完全的结束位置
        isMoving = false;  // 移动结束时设置为false
    }
}