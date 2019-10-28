using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queue : MonoBehaviour
{
    #region ///  VARIABLES  ///
    [HideInInspector] public List<GameObject> freeSpaces;
    [HideInInspector] public List<GameObject> takenSpaces;
    #endregion

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
             freeSpaces.Add(transform.GetChild(i).gameObject);
        }
    }

    //Donne une position dans la queue
    public GameObject GivePositionInQueue()
    {
        GameObject temp = freeSpaces[0];
        SwitchQueueSpace(temp);
        return temp;
    }

    //Fonction qui libère ou réserve des places dans la queue
    public void SwitchQueueSpace(GameObject placeToSwitch)
    {
        if (freeSpaces.Contains(placeToSwitch))
        {
            freeSpaces.Remove(placeToSwitch);
            takenSpaces.Add(placeToSwitch);
        }

        else if (takenSpaces.Contains(placeToSwitch))
        {
            takenSpaces.Remove(placeToSwitch);
            freeSpaces.Add(placeToSwitch);
        }
    }
}

    #region ///  VIEUX CODE  ///
/*
public class Queue : Furniture {

public List<GameObject> freeQueuePositions = new List<GameObject>(), takenQueuePositions = new List<GameObject>();

// Use this for initialization
private void Start() {
    base.Initialization();
    type = Data.FurnitureType.Queue;
}

public GameObject GivePositionInQueue()
{
    GameObject temp = freeQueuePositions[Random.Range(0, freeQueuePositions.Count)];
    freeQueuePositions.Remove(temp);
    takenQueuePositions.Add(temp);
    if (freeQueuePositions.Count == 0) full = true;
    return temp;
}

public void FreePositionInQueue(GameObject positionToFree)
{
    takenQueuePositions.Remove(positionToFree);
    freeQueuePositions.Add(positionToFree);
    full = false;
}
*/
    #endregion
