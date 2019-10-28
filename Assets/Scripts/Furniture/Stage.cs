using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public Chair[] chairs;
    public List<Transform> bardPositions = new List<Transform>();

    private void Start()
    {
        foreach (Chair chair in chairs)
        {
            PlayerManager.instance.tavern.availableStageChairs.Add(chair);
        }
    }
}
