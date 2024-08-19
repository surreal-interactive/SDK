#if UNITY_EDITOR && UNITY_IOS
using System.IO;
using UnityEngine;
using UnityEditor;

public class SVRBuildPostProcessor
{
    [UnityEditor.Callbacks.PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string path)
    {
        if (buildTarget == UnityEditor.BuildTarget.iOS)
        {
            string plistPath = path + "/Info.plist";
            UnityEditor.iOS.Xcode.PlistDocument plist = new UnityEditor.iOS.Xcode.PlistDocument();
            plist.ReadFromFile(plistPath);

            UnityEditor.iOS.Xcode.PlistElementDict rootDict = plist.root;
            rootDict.SetString("Privacy - Bluetooth Always Usage Description", "");
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}

#endif
