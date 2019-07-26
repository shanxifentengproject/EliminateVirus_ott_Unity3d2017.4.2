using UnityEngine;
using System;
using System.Collections.Generic;
[AddComponentMenu("UniAudio/AudioListenerCtrl")]
class AudioListenerCtrl : MonoBehaviourIgnoreGui
{
    //当前系统的声音定义
    //可更改
    //protected static float SystemVoiceVolume { get { return 1.0f; } }
    protected static float SystemVoiceVolume { get { return UniGameOptionsDefine.gameVolume; } }
    
    protected static bool m_mute = false;
    public static bool mute
    {
        get { return m_mute; }
        set
        {
            m_mute = value;
            if (m_mute)
            {
                AudioListener.volume = 0.0f;
            }
            else
            {
                if (activeAudio != null)
                {
                    //AudioListener.volume = active.volume;
                    AudioListener.volume = SystemVoiceVolume * activeAudio.volume;
                }
            }
        }
    }

    protected static AudioListenerCtrl activeAudioListenerCtrl = null;
    public static AudioListenerCtrl activeAudio
    {
        get { return activeAudioListenerCtrl; }
        set
        {
            if (activeAudioListenerCtrl == value)
                return;
            AudioListener audioListener;
            if (value != null)
            {
                audioListener = (AudioListener)value.GetComponent(typeof(AudioListener));
                audioListener.enabled = true;
                if (mute)
                {
                    AudioListener.volume = 0.0f;
                }
                else
                {
                    //AudioListener.volume = value.volume;
                    AudioListener.volume = SystemVoiceVolume * value.volume;
                }
            }
            if (activeAudioListenerCtrl != null)
            {
                audioListener = (AudioListener)activeAudioListenerCtrl.GetComponent(typeof(AudioListener));
                audioListener.enabled = false;
                activeAudioListenerCtrl.OnNoActive();
            }
            activeAudioListenerCtrl = value;
            if (value != null)
            {
                value.OnActive();
            }
        }
    }
    public static float activeVolume
    {
        get
        {
            if (activeAudio == null)
                return 0.0f;
            return AudioListener.volume;
        }
        set
        {
            if (activeAudio == null)
                return;
            if (mute)
            {
                AudioListener.volume = 0.0f;
            }
            else
            {
                AudioListener.volume = SystemVoiceVolume * activeAudio.volume;
            }
        }
    }

    public float volume = 1.0f;
    protected override void Awake()
    {
        base.Awake();
        AudioListener audioListener = GetComponent(typeof(AudioListener)) as AudioListener;
        if (AudioListenerCtrl.activeAudio != this)
        {
            audioListener.enabled = false;
        }
    }
    public virtual void OnActive()
    {
        
    }
    public virtual void OnNoActive()
    {

    }
}











































//using UnityEngine;
//using System;
//using System.Collections.Generic;

//class AudioListenerCtrl : MonoBehaviour
//{
//    static bool m_mute = false;
//    static public bool mute
//    {
//        get { return m_mute; }
//        set
//        {
//            m_mute = value;
//            if (m_mute)
//            {
//                AudioListener.volume = 0.0f;
//            }
//            else
//            {
//                if (active != null)
//                {
//                    AudioListener.volume = active.volume;
//                }
//            }
//        }
//    }

//    static AudioListenerCtrl activeAudioListenerCtrl = null;
//    static public AudioListenerCtrl active
//    {
//        get { return activeAudioListenerCtrl; }
//        set
//        {
//            if (activeAudioListenerCtrl == value)
//                return;
//            AudioListener audioListener = null;
//            //如果之前有激活的了,则使用之前已经存在的监听源
//            if (activeAudioListenerCtrl != null)
//            {
//                audioListener = activeAudioListenerCtrl.audioListener;
//                activeAudioListenerCtrl.audioListener = null;
//            }
//            //如果之前不存在监听源，就需要构造一个监听源使用了
//            if (value != null && audioListener == null)
//            {
//                //GameObject listener = new GameObject("AudioListener", typeof(AudioListener), typeof(AudioLowPassFilter));
//                GameObject listener = new GameObject("AudioListener", typeof(AudioListener));
//                audioListener = listener.GetComponent(typeof(AudioListener)) as AudioListener;
//                audioListener.velocityUpdateMode = AudioVelocityUpdateMode.Fixed;

//                //AudioLowPassFilter filter = listener.GetComponent(typeof(AudioLowPassFilter)) as AudioLowPassFilter;
//                //filter.cutoffFrequency = 22000f;
//            }
//            activeAudioListenerCtrl = value;
//            if (activeAudioListenerCtrl != null)
//            {
//                //将监听源移动到当前监听控制对象下
//                audioListener.transform.parent = activeAudioListenerCtrl.transform;
//                audioListener.transform.localPosition = Vector3.zero;
//                audioListener.transform.localRotation = Quaternion.identity;
//                activeAudioListenerCtrl.audioListener = audioListener;
//                if (mute)
//                {
//                    AudioListener.volume = 0.0f;
//                }
//                else
//                {
//                    AudioListener.volume = activeAudioListenerCtrl.volume;
//                }
//            }
//            else if (audioListener != null)
//            {
//                //没有任何监听源激活就释放掉当前监听源
//                UnityEngine.Object.DestroyImmediate(audioListener.gameObject);
//            }
//        }
//    }
//    public enum AudioListenerDistance
//    {
//        Distance_Near,  //近距离
//        Distance_Far    //远距离
//    }

//    public float volume = 1.0f;
//    public AudioListenerDistance distanceType = AudioListenerDistance.Distance_Near;
//    public AudioListener audioListener { get; set; }
//    void Start()
//    {
//        AudioListener listener = GetComponent(typeof(AudioListener)) as AudioListener;
//        listener.enabled = false;
//    }
//}
