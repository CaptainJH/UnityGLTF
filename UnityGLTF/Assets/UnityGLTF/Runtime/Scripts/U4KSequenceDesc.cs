using UnityEngine;
using Newtonsoft.Json;

public class U4KSequenceDesc : MonoBehaviour
{
	public string SequenceName = string.Empty;
    public U4KSequence sequence;
	public string jsonString = string.Empty;

    public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.All,
        NullValueHandling = NullValueHandling.Ignore,
        // DefaultValueHandling = DefaultValueHandling.Ignore,
        Formatting = Formatting.Indented,
    };

    public void SaveToJson()
	{
        sequence.Name = SequenceName;
        jsonString = JsonConvert.SerializeObject(sequence, JsonSettings);
	}

	public U4KSequence LoadFromJson()
	{
		sequence = JsonConvert.DeserializeObject<U4KSequence>(jsonString, JsonSettings);
		return sequence;
	}
}
