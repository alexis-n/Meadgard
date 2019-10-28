using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour {

    [Header("Ce script ne fonctionnera que si l'objet auquel il est attaché possède des enfants " +
        "appelés <b>destination</b> et <b>plate</b> et qu'il est dans la hiérarchie d'un <b>TableSystem</b>")]
    [HideInInspector] public GameObject destination, plate;
    [HideInInspector] public Table table;
    [HideInInspector] public Stage stage;
    public int chairID;

    private void Start() {
        destination = transform.Find("destination").gameObject;
        plate = transform.Find("plate").gameObject;
    }
}
