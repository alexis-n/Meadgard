using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
    public static God god;
    public GameplayData data;
	
	GameObject tavern;

	private void Awake()
	{
		if (SceneManager.GetActiveScene().name == "Ingame" && !god)
        {
            SceneManager.LoadScene("MainMenu");
        }
		else
		{
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
            else Destroy(this);		
		}
	}

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
