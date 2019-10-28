using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    void AddListeners();
    void SaveValues();
    void LoadValues();
}
