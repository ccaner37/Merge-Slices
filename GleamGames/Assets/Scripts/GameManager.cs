using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public int score = 2;
	public Text scoreText;

	private static GameManager instance = null;
	public static GameManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject("GameManager").AddComponent<GameManager>();
			}

			return instance;
		}
	}

	private void OnEnable()
	{
		instance = this;
	}

    private void Start()
    {
		InvokeRepeating("UpdateScore", 1, 1);
    }

    void UpdateScore()
    {
		scoreText.text = "Score: " + score;
    }

}
