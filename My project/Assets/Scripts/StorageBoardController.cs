using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class StorageBoardController : MonoBehaviour
{
    public bool isOccupied = false;

    public GameObject roll;

    private Sprite sprite;

    // 对已经被骰子占用的方格进行标记
    public bool CheckOccupied()
    {
        return isOccupied;
    }

    private void Update()
    {

    }

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }

    public void AddBoard()
    {
        if (gameObject.tag == "Add" && roll != null)
        {
            GameUtils.UpRound++;
        }
        if (GameUtils.UpRound == 2 && gameObject.tag == "Add" && roll != null)
        {
            if (roll.GetComponent<RollController>().num < 6)
            {
                roll.GetComponent<RollController>().num++;
                int num = roll.GetComponent<RollController>().num;
                if (File.Exists(Path.Combine(Application.dataPath, "Resources/Arts/Roll/Roll_" + num.ToString() + ".png")))
                {
                    sprite = Resources.Load<Sprite>("Arts/Roll/Roll_" + num.ToString());
                    roll.GetComponent<SpriteRenderer>().sprite = sprite;
                }
                else
                {
                    Debug.Log("资源不存在");
                }
            }
            GameUtils.UpRound = 0;
        }
    }

    public void FrozenBoard()
    {
        if (gameObject.tag == "Frozen" && roll != null)
        {
            GameUtils.FrozenRound++;
        }
        if (GameUtils.FrozenRound == 2 && gameObject.tag == "Frozen" && roll != null)
        {
            roll.GetComponent<RollController>().isFrozen = true;
            roll.GetComponent<RollController>().type = GameUtils.RollType.frozenType;
            GameUtils.FrozenRound = 0;
        }
    }

    public void FireBoard()
    {
        if (gameObject.tag == "Fire" && roll != null)
        {
            GameUtils.FireRound++;
        }
        if (GameUtils.FireRound == 2 && gameObject.tag == "Fire" && roll != null)
        {
            roll.GetComponent<RollController>().isFire = true;
            roll.GetComponent<RollController>().type = GameUtils.RollType.fireType;
            GameUtils.FireRound = 0;
        }
    }
}