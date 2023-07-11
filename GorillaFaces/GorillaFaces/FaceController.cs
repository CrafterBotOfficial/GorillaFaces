using GorillaFaces.Models;
using HarmonyLib;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GorillaFaces
{
    internal static class FaceController
    {
        private static bool _facesLoaded;
        internal static Dictionary<string, CustomFace> CachedFaces = new Dictionary<string, CustomFace>();

        internal static List<VRRig> Rigs = new List<VRRig>();
        // internal static Dictionary<Player, VRRig> PlayerRigs = new Dictionary<Player, VRRig>();

        /* VRRig equiping controllers */

        internal static void UnEquipFace(Player player)
        {
            VRRig Rig = FindVRRigForPlayer(player);
            EquipFace(Rig, CachedFaces.ElementAt(0).Key);
        }

        internal static void EquipFace(Player player, string Id)
        {
            VRRig Rig = FindVRRigForPlayer(player);
            EquipFace(Rig, Id);
        }

        internal static void EquipFace(VRRig Rig, string Id)
        {
            if (!_facesLoaded)
            {
                Main.Log("Faces not loaded yet, this is a bit of a problem...", BepInEx.Logging.LogLevel.Error);
                return;
            }
            if (!CachedFaces.ContainsKey(Id))
            {
                Main.Log("Face not found: " + Id, BepInEx.Logging.LogLevel.Warning);
                return;
            }

            MeshRenderer meshRenderer = Rig.transform.Find("rig/body/head/gorillaface").GetComponent<MeshRenderer>();
            Material CustomMaterial = new Material(meshRenderer.material);
            CustomMaterial.SetTexture("_MainTex", CachedFaces[Id].face);
            meshRenderer.material = CustomMaterial;
        }

        internal static VRRig FindVRRigForPlayer(Player player)
        {
            return GameObject.FindObjectsOfType<VRRig>().First(x => Traverse.Create(x).Field("creator").GetValue<Player>() == player);
        }


        /* Data retreaving & formatting */

        internal static async void LoadFaces(bool AppendToOfflineOnFinish)
        {
            Main.Log("Loading faces...");
            CachedFaces.Clear();

            string path = Path.Combine(Directory.GetParent(typeof(FaceController).Assembly.Location).FullName, "CustomFaces");
            var enumerator = Directory.GetFiles(path, "*.Face").GetEnumerator();

            if (AppendToOfflineOnFinish)
            {
                Main.Log("Adding default face to the cache...");
                Texture2D texture2D = (Texture2D)GameObject.Find("Global/Local VRRig/Local Gorilla Player/rig/body/head/gorillaface").GetComponent<MeshRenderer>().material.mainTexture;
                Package package = new Package("Default", "Another-Axiom");
                CachedFaces.Add(GetId(package), new CustomFace(package, Object.Instantiate(texture2D)));
            }

            while (enumerator.MoveNext())
            {
                string currentPath = Path.GetFullPath((string)enumerator.Current);

                using (var zip = ZipFile.Open(currentPath, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry imageEntry = zip.GetEntry("image.png");
                    ZipArchiveEntry packageEnry = zip.GetEntry("package.json");

                    if (imageEntry == null || packageEnry == null)
                    {
                        Main.Log("Invalid face: " + currentPath);
                        continue;
                    }
                    using (StreamReader imageReader = new StreamReader(imageEntry.Open()))
                    using (StreamReader packageReader = new StreamReader(packageEnry.Open()))
                    {
                        Package package = Newtonsoft.Json.JsonConvert.DeserializeObject<Package>(packageReader.ReadToEnd());

                        // Loading the texture
                        Texture2D texture = new Texture2D(2, 2);
                        byte[] buffer = new byte[imageEntry.Length];
                        imageReader.BaseStream.Read(buffer, 0, buffer.Length);
                        texture.LoadImage(buffer);

                        CustomFace face = new CustomFace(package, texture);
                        CachedFaces.Add(GetId(face), face);
                    }
                }

                await Task.Yield(); // Prevents the game from studdering, cough cough Kyle
            }

            Main.Log("Loaded " + CachedFaces.Count + " faces.");
            _facesLoaded = true;

            // AppendToOfflineOnFinish
            if (AppendToOfflineOnFinish && GorillaTagger.hasInstance)
            {
                Main.Log("Auto appending face to offline VR rig.");
                EquipFace(GorillaTagger.Instance.offlineVRRig, Configuration.SelectedFace.Value);
                UpdateCustomProperties();
            }
        }

        internal static void UpdateCustomProperties()
        {
            Photon.Pun.PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { Main.PropertyKey, Configuration.SelectedFace.Value } });
        }

        private static string GetId(CustomFace customFaceModel)
        {
            return GetId(customFaceModel.Package);
        }

        private static string GetId(Package package)
        {
            return package.Name + "_" + package.Author;
        }
    }
}

/*        
        internal static VRRig FindVRRigForPlayer(Player player)
        {
            if (GorillaGameManager.instance is object)
            {
                return GorillaGameManager.instance.FindPlayerVRRig(player);
            }
        return GameObject.FindObjectsOfType<VRRig>().First(x => Traverse.Create(x).Field("photonView").GetValue<PhotonView>().Owner == player && !x.isOfflineVRRig);
    }
*/