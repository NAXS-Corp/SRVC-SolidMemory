using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvtSys_Preset : MonoBehaviour {

	public bool ForcePresetOnBuild = true;
	public GameObject[] ActiveOnAwake;
	public GameObject[] DeactiveOnAwake;
	public GameObject[] DestroyOnAwake;
	// Use this for initialization
	void Awake () {

		if(this.enabled){
			setup();
		}
		#if !UNITY_EDITOR
		if(ForcePresetOnBuild){
			setup();
		}
		#endif
	}
	// void OnEnable(){
	// 		setup();
	// }

	void setup(){

		foreach(GameObject obj in ActiveOnAwake){
			if(obj){
				obj.SetActive(true);
			}
		}
		foreach(GameObject obj in DeactiveOnAwake){
			if(obj){
				obj.SetActive(false);
			}
		}
		foreach(GameObject obj in DestroyOnAwake){
			if(obj){
				Destroy(obj);
			}
		}
	}
}
