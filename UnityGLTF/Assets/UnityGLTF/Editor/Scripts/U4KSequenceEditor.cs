using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(U4KSequenceDesc))]
public class U4KSequenceEditor : Editor
{
	SerializedProperty NameProp;

	U4KSequenceDesc seqDesc;

	string[] triggerTypes = {
        "OnClick",
        "OnPluginEvent",
		"OnEnterSlide",
        };
	int triggerTypeIndex = 0;


	string[] actionTypes = {
		//"RunPython",
		"CallPluginAction",
		"Appear",
		"Disappear",
		//"Flash",
		//"Rotate",
		//"Move",
		//"Wipe",
		//"TextWriterEffect",
		//"ChangeColor",
		//"ChangeBackgroundColor",
		//"ChangeContent",
		//"GotoAndPlay",
		//"GotoAndStop",
		"PlayAnimation",
		"CameraTurnTo",
		};
	int actionTypeIndex = 0;

	void OnEnable()
	{
		NameProp = serializedObject.FindProperty("SequenceName");
		seqDesc = target as U4KSequenceDesc;
		seqDesc.LoadFromJson();
		if (seqDesc.sequence == null) seqDesc.sequence = new CoursePlayer.Core.SceneSequence();

		if (seqDesc.sequence.Trigger.GetType() == typeof(CoursePlayer.Core.ClickEventDesc))
			triggerTypeIndex = 0;
		else if (seqDesc.sequence.Trigger.GetType() == typeof(CoursePlayer.Core.PluginEventDesc))
			triggerTypeIndex = 1;
		else if (seqDesc.sequence.Trigger.GetType() == typeof(CoursePlayer.Core.OnEnterSlideEventDesc))
			triggerTypeIndex = 2;
	}

