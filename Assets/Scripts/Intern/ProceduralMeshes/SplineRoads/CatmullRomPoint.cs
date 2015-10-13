using UnityEngine;
using System.Collections;

public class CatmullRomPoint : MonoBehaviour {

	[SerializeField] [Range(0,100)] float weight = 10; 

	public float Weight {
		get {
			return weight;
		}
		set {
			weight = value;
		}
	}
}
