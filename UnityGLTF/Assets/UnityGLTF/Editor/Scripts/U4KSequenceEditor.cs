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
        "OnDrag",
        "OnDrop",
        "OnMouseEnter",
        "OnMouseExit",
        "OnSequenceEvent",
        "OnContentUpdate",
        "OnFrame",
        "OnBreakPoint",
        "OnPrint",
        };
	int triggerTypeIndex = 0;


	string[] actionTypes = {
		"RunPython",
		"CallPluginAction",
		"SendEvent",
		"Appear",
		"Disappear",
		"Flash",
		"Rotate",
		"Move",
		"Wipe",
		"TextWriterEffect",
		"ChangeColor",
		"ChangeBackgroundColor",
		"ChangeContent",
		"GotoAndPlay",
		"GotoAndStop",
		};
	int actionTypeIndex = 0;

	void OnEnable()
	{
		NameProp = serializedObject.FindProperty("SequenceName");
		seqDesc = target as U4KSequenceDesc;
		seqDesc.LoadFromJson();
		if (seqDesc.sequence == null) seqDesc.sequence = new U4KSequence();
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
        EditorGUILayout.PropertyField(NameProp);

        var originIndex = triggerTypeIndex;
		triggerTypeIndex = EditorGUILayout.Popup(triggerTypeIndex, triggerTypes);
		if (originIndex != triggerTypeIndex)
		{
			switch (triggerTypeIndex)
			{
				case 0:
					seqDesc.sequence.Trigger = new ClickEventDesc();
					break;

				case 1:
					seqDesc.sequence.Trigger = new DragEventDesc();
					break;

				case 2:
					seqDesc.sequence.Trigger = new DropEventDesc();
					break;

				default:
					break;
			}
		}

		if (seqDesc.sequence.Trigger != null)
		{
			var triggerType = seqDesc.sequence.Trigger.GetType();
			PropertyInfo[] props = null;
			if (seqDesc.sequence.Trigger is ClickEventDesc)
				props = typeof(ClickEventDesc).GetProperties();
			else if (seqDesc.sequence.Trigger is DragEventDesc)
				props = typeof(DragEventDesc).GetProperties();
			else if (seqDesc.sequence.Trigger is DropEventDesc)
				props = typeof(DropEventDesc).GetProperties();

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
							p.SetValue(seqDesc.sequence.Trigger, allEnums.GetValue(nextIndex));
					}
				}
			}
			seqDesc.SaveToJson();
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
						EditorGUILayout.TextField(originV);
						EditorGUILayout.EndHorizontal();
					}
				}
				//obj = EditorGUILayout.ObjectField(obj, typeof(Camera));
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
	}

	void OnAddAction(object userData)
	{
		string newActionName = userData as string;
		AnimAction newAction = null;
		if (newActionName == "CallPluginAction")
		{
			newAction = new CallPluginAction();
		}
		else if (newActionName == "SendEvent")
		{
			newAction = new SendEventAction();
		}

		if (newAction != null) seqDesc.sequence.Actions.Add(newAction);
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
