/*
MIT License

Copyright (c) 2016 xiaobin83

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


namespace ui
{
	class AtlasConfig
	{
		public static string[] names;
		public static Rect[] rects;
		public static string path;
		public static bool mipmap;
		public static string packingTag;
		public static SpriteImportMode spriteImportMode;
	}


	public class AtlasPostProcessor : AssetPostprocessor
	{
		void OnPreprocessTexture()
		{
			if (!string.IsNullOrEmpty(AtlasConfig.path))
			{
				if (assetPath.ToLower() == AtlasConfig.path.ToLower())
				{
					TextureImporter textureImporter = (TextureImporter)assetImporter;
					textureImporter.mipmapEnabled = AtlasConfig.mipmap;
					textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
					textureImporter.alphaIsTransparency = true;
					textureImporter.spriteImportMode = AtlasConfig.spriteImportMode;
					if (AtlasConfig.spriteImportMode == SpriteImportMode.Multiple)
					{
						// TODO:
					}
				}
			}
		}
	}

	public class AtlasBakerWizard : ScriptableWizard
	{

		public delegate void OnAtlasBaked(string texturePath, string[] names, Rect[] uvs);

		public string[] pathToTextures;
		public int maxAtlasSize = 2048;
		public int padding = 2;
		public bool mipmap = false;
		public string packingTag = "";
		public SpriteImportMode spriteImportMode = SpriteImportMode.None;

		OnAtlasBaked onAtlasBaked;

		[MenuItem("Emoji/Atlas Baker")]
		public static void ShowAtlasBakerWizard()
		{
			ShowAtlasBakerWizard(null);
		}

		void Init(OnAtlasBaked onAtlasBaked)
		{
			this.onAtlasBaked = onAtlasBaked;
		}

		public static void ShowAtlasBakerWizard(OnAtlasBaked onAtlasBaked)
		{
			var wizard = DisplayWizard<AtlasBakerWizard>("Atlas Baker", "Exit", "Atlas");
			wizard.Init(onAtlasBaked);
		}


		void DoAtlas()
		{
			if (pathToTextures == null || pathToTextures.Length == 0)
				return;

			// 1. get all textures
			var names = new List<string>();
			var texturesToAltas = new List<Texture2D>();

			var files = new List<string>();
			for (int i = 0; i < pathToTextures.Length; ++i)
			{
				if (System.IO.Directory.Exists(pathToTextures[i]))
				{
					var f = System.IO.Directory.GetFiles(pathToTextures[i], "*.png");
					files.AddRange(f);
				}
			}

			foreach (var f in files)
			{
				var t = new Texture2D(2, 2);
				using (var fs = new System.IO.FileStream(f, System.IO.FileMode.Open))
				{
					var br = new System.IO.BinaryReader(fs);
					var data = br.ReadBytes((int)fs.Length);
					if (t.LoadImage(data))
					{
						names.Add(System.IO.Path.GetFileNameWithoutExtension(f));
						texturesToAltas.Add(t);
					}
				}
			}

			// 2. do atlas
			var texture = new Texture2D(2, 2);
			var rects = texture.PackTextures(texturesToAltas.ToArray(), padding, maxAtlasSize);
			var assetPathOfTexture = AssetDatabase.GenerateUniqueAssetPath("Assets/BakedAtlasTexture") + ".png";
			var bytes = texture.EncodeToPNG();
			System.IO.File.WriteAllBytes(assetPathOfTexture, bytes);


			// 3. prepare sprites
			AtlasConfig.path = assetPathOfTexture;
			AtlasConfig.names = names.ToArray();
			AtlasConfig.rects = rects;
			AtlasConfig.mipmap = mipmap;
			AtlasConfig.packingTag = packingTag;
			AtlasConfig.spriteImportMode = spriteImportMode;

			// 4. import
			AssetDatabase.ImportAsset(assetPathOfTexture);
			AssetDatabase.SaveAssets();

			if (onAtlasBaked != null)
			{
				onAtlasBaked(assetPathOfTexture, names.ToArray(), rects);
			}
		}

		void OnWizardUpdate()
		{
			if (pathToTextures == null || pathToTextures.Length == 0)
			{
				errorString = "Please provide path to textures to altas.";
				return;
			}

			foreach (var p in pathToTextures)
			{
				if (string.IsNullOrEmpty(p))
				{
					errorString = "empty path found!";
					return;
				}
				if (!System.IO.Directory.Exists(p))
				{
					errorString = p + " not found";
					return;
				}
			}

			errorString = string.Empty;
		}

		void OnWizardCreate()
		{
		}

		void OnWizardOtherButton()
		{
			DoAtlas();
		}



	}
}


