using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Client>() && NPCManager.instance.hiredBouncers.Count > 0)
        {
            if (other.GetComponent<Client>().bandit != null)
            {
                var totalPoints = 0;
                foreach (Bouncer item in NPCManager.instance.hiredBouncers)
                {
                    totalPoints += item.employeeValues.employeeSkills[0];
                }
                var chanceToGetCaught = Random.Range(0, 100);
                if (chanceToGetCaught <= totalPoints * 7)
                {
                    for (int i = 0; i < NPCManager.instance.hiredBouncers.Count; i++)
                    {
                        if (NPCManager.instance.hiredBouncers[i].state == Bouncer.State.Free)
                            NPCManager.instance.hiredBouncers[i].KickOut((other.GetComponent<Client>()));
                    }
                }
            }
        }
    }
}
