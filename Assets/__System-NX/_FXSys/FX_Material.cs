using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class MatCtrl{
	public float fadeTime;
	public bool doColorFading;
	public bool doFloatFading;
	public string targetProperty;
	public Color fromColor;
	public Color targetColor;
	public float fromFloat;
	public float targetFloat;
}

public class FX_Material : MonoBehaviour {

	[Header("Basic")]
	public bool StartOnEnable;
	public MeshRenderer targetRenderer;
	public ParticleSystem targetParticle;
	private Material _mat;
	[Header("_Color / _EmissionColor")]
	public MatCtrl[] Paramaters = new MatCtrl[1];

	// Use this for initialization
	void OnEnable () {
		if(StartOnEnable){
			StartFadingAll();
		}
	}

	public void StartFadingAll(){
		if(targetParticle){
			_mat = targetParticle.GetComponent<ParticleSystemRenderer>().material;
		}else{
			_mat = targetRenderer.material;
		}


		foreach(MatCtrl par in Paramaters){
			if(par.doColorFading){
				_mat.SetColor(par.targetProperty, par.fromColor);
				_mat.DOColor(par.targetColor, par.targetProperty, par.fadeTime);
			}else if(par.doFloatFading){
				_mat.SetFloat(par.targetProperty, par.fromFloat);
				_mat.DOFloat(par.targetFloat, par.targetProperty, par.fadeTime);
			}
		}
	}
}
