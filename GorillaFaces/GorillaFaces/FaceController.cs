using GorillaFaces.Models;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Photon.Pun;

namespace GorillaFaces
{
    public static class FaceController
    {
        private static bool _facesLoaded;
        internal static Dictionary<string, CustomFace> CachedFaces = new Dictionary<string, CustomFace>();

        /* VRRig equiping controllers */

        public static void UnEquipFace(Player player)
        {
            VRRig Rig = FindVRRigForPlayer(player);
            if (Rig is object)
            {
                EquipFace(Rig, CachedFaces.ElementAt(0).Key);
                return;
            }
            Main.Log("It looks like the rig was never cached? Either way its not the end of the world, since it will be unequiped when the next person posses this rig.", BepInEx.Logging.LogLevel.Warning);
        }

        /// <summary>
        /// Attempts to equip a face to the client, if the player doesn't have the mod it will unequip current face to clean up.
        /// </summary>
        public static void AttemptEquip(Player player)
        {
            if (player.CustomProperties.TryGetValue(Main.PropertyKey, out object value)) EquipFace(player, (string)value);
            else UnEquipFace(player);
        }

        public static void EquipFace(Player player, string Id)
        {
            VRRig Rig = FindVRRigForPlayer(player);
            if (Rig is object)
            {
                EquipFace(Rig, Id);
                return;
            }
            Main.Log("Player rig not found: " + player.NickName, BepInEx.Logging.LogLevel.Error);
        }

        public static void EquipFace(VRRig Rig, string Id)
        {
            if (!_facesLoaded)
            {
                Main.Log("Faces not loaded yet, this is a bit of a problem...", BepInEx.Logging.LogLevel.Error);
                return;
            }
            if (!CachedFaces.ContainsKey(Id))
            {
                Main.Log("Face not found: " + Id, BepInEx.Logging.LogLevel.Warning);
                EquipFace(Rig, CachedFaces.ElementAt(0).Key); // Incase the rig already has a face, we need to clean it up
                return;
            }

            MeshRenderer FaceRenderer = Rig.transform.Find("rig/body/head/gorillaface").GetComponent<MeshRenderer>();
            MeshRenderer BodyRenderer = Rig.transform.Find("rig/body/gorillachest").GetComponent<MeshRenderer>();

            Material NewMaterial = CachedFaces[Id].FaceMaterial;
            FaceRenderer.material = NewMaterial;
            BodyRenderer.material = NewMaterial;
        }

        public static VRRig FindVRRigForPlayer(Player player)
        {
            return GorillaGameManager.StaticFindRigForPlayer(player); // :P - I feel like this was made for modding
        }


        /* Data retreaving & formatting */

        public static async void LoadFaces()
        {
            Main.Log("Loading faces...");
            CachedFaces.Clear();

            string path = Path.Combine(Directory.GetParent(typeof(FaceController).Assembly.Location).FullName, "CustomFaces");
            var enumerator = Directory.GetFiles(path, "*.Face").GetEnumerator();

            Main.Log("Adding default face to the cache...");
            Material DefaultMaterial = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/head/gorillaface").GetComponent<MeshRenderer>().material;
            Package DefaultPackage = new Package("Default", "Another-Axiom");
            CachedFaces.Add(GetId(DefaultPackage), new CustomFace(DefaultPackage, (Texture2D)DefaultMaterial.mainTexture, DefaultMaterial));

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
                        texture.filterMode = FilterMode.Point; // Prevents the texture from being blurry - Credit LunaKitty for suggesting this as a fix

                        // Load the material
                        Material NewMaterial = new Material(DefaultMaterial);
                        NewMaterial.mainTexture = texture;

                        CustomFace face = new CustomFace(package, texture, NewMaterial);
                        CachedFaces.Add(GetId(face), face);
                    }
                }

                await Task.Yield(); // Prevents the game from studdering, cough cough Kyle
            }

            Main.Log("Loaded " + CachedFaces.Count + " faces");
            _facesLoaded = true;

            // Append To Offline On Finish
            Main.Log("Auto appending face to offline VR rig.");
            EquipFace(GorillaTagger.Instance.offlineVRRig, Configuration.SelectedFace.Value);
            UpdateCustomProperties();
        }

        internal static void UpdateCustomProperties()
        {
            string selectedFace = Configuration.SelectedFace.Value;
            var hashTable = new ExitGames.Client.Photon.Hashtable() { { Main.PropertyKey, selectedFace } };
            if (hashTable is null)
            {
                Main.Log("A fatal error has occured whilst attempting to update the custom properties. Face:" + selectedFace, BepInEx.Logging.LogLevel.Fatal);
                throw;
            }
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashTable);

            // Since Utilla experienced a problem where it would ban the user by breaking the hash table I want to be extra careful.
            if (PhotonNetwork.LocalPlayer.CustomProperties is null || !Photon.Pun.PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("didTutorial"))
            {
                Main.Log("Quitting application! It appears that the hashtable is either null or missing a core property.");
                PhotonNetwork.Disconnect(); // Prevents tag lag or other issues if the player is the master.
                UnityEngine.Application.Quit();
            }
        }

        private static string GetId(CustomFace customFaceModel)
        {
            return GetId(customFaceModel.Package);
        }

        private static string GetId(Package package)
        {
            string Id = package.Name + "_" + package.Author;
            return Id.Length > 25 ? Id.Substring(0, 25) : Id; // Someone could theoretically spam servers by having a extremely long name and author, this prevents that.
        }
    }
}
