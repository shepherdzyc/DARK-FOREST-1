using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuView : MonoBehaviour
{
    public GameObject itemCell;

    public Transform content;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RankButton()
    {

    }

    public void RegisterButton()
    {

    }

    public void LoginButton()
    {

    }

    // 更新排行榜方法
    public void UpdateLeaderboard(string[] playerNames, int[] scores)
    {
        // 清空现有的排行榜
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 生成新的排行榜项
        for (int i = 0; i < playerNames.Length; i++)
        {
            GameObject newItem = Instantiate(itemCell);
            Text[] texts = newItem.GetComponentsInChildren<Text>();
            texts[0].text = playerNames[i]; // 玩家名
            texts[1].text = scores[i].ToString(); // 分数
        }

        // 更新content的height以适应所有的排行榜项
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, playerNames.Length * itemCell.GetComponent<RectTransform>().sizeDelta.y);
    }
}
