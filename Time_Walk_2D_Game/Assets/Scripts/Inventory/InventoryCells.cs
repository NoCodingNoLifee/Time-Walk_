using UnityEngine;
using System.Collections;

public class InventoryCells : MonoBehaviour {

	[SerializeField] private RectTransform _rectTransform;
	public string item { get; set; }
	public bool isLocked { get; set; }

	public RectTransform rectTransform
	{
		get{ return _rectTransform; }
	}

	public void SetRectTransform(RectTransform tr)
	{
		_rectTransform = tr;
	}
}
