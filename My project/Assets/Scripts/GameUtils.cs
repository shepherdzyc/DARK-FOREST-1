using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameUtils
{
    public enum rollType
    {
        rowType,
        colType,
        aroundType,
    }

    //存放敌人的位置，随时更新
    public static int[][] enenyPos = new int[10][];

    //存放三个骰子的位置，并随时更新，骰子位置不能重叠
    public static int[][] randomPos = new int[3][];

    public static int[,] blockNumArr = new int[6, 5];

    //存放棋盘上所有骰子
    public static List<GameObject> rollsArr = new List<GameObject>();

    //存放棋盘上所有敌人
    public static List<GameObject> enemysArr = new List<GameObject>();

    //存放棋盘中已存在的位置
    public static List<List<int>> posArr = new List<List<int>>();

    //随机创建1~6出三个随机数的数组
    public static int[] CreateRandomNum()
    {
        int[] randomArr = new int[3];
        System.Random rand = new System.Random();
        for (int i = 0; i < 3; i++)
        {
            randomArr[i] = rand.Next(1, 7);
        }
        return randomArr;
    }

    //随机生成6行5列的三个坐标数组，确保坐标不重复
    public static int[][] CreateRandomPos()
    {
        System.Random rand = new System.Random();
        HashSet<(int, int)> coordinatesSet = new HashSet<(int, int)>();
        foreach (List<int> pair in posArr)
        {
            if (pair.Count == 2)
            {
                coordinatesSet.Add((pair[0], pair[1]));
            }
        }
        for (int i = 0; i < 3; i++)
        {
            int x, y;
            do
            {
                x = rand.Next(0, 6);
                y = rand.Next(0, 5);
            } while (!coordinatesSet.Add((x, y)));
            randomPos[i] = new int[] { x, y };
            AddPair(x, y);
        }
        return randomPos;
    }

    //随机生成骰子类型
    public static rollType CreateRandomType()
    {
        rollType[] allTypes = (rollType[])System.Enum.GetValues(typeof(rollType));
        int randomIndex = Random.Range(0, allTypes.Length);
        rollType randomType = allTypes[randomIndex];
        return randomType;
    }

    //查找骰子当前位置是否重复
    public static bool findPos(int x, int y)
    {
        for (int i = 0; i < posArr.Count; i++)
        {
            if (x == posArr[i][0] && y == posArr[i][1])
            {
                return true;
            }
        }
        return false;
    }

    //更新骰子位置
    public static void updatePos(int oldPosX, int oldPosY, int newPosX, int newPosY)
    {
        for (int i = 0; i < posArr.Count; i++)
        {
            if (oldPosX == posArr[i][0] && oldPosY == posArr[i][1])
            {
                posArr[i][0] = newPosX;
                posArr[i][1] = newPosY;
                break;
            }
        }
    }

    //将blockNumArr中所有数字归零
    public static void delBlockNumArr()
    {
        for (int i = 0; i < blockNumArr.GetLength(0); i++)
        {
            for (int j = 0; j < blockNumArr.GetLength(1); j++)
            {
                blockNumArr[i, j] = 0;
            }
        }
    }

    //移除posArr中不需要的坐标
    public static void RemovePair(int x, int y)
    {
        posArr.RemoveAll(pair => pair.Count == 2 && pair[0] == x && pair[1] == y);
    }

    //向posArr中添加坐标
    public static void AddPair(int x, int y)
    {
        List<int> pair = new List<int> { x, y };
        posArr.Add(pair);
    }

    //检查posArr中是否已经存在坐标x和y
    public static bool IsCoordinatePair(int x, int y)
    {
        foreach (List<int> pair in posArr)
        {
            if (pair.Count == 2 && pair[0] == x && pair[1] == y)
            {
                return true;
            }
        }
        return false;
    }
}