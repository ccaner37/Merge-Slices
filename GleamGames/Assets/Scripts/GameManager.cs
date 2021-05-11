using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public int score = 2;
	public Text scoreText;
	public Text experience;

	public Text currentLevel;
	public Text nextLevel;

	public Image fillBar;
	float exp;
	float goalExp = 25;
	float expReq;
	float level;
	float value;
	float value2;

	Material material;

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

		level = 1;
    }

    void UpdateScore()
    {
		scoreText.text = score.ToString() + "\n Next Unlock: 16";
    }

	public void GiveColor(int colornumber, GameObject part)
    {
		Material newMat;

		newMat = Resources.Load(colornumber.ToString(), typeof(Material)) as Material;

		part.GetComponent<Renderer>().material = newMat;

		GiveExperience(colornumber);

	}

	void GiveExperience(int number)
    {
		exp += number * 2;
		experience.text = "Exp: " + exp.ToString();
		UpdateBar();
    }

	void UpdateBar()
    {
		currentLevel.text = level.ToString();
		nextLevel.text = (level + 1).ToString();

        if (exp >= goalExp)
        {
			level++;
			goalExp = level * 25 + level * 1.25f;
			fillBar.fillAmount = 0;
        }

		expReq = goalExp - exp;

		value = exp / goalExp;


	//	print(value);
	}

    private void Update()
    {
		fillBar.fillAmount = Mathf.Lerp(fillBar.fillAmount, value, 5 * Time.deltaTime);
	}

}
