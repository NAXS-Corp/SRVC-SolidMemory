using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Sirenix.OdinInspector;
// using NetOpt;

// public class AnimParam{
//     public string name;
//     private int _nameHash = -1;
//     public bool boolState;
//     public float floatState;
//     public AnimatorControllerParameterType type;
//     public int nameHash{
//         get{
//             if(_nameHash != -1) return _nameHash;
//             _nameHash = Animator.StringToHash(this.name);
//             return _nameHash;
//         }
//     }
// }

public class MirrorSys_SimpleAnimator : NetworkBehaviour
{
    // // public AnimParam[] AnimatorParams;

    // [System.Serializable]
    // public struct SimpleParams
    // {
    //     public float horizontalSpeed;
    //     public float verticalSpeed;
    //     public bool isGrounded;
    //     public bool isEquals(SimpleParams param){
    //         return param.horizontalSpeed == horizontalSpeed && param.verticalSpeed == verticalSpeed && param.isGrounded == isGrounded;
    //     }
    // }
    // public SimpleParams m_AnimParams;
    // private SimpleParams lastAnimParams;
    
    // int horAnimHash;
    // int verAnimHash;
    // int groundedAnimHash;

    // // [SyncVar]
    // // public float horizontalSpeed;
    // // // public int hash
    // // [SyncVar]
    // // public float verticalSpeed;
    // // [SyncVar]
    // // public bool isGrounded;
    // public Animator m_Animator;


    // // Serialization
    // PacketBuffer dataBuffer;
    // byte[] lastDataBuffer;
    // private Vector2 floatRange = new Vector2(0, 5);
    // private float dataPercision = 0.01f;
    

    // void Start()
    // {
    //     dataBuffer = new PacketBuffer(1024);

    //     horAnimHash = Animator.StringToHash("HorizontalSpeed");
    //     verAnimHash = Animator.StringToHash("VerticalSpeed");
    //     groundedAnimHash = Animator.StringToHash("IsGrounded");
    // }

    // void FixedUpdate()
    // {
    //     if(isLocalPlayer){
    //         UpdateLocalPlayer();
    //     }
    // }

    // void UpdateLocalPlayer(){

    //     if(!m_Animator) return;

    //     m_AnimParams.horizontalSpeed = m_Animator.GetFloat(horAnimHash);
    //     m_AnimParams.verticalSpeed = m_Animator.GetFloat(verAnimHash);
    //     m_AnimParams.isGrounded = m_Animator.GetBool(groundedAnimHash);

    //     if(!m_AnimParams.isEquals(lastAnimParams)){
    //         Serialize();
    //         CmdSendSerializedData(dataBuffer.GetBytes());
    //     }

    //     lastAnimParams = m_AnimParams;
    // }    

    // public int Serialize()
    // {
    //     PacketWriter writer= new PacketWriter(dataBuffer);

    //     writer.PackFloat(m_AnimParams.horizontalSpeed, floatRange.x, floatRange.y, dataPercision);
    //     writer.PackFloat(m_AnimParams.verticalSpeed, floatRange.x, floatRange.y, dataPercision);
    //     writer.PackBool(m_AnimParams.isGrounded);
    //     return writer.FlushFinalize();
    // }

    // [Command]
    // void CmdSendSerializedData(byte[] paramData){
    //     RpcBroadcastData(paramData);
    // }

    // [ClientRpc(excludeOwner = true)]
    // void RpcBroadcastData(byte[] paramData){
    //     Deserialize(paramData);
    // }
    
    // public void Deserialize(byte[] paramData)
    // {
    //     PacketReader reader = new PacketReader(new PacketBuffer(paramData, false));
    //     SimpleParams newParams;
    //     reader.Unpack(out newParams.horizontalSpeed, floatRange.x, floatRange.y, dataPercision);
    //     reader.Unpack(out newParams.verticalSpeed, floatRange.x, floatRange.y, dataPercision);
    //     reader.Unpack(out newParams.isGrounded);
        
    //     SetAnimator(newParams);
    // }
    // public void TestDeserialize(byte[] paramData)
    // {
    //     PacketReader reader = new PacketReader(new PacketBuffer(paramData, false));
    //     SimpleParams newParams;
    //     reader.Unpack(out newParams.horizontalSpeed, floatRange.x, floatRange.y, dataPercision);
    //     reader.Unpack(out newParams.verticalSpeed, floatRange.x, floatRange.y, dataPercision);
    //     reader.Unpack(out newParams.isGrounded);
    // }

    // void SetAnimator(SimpleParams newParams){
    //     m_Animator.SetFloat(horAnimHash, newParams.horizontalSpeed);
    //     m_Animator.SetFloat(verAnimHash, newParams.verticalSpeed);
    //     m_Animator.SetBool(groundedAnimHash, newParams.isGrounded);
    // }

    
    // // with no serialization
    // // void UpdateLocalPlayer(){
    // //     CmdUpdateSyncVarOnServer(hor, ver, grounded);
    // // }

    // // [Command]
    // // void CmdUpdateSyncVarOnServer(float horSpeed, float verSpeed, bool grounded){
    // //     horizontalSpeed = horSpeed;
    // //     verticalSpeed = verSpeed;
    // //     isGrounded = grounded;
    // // }

    // // void UpdateRemotePlayer(){
    //     // with no serialization
    //     // m_Animator.SetFloat("HorizontalSpeed", horizontalSpeed);
    //     // m_Animator.SetFloat("VerticalSpeed", verticalSpeed);
    //     // m_Animator.SetBool("IsGrounded", isGrounded);
    // // }
}
