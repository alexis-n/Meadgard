using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    #region Enums
    public enum InteractableObjectType { Dish, Client, Waitress, Cook, Table, Counter, Furnace, Shelf};

    public enum CurrentPhase { Prep, Service, Recap};

    public enum Instruction { Empty, GivingInstruction };

    public enum CookState { Free, GettingResource, Cooking, PuttingOrderOnTable};

    public enum OrderState { WaitingToBePicked, WaitingToBeCooked, WaitingToBeServed};

    public enum RessourceType { Food, Drink};
    public enum RessourceDetail { Chicken, Fish, Boar, Pinte, Bottle, Keg};
    public enum SelectableType { Furniture, NPC, Dish};

    public enum FurnitureType { Table, Chair, Queue, Counter, Shelf, CookingStation};
    public enum Race { Human, Elf, Dwarf, Jottun};
    #endregion

    #region Valeur de départ des ressources

    public static int startFoodCapacity = 0;
    public static int startDrinksCapacity = 0;

    public static int startPopularity = 25;

    public static int startPriceFood = 2;
    public static int startPriceDrink = 2;
    #endregion

    [Header("WaitressData")]
    public static float cleanTime = 4f;
}
