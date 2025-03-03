using SuperUnityBuild.BuildActions;
using SuperUnityBuild.BuildTool;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class DeleteDebugFiles : BuildAction, IPostBuildPerPlatformAction
{
    public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
    {
        base.PerBuildExecute(releaseType, platform, architecture, scriptingBackend, distribution, buildTime, ref options, configKey, buildPath);
        string outputPath = buildPath;
        try
        {
            string applicationName = releaseType.productName;
            string outputFolder = outputPath;
            Assert.IsNotNull(outputFolder);
            outputFolder = Path.GetFullPath(outputFolder);

            //Delete Burst Debug Folder
            string burstDebugInformationDirectoryPath = Path.Combine(outputFolder, $"{applicationName}_BurstDebugInformation_DoNotShip");

            if (Directory.Exists(burstDebugInformationDirectoryPath))
            {
                Debug.Log($"Deleting Burst debug information folder at path '{burstDebugInformationDirectoryPath}'...");
                FileOperation.Delete(burstDebugInformationDirectoryPath);
                //Directory.Delete(burstDebugInformationDirectoryPath, true);
            }

            //Delete il2cpp Debug Folder
            string il2cppDebugInformationDirectoryPath = Path.Combine(outputFolder, $"{applicationName}_BackUpThisFolder_ButDontShipItWithYourGame");

            if (Directory.Exists(il2cppDebugInformationDirectoryPath))
            {
                Debug.Log($"Deleting Burst debug information folder at path '{il2cppDebugInformationDirectoryPath}'...");
                FileOperation.Delete(il2cppDebugInformationDirectoryPath);
                //Directory.Delete(il2cppDebugInformationDirectoryPath, true);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"An unexpected exception occurred while performing build cleanup: {e}");
        }
    }
}
