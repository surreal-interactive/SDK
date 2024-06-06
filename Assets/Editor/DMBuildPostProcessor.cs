#if UNITY_EDITOR && UNITY_IOS
using System.IO;
using UnityEngine;
using UnityEditor;


namespace Unity.XR.DMProvider
{

public class DMBuildPostProcessor
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

                Debug.Log(">> Automation, plist ... <<");

                // example of changing a value:
                // rootDict.SetString("CFBundleVersion", "6.6.6");
                rootDict.SetString("Privacy - Bluetooth Always Usage Description", "");
                // example of adding a boolean key...
                // < key > ITSAppUsesNonExemptEncryption </ key > < false />
                //rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }

    }

}
#endif
