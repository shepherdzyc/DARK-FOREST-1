using System.Collections;
using System.Collections.Generic;
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

    //存放三个骰子的位置，并随时更新，骰子位置不能重叠
    public static int[][] randomPos = new int[3][];

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
        for (int i = 0; i < 3; i++)
        {
            int x, y;
            do
            {
                x = rand.Next(0, 6);
                y = rand.Next(0, 5);
            } while (!coordinatesSet.Add((x, y)));
            randomPos[i] = new int[] { x, y };
        }
        return randomPos;
    }

    public static rollType CreateRandomType()
    {
        rollType[] allTypes = (rollType[])System.Enum.GetValues(typeof(rollType));
        int randomIndex = Random.Range(0, allTypes.Length);
        rollType randomType = allTypes[randomIndex];
        return randomType;
    }

    public static bool findPos(int x, int y)
    {
        for (int i = 0; i < randomPos.Length; i++)
        {
            if (x == randomPos[i][0] && y == randomPos[i][1])
            {
                return true;
            }
        }
        return false;
    }

    public static void updatePos(int oldPosX, int oldPosY, int newPosX, int newPosY)
    {
        for (int i = 0; i < randomPos.Length; i++)
        {
            if (oldPosX == randomPos[i][0] && oldPosY == randomPos[i][1])
            {
                randomPos[i][0] = newPosX;
                randomPos[i][1] = newPosY;
                break;
            }
        }
    }
}
