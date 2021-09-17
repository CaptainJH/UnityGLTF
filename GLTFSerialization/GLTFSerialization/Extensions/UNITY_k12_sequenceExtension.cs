using System;
using GLTF.Math;
using GLTF.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GLTF.Schema
{
	class UNITY_k12_sequenceExtension : IExtension
	{
		public string JsonString = string.Empty;

		public IExtension Clone(GLTFRoot root)
		{
			return new UNITY_k12_sequenceExtension();
		}

		public JProperty Serialize()
		{
			JObject ext = new JObject();

			return new JProperty(UNITY_k12_sequenceExtensionFactory.EXTENSION_NAME, ext);
		}
	}
}
