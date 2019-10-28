using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meadgapedia : Menu
{
    [SerializeField] private GameObject firstPage;

    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    //focntion qui permet la meadgapedia à la page souhaitée
    public void OpenAtPage(GameObject page)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        page.transform.SetAsLastSibling();
    }
}
