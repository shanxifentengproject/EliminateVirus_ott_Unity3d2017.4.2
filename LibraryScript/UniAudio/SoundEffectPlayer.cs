using System.Collections.Generic;
using UnityEngine;
using System;
using FTLibrary.Command;
using FTLibrary.XML;
[AddComponentMenu("UniAudio/SoundEffectPlayer")]
class SoundEffectPlayer : MonoBehaviourIgnoreGui
{
    class SoundEffectData
    {
        public int Id = 0;
        public string resourceName = "";
        public float volume = 0.0f;
        //是否是语言音效
        public bool isLanguage = false;
        //用来存储当前播放的音频对象
        public AudioSource playerSource;
        public SoundEffectData(string name,string rname,float v,bool language)
        {
            Id = (int)FTUID.StringGetHashCode(name);
            resourceName = rname;
            volume=v;
            playerSource = null;
            isLanguage = language;
        }
        public SoundEffectData(string name, string rname, float v,bool language,AudioSource player)
        {
            Id = (int)FTUID.StringGetHashCode(name);
            resourceName = rname;
            volume = v;
            playerSource = player;
            isLanguage = language;
        }
    }
    static private Dictionary<int, SoundEffectData> soundEffectList = new Dictionary<int, SoundEffectData>(32);
    static SoundEffectData FindSoundEffect(string name)
    {
        SoundEffectData ret;
        if (!soundEffectList.TryGetValue((int)FTUID.StringGetHashCode(name), out ret))
            return null;
        return ret;
    }
    static public void Initialization()
    {
        XmlDocument doc = UniGameResources.currentUniGameResources.LoadResource_XmlFile("SoundEffect.xml");
        if (doc == null)
            throw new Exception("SoundEffectPlayer Initialization Err!");
        XmlNode root = doc.SelectSingleNode("SoundEffect");
        XmlNodeList nodelist = root.SelectNodes("item");
        for (int i = 0; i < nodelist.Count;i++ )
        {
            XmlNode n=nodelist[i];
            SoundEffectData data = new SoundEffectData(n.Attribute("name"),
                                    n.Attribute("resourcename"), 
                                    Convert.ToSingle(n.Attribute("volume")),
                                    n.Attribute("language") == "1");
            try
            {
                soundEffectList.Add(data.Id, data);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
    }
    static private SoundEffectPlayer soundEffectPlayer = null;
    private List<AudioSource> audioSourceList = new List<AudioSource>(32);
    public AudioSource audioSource
    {
        get
        {
            AudioSource audioSource;
            int count = audioSourceList.Count;
            for (int i = 0; i < count; i++)
            {
                audioSource = audioSourceList[i] as AudioSource;
                if (!audioSource.isPlaying)
                {
                    //就使用这个了
                    //从队列里移出来
                    audioSourceList.RemoveAt(i);
                    //然后放到队列的后面
                    audioSourceList.Add(audioSource);
                    return audioSource;
                }
            }
            //没有，需要构造
            audioSource = UniGameResources.NormalizePrefabs(UniGameResources.currentUniGameResources.LoadResource_Prefabs("SoundEffectAudioSource.prefab"),
                                    transform, typeof(AudioSource)) as AudioSource;
            //加入队列
            audioSourceList.Add(audioSource);
            return audioSource;
        }
    }
    public AudioSource CreateStandAloneAudioSource()
    {
        return UniGameResources.NormalizePrefabs(UniGameResources.currentUniGameResources.LoadResource_Prefabs("SoundEffectAudioSource.prefab"),
                                    transform, typeof(AudioSource)) as AudioSource;
    }
    protected override void Awake()
    {
        base.Awake();
        UnityEngine.Object.DontDestroyOnLoad(this);
        soundEffectPlayer = this;
    }
    void OnDestroy()
    {
        soundEffectPlayer = null;
    }

    static public void Play(string name)
    {
        SoundEffectData data = FindSoundEffect(name);
        if (data == null)
            return;
        if (soundEffectPlayer == null)
            return;
        AudioSource audioSource = soundEffectPlayer.audioSource;
        if (audioSource == null)
            return;
        AudioClip sound = null;
        if (data.isLanguage)
        {
            sound = UniGameResources.currentUniGameResources.LoadLanguageResource_AudioClip(data.resourceName);
        }
        else
        {
            sound = UniGameResources.currentUniGameResources.LoadResource_AudioClip(data.resourceName);
        }
        
        if (sound == null)
            return;
        audioSource.clip = sound;
        audioSource.volume = data.volume;
        audioSource.Play();
    }
    public static void PlayStandalone(string name)
    {
        SoundEffectData data = FindSoundEffect(name);
        if (data == null)
            return;
        if (soundEffectPlayer == null)
            return;
        if (data.playerSource != null)
        {
            if (data.playerSource.isPlaying)
                return;
            data.playerSource.Play();
        }
        else
        {
            data.playerSource = soundEffectPlayer.CreateStandAloneAudioSource();
            AudioClip sound = null;
            if (data.isLanguage)
            {
                sound = UniGameResources.currentUniGameResources.LoadLanguageResource_AudioClip(data.resourceName);
            }
            else
            {
                sound = UniGameResources.currentUniGameResources.LoadResource_AudioClip(data.resourceName);
            }
            if (sound == null)
                return;
            data.playerSource.clip = sound;
            data.playerSource.volume = data.volume;
            data.playerSource.Play();
        }
    }
}
