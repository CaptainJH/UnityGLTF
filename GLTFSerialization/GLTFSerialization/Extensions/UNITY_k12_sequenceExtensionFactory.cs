using System;
using GLTF.Math;
using GLTF.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GLTF.Schema
{
	class UNITY_k12_sequenceExtensionFactory : ExtensionFactory
	{
		public const string EXTENSION_NAME = "UNITY_k12_sequence";

		public override IExtension Deserialize(GLTFRoot root, JProperty extensionToken)
		{

			if (extensionToken != null)
			{

			}

			return new UNITY_k12_sequenceExtension();
		}
	}
}
