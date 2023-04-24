using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppInterop.Runtime.Attributes;

using UnityEngine;

using AmongUs.Data;

using Dropship.Utilities;

namespace Dropship.Compat;

public sealed class BepInExUpdater : MonoBehaviour
{
    public static bool UpdateRequired => 
        typeof(IL2CPPChainloader).Assembly.GetName().Version < 
        Version.Parse(RequireBepInExVer);

    private static string RequireBepInExVer
    {
        get => requireBepInExVersion == string.Empty ?
            minimumBepInExVersion : requireBepInExVersion;
        set
        {
            requireBepInExVersion = value;
        }
    }

    private static string DlUrl
    {
        get => dlUrl == string.Empty ?
            dlUrl : bepInExDownloadURL;
        set
        {
            dlUrl = value;
        }
    }

    private static string requireBepInExVersion = string.Empty;
    private static string dlUrl = string.Empty;

    private const string minimumBepInExVersion = "6.0.0.565";
    private const string bepInExDownloadURL = "https://builds.bepinex.dev/projects/bepinex_be/565/BepInEx_UnityIL2CPP_x86_265107c_6.0.0-be.565.zip";

    private const string exeFileName = "DropshipBepInExInstaller.exe";

    public static void ReplaceRequireVersion(string newVersion, string dlUrl)
    {
        if (Version.Parse(newVersion) < Version.Parse(minimumBepInExVersion))
        {
            Logger<DropshipPlugin>.Error("Dropship Require new BepinEx!!");
            return;
        }
        RequireBepInExVer = newVersion;
        DlUrl = dlUrl;
    }

    public void Awake()
    {
        Logger<DropshipPlugin>.Info("BepInEx Update Required...");
        this.StartCoroutine(Excute());
    }

    [HideFromIl2Cpp]
    internal IEnumerator Excute()
    {
        string showStr = "ReqBepInExUpdate";

        Task.Run(() => DllApi.MessageBox(
            IntPtr.Zero,
            showStr, "Dropship", 0));

        string tmpFolder = Path.Combine(Paths.GameRootPath, "tmp");
        string zipPath = Path.Combine(tmpFolder, "BepInEx.zip");
        string extractPath = Path.Combine(tmpFolder, "BepInEx");

        FileIO.RecreateDir(zipPath);

        yield return FileIO.DlZipToSave(DlUrl, zipPath);
        
        ZipFile.ExtractToDirectory(zipPath, extractPath);

        Assembly asm = Assembly.GetExecutingAssembly();
        string exePath = asm.GetManifestResourceNames().FirstOrDefault(
            n => n.EndsWith(exeFileName));

        using (var resource = asm.GetManifestResourceStream(exePath))
        {
            using (var file = new FileStream(
                Path.Combine(tmpFolder, exeFileName),
                FileMode.OpenOrCreate, FileAccess.Write))
            {
                resource!.CopyTo(file);
            }
        }

        Process.Start(
            Path.Combine(Paths.GameRootPath, "tmp", exeFileName),
            $"{Paths.GameRootPath} {extractPath} {(uint)DataManager.Settings.Language.CurrentLanguage}");

        Application.Quit();
    }
}
