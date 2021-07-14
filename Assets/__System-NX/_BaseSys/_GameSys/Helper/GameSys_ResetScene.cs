using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSys_ResetScene : MonoBehaviour {
    public int DefaultSceneIdx;

    private void Start() {
    }

    public void ResetScene(int sceneIdx){
        
        SceneManager.LoadScene(sceneIdx);
    }
    
    public void ResetScene(){
        SceneManager.LoadScene(DefaultSceneIdx);
    }

    private void Update() {
        if(Input.GetKey(KeyCode.LeftShift)){
            if(Input.GetKeyDown(KeyCode.R)){
                ResetScene();
            }
        }
    }
    
}