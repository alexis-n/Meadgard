using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Name Generator", menuName = "Name Generator", order = 1)]
public class NPCNames : ScriptableObject
{
    [SerializeField]
    private string[] dwarfFirstNames, dwarfPrefixes, dwardSuffixes,
        elfNames, elfTitles,
        humanFemaleNames, humanMaleNames,
        jottunNames, jottunTitles,
        thiefNames;

    public string HumanFemale()
    {
        var firstName = humanFemaleNames[Random.Range(0, humanFemaleNames.Length)];
        var lastName = (humanFemaleNames[Random.Range(0, humanFemaleNames.Length)] + "dottir");
        return firstName + " " + lastName;
    }

    public string HumanMale()
    {
        var firstName = humanMaleNames[Random.Range(0, humanMaleNames.Length)];
        var lastName = (humanMaleNames[Random.Range(0, humanMaleNames.Length)] + "sson");
        return firstName + " " + lastName;
    }

    public string Dwarf()
    {
        var firstName = dwarfFirstNames[Random.Range(0, dwarfFirstNames.Length)];
        var lastName = (dwardSuffixes[Random.Range(0, dwardSuffixes.Length)] + dwarfPrefixes[Random.Range(0, dwarfPrefixes.Length)]);
        return firstName + " " + lastName;
    }

    public string Elf()
    {
        var firstName = elfTitles[Random.Range(0, elfTitles.Length)];
        var lastName = elfNames[Random.Range(0, elfNames.Length)];
        return firstName + " " + lastName;
    }

    public string Jottun()
    {
        var firstName = jottunNames[Random.Range(0, jottunNames.Length)];
        var lastName = jottunTitles[Random.Range(0, jottunTitles.Length)];
        return firstName + " " + lastName;
    }

    public string Thief()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        return thiefNames[Random.Range(0, thiefNames.Length)];
    }
}
