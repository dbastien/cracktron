using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class TextureManagementTreeViewItemData
{
    public Texture2D Texture;
    public TextureImporter Importer;
    public TextureImporterPlatformSettings ImporterPlatformDefaultSettings;
    public TextureImporterPlatformSettings ImporterPlatformActiveSettings;
    
    const string PlatformString = "Standalone";

    public TextureManagementTreeViewItemData(Texture2D texture, TextureImporter importer)
    {
        this.Texture = texture;
        this.Importer = importer;

        ImporterPlatformActiveSettings = Importer.GetPlatformTextureSettings(PlatformString);
        ImporterPlatformDefaultSettings = Importer.GetDefaultPlatformTextureSettings();
    }
}