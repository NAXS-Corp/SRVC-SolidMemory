using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class FX_GLPlayers : MonoBehaviour
{
    public static FX_GLPlayers instance;
    [Header("Look")]
    public Material LineMaterial;
    public float LineHeight;
    public Color LineColor;
    public Vector2 AlphaRange = new Vector2(0,1);
    public Vector3 RandomPos;

    [Header("Ref")]
    public List<Transform> Players;

    void Awake()
    {
        instance = this;
        Players = new List<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!Application.isPlaying)
            AddPlayer(transform);
        // NXEventManager.StartListening ("test", OnPlayerAdded);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnRenderObject() {
        LineMaterial.SetPass(0);
        for(int i = 0; i < Players.Count; i++){
            Vector3 pos = Players[i].position + new Vector3(RandomPos.x *Random.Range(-1f, 1f), 0, RandomPos.z *Random.Range(-1f, 1f));
            GL.Begin(GL.LINES);
            GL.Color(new Color(LineColor.r, LineColor.g, LineColor.b, Random.Range(AlphaRange.x, AlphaRange.y)));
            GL.Vertex(pos);
            GL.Vertex(pos + Vector3.up * 100);
            GL.End();
        }
        return;
    }

    public void AddPlayer(Transform newPlayer){
        Players.Add(newPlayer);
    }

    
    public void RemovePlayer(Transform player){
        Players.Remove(player);
    }
}
