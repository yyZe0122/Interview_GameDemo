using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器资源管理器
/// 只有在开发时使用的
/// 发布后不可使用
/// </summary>
public class EditorResMgr : BaseManager<EditorResMgr>
{
    //用于放置放置需要打包进AB包的资源路径
    private string rootPath = "Assets/Editor/ArtRes/";
    
    /// <summary>
    /// 加载单个资源
    /// 需要添加后缀名(基类中已反射完成 调用时不需要填入)
    /// 如果有新的 需要添加新的后缀名代码
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T LoadEditorRes<T>(string path) where T : Object
    {
#if UNITY_EDITOR
        //后缀名
        string suffixName = "";
        //判断是否为预设体 纹理 材质球 音效等等
        //预设体
        if (typeof(T) == typeof(GameObject))
        {
            suffixName = ".prefab";
        }
        //图片
        else if (typeof(T) == typeof(Sprite))
        {
            suffixName = ".png";
        }
        //音效
        else if (typeof(T) == typeof(AudioClip))
        {
            suffixName = ".mp3";
        }
        //加载资源
        T res = AssetDatabase.LoadAssetAtPath<T>(rootPath + path + suffixName);
        return res;
#else
        return null;
#endif 
    }
    
    /// <summary>
    /// 加载图集相关资源
    /// </summary>
    /// <param name="path">图集的名字</param>
    /// <param name="spriteName">图片的名字</param>
    /// <returns></returns>
    public Sprite LoadSprite(string path, string spriteName)
    {
#if UNITY_EDITOR
        //加载图集中的所有子资源
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        //遍历所有子资源 得到同名图片返回
        foreach (var item in sprites)
        {
            if (spriteName == item.name)
            {
                return item as Sprite;
            }
        }
        return null;
#else
        return null;
#endif 
    }

    /// <summary>
    /// 用字典加载图集文件中的所有图片 并且返回给外部
    /// </summary>
    /// <param name="path">图集资源地址</param>
    /// <returns></returns>
    public Dictionary<string, Sprite> LoadSprites(string path)
    {
#if UNITY_EDITOR
        Dictionary<string, Sprite> spritesDic = new Dictionary<string, Sprite>();
        //加载图集中的所有子资源
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        //遍历所有子资源 得到同名图片返回
        foreach (var item in sprites)
        {
            spritesDic.Add(item.name,item as Sprite);
        }
        return spritesDic;
#else
        return null;
#endif 
    }
    
    private EditorResMgr()
    {
        
    }
}
