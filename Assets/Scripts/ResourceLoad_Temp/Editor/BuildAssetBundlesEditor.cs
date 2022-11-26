using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
 
public class BuildAssetBundlesEditor : MonoBehaviour
{
    /// <summary>
    /// AssetBundleManifestName == 对应AB依赖列表文件
    /// </summary>
 
//     private static string AssetBundle_BuildDirectory_Path = @Application.streamingAssetsPath + "/../../../" + "AssetBundles";
//     private static string AssetBundle_TargetDirectory_Path = @Application.streamingAssetsPath + "/" + "ABFiles";
//     [MenuItem("Tools/Asset Bundle/Build Asset Bundles", false, 0)]
//     public static void BuildAssetBundleAndroid()
//     {
//         //Application.streamingAssetsPath对应的StreamingAssets的子目录
//         DirectoryInfo AB_Directory = new DirectoryInfo(AssetBundle_BuildDirectory_Path);
//         if (!AB_Directory.Exists)
//         {
//             AB_Directory.Create();
//         }
//         FileInfo[] filesAB = AB_Directory.GetFiles();
//         foreach (var item in filesAB)
//         {
//             Debug.Log("******删除旧文件：" + item.FullName + "******");
//             item.Delete();
//         }
// #if UNITY_ANDROID
//         BuildPipeline.BuildAssetBundles(AB_Directory.FullName, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
// #elif UNITY_IPHONE
//         BuildPipeline.BuildAssetBundles(AB_Directory.FullName, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
// #endif
//         Debug.Log("******AssetBundle打包完成******");
//  
//         Debug.Log("将要转移的文件夹是：" + AssetBundle_TargetDirectory_Path);
//         FileInfo[] filesAB_temp = AB_Directory.GetFiles();
//  
//         DirectoryInfo streaming_Directory = new DirectoryInfo(AssetBundle_TargetDirectory_Path);
//  
//         FileInfo[] streaming_files = streaming_Directory.GetFiles();
//         foreach (var item in streaming_files)
//         {
//             item.Delete();
//         }
//         AssetDatabase.Refresh();
//         foreach (var item in filesAB_temp)
//         {
//             if (item.Extension == "")
//             {
//                 item.CopyTo(AssetBundle_TargetDirectory_Path + "/" + item.Name, true);
//             }
//         }
//         AssetDatabase.Refresh();
//         Debug.Log("******文件传输完成******");
//     }
    
    [MenuItem("Tools/Asset Bundle/Build Asset Bundles", false, 0)]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        if(!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, 
            BuildAssetBundleOptions.None, 
            BuildTarget.StandaloneWindows);
    }
    private static string _dirName = "";
    /// <summary>
    /// 批量命名所选文件夹下资源的AssetBundleName.
    /// </summary>
    [MenuItem("Tools/Asset Bundle/Set Asset Bundle Name")]
    static void SetSelectFolderFileBundleName()
    {
        UnityEngine.Object[] selObj = Selection.GetFiltered(typeof(Object), SelectionMode.Unfiltered);
        foreach (Object item in selObj)
        {
            string objPath = AssetDatabase.GetAssetPath(item);
            DirectoryInfo dirInfo = new DirectoryInfo(objPath);
            if (!dirInfo.Exists)
            {
                Debug.LogError("******请检查，是否选中了非文件夹对象******");
                return;
            }
            _dirName = dirInfo.Name;
 
            string filePath = dirInfo.FullName.Replace('\\', '/');
            filePath = filePath.Replace(Application.dataPath, "Assets");
            AssetImporter ai = AssetImporter.GetAtPath(filePath);
            ai.assetBundleName = _dirName;
 
            SetAssetBundleName(dirInfo);
        }
        AssetDatabase.Refresh();
        Debug.Log("******批量设置AssetBundle名称成功******");
    }
    static void SetAssetBundleName(DirectoryInfo dirInfo)
    {
        FileSystemInfo[] files = dirInfo.GetFileSystemInfos();
        foreach (FileSystemInfo file in files)
        {
            if (file is FileInfo && file.Extension != ".meta" && file.Extension != ".txt")
            {
                string filePath = file.FullName.Replace('\\', '/');
                filePath = filePath.Replace(Application.dataPath, "Assets");
                AssetImporter ai = AssetImporter.GetAtPath(filePath);
                ai.assetBundleName = _dirName;
            }
            else if (file is DirectoryInfo)
            {
                string filePath = file.FullName.Replace('\\', '/');
                filePath = filePath.Replace(Application.dataPath, "Assets");
                AssetImporter ai = AssetImporter.GetAtPath(filePath);
                ai.assetBundleName = _dirName;
                SetAssetBundleName(file as DirectoryInfo);
            }
        }
    }
    /// <summary>
    /// 批量清空所选文件夹下资源的AssetBundleName.
    /// </summary>
    [MenuItem("Tools/Asset Bundle/Reset Asset Bundle Name")]
    static void ResetSelectFolderFileBundleName()
    {
        UnityEngine.Object[] selObj = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Unfiltered);
        foreach (UnityEngine.Object item in selObj)
        {
            string objPath = AssetDatabase.GetAssetPath(item);
            DirectoryInfo dirInfo = new DirectoryInfo(objPath);
            if (dirInfo == null)
            {
                Debug.LogError("******请检查，是否选中了非文件夹对象******");
                return;
            }
            _dirName = null;
 
            string filePath = dirInfo.FullName.Replace('\\', '/');
            filePath = filePath.Replace(Application.dataPath, "Assets");
            AssetImporter ai = AssetImporter.GetAtPath(filePath);
            ai.assetBundleName = _dirName;
 
            SetAssetBundleName(dirInfo);
        }
        AssetDatabase.Refresh();
        Debug.Log("******批量清除AssetBundle名称成功******");
       
    }
}