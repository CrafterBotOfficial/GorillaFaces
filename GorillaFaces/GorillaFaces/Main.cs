using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Newtonsoft.Json;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;

namespace GorillaFaces
{
    [BepInPlugin(ID, NAME, VERSION), BepInDependency("tonimacaroni.computerinterface"), BepInDependency("dev.auros.bepinex.bepinject")]
    internal class Main : BaseUnityPlugin
    {
        internal const string
            ID = "crafterbot.gorillafaces",
            NAME = "GorillaFaces",
            VERSION = "1.0.0",
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
            new HarmonyLib.Harmony(ID).PatchAll();
        }

        internal void EquipFace(string Id)
        {
            manualLogSource.LogInfo($"Equipping face {Id}");

            SelectedFaceId.Value = Id;
            List<VRRig> Rigs = new List<VRRig>() { GorillaTagger.Instance.offlineVRRig };
            if (GorillaTagger.Instance.myVRRig is object)
                Rigs.Add(GorillaTagger.Instance.myVRRig);

            EquipFace(Rigs.ToArray(), Id);
            Photon.Pun.PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { PROPERTIES_KEY, Id } });
        }
        internal void EquipFace(Player player, string Id)
        {
            if (!Faces.ContainsKey(Id))
                return;
            VRRig Rig = FindObjectsOfType<VRRig>().First(x => x.photonView.Owner == player);
            EquipFace(new VRRig[] { Rig }, Id);
        }

        internal void EquipFace(VRRig[] Rig, string Id)
        {
            Texture2D texture2D = Faces[Id].face;
            Material FaceMaterial = new Material(Shader.Find("Standard"));
            FaceMaterial.mainTexture = texture2D;

            var enumerator = Rig.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Transform transform = (enumerator.Current as VRRig).transform.Find("rig/body/head/gorillaface");
                manualLogSource.LogInfo(transform);
                transform.GetComponent<MeshRenderer>().material = FaceMaterial;
            }
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
                        Models.Package PackageModel = JsonConvert.DeserializeObject<Models.Package>(Package);

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
