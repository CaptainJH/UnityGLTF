using System;
using GLTF.Math;
using GLTF.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GLTF.Schema
{
	public class UNITY_k12_sequenceExtensionFactory : ExtensionFactory
	{
		public const string EXTENSION_NAME = "UNITY_k12_sequence";
		public const string JSONTEXT = "U4KSequenceJsonString";

		public override IExtension Deserialize(GLTFRoot root, JProperty extensionToken)
		{
			string jsonTex = string.Empty;
			if (extensionToken != null)
			{
				JToken texToken = extensionToken.Value[JSONTEXT];
				jsonTex = texToken.ToString();
			}

			return new UNITY_k12_sequenceExtension(jsonTex);
		}
	}
}
