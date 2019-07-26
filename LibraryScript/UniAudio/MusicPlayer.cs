using System.Collections.Generic;
using UnityEngine;
using System;
using FTLibrary.Command;
using FTLibrary.XML;
[AddComponentMenu("UniAudio/MusicPlayer")]
class MusicPlayer : MonoBehaviourIgnoreGui
{
    public class MusicData
    {
        public int Id = 0;
        public string resourceName = "";
        public float volume = 0.0f;
        public bool isunload = false;
        public AudioClip sound = null;
        public MusicData(string name, string rname, float v,bool unload)
        {
            Id = (int)FTUID.StringGetHashCode(name);
            resourceName = rname;
            volume=v;
            isunload = unload;
        }
    }
    public class MusicGroup
    {
        private List<MusicData> musicList = new List<MusicData>();
        public MusicGroup(params string[] musicname)
        {
            for (int i = 0; i < musicname.Length;i++ )
            {
                int id = (int)FTUID.StringGetHashCode(musicname[i]);
                MusicData musciData = MusicPlayer.FindMusic(id);
                musicList.Add(musciData);
            }
        }
        public void PlayRandom(bool immediatelyPlay)
        {
            int index = FTLibrary.Command.FTRandom.Next(musicList.Count);
            MusicPlayer.Play(musicList[index].Id, immediatelyPlay);
        }
    }
    private static MusicGroup musicGroup = null;
    public static void CreateMusicGroup(params string[] musicname)
    {
        musicGroup = new MusicGroup(musicname);
    }
    public static void MusicGroupPlayRandom(bool immediatelyPlay)
    {
        musicGroup.PlayRandom(immediatelyPlay);
        MusicPlayer.musicPlayer.CancelInvoke("MusicGroupPlayStandbyEnd");
    }
    public static void MusicGroupPlayStandby()
    {
        MusicGroupPlayRandom(true);
        MusicPlayer.musicPlayer.Invoke("MusicGroupPlayStandbyEnd", MusicPlayer.musicPlayer.audioSource.clip.length);
    }
    private void MusicGroupPlayStandbyEnd()
    {
        MusicPlayer.MusicGroupPlayStandby();
    }



