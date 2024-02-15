using UnityEngine;

namespace GorillaFaces.Models;

public class CustomFace
{
    public Package Package;
    public Texture2D FaceTexture;
    public Material FaceMaterial;

    public CustomFace(Package package, Texture2D faceTexture, Material faceMaterial)
    {
        Package = package;
        FaceTexture = faceTexture;
        FaceMaterial = faceMaterial;
    }

    public string GetID()
    {
        return Package.Name + "_" + Package.Author;
    }
}