using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class InventoryIcon : MonoBehaviour, IPointerDownHandler {

	[SerializeField] private RectTransform _rectTransform;
	[SerializeField] private Image _iconImage;
	[SerializeField] private RectTransform[] _element;
	[SerializeField] private InventoryEnum _size;
	[SerializeField] private Text _iconCountText;
	[SerializeField] private GameObject _iconCountObject;

	public string item { get; set; }
	public int counter { get; set; }
	public bool isInside { get; set; }

	public void OnPointerDown(PointerEventData eventData)
	{
		if(Input.GetMouseButtonDown(0)) Inventory.Internal.BeginDrag(this);
	}
	public InventoryEnum size
	{
		get{ return _size; }
	}

	public GameObject iconCountObject
	{
		get{ return _iconCountObject; }
	}

	public RectTransform rectTransform
	{
		get{ return _rectTransform; }
	}

	public Image iconImage
	{
		get{ return _iconImage; }
	}

	public RectTransform[] element
	{
		get{ return _element; }
	}

	public Text iconCountText
	{
		get{ return _iconCountText; }
	}
}
