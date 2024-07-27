using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBoardController : MonoBehaviour
{
    public bool isOccupied = false;

    // Method to check if the slot is occupied
    public bool CheckOccupied()
    {
        return isOccupied;
    }

    // Method to set the slot as occupied
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
}
