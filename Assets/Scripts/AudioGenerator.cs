using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGenerator : MonoBehaviour
{
    [SerializeField]
    ClientData clientData;
    [SerializeField]
    AudioSource source;

    AudioClip clip;

    [SerializeField] private Client client;
    Order order;
   
   
    void Start(){
     
     StartCoroutine("PlayNpcStateSound");
    }
    //Coroutine qui par la classe Client récupère l'état du client et charge le clip audio correspondant a celui ci puis le joue
    IEnumerator PlayNpcStateSound()
    {
        while (true)
        {
            switch(client.state)
            {
            case Client.State.Ordering:
             if(client.satisfaction <= GameManager.instance.data.unhappyThreshold){

                Random.InitState(System.DateTime.Now.Millisecond);
                source.clip = clientData.talksBad[Random.Range(0,clientData.talksBad.Length)];
                source.pitch = Random.Range(0.9f,1.1f);
                  source.Play();

             }
             else{
                 Random.InitState(System.DateTime.Now.Millisecond);
                source.clip = clientData.talksGood[Random.Range(0,clientData.talksGood.Length)];
                source.pitch = Random.Range(0.9f,1.1f);
                  source.Play();

            }
            break;

            case Client.State.Consuming:
            if (client.order.ressourceType == Data.RessourceType.Food)
            {
                Random.InitState(System.DateTime.Now.Millisecond);
                source.clip = clientData.eat[Random.Range(0, clientData.eat.Length)];
                source.pitch = Random.Range(0.9f,1.1f);
                  source.Play();
            }
            else{
                Random.InitState(System.DateTime.Now.Millisecond);
                source.clip = clientData.drink[Random.Range(0, clientData.drink.Length)];
                source.pitch = Random.Range(0.9f,1.1f);
                  source.Play();
            }
            break;

            default:
            
            source.Stop();
            source.clip = null;

            break;
            }

            float temp = 0f;
            if (source.clip) 
            {
                temp = source.clip.length;
            }
            //var timeToWait = Random.Range(temp, temp+2f);
            yield return new WaitForSeconds(temp);
        }
    }

    // Update is called once per frame

}
