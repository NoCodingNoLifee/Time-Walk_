using UnityEngine;
using System.Collections;

public class InventoryComponent : MonoBehaviour {

	[SerializeField] private string _item = "myItem"; // идентификатор
	[SerializeField] private InventoryEnum _size;

	public string item
	{
		get{ return _item; }
	}

	public InventoryEnum size
	{
		get{ return _size; }
	}
}
