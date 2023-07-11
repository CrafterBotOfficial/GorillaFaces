using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GorillaFaces.Models
{
    internal class CustomFace
    {
        internal Package Package { get; set; }
        internal Texture2D face { get; set; }

        internal CustomFace(Package package, Texture2D face)
        {
            Package = package;
            this.face = face;
        }

        internal string GetID()
        {
            return Package.Name + "_" + Package.Author;
        }
    }
}
