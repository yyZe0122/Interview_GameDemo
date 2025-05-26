using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

/// <summary>
/// 音乐 音效管理器
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    //音乐播放组件
    private AudioSource musicsSource = null;

    //音乐大小默认值
    private float musicValue = 0.2f;
    
    //管理正在播放的音效
    private List<AudioSource> soundList = new List<AudioSource>();
    //音效大小
    private float soundValue = 0.2f;
    //音效是否在暂停 暂停的话不销毁
    private bool soundIsPlay = true;
    
    public void LoadMusicOrSound()
    {
        MusicData musicData = GameDataMgr.Instance.musicData;
        musicValue = musicData.MusicValue;
        soundValue = musicData.SoundValue;
    }
    
    #region 音乐(背景音乐)
 
    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="name">音乐文件名</param>
    public void PlayMusic(string name)
    {
        //动态生成播放音乐的音乐播放器游戏对象
        //并且不在切换场景时移除
        if (musicsSource == null)
        {
            GameObject obj = new GameObject();
            obj.name = "Music";
            GameObject.DontDestroyOnLoad(obj);
            //获取音乐播放器上的AudioSource组件
            musicsSource = obj.AddComponent<AudioSource>();
        }
        //根据传入的音乐名 加载音乐
        ABResMgr.Instance.LoadResAsync<AudioClip>("music",name, (clip) =>
        {
            //添加切片文件
            musicsSource.clip = clip;
            //开启循环播放
            musicsSource.loop = true;
            //音乐大小
            musicsSource.volume = musicValue;
            //播放音乐
            musicsSource.Play();
        });
    }
    
    /// <summary>
    /// 停止播放音乐
    /// 再播放后是重新播放
    /// </summary>
    public void StopMusic()
    {
        if (musicsSource == null)
        {
            return;
        }
        //停止音乐
        musicsSource.Stop();
    }
    
    /// <summary>
    /// 暂停播放音乐
    /// 再播放后是继续播放
    /// </summary>
    public void PauseMusic()
    {
        if (musicsSource == null)
        {
            return;
        }
        //暂停音乐
        musicsSource.Pause();
    }
    //设置音乐大小
    public void ChangeMusicValue(float value)
    {
        musicValue = value;
        if (musicsSource == null)
        {
            return;
        }
        //播放时修改 或者 修改值
        musicsSource.volume = musicValue;
    }

    #endregion

    #region 音效

    //需要在私有函数中加入循环基类
    //MonoMgr.Instance.AddFixedUpdateListener(Update);
    private void Update()
    {
        if (!soundIsPlay)
        {
            return;
        }
        //不停遍历容器 检测有没有音效播放完毕 播放完毕 那么就销毁
        //为了避免边遍历边移除出问题 我们采用逆向遍历
        for (int i = soundList.Count - 1; i >= 0; i--)
        {
            //如果没有播放
            if (!soundList[i].isPlaying)
            {
                //音效播放完毕不再使用 将音效文件置空
                soundList[i].clip = null;
                //不删除 而是存入缓存池中
                PoolMgr.Instance.PushObj(soundList[i].gameObject);
                soundList.RemoveAt(i);
            }
        }
    }
    

    
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效名</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="isSync">是否同步加载</param>
    /// /// <param name="callBack">加载结束后的回调函数</param>
    public void PlaySound(string name, bool isLoop = false, bool isSync = false,
        UnityAction<AudioSource> callBack = null)
    {
        //加载音效资源
        ABResMgr.Instance.LoadResAsync<AudioClip>("sound", name, (clip) =>
        {
            //从缓存池中取出音效对象 得到对应组件
            AudioSource source = PoolMgr.Instance.GetObject("Sound/soundObj").GetComponent<AudioSource>();
            //如果取出来的是之前正在使用的 先停止他
            source.Stop();
        
            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
            //由于从缓存池中取出对象 可能取出之前正在使用的(超上限)
            //所以需要判断 容器中没有记录再去记录 不要重复添加即可
            if (!soundList.Contains(source))
            {
                //存储容器 用于记录 方便之后判断是否停止
                soundList.Add(source);
            }
        
            //传递给外部使用
            callBack?.Invoke(source);
        });
    }
    
    /// <summary>
    /// 停止播放指定音效
    /// </summary>
    /// <param name="source">音效组件对象</param>
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            //停止播放
            source.Stop();
            //从容器中移除
            soundList.Remove(source);
            //音效播放完毕不再使用 将音效文件置空
            source.clip = null;
            //将对象加入缓存池中
            PoolMgr.Instance.PushObj(source.gameObject);
        }
    }
    
    /// <summary>
    /// 设置音效大小
    /// </summary>
    /// <param name="value">值</param>
    public void ChangeSoundValue(float value)
    {
        soundValue = value;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = soundValue;
        }
    }

    /// <summary>
    /// 继续播放或者暂停所有音效
    /// </summary>
    /// <param name="isPlay">是否继续播放 true播放</param>
    public void PlayOrPauseSound(bool isPlay)
    {
        //继续播放
        if (isPlay)
        {
            soundIsPlay = true;
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Play();
            }
        }
        //暂停
        else
        {
            soundIsPlay = false;
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Pause();
            }
        }
    }
    
    /// <summary>
    /// 清空音效相关记录
    /// 过场景时在清空缓存池之前调用它
    /// 过场景时在清空缓存池之前调用它
    /// 过场景时在清空缓存池之前调用它
    /// </summary>
    public void ClearSound()
    {
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].Stop();
            soundList[i].clip = null;
            PoolMgr.Instance.PushObj(soundList[i].gameObject);
        }
        //清空音效列表
        soundList.Clear();
    }
    #endregion
    
    private MusicMgr()
    {
        MonoMgr.Instance.AddFixedUpdateListener(Update);
        LoadMusicOrSound();
    }
    
}
