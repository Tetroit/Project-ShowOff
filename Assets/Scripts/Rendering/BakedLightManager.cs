using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering.HighDefinition;
using JetBrains.Annotations;

[RequireComponent (typeof(Volume))]
public class BakedLightManager : MonoBehaviour
{
    private string jsonFileNameLightmaps = "mapconfig.txt"; // Name of the json data file.
    private string jsonFileNameSkybox = "skyconfig.txt"; // Name of the json data file.
    [SerializeField] private string m_resourceFolder = "LightMapDataTest1";
    public string resourceFolder { get { return m_resourceFolder; } }

    private string absoluteName;

    [System.Serializable]
    private class SphericalHarmonics
    {
        public float[] coefficients = new float[27];
    }

    [System.Serializable]
    private class RendererInfo
    {
        public Renderer renderer;
        public int lightmapIndex;
        public Vector4 lightmapOffsetScale;
    }

    [System.Serializable]
    private class LightingScenarioData
    {
        public RendererInfo[] rendererInfos;
        public Texture2D[] lightmaps;
        public Texture2D[] lightmapsDir;
        public Texture2D[] lightmapsShadow;
        public LightmapsMode lightmapsMode;
        public SphericalHarmonics[] lightProbes;
    }

    [System.Serializable]
    private class SkyboxInfo
    {
        //public CubemapParameter cubemap;
    }
    [SerializeField]
    private LightingScenarioData lightingScenariosData;
    [SerializeField]
    private SkyboxInfo skyboxInfo;

    [SerializeField] private bool loadOnAwake = false; // Load the selected lighmap when this script wakes up (aka when game starts).

    //TODO : enable logs only when verbose enabled
    [SerializeField] private bool verbose = false;

    public string resourceDir => Path.Combine(Application.dataPath, "Resources", m_resourceFolder);


