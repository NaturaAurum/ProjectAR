/*
 * https://github.com/5minlab/capture-the-base/blob/master/CaptureTheBase/Assets/FMLib/AssetNameAttribute.cs
 * Toy Clash 사운드 시스템(?) 을 이용해보자.
 */

using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

public class AssetNameAttribute : Attribute
{
    public string assetName;
    public AssetNameAttribute(string name)
    {
        assetName = name;
    }

#if UNITY_EDITOR
    public UnityEngine.Object FindAsset(Type t)
    {
        var ext = Path.GetExtension(assetName);
        bool hasExtension = (ext != "");

        var filename = Path.GetFileNameWithoutExtension(assetName);
        var founds = AssetDatabase.FindAssets(filename);
        foreach (var guid in founds)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);

            bool matching = false;
            if (hasExtension)
            {
                matching = IsMatchingWithExtension(assetPath, assetName);
            }
            else
            {
                matching = IsMatchingWithoutExtension(assetPath, assetName);
            }
            if (matching)
            {
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, t);
                return asset;
            }
        }
        return null;
    }

    bool IsMatchingWithExtension(string assetPath, string name)
    {
        return (Path.GetFileName(assetPath) == name);
    }

    bool IsMatchingWithoutExtension(string assetPath, string name)
    {
        return (Path.GetFileNameWithoutExtension(assetPath) == name);
    }

    public static void ConnectMemberAssets(object obj)
    {
        var t = obj.GetType();
        var fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        foreach (var f in fields)
        {
            var attrs = f.GetCustomAttributes(typeof(AssetNameAttribute), false);
            if (attrs.Length == 0)
            {
                continue;
            }

            var attr = attrs[0] as AssetNameAttribute;
            var prev = f.GetValue(obj);
            if (prev == null || prev.ToString() == "null")
            {
                var asset = attr.FindAsset(f.FieldType);
                f.SetValue(obj, asset);
            }
        }

    }
#endif
}

class AudioClipLoadTypeAttribute : Attribute
{
    public AudioClipLoadType LoadType { get; private set; }
    public AudioClipLoadTypeAttribute(AudioClipLoadType loadType)
    {
        this.LoadType = loadType;
    }
}

class StreamingLoadTypeAttribute : AudioClipLoadTypeAttribute
{
    public StreamingLoadTypeAttribute() : base(AudioClipLoadType.Streaming) { }
}

class AudioClipCompressionFormatAttribute : Attribute
{
    public AudioCompressionFormat Format { get; private set; }
    public AudioClipCompressionFormatAttribute(AudioCompressionFormat format)
    {
        this.Format = format;
    }
}