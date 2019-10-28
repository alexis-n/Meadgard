using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimStates{

    public static int sit = Animator.StringToHash("Sit");
    public static int walk = Animator.StringToHash("Walk");
    public static int drunk = Animator.StringToHash("Drunk");
    public static int eat = Animator.StringToHash("Eat");
    public static int drink = Animator.StringToHash("Drink");

    public static int idle = Animator.StringToHash("Idle");

    public static int action = Animator.StringToHash("Action");
    public static int clean = Animator.StringToHash("Clean");
    public static int hold = Animator.StringToHash("Hold");
    public static int kick = Animator.StringToHash("Kick");

    public static int instrument = Animator.StringToHash("Instrument");
}
