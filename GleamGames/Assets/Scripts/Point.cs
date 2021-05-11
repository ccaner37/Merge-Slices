using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
	public int PartPoint; 
	public void SetPoint(int x) { PartPoint = x; }

	public int Number { get; private set; }
	public void SetNumber(int y) { Number = y; }

	public GameObject myPartPrefab { get; set; }
	public void SetmyPartPrefab(GameObject z) { myPartPrefab = z; }

}
