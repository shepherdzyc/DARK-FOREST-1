using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class Leaderboard : MonoBehaviour
{
    string leaderboardID = "23449";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator SubmitScoreRoutine(int scoreToUpload)
    {
        bool done = false;
        string playerID = PlayerPrefs.GetString("PlayerID");
        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, leaderboardID, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully uploaded score");
                done = true;
            }
            else
            {
                Debug.Log("Failed" + response.errorData);
            }
        });
        yield return new WaitWhile(() => done == false);
    }
}
