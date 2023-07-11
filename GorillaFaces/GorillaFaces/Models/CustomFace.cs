using UnityEngine;

namespace GorillaFaces.Models
{
    internal class CustomFace
    {
        internal Package Package { get; set; }
        internal Texture2D FaceTexture { get; set; }
        internal Material FaceMaterial { get; set; }

        internal CustomFace(Package package, Texture2D faceTexture, Material faceMaterial)
        {
            Package = package;
            FaceTexture = faceTexture;
            FaceMaterial = faceMaterial;
        }

        internal string GetID()
        {
            return Package.Name + "_" + Package.Author;
        }
    }
}
