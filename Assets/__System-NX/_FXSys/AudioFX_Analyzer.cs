/*
OutputVolume.cs - Part of Simple Spectrum V2.1 by Sam Boyer.
*/

#if UNITY_WEBGL && !UNITY_EDITOR
#define WEB_MODE
#endif

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Rewrite from SimpleSpectrum, require SSWebInteract.cs
/// </summary>
public class AudioFX_Analyzer : MonoBehaviour
{
    public static AudioFX_Analyzer singleton;
    public enum SourceType
    {
        AudioSource, AudioListener, Custom
    }


    #region SAMPLING PROPERTIES
    /// <summary>
    /// Enables or disables the processing and display of volume data. 
    /// </summary>
    [Tooltip("Enables or disables the processing and display of volume data.")]
    public bool isEnabled = true;
    /// <summary>
    /// The type of source for volume data.
    /// </summary>
    [Tooltip("The type of source for volume data.")]
    public SourceType sourceType = SourceType.AudioListener;
    /// <summary>
    /// The AudioSource to take data from. Can be empty if sourceType is not AudioSource.
    /// </summary>
    [Tooltip("The AudioSource to take data from.")]
    public AudioSource audioSource;
    /// <summary>
    /// The number of samples to use when sampling. Must be a power of two.
    /// </summary>
    [Tooltip("The number of samples to use when sampling. Must be a power of two.")]
    public int sampleAmount = 256;
    /// <summary>
    /// The audio channel to take data from when sampling.
    /// </summary>
    [Tooltip("The audio channel to take data from when sampling.")]
    public int channel = 0;
    /// <summary>
    /// The amount of dampening used when the new scale is higher than the bar's existing scale. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
    [Range(0, 1)]
    [Tooltip("The amount of dampening used when the new scale is higher than the bar's existing scale.")]
    public float attackDamp = .75f;
    /// <summary>
    /// The amount of dampening used when the new scale is lower than the bar's existing scale. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
    [Range(0, 1)]
    [Tooltip("The amount of dampening used when the new scale is lower than the bar's existing scale.")]
    public float decayDamp = .25f;
    #endregion

    /// <summary>
    /// Sets the input value for scaling. Can only be used when sourceType is Custom. 
    /// </summary>
    public float inputValue
    {
        set
        {
            if (sourceType == SourceType.Custom)
                newValue = value;
            else
                Debug.LogError("Error from OutputVolume: inputValue cannot be set while sourceType is not Custom.");
        }
    }

    /// <summary>
    /// The output value of the scaler, after attack/decay (Read Only).
    /// </summary>
    public float outputValue
    {
        get { return oldScale; }
    }

    //float[] samples;

    GameObject bar;

    Transform barT;

    float newValue;
    float oldScale;
    float oldColorVal = 0;
    Material mat;

    int mat_ValId;
    bool materialColourCanBeUsed = true;

#if UNITY_EDITOR
    [Range(0f, 1f)] public float OutputPreview;
#endif

    void Awake()
    {
        if (!AudioFX_Analyzer.singleton)
        {
            AudioFX_Analyzer.singleton = this;
        }
        else
        {
            Debug.LogError("Dosen't allow multiple AudioFX_Analyzer");
            Destroy(this);
        }
    }

    void Start()
    {

#if WEB_MODE
        sampleAmount = SSWebInteract.SetFFTSize(sampleAmount);
#endif
    }

    void Update()
    {

        if (isEnabled && sourceType != SourceType.Custom)
        {
            if (sourceType == SourceType.AudioListener)
                newValue = GetRMS(sampleAmount, channel);
            else
                newValue = GetRMS(audioSource, sampleAmount, channel);
        }

        float newScale = newValue > oldScale ? Mathf.Lerp(oldScale, newValue, attackDamp) : Mathf.Lerp(oldScale, newValue, decayDamp);

        oldScale = newScale;


#if UNITY_EDITOR
        OutputPreview = oldScale;
#endif

    }

    /// <summary>
    /// Returns the current output volume of the specified AudioSource, using the RMS method.
    /// </summary>
    /// <param name="aSource">The AudioSource to reference.</param>
    /// <param name="sampleSize">The number of samples to take, as a power of two. Higher values mean more precise volume.</param>
    /// <param name="channelUsed">The audio channel to take data from.</param>
    public static float GetRMS(AudioSource aSource, int sampleSize, int channelUsed = 0)
    {
#if UNITY_WEBGL
        Debug.LogError("Error from SimpleSpectrum: You can't use OutputVolume.GetRMS against a single AudioSource in WebGL!");
        return 0;
#endif

        sampleSize = Mathf.ClosestPowerOfTwo(sampleSize);
        float[] outputSamples = new float[sampleSize];
        aSource.GetOutputData(outputSamples, channelUsed);

        float rms = 0;
        foreach (float f in outputSamples)
        {
            rms += f * f; //sum of squares
        }
        return Mathf.Sqrt(rms / (outputSamples.Length)); //mean and root
    }

    /// <summary>
    /// Returns the current output volume of the scene's AudioListener, using the RMS method.
    /// </summary>
    /// <param name="sampleSize">The number of samples to take, as a power of two. Higher values mean more precise volume.</param>
    /// <param name="channelUsed">The audio channel to take data from.</param>
    public static float GetRMS(int sampleSize, int channelUsed = 0)
    {
#if WEB_MODE
        return SSWebInteract.GetLoudness();
#else
        sampleSize = Mathf.ClosestPowerOfTwo(sampleSize);
        float[] outputSamples = new float[sampleSize];
        AudioListener.GetOutputData(outputSamples, channelUsed);

        float rms = 0;
        foreach (float f in outputSamples)
        {
            rms += f * f; //sum of squares
        }
        return Mathf.Sqrt(rms / (outputSamples.Length)); //mean and root
#endif
    }
}
