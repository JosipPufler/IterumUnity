using Assets.Scripts.Utils.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class TextureMemorizer
    {
        public static Dictionary<string, Texture> textures = new();

        private static readonly string defaultImagePath = "Textures/default";

        public static void LoadTexture(string textureName, Action<Texture> onTextureFound) {
            if (textures.TryGetValue(textureName, out var texture)) { 
                onTextureFound.Invoke(texture);
                return;
            }
            Texture2D localTexture = Resources.Load<Texture2D>(textureName);

            if (localTexture != null)
            {
                textures.Add(textureName, localTexture);
                onTextureFound(localTexture);
            }
            else
            {
                AssetManager.Instance.PreviewImage(textureName, texture => {
                    textures.Add(textureName, texture);
                    onTextureFound.Invoke(texture); 
                }, error => { 
                    Debug.Log(error);
                    if (textures.TryGetValue(defaultImagePath, out var defaultTexture))
                    {
                        onTextureFound.Invoke(defaultTexture);
                    }
                    else
                    {
                        Texture2D defaultLoadedTexture = Resources.Load<Texture2D>(defaultImagePath);
                        textures.Add(defaultImagePath, defaultLoadedTexture);
                        onTextureFound.Invoke(defaultLoadedTexture);
                    }
                });
            }
        }
    }
}