	public override void OnInspectorGUI()
	{
		var allSeqDesc = GameObject.FindObjectsOfType<U4KSequenceDesc>();
		serializedObject.Update();
        EditorGUILayout.PropertyField(NameProp);
		EditorGUILayout.LabelField(seqDesc.GUID);

        var originIndex = triggerTypeIndex;
		triggerTypeIndex = EditorGUILayout.Popup(triggerTypeIndex, triggerTypes);
		if (originIndex != triggerTypeIndex)
		{
			switch (triggerTypeIndex)
			{
				case 0:
					seqDesc.sequence.Trigger = new CoursePlayer.Core.ClickEventDesc();
					break;

				case 1:
					seqDesc.sequence.Trigger = new CoursePlayer.Core.PluginEventDesc();
					break;

				case 2:
					seqDesc.sequence.Trigger = new CoursePlayer.Core.OnEnterSlideEventDesc();
					break;

				default:
					break;
			}
			EditorUtility.SetDirty(target);
		}

		if (seqDesc.sequence.Trigger != null)
		{
			var triggerType = seqDesc.sequence.Trigger.GetType();
			PropertyInfo[] props = null;
			props = seqDesc.sequence.Trigger.GetType().GetProperties();

			if (props != null)
			{
				foreach (var p in props)
				{
					if (p.PropertyType.IsEnum)
					{
						var allEnums = System.Enum.GetValues(p.PropertyType);
						var allEnumsArray = allEnums.OfType<object>().Select(o => o.ToString()).ToArray();
						var curValue = p.GetValue(seqDesc.sequence.Trigger);
						var curIndex = Array.FindIndex(allEnumsArray, e => e == curValue.ToString());
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(p.Name);
						var nextIndex = EditorGUILayout.Popup(curIndex, allEnumsArray);
						EditorGUILayout.EndHorizontal();

						if (curIndex != nextIndex)
						{
							p.SetValue(seqDesc.sequence.Trigger, allEnums.GetValue(nextIndex));
							EditorUtility.SetDirty(target);
						}
					}
					else if (p.PropertyType == typeof(string))
					{
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(p.Name);
						var eventName = p.GetValue(seqDesc.sequence.Trigger) as string;
						var newName = EditorGUILayout.TextField(eventName);
						if (newName != eventName)
						{
							p.SetValue(seqDesc.sequence.Trigger, newName);
							EditorUtility.SetDirty(target);
						}
						EditorGUILayout.EndHorizontal();
					}
				}
			}
		}

		if (seqDesc.sequence.Actions.Count > 0)
		{
			currentActionIndex = 0;
			foreach (var ac in seqDesc.sequence.Actions)
			{
				EditorGUILayout.BeginFoldoutHeaderGroup(true, ac.ToString(), EditorStyles.centeredGreyMiniLabel, ShowActionContextMenu);

				var t = ac.GetType();
				var props = t.GetProperties();
				foreach (var p in props)
				{
					if (p.PropertyType == typeof(string))
					{
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(p.Name);
						var originV = p.GetValue(ac) as string;
						var curV = EditorGUILayout.TextField(originV);
						if (curV != originV)
						{
							p.SetValue(ac, curV);
							EditorUtility.SetDirty(target);
						}
						EditorGUILayout.EndHorizontal();
					}
					else if (p.PropertyType == typeof(float))
					{
						var originV = (float)p.GetValue(ac);
						var curV = EditorGUILayout.FloatField(p.Name, originV);
						if (curV != originV)
						{
							p.SetValue(ac, curV);
							EditorUtility.SetDirty(target);
						}
					}
					else if (p.PropertyType == typeof(U4KSequenceDesc))
					{
						if (ac is CoursePlayer.Core.CameraTurnToAction)
						{
							var cta = ac as CoursePlayer.Core.CameraTurnToAction;
							U4KSequenceDesc originTarget = null;
							if (!string.IsNullOrEmpty(cta.TargetGUID) && cta.Target == null)
							{
								foreach (var desc in allSeqDesc)
									if (desc.GUID == cta.TargetGUID)
									{
										originTarget = desc;
										break;
									}
							}
							else
								originTarget = p.GetValue(ac) as U4KSequenceDesc;
							var curV = EditorGUILayout.ObjectField(p.Name, originTarget, typeof(U4KSequenceDesc));
							if (originTarget != curV && ac is CoursePlayer.Core.CameraTurnToAction)
							{
								cta.TargetGUID = (curV as U4KSequenceDesc).GUID;
								p.SetValue(ac, curV);
								EditorUtility.SetDirty(target);
							}
						}
					}
					else if (p.Name == "AffectedControls")
					{
						EditorGUILayout.LabelField("Affected Scene Objects");
						int newCount = EditorGUILayout.IntField("Count", ac.AffectedControls.Count);
						if (newCount >= 0)
						{
							if (newCount < ac.AffectedControls.Count)
							{
								ac.AffectedControls.RemoveRange(newCount, ac.AffectedControls.Count - newCount);
								EditorUtility.SetDirty(target);
							}
							else if (newCount > ac.AffectedControls.Count)
							{
								var originCount = ac.AffectedControls.Count;
								for (int i = 0; i < newCount - originCount; ++i)
									ac.AffectedControls.Add(string.Empty);
								EditorUtility.SetDirty(target);
							}
						}

						for (int i = 0; i < ac.AffectedControls.Count; ++i)
						{
							var objGUID = ac.AffectedControls[i];
							U4KSequenceDesc sceneObj = null;
							if (!string.IsNullOrEmpty(objGUID))
							{
								sceneObj = Array.Find(allSeqDesc, (U4KSequenceDesc seq) =>
								{
									return seq.GUID == objGUID;
								});
							}

							var newObj = EditorGUILayout.ObjectField(sceneObj, typeof(U4KSequenceDesc)) as U4KSequenceDesc;
							if (newObj != sceneObj)
							{
								ac.AffectedControls[i] = newObj.GUID;
								EditorUtility.SetDirty(target);
							}
						}
					}

				}

                EditorGUILayout.EndFoldoutHeaderGroup();
				EditorGUILayout.Separator();
				++currentActionIndex;
            }
		}

		if (GUILayout.Button("Add Action", EditorStyles.toolbarButton))
		{
			GenericMenu toolsMenu = new GenericMenu();
			foreach (var it in actionTypes)
			{
				toolsMenu.AddItem(new GUIContent(it), false, OnAddAction, it);
			}
			// Offset menu from right of editor window
			toolsMenu.DropDown(new Rect(Screen.width - 216 - 40, 20, 0, 16));
			EditorGUIUtility.ExitGUI();
		}
		serializedObject.ApplyModifiedProperties();

		seqDesc.SaveToJson();
	}

	void OnAddAction(object userData)
	{
		string newActionName = userData as string;
		CoursePlayer.Core.AnimAction newAction = null;
		if (newActionName == "CallPluginAction")
		{
			newAction = new CoursePlayer.Core.CallPluginAction();
		}
		else if (newActionName == "Appear")
		{
			newAction = new CoursePlayer.Core.AppearAction();
		}
		else if (newActionName == "Disappear")
		{
			newAction = new CoursePlayer.Core.DisappearAction();
		}
		else if (newActionName == "PlayAnimation")
		{
			newAction = new CoursePlayer.Core.PlayAnimationAction();
		}
		else if (newActionName == "CameraTurnTo")
		{
			newAction = new CoursePlayer.Core.CameraTurnToAction();
		}

		if (newAction != null)
		{
			seqDesc.sequence.Actions.Add(newAction);
			EditorUtility.SetDirty(target);
		}
	}

	void ShowActionContextMenu(Rect position)
	{
		var menu = new GenericMenu();
		menu.AddItem(new GUIContent("Remove Action"), false, OnRemoveAction, currentActionIndex);
		menu.DropDown(position);
	}
	void OnRemoveAction(object index)
	{
		int actionIndex = (int)index;
		seqDesc.sequence.Actions.RemoveAt(actionIndex);
	}
	int currentActionIndex = 0;

	UnityEngine.Object obj = null;
}
