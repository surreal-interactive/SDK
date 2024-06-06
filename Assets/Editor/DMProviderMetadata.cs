using System.Collections.Generic;
using UnityEngine;
/*
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.Management.Metadata;
#endif

namespace Unity.XR.DMProvider
{
#if UNITY_EDITOR
    public class DMProviderMetadata : IXRPackage
{

    private class DMProviderLoaderMetadata : IXRLoaderMetadata
    {
        public string loaderName { get; set; }
        public string loaderType { get; set; }
        public List<UnityEditor.BuildTargetGroup> supportedBuildTargets { get; set; }
    }

    private class DMProviderPackageMetadata : IXRPackageMetadata
    {
        public string packageName { get; set; }
        public string packageId { get; set; }
        public string settingsType { get; set; }

        public List<IXRLoaderMetadata> loaderMetadata { get; set; }
    }

    private static IXRPackageMetadata s_Metadata = new DMProviderPackageMetadata()
    {
        packageName = "DMProvider",
        packageId = "com.unity.xr.dmprovider",
        settingsType = "Unity.XR.DMProvider.DMProviderSettings",
        loaderMetadata = new List<IXRLoaderMetadata>() {
                new DMProviderLoaderMetadata() {
                        loaderName = "DMProviderLoader",
                        loaderType = "Unity.XR.DMProvider.DMProviderLoader",
                        supportedBuildTargets = new List<BuildTargetGroup>() {
                            BuildTargetGroup.Standalone,
                            BuildTargetGroup.Android,
                            BuildTargetGroup.iOS
                        }
                    },
                }
    };

    public IXRPackageMetadata metadata => s_Metadata;

    public bool PopulateNewSettingsInstance(ScriptableObject obj)
    {
        // SampleSettings packageSettings = obj as SampleSettings;
        // if (packageSettings != null)
        // {
            // Do something here if you need to...
        // }
        return true;
        // return false;
        
    }

}
#endif
}
*/
