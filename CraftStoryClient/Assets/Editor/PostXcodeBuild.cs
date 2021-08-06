using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class PostXcodeBuild
{
#if UNITY_IOS
    [PostProcessBuild]
    public static void SetXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        // pbx
        {
            var projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            var pbx = new PBXProject();
            pbx.ReadFromFile(projectPath);
            string targetGuid = GetTargetGuid(pbx);

            // admob
            pbx.AddFrameworkToProject(targetGuid, "AppTrackingTransparency.framework", true);

            pbx.WriteToFile(projectPath);
        }

        // plist
        {
            var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // skad
            var array = plist.root.CreateArray("SKAdNetworkItems");
            PlistElementDict dict = array.AddDict();
            dict.SetString("SKAdNetworkIdentifier", "cstr6suwn9.skadnetwork");
            File.WriteAllText(plistPath, plist.WriteToString());

            var root = plist.root;

            // track description
            root.SetString("NSUserTrackingUsageDescription", "本アプリは広告効果測定・分析のためにIDFA(広告識別子)を利用します。");
            plist.WriteToFile(plistPath);
        }

    }

    private static string GetTargetGuid(PBXProject pbx)
    {
#if UNITY_2019_3_OR_NEWER
        return pbx.GetUnityFrameworkTargetGuid();
#else
        return pbx.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif
    }
#endif
}