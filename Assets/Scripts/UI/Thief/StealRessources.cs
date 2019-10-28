using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealRessources : ThiefMission
{
    [SerializeField] private int stolenAmount = 10;

    public override void Success(bool critical)
    { 
        var temp = stolenAmount;
        if (critical) temp = temp*2;

        base.Success(critical);

        PlayerManager.instance.Restock(temp, temp);
    }
}
