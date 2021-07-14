
using System.Collections.Generic;
using UnityEngine;

namespace BuildReportTool
{
	[System.Serializable, System.Xml.Serialization.XmlRoot("TextureData")]
	public class BRT_TextureData : MonoBehaviour
	{
		// ==================================================================================

		public struct Entry
		{
			public string AssetPath;

			public string TextureType; // whether this image is used as a Texture, GUI, Sprite, Cursor, Lightmap, etc.
			public string TextureFormat; // image format upon import (DXT, PVRTC, RGBA32 uncompressed, etc.)

			public string CompressionType; // maps to UnityEditor.TextureImporterCompression (only in Unity 5.5+)
			public int CompressionQuality; // maps to UnityEditor.TextureImporter.compressionQuality goes from 0 to 100
			public bool CompressionIsCrunched; // only in Unity 5.5+

			public int MaxTextureSize; // maps to UnityEditor.TextureImporter.maxTextureSize
			public string NPotScale; // how the image is resized if height/width isn't a power of two, if at all

			public bool IsReadable;
			public bool MipMapGenerated; // maps to UnityEditor.TextureImporter.mipmapEnabled
			public string WrapMode; // whether repeated or clamped

			// true: sRGB false: Linear.
			// for Unity 5.4 and below: maps to UnityEditor.TextureImporter.linearTexture (inverted value)
			// for Unity 5.5 and above: maps to UnityEditor.TextureImporter.sRGBTexture
			public bool IsSrgb;

			public int Width;
			public int Height;
		}

		// ==================================================================================

		Dictionary<string, Entry> _textureData = new Dictionary<string, Entry>();

		public List<string> Assets;
		public List<Entry> Data;

		public Dictionary<string, Entry> GetTextureData()
		{
			return _textureData;
		}

		// ==================================================================================

		public void OnBeforeSave()
		{
			if (Assets != null)
			{
				Assets.Clear();
			}
			else
			{
				Assets = new List<string>();
			}
			Assets.AddRange(_textureData.Keys);

			if (Data != null)
			{
				Data.Clear();
			}
			else
			{
				Data = new List<Entry>();
			}
			Data.AddRange(_textureData.Values);
		}

		public void OnAfterLoad()
		{
			_textureData.Clear();

			var len = Mathf.Min(Assets.Count, Data.Count);
			for (int n = 0; n < len; ++n)
			{
				_textureData.Add(Assets[n], Data[n]);
			}
		}

		// ==================================================================================
	}

}