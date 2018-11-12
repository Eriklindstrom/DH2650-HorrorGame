// ExportPackage.cs
using UnityEngine;
using UnityEditor;

public class ExportPackage
{
    [MenuItem("Export/MyExport")]
    static void export()
    {
        AssetDatabase.ExportPackage(AssetDatabase.GetAllAssetPaths(), PlayerSettings.productName + ".unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);
    }
}
