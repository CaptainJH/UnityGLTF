using UnityEngine;
using Newtonsoft.Json;

public class U4KSequenceDesc : MonoBehaviour
{
	public string SequenceName = string.Empty;
    public CoursePlayer.Core.SceneSequence sequence;
	public string jsonString = string.Empty;
	public string GUID
	{
		get
		{
			if (string.IsNullOrEmpty(sequence.GUID))
				SetupGUID();
			return sequence.GUID;
		}
	}

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

	private void SetupGUID()
	{
		var seqs = transform.GetComponents<U4KSequenceDesc>();
		if (seqs.Length == 1)
		{
			sequence.GUID = System.Guid.NewGuid().ToString();
		}
		else
		{
			string guid = string.Empty;
			foreach (var seq in seqs)
			{
				if (seq != this)
				{
					if (string.IsNullOrEmpty(guid))
						guid = seq.sequence.GUID;

					Debug.Assert(guid == seq.sequence.GUID);
				}
				else if (seq == this)
					sequence.GUID = guid;
			}
		}

	}
}
