using UnityEngine;
using Newtonsoft.Json;

public class U4KSequenceDesc : MonoBehaviour
{
	public string SequenceName = string.Empty;
    public CoursePlayer.Core.SceneSequence sequence;
	public string jsonString = string.Empty;
	public string GUID = string.Empty;

    public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.All,
        NullValueHandling = NullValueHandling.Ignore,
        // DefaultValueHandling = DefaultValueHandling.Ignore,
        Formatting = Formatting.Indented,
	};

    public void SaveToJson()
	{
		if (sequence == null) return;
        sequence.Name = SequenceName;
        jsonString = JsonConvert.SerializeObject(sequence, JsonSettings);
	}

	public CoursePlayer.Core.SceneSequence LoadFromJson()
	{
		sequence = JsonConvert.DeserializeObject<CoursePlayer.Core.SceneSequence>(jsonString, JsonSettings);
		return sequence;
	}
}
