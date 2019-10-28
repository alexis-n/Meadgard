using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCBubble : MonoBehaviour
{
    #region /// VARIABLES ///
    [SerializeField] private GameObject prefab;
    [HideInInspector] public Bubble bubble;
    [HideInInspector] public bool activate = true;
    private NPC npc;
    [HideInInspector] public float timeToWait, timeWaited;

    [SerializeField] private Gradient gradient;
    #endregion

    // Update is called once per frame
    void LateUpdate()
    {
        if (bubble != null)
        {
            Vector3 bubblePos = Camera.main.WorldToScreenPoint(transform.position);
            bubble.rect.position = bubblePos;

            if (npc.GetComponent<Client>() && npc.GetComponent<Client>().state == Client.State.Ordering ||
                npc.GetComponent<Client>() && npc.GetComponent<Client>().state == Client.State.AwaitingDish)
            {
                bubble.filler.color = gradient.Evaluate(npc.GetComponent<Client>().Satisfaction / 100);
                bubble.filler.fillAmount = npc.GetComponent<Client>().Satisfaction / 100;
            }
            else
            {
                timeWaited -= Time.deltaTime;
                bubble.filler.fillAmount = timeWaited / timeToWait;
            }
        }

        if (bubble != null && activate && Camera.main.transform.position.y <= PlayerManager.instance.cameraZoomLimit)
            bubble.gameObject.SetActive(true);
        else if (bubble != null) bubble.gameObject.SetActive(false);
    }

    //Fonction qui fait apparaître la bulle
    public void InstantiateBubble(NPC associatedNPC)
    {
        npc = associatedNPC;
        bubble = Instantiate(prefab, UIManager.instance.canvas.transform).GetComponent<Bubble>();

        if (npc.GetComponent<Employee>())
        {
            bubble.filler.color = npc.data.color;
            bubble.diamondObj.SetActive(true);
            bubble.diamondColor.color = npc.GetComponent<Employee>().diamondColor;
        }
        else bubble.diamondObj.SetActive(false);

        Vector3 bubblePos = Camera.main.WorldToScreenPoint(this.transform.position);
        bubble.rect.position = bubblePos;
        bubble.rect.SetAsFirstSibling();
    }

    //fonction qui active la bulle
    public void ActivateBubble(float time)
    {
        bubble.filler.fillAmount = 1;
        timeToWait = time;
        timeWaited = timeToWait;
    }

}
