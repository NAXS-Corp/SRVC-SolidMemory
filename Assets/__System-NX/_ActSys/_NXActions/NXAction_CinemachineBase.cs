using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NAXS.Base;

public abstract class NXAction_CinemachineBase : NXAction
{
    private CinemachineBrain _MainCineBrain;
    protected CinemachineBrain MainCineBrain
    {
        get {  
            if(!_MainCineBrain)
                _MainCineBrain  = NXBase_ObjManager.GetMainCam.GetComponent<CinemachineBrain>();
            return _MainCineBrain;
        }
    }
    protected CinemachineVirtualCamera ActiveVirtualCamera
    {
        get { return MainCineBrain == null ? null : MainCineBrain.ActiveVirtualCamera as CinemachineVirtualCamera; }
    }
    
}
