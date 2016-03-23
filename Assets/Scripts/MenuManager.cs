using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	private float logoTimer;
	private Animator animator;

	void Start () {
		logoTimer = Time.time;
		animator = ((Animator)GameObject.Find("LogoAlderaans").GetComponent<Animator>());
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time >= logoTimer + 8F && animator.GetCurrentAnimatorStateInfo (0).IsName ("Stationnary"))
		{
			animator.Play("LogoAnim", -1, 0F);
			logoTimer = Time.time + animator.GetCurrentAnimatorStateInfo (0).length;
		}
	}
}