    //声音过渡时间
    static public float volumeTransitionTime=0.0f;
    static private Dictionary<int, MusicData> musicList = new Dictionary<int, MusicData>(16);
    static MusicData FindMusic(int id)
    {
        MusicData ret;
        if (!musicList.TryGetValue(id, out ret))
            return null;
        return ret;
    }
    public static void Initialization()
    {
        XmlDocument doc = UniGameResources.currentUniGameResources.LoadResource_XmlFile("MusicAudio.xml");
        if (doc == null)
            throw new Exception("MusicPlayer Initialization Err!");
        XmlNode root = doc.SelectSingleNode("MusicAudio");
        volumeTransitionTime = Convert.ToSingle(root.Attribute("volumetransitiontime"));
        XmlNodeList nodelist = root.SelectNodes("item");
        for (int i = 0; i < nodelist.Count; i++)
        {
            XmlNode n = nodelist[i];
            MusicData data = new MusicData(n.Attribute("name"),
                        n.Attribute("resourcename"),
                        Convert.ToSingle(n.Attribute("volume")),
                        n.Attribute("unload") == "1");
            try
            {
                musicList.Add(data.Id, data);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
    }

    public static MusicPlayer musicPlayer = null;
    public static void LoadAllUnloadAudioClip()
    {
        //所有被标记为不卸载的资源都在这里构造
        //这个只在游戏正式启动的时候处理
        Dictionary<int, MusicData>.Enumerator list = MusicPlayer.musicList.GetEnumerator();
        while (list.MoveNext())
        {
            if (list.Current.Value.isunload)
            {
                list.Current.Value.sound = UniGameResources.currentUniGameResources.LoadResource_AudioClip(list.Current.Value.resourceName);
            }
        }
        list.Dispose();
    }

    private AudioSource m_AudioSource = null;
    public AudioSource audioSource
    {
        get
        {
            return m_AudioSource;
        }
    }
    private bool IsCurrentAudioUnLoad = false;

    private AudioListenerCtrl m_AudioListenerCtrl = null;
    public AudioListenerCtrl audioListenerCtrl
    {
        get { return m_AudioListenerCtrl; }
    }
    protected override void Awake()
    {
        base.Awake();
        UnityEngine.Object.DontDestroyOnLoad(this);
        m_AudioSource = GetComponent(typeof(AudioSource)) as AudioSource;
        m_AudioListenerCtrl = GetComponent(typeof(AudioListenerCtrl)) as AudioListenerCtrl;
        MusicPlayer.musicPlayer = this;
    }
    void OnDestroy()
    {
        m_AudioListenerCtrl = null;
        m_AudioSource = null;
        MusicPlayer.musicPlayer = null;
    }
    const float VolumeUpdateTime = 0.1f;
    private float onceVolume = 0.0f;
    private float startVolume = 0.0f;
    private float endVolume = 0.0f;
    
    void PlayMusic()
    {
        endVolume = audioSource.volume;
        startVolume = 0.0f;
        onceVolume = (endVolume - startVolume) / (MusicPlayer.volumeTransitionTime / VolumeUpdateTime);

        audioSource.volume = startVolume;
        audioSource.Play();

        CancelInvoke("VolumeUpdateFuntion");
        CancelInvoke("VolumeUpdateFuntion1");
        InvokeRepeating("VolumeUpdateFuntion", VolumeUpdateTime, VolumeUpdateTime);
    }
    void StopMusic()
    {
        endVolume = 0.0f;
        startVolume = audioSource.volume;
        onceVolume = (endVolume - startVolume) / (MusicPlayer.volumeTransitionTime / VolumeUpdateTime);

        CancelInvoke("VolumeUpdateFuntion");
        CancelInvoke("VolumeUpdateFuntion1");
        InvokeRepeating("VolumeUpdateFuntion1", VolumeUpdateTime, VolumeUpdateTime);

    }
    void VolumeUpdateFuntion()
    {
        startVolume+=onceVolume;
        if (startVolume < endVolume)
        {
            audioSource.volume = startVolume;
        }
        else
        {
            audioSource.volume = endVolume;
            CancelInvoke("VolumeUpdateFuntion");
            if (endVolume == 0.0f)
            {
                audioSource.Stop();
            }
        }
    }
    void VolumeUpdateFuntion1()
    {
        startVolume += onceVolume;
        if (startVolume > 0.0f)
        {
            audioSource.volume = startVolume;
        }
        else
        {
            audioSource.volume = endVolume;
            CancelInvoke("VolumeUpdateFuntion1");
            if (endVolume == 0.0f)
            {
                audioSource.Stop();
            }
        }
    }


    public static MusicData currentMusicData = null;
    public static void PlayTest()
    {
        Dictionary<int, MusicData>.Enumerator list = musicList.GetEnumerator();
        if (!list.MoveNext())
        {
            list.Dispose();
            return;
        }
        int id = list.Current.Value.Id;
        list.Dispose();
        Play(id, true);
    }
    static public void Play(string name, bool immediatelyPlay)
    {
        int id = (int)FTUID.StringGetHashCode(name);
        Play(id, immediatelyPlay);
    }
    static public void Play(int id, bool immediatelyPlay)
    {
        if (currentMusicData != null && currentMusicData.Id == id)
            return;
        currentMusicData = FindMusic(id);
        if (currentMusicData == null)
            return;
        if (musicPlayer == null)
            return;
        if (musicPlayer.audioSource == null)
            return;
        AudioClip sound = null;
        if (currentMusicData.sound != null)
        {
            sound = currentMusicData.sound;
        }
        else
        {
            sound = UniGameResources.currentUniGameResources.LoadResource_AudioClip(currentMusicData.resourceName);
        }
        if (sound == null)
            return;
        musicPlayer.audioSource.Stop();
        musicPlayer.audioSource.clip = sound;
        if (workMode == MusicPlayerWorkMode.Mode_Standby)
        {
            musicPlayer.audioSource.volume = currentMusicData.volume*UniGameOptionsDefine.StandByMusicVolume;
        }
        else
        {
            musicPlayer.audioSource.volume = currentMusicData.volume;
        }
        //标记是否在下载一个资源loading的时候不卸载
        musicPlayer.IsCurrentAudioUnLoad = currentMusicData.isunload;
        if (immediatelyPlay)
        {
            musicPlayer.audioSource.Play();
        }
        else
        {
            musicPlayer.PlayMusic();
        }
        
    }
    static public void Loading(string name)
    {
        int id = (int)FTUID.StringGetHashCode(name);
        if (currentMusicData != null && currentMusicData.Id == id)
            return;
        currentMusicData = FindMusic(id);
        if (currentMusicData == null)
            return;
        if (musicPlayer == null)
            return;
        if (musicPlayer.audioSource == null)
            return;
        AudioClip sound = null;
        if (currentMusicData.sound != null)
        {
            sound = currentMusicData.sound;
        }
        else
        {
            sound = UniGameResources.currentUniGameResources.LoadResource_AudioClip(currentMusicData.resourceName);
        }
        if (sound == null)
            return;
        musicPlayer.audioSource.Stop();
        if (musicPlayer.audioSource.clip != null)
        {
            //如果被标记为不卸载则不卸载了
            if (!musicPlayer.IsCurrentAudioUnLoad)
            {
                UniGameResources.ReleaseOneAssets(musicPlayer.audioSource.clip);
            }
            musicPlayer.audioSource.clip = null;
        }
        musicPlayer.audioSource.clip = sound;
        musicPlayer.audioSource.volume = 0.0f;
        musicPlayer.audioSource.Play();
        musicPlayer.audioSource.Stop();
        //标记这个资源是否不卸载
        musicPlayer.IsCurrentAudioUnLoad = currentMusicData.isunload;
        currentMusicData = null;

    }
    static public void Stop(bool immediatelyStop)
    {
        currentMusicData = null;
        if (musicPlayer == null)
            return;
        if (immediatelyStop)
        {
            musicPlayer.audioSource.Stop();
        }
        else
        {
            musicPlayer.StopMusic();
        }
        
    }
    static public float volume
    {
        get { return musicPlayer.audioSource.volume; }
        set { musicPlayer.audioSource.volume = value; }
    }
    public static bool IsPlay
    {
        get
        {
            return currentMusicData != null;
        }
    }
    //void PlayDelayFuntion()
    //{
    //    audioSource.Play();
    //}
    //static public void PlayDelay(string name,float delayTime)
    //{
    //    MusicData data = FindMusic(name);
    //    if (data == null)
    //        return;
    //    if (musicPlayer == null)
    //        return;
    //    if (musicPlayer.audioSource == null)
    //        return;
    //    AudioClip sound = GameRoot.m_GameResources.InstantiateResourcesAudioClip(data.resourceName);
    //    if (sound == null)
    //        return;
    //    musicPlayer.audioSource.Stop();
    //    musicPlayer.audioSource.clip = sound;
    //    musicPlayer.audioSource.volume = data.volume;
    //    musicPlayer.Invoke("PlayDelayFuntion", delayTime);
    //}

    //激活我的音源监听
    static public bool activeMyListener
    {
        set
        {
            AudioListenerCtrl.activeAudio = value ? musicPlayer.audioListenerCtrl : null;
        }
        get
        {
            return AudioListenerCtrl.activeAudio == musicPlayer.audioListenerCtrl;
        }
    }

    //当前工作模式
    public enum MusicPlayerWorkMode
    {
        Mode_Normal,
        Mode_Standby
    }
    public static MusicPlayerWorkMode workMode = MusicPlayerWorkMode.Mode_Normal;
}


//enum MusicState
//{
//    State_Play = 0,
//    State_Stop = 1,
//    State_Pause = 2
//}
//class MusicPlayer : MonoBehaviour
//{
//    //音量
//    public float volume = 0.5f;
    
//    AudioSource audioSource = null;
//    string[] trackList = null;
//    int trackIndex = 0;

//    MusicTrack currentMusicTrack = null;
//    int currentMusicTrackIndex=-1;

//    MusicState musicState = MusicState.State_Stop;

//    public bool nextTrack = false;
//    void Start()
//    {
//        audioSource = (AudioSource)GetComponent(typeof(AudioSource));
//    }
//    public void SetTrackList(string[] list)
//    {
//        trackList = list;
//        //播放器模式随机选择，其他情况下顺序播放
//        if (ConsoleCenter.CurrentSceneWorkMode == SceneWorkMode.SCENEWORKMODE_VIDEOPLAY)
//        {
//            RandTrack();
//        }
//        else
//        {
//            trackIndex = 0;
//        } 
//    }
//    MusicTrack GetTrack(int nIndex)
//    {
//        if (currentMusicTrack != null)
//        {
//            if (currentMusicTrackIndex == nIndex)
//            {
//                return currentMusicTrack;
//            }
//            UnityEngine.Object.DestroyObject(currentMusicTrack.gameObject);
//            currentMusicTrack = null;
//        }
//        GameObject obj = GameRoot.m_GameResources.InstantiateResourcesPrefabs(trackList[nIndex]);
//        obj.transform.parent = transform;
//        currentMusicTrack = (MusicTrack)obj.GetComponent(typeof(MusicTrack));

//        currentMusicTrackIndex = nIndex;
//        return currentMusicTrack;
//    }
//    void NextTrack()
//    {
//        trackIndex += 1;
//        if (trackIndex >= trackList.Length)
//            trackIndex = 0;
//    }
//    void RandTrack()
//    {
//        trackIndex = UnityEngine.Random.Range(0, trackList.Length-1);
//    }
//    public void Play()
//    {
//        if (musicState == MusicState.State_Play)
//            return;
//        if (audioSource.clip == null)
//        {
//            audioSource.clip = GetTrack(trackIndex).clip;
//        }
//        audioSource.Play();
//        musicState = MusicState.State_Play;
//    }
//    public void PlayDelay(float delayTime)
//    {
//        Invoke("Play", delayTime);
//    }
//    public void Pause()
//    {
//        if (musicState == MusicState.State_Pause)
//            return;
//        audioSource.Pause();
//        musicState = MusicState.State_Pause;
//    }
//    public void Stop()
//    {
//        if (musicState == MusicState.State_Stop)
//            return;
//        audioSource.Stop();
//        musicState = MusicState.State_Stop;
//    }
//    void FixedUpdate()
//    {
//        if (nextTrack || (musicState == MusicState.State_Play && !audioSource.isPlaying))
//        {
//            //播放器模式随机选择，其他情况下顺序播放
//            if (ConsoleCenter.CurrentSceneWorkMode == SceneWorkMode.SCENEWORKMODE_VIDEOPLAY)
//            {
//                RandTrack();
//            }
//            else
//            {
//                NextTrack();
//            }
//            audioSource.clip = GetTrack(trackIndex).clip;
//            audioSource.Play();
//            nextTrack = false;
//        }
//    }
//}
