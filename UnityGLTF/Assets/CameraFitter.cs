using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFitter : MonoBehaviour
{
	public RectTransform FitTarget = null;
	public Camera MainCamera = null;

    // Start is called before the first frame update
    void Start()
    {
		Vector3[] worldCorners = new Vector3[4];
		FitTarget.GetWorldCorners(worldCorners);
		Vector2 rightTop = RectTransformUtility.WorldToScreenPoint(MainCamera, worldCorners[2]);
		Vector2 leftBottom = RectTransformUtility.WorldToScreenPoint(MainCamera, worldCorners[0]);
		Rect rect = new Rect(leftBottom.x / Screen.width, leftBottom.y / Screen.height,
			(rightTop.x - leftBottom.x) / Screen.width, (rightTop.y - leftBottom.y) / Screen.height);

		var thisCamera = GetComponent<Camera>();
		thisCamera.rect = rect;
	}


}