    private void Start()
    {
        Load();
    }
    int n = 1;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            n = n % 2 + 1;
            Load("LightMapDataTest" + n);
        }
    }
    public string GetResourcesFile(string fileName)
    {
        return Path.Combine(resourceDir, fileName);
    }

    public bool CheckResourcesDirectoryExists(string dir)
    {
        CharacterController cc = gameObject.GetComponent<CharacterController>();
        return Directory.Exists(dir);
    }

    private void CreateResourcesDirectory(string dir)
    {
        if (!CheckResourcesDirectoryExists(m_resourceFolder))
        {
            Directory.CreateDirectory(resourceDir);
        }
    }

    public void Load(string folderName)
    {
        m_resourceFolder = folderName;
        Load();
    }
    public void Load()
    {
        lightingScenariosData = JsonUtils.LoadJsonData<LightingScenarioData>(GetResourcesFile(jsonFileNameLightmaps));

        var newLightmaps = new LightmapData[lightingScenariosData.lightmaps.Length];

        for (int i = 0; i < newLightmaps.Length; i++)
        {
            newLightmaps[i] = new LightmapData();
            newLightmaps[i].lightmapColor = Resources.Load<Texture2D>(m_resourceFolder + "/" + lightingScenariosData.lightmaps[i].name);

            if (lightingScenariosData.lightmapsMode != LightmapsMode.NonDirectional)
            {
                newLightmaps[i].lightmapDir = Resources.Load<Texture2D>(m_resourceFolder + "/" + lightingScenariosData.lightmapsDir[i].name);
                if (i<lightingScenariosData.lightmapsShadow.Length && lightingScenariosData.lightmapsShadow[i] != null)
                { // If the textuer existed and was set in the data file.
                    newLightmaps[i].shadowMask = Resources.Load<Texture2D>(m_resourceFolder + "/" + lightingScenariosData.lightmapsShadow[i].name);
                }
            }
        }

        LoadLightProbes();
        LoadSkybox();
        ApplyRendererInfo(lightingScenariosData.rendererInfos);

        LightmapSettings.lightmaps = newLightmaps;
    }

    private void LoadLightProbes()
    {
        var sphericalHarmonicsArray = new SphericalHarmonicsL2[lightingScenariosData.lightProbes.Length];

        for (int i = 0; i < lightingScenariosData.lightProbes.Length; i++)
        {
            var sphericalHarmonics = new SphericalHarmonicsL2();

            // j is coefficient
            for (int j = 0; j < 3; j++)
            {
                //k is channel ( r g b )
                for (int k = 0; k < 9; k++)
                {
                    sphericalHarmonics[j, k] = lightingScenariosData.lightProbes[i].coefficients[j * 9 + k];
                }
            }

            sphericalHarmonicsArray[i] = sphericalHarmonics;
        }

        try
        {
            LightmapSettings.lightProbes.bakedProbes = sphericalHarmonicsArray;
        }
        catch { Debug.LogWarning("Warning, error when trying to load lightprobes for scenario "); }
    }

    private void ApplyRendererInfo(RendererInfo[] infos)
    {
        try
        {
            for (int i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                info.renderer.lightmapIndex = infos[i].lightmapIndex;
                if (!info.renderer.isPartOfStaticBatch)
                {
                    info.renderer.lightmapScaleOffset = infos[i].lightmapOffsetScale;
                }
                if (info.renderer.isPartOfStaticBatch && verbose == true)
                {
                    Debug.Log("Object " + info.renderer.gameObject.name + " is part of static batch, skipping lightmap offset and scale.");
                }
            }
        }

        catch (Exception e)
        {
            Debug.LogError("Error in ApplyRendererInfo:" + e.GetType().ToString());
        }
    }



    public static BakedLightManager instance;
    private void Awake()
    {
        if (instance != null)
        { // Singleton pattern.
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        if (loadOnAwake)
        {
            Load();
        }
    }

    public void GenerateLightmapInfoStore()
    {
        lightingScenariosData = new LightingScenarioData();
        var newRendererInfos = new List<RendererInfo>();
        var newLightmapsTextures = new SortedList<int, Texture2D>();
        var newLightmapsTexturesDir = new SortedList<int, Texture2D>();
        var newLightmapsTexturesShadow = new SortedList<int, Texture2D>();
        var newLightmapsMode = new LightmapsMode();
        var newSphericalHarmonicsList = new List<SphericalHarmonics>();

        newLightmapsMode = LightmapSettings.lightmapsMode;

        var renderers = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        Debug.Log("stored info for " + renderers.Length + " meshrenderers");
        foreach (MeshRenderer renderer in renderers)
        {

            if (renderer.lightmapIndex != -1)
            {
                RendererInfo info = new RendererInfo();
                info.renderer = renderer;
                info.lightmapOffsetScale = renderer.lightmapScaleOffset;
                info.lightmapIndex = renderer.lightmapIndex;

                Texture2D lightmaplight = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;
                if (!newLightmapsTextures.ContainsKey(renderer.lightmapIndex))
                {
                    newLightmapsTextures.Add(renderer.lightmapIndex, lightmaplight);
                }

                if (newLightmapsMode != LightmapsMode.NonDirectional)
                {
                    //first directional lighting
                    Texture2D lightmapdir = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapDir;
                    if (!newLightmapsTexturesDir.ContainsKey(renderer.lightmapIndex))
                    {
                        newLightmapsTexturesDir.Add(renderer.lightmapIndex, lightmapdir);
                    }
                    //now the shadowmask
                    Texture2D lightmapshadow = LightmapSettings.lightmaps[renderer.lightmapIndex].shadowMask;
                    if (!newLightmapsTexturesShadow.ContainsKey(renderer.lightmapIndex))
                    {
                        newLightmapsTexturesShadow.Add(renderer.lightmapIndex, lightmapshadow);
                    }

                }
                newRendererInfos.Add(info);
            }
        }

        lightingScenariosData.lightmapsMode = newLightmapsMode;
        lightingScenariosData.lightmaps = newLightmapsTextures.Values.ToArray();

        if (newLightmapsMode != LightmapsMode.NonDirectional)
        {
            lightingScenariosData.lightmapsDir = newLightmapsTexturesDir.Values.ToArray();
            lightingScenariosData.lightmapsShadow = newLightmapsTexturesShadow.Values.ToArray();
        }

        lightingScenariosData.rendererInfos = newRendererInfos.ToArray();

        var scene_LightProbes = new SphericalHarmonicsL2[LightmapSettings.lightProbes.bakedProbes.Length];  
        scene_LightProbes = LightmapSettings.lightProbes.bakedProbes;

        for (int i = 0; i < scene_LightProbes.Length; i++)
        {
            var SHCoeff = new SphericalHarmonics();

            // j is coefficient
            for (int j = 0; j < 3; j++)
            {
                //k is channel ( r g b )
                for (int k = 0; k < 9; k++)
                {
                    SHCoeff.coefficients[j * 9 + k] = scene_LightProbes[i][j, k];
                }
            }

            newSphericalHarmonicsList.Add(SHCoeff);
        }

        lightingScenariosData.lightProbes = newSphericalHarmonicsList.ToArray();

        // write the files and map config data.
        CreateResourcesDirectory(m_resourceFolder);

        CopyTextureToResources(resourceDir, lightingScenariosData.lightmaps);
        CopyTextureToResources(resourceDir, lightingScenariosData.lightmapsDir);
        CopyTextureToResources(resourceDir, lightingScenariosData.lightmapsShadow);

        string json = JsonUtility.ToJson(lightingScenariosData, true);
        JsonUtils.WriteJsonFile(Path.Combine(resourceDir, jsonFileNameLightmaps), json);
    }

    private void CopyTextureToResources(string toPath, Texture2D[] textures)
    {
        for (int i = 0; i < textures.Length; i++)
        {
            Texture2D texture = textures[i];
            if (texture != null) // Maybe the optional shadowmask didn't exist?
            {
                FileUtil.ReplaceFile(
                    AssetDatabase.GetAssetPath(texture),
                    Path.Combine(toPath, Path.GetFileName(AssetDatabase.GetAssetPath(texture)))
                );
                AssetDatabase.Refresh(); // Refresh so the newTexture file can be found and loaded.
                Texture2D newTexture = Resources.Load<Texture2D>(Path.Combine(m_resourceFolder,texture.name)); // Load the new texture as an object.

                CopyTextureImporterProperties(textures[i], newTexture); // Ensure new texture takes on same properties as origional texture.

                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newTexture)); // Re-import texture file so it will be successfully compressed to desired format.
                EditorUtility.CompressTexture(newTexture, textures[i].format, TextureCompressionQuality.Best); // Now compress the texture.

                textures[i] = newTexture; // Set the new texture as the reference in the Json file.
            }
        }
    }

    private void CopyTextureImporterProperties(Texture2D fromTexture, Texture2D toTexture)
    {
        TextureImporter fromTextureImporter = GetTextureImporter(fromTexture);
        TextureImporter toTextureImporter = GetTextureImporter(toTexture);

        toTextureImporter.wrapMode = fromTextureImporter.wrapMode;
        toTextureImporter.anisoLevel = fromTextureImporter.anisoLevel;
        toTextureImporter.sRGBTexture = fromTextureImporter.sRGBTexture;
        toTextureImporter.textureType = fromTextureImporter.textureType;
        toTextureImporter.textureCompression = fromTextureImporter.textureCompression;
    }

    private TextureImporter GetTextureImporter(Texture2D texture)
    {
        string newTexturePath = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(newTexturePath) as TextureImporter;
        return importer;
    }

    public void SaveSkybox()
    {
        SkyboxInfo newSkyboxInfo = new SkyboxInfo();
        var volume = GetComponent<Volume>();
        if (volume.profile.TryGet<PhysicallyBasedSky>(out var skyboxOverride))
        {
            var cubemap = skyboxOverride;
        }
        //newSkyboxInfo.cubemap = cubemap;
        JsonUtils.WriteJsonData(GetResourcesFile(jsonFileNameSkybox), newSkyboxInfo);

    }

    public void LoadSkybox()
    {
        if (!File.Exists(GetResourcesFile(jsonFileNameSkybox)))
        {
            Debug.Log("Skybox info not found, no changes were made regarding the skybox");
            return;
        }
        skyboxInfo = JsonUtils.LoadJsonData<SkyboxInfo>(GetResourcesFile(jsonFileNameSkybox));

        var volume = GetComponent<Volume>();
        volume.profile.TryGet<HDRISky>(out var skyboxOverride);
        //skyboxOverride.hdriSky.Override(skyboxInfo.cubemap.value);
    }
}
