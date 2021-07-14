
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BuildReportTool
{
	public static class TextureDataGenerator
	{
		// create a BRT_TextureData, populate it with all images of the project
		// by going through all the files in BuildInfo.UsedAssets.All, looping through that
		// and checking if the Name matches what we expect image files to be (Util.IsTextureFile())
	}
}