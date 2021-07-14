using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;
using System.Linq;

[System.Serializable]
public class AudioSnapshotCtrl{
    [TableColumnWidth(70)]
    public AudioMixerSnapshot Snapshot;
    [TableColumnWidth(30)]
    public float value;
}

[AddComponentMenu("_NXAction/Audio/NX Mixer Snapshot")]
public class NXAction_AudioMixerSnapshot : NXAction
{
    public AudioMixer TargetMixer;
    public Vector2 FadeTime = new Vector2(3f, 3f);

    [TableList]
    public AudioSnapshotCtrl[] SnapshotCtrls;

    List<AudioMixerSnapshot> TargetSnapshots;
    List<float> SnapshotValues;
    List<float> LastSnapshotValues;

    protected override void Awake() { }
    protected override void Start() { }
    public override void StartAction() {
        // CreateList();
        // TargetMixer.TransitionToSnapshots(TargetSnapshots.ToArray(), SnapshotValues.ToArray(), FadeTime.x);
    }
    public override void StopAction() { }
    public override void UpdateAction() { }



    /////////////////////////////
    //private Methods////////////
    /////////////////////////////
    void RecordLastValues()
    {
        foreach(AudioSnapshotCtrl item in SnapshotCtrls)
        {
            // TargetSnapshots.Add(item.Snapshot);
            // float lastValue = TargetMixer.FindSnapshot(item.Snapshot.name).
            // LastSnapshotValues.;
        }
    }
    void CreateList()
    {
        TargetSnapshots = new List<AudioMixerSnapshot>();
        SnapshotValues = new List<float>();
        foreach(AudioSnapshotCtrl item in SnapshotCtrls)
        {
            TargetSnapshots.Add(item.Snapshot);
            SnapshotValues.Add(item.value);
        }

    }
}
