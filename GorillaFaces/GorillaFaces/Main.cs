using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;

namespace GorillaFaces
{
    [BepInPlugin(Id, Name, Version), BepInDependency("tonimacaroni.computerinterface"), BepInDependency("dev.auros.bepinex.bepinject")]
    internal class Main : BaseUnityPlugin
    {
        internal const string
            Id = "crafterbot.gorillafaces",
            Name = "GorillaFaces",
            Version = "0.0.7",
            PROPERTIES_KEY = "FaceId";

        internal static Main Instance;

        internal ManualLogSource manualLogSource => Logger;

        internal ConfigEntry<bool> EnableMirrorOnStartup;
        internal ConfigEntry<string> SelectedFaceId;

        internal Dictionary<string, Models.CustomFace> Faces;

        internal Main()
        {
            Instance = this;
            manualLogSource.LogInfo("GorillaFaces loaded");

            EnableMirrorOnStartup = Config.Bind("Settings", "Enable Mirror On Startup", true);
            SelectedFaceId = Config.Bind("SaveData", "Selected Face", "");

            Faces = new Dictionary<string, Models.CustomFace>();
            LoadAll(Directory.GetParent(typeof(Main).Assembly.Location) + "/CustomFaces");

            Bepinject.Zenjector.Install<Interface.MainInstaller>().OnProject();
            new HarmonyLib.Harmony(Id).PatchAll();
        }

        internal async void OfflineRigInitialized(VRRig __instance)
        {
            // await System.Threading.Tasks.Task.Delay(1000);

            new GameObject("Callbacks").AddComponent<Behaviours.Callbacks>();
            Main.Instance.Faces.ElementAt(0).Value.face = __instance.transform.Find("rig/body/head/gorillaface").GetComponent<MeshRenderer>().material.mainTexture as Texture2D;

            if (Main.Instance.Faces.ContainsKey(Main.Instance.SelectedFaceId.Value))
                Main.Instance.EquipFace(Main.Instance.SelectedFaceId.Value);
            if (Main.Instance.EnableMirrorOnStartup.Value)
            {
                GameObject mirror = GameObject.Find("/Level/lower level/mirror (1)");
                mirror.gameObject.SetActive(true);
                foreach (Collider collider in mirror.GetComponentsInChildren<Collider>())
                    GameObject.Destroy(collider);
                mirror.GetComponentInChildren<Camera>().cullingMask = 1788280631;
            }
        }

        internal void EquipFace(string Id)
        {
            SelectedFaceId.Value = Id;
            manualLogSource.LogMessage($"Equipping face {Id}");

            EquipFace(GorillaTagger.Instance.offlineVRRig, Id);
            Photon.Pun.PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { PROPERTIES_KEY, Id } });
        }

        internal void EquipFace(VRRig Rig, string Id)
        {
            // Validate the face id
            if (!Faces.ContainsKey(Id))
            {
                manualLogSource.LogError($"Face {Id} does not exist");
                return;
            }

            // Create the material
            Texture2D texture2D = Faces[Id].face;
            Material FaceMaterial = new Material(Shader.Find("Standard"));
            FaceMaterial.mainTexture = texture2D;

            // Equip the face
            Transform transform = Rig.transform.Find("rig/body/head/gorillaface");
            manualLogSource.LogInfo(transform);
            transform.GetComponent<MeshRenderer>().material = FaceMaterial;
        }

        private async void LoadAll(string TargetDirectory)
        {
            Faces.Add("none", new Models.CustomFace(new Models.Package("Default", "Another-Axiom"), new Texture2D(2, 2)));

            string[] Files = Directory.GetFiles(TargetDirectory, "*.Face");
            for (int i = 0; i < Files.Length; i++)
            {
                string FilePath = Files[i];
                manualLogSource.LogInfo($"Loading {FilePath}");
                // using (Stream stream = File.OpenRead(FilePath))
                using (ZipArchive zip = ZipFile.OpenRead(FilePath))
                {
                    ZipArchiveEntry PackageEntry = zip.GetEntry("package.json");
                    ZipArchiveEntry PNG = zip.GetEntry("image.png");

                    using (Stream PackageStream = PackageEntry.Open())
                    using (Stream PNGStream = PNG.Open())
                    {
                        StreamReader PackageReader = new StreamReader(PackageStream);
                        string Package = await PackageReader.ReadToEndAsync();
                        Models.Package PackageModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Package>(Package);

                        StreamReader PNGReader = new StreamReader(PNGStream);
                        byte[] PNGBytes = new byte[PNG.Length];
                        await PNGStream.ReadAsync(PNGBytes, 0, (int)PNG.Length);

                        Texture2D PNGTexture = new Texture2D(2, 2);
                        PNGTexture.LoadImage(PNGBytes);

                        Models.CustomFace Face = new Models.CustomFace(PackageModel, PNGTexture);
                        Faces.Add(PackageModel.Name + "_" + PackageModel.Author, Face);
                        manualLogSource.LogInfo($"Found {PackageModel.Name}");
                    }
                }
            }
        }
    }
}
