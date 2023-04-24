using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

using Dropship.Unity.Extension;

namespace Dropship.Unity;

public static class SpriteHelper
{
    private static Dictionary<string, Sprite> cachedSprite = new Dictionary<string, Sprite>();

    public static Sprite GetSpriteFromEmbeddedResources(
        string path, Vector2 pivot, Func<Texture2D, float> pixelsCompute)
    {
        try
        {
            if (cachedSprite.TryGetValue(path, out Sprite sprite)) { return sprite; }
            sprite = CreateAndRegisterSpriteFromTexture(
                path, LoadTextureFromResources(
                    path, Assembly.GetCallingAssembly()),
                pivot, pixelsCompute);
            return sprite;
        }
        catch (Exception e)
        {
            Logger<DropshipPlugin>.Error($"Get Sprite Error!!  Exc:{e}");
        }
        return null;
    }

    public static Sprite GetSpriteFromStorage(
        string path, Vector2 pivot, Func<Texture2D, float> pixelsCompute)
    {
        try
        {
            if (cachedSprite.TryGetValue(path, out Sprite sprite)) { return sprite; }

            sprite = CreateAndRegisterSpriteFromTexture(
                path, LoadTextureFromDisk(path),
                pivot, pixelsCompute);
            return sprite;
        }
        catch (Exception e)
        {
            Logger<DropshipPlugin>.Error($"Get Sprite Error!!  Exc:{e}");
        }
        return null;
    }

    public static Sprite CreateAndRegisterSpriteFromTexture(
        string registerKey, Texture2D texture,
        Vector2 pivot, Func<Texture2D, float> pixelsCompute)
    {
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            pivot, pixelsCompute.Invoke(texture));

        sprite.DontUnload();
        Register(registerKey, sprite);

        return sprite;
    }

    public static Texture2D LoadTextureFromDisk(string path)
    {
        try
        {
            Texture2D texture = CreateEmptyTexture();
            byte[] byteTexture = File.ReadAllBytes(path);
            ImageConversion.LoadImage(texture, byteTexture, false);

            return texture;
        }
        catch (Exception e)
        {
            Logger<DropshipPlugin>.Warning($"Create Texture Faill!!  from:{path}  Exc:{e}");
        }
        return null;
    }

    public static unsafe Texture2D LoadTextureFromResources(
        string path, Assembly callAssembly)
    {
        try
        {
            Texture2D texture = CreateEmptyTexture();
            Stream stream = callAssembly.GetManifestResourceStream(path);
            long length = stream.Length;
            var byteTexture = new Il2CppStructArray<byte>(length);
            var read = stream.Read(new Span<byte>(
                IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
            ImageConversion.LoadImage(texture, byteTexture, false);
            return texture;
        }
        catch (Exception e)
        {
            Logger<DropshipPlugin>.Warning($"Create Texture Faill!!  from:{path}  Exc:{e}");
        }
        return null;
    }

    internal static void Register(string key, Sprite img)
    {
        cachedSprite.Add(key, img);
    }

    private static Texture2D CreateEmptyTexture() =>
        new Texture2D(2, 2, TextureFormat.ARGB32, true);
}
