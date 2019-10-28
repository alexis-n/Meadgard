using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IngameDebug: MonoBehaviour
{
    public static IngameDebug instance;
    public UnityEvent Method1 = new UnityEvent();
    public UnityEvent Method2 = new UnityEvent();

    public bool debugMode = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this);
    }

    public void DebugMode()
    {
        debugMode = !debugMode;
        UIManager.instance.DebugInfo(debugMode);
    }

    public void GiveMoney()
    {
        PlayerManager.instance.PlayerMoney = 999;
    }

    public void FillFood()
    {
        PlayerManager.instance.Restock((PlayerManager.instance.PlayerFoodCapacity - PlayerManager.instance.PlayerFood), 0);
    }

    public void FillDrinks()
    {
        PlayerManager.instance.Restock(0, (PlayerManager.instance.PlayerDrinksCapacity - PlayerManager.instance.PlayerDrinks));
    }

    public void HireWaitress()
    {
        if (NPCManager.instance.applyingWaitresses.Count > 0)
            NPCManager.instance.HireEmployee(NPCManager.instance.applyingWaitresses[0], false);
    }

    public void HireCook()
    {
        if (NPCManager.instance.applyingCooks.Count > 0)
            NPCManager.instance.HireEmployee(NPCManager.instance.applyingCooks[0], false);
    }

    public void DebugMethod1()
    {
        //remplir avec ce que l'on veut tester
        Method1.Invoke();
    }
    public void DebugMethod2()
    {
        //remplir avec ce que l'on veut tester
        Method2.Invoke();
    }
}
