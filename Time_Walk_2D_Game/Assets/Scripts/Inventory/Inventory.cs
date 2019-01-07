using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Inventory : MonoBehaviour {

	public delegate void MethodEndDrag(string item, int count);
	public event MethodEndDrag OnEndDrag;

	[SerializeField] private KeyCode key;
	[SerializeField] private GameObject inventory;
	[SerializeField] private RectTransform overlap; // здесь иконка, которая в данный момент перетаскивается
	[SerializeField] private RectTransform content; // здесь все иконки
	[SerializeField] private RectTransform objects;
	[SerializeField] private RectTransform grid;
	[SerializeField] private Transform items; // батька всех айтемов
	[SerializeField] private int width = 5, height = 5;
	[SerializeField] private InventoryCells[] cells;
	[SerializeField] private InventoryIcon[] icons;
	//[SerializeField] private Camera main_camera;

	private bool isCrypt = false;
	private string fileName = "Player.inventory";
	private int cellsSize = 50;
	private string iconPath = "Icons"; // Resources+iconPath --> путь к нашим иконкам
	private List<InventoryCells> targetCell;
	private List<InventoryCells> lastCell;
	private InventoryComponent current;
	private InventoryIcon icon;
	private List<GameObject> items_list;
	private List<InventoryIcon> pIcon;
	private InventoryCells[,] field;
	private Vector2 lastPosition;
	private GameObject currentObject;
	private static Inventory _inv;
	private static bool _Active;

	public static bool isActive
	{
		get{ return _Active; }
	}

	public static Inventory Internal
	{
		get{ return _inv; }
	}

	public bool CanUseItem(string value)
	{
		foreach(var i in pIcon)
		{
			if(i.item == value) return true;
		}

		return false;
	}

	public void UseItem(string value)
	{
		int j = 0;
		foreach(var i in pIcon)
		{
			if(i.item == value && i.counter > 1)
			{
				i.counter--;
				i.iconCountText.text = i.counter.ToString();
				return;
			}
			else if(i.item == value && i.counter == 1)
			{
				pIcon.RemoveAt(j);
				Destroy(i.gameObject);
				return;
			}
			j++;
		}
	}

	void LoadSettings()
	{
		if(!File.Exists(Path())) return;

		InventoryIcon tmp = null;
		string item = string.Empty;
		int counter = 0;
		InventoryEnum size = InventoryEnum.size1x1;
		Vector2 pos = Vector2.zero;

		string[] f_isLocked = new string[]{};
		string[] f_item = new string[]{};

		StreamReader reader = new StreamReader(Path());

		while(!reader.EndOfStream)
		{
			string value = Crypt(reader.ReadLine());

			string[] result = value.Split(new char[]{'='});

			if(result.Length == 1 && item == string.Empty)
			{
				item = value.Trim(new char[]{'[', ']'});
			}

			switch(result[0]) // фильтруем ключи
			{
			case "IconSize":
				size = (InventoryEnum)System.Enum.Parse(typeof(InventoryEnum), result[1]); // string переводим в enum
				break;
			case "IconCounter":
				counter = int.Parse(result[1]);
				break;
			case "IconPositionX":
				pos.x = float.Parse(result[1]);
				break;
			case "IconPositionY":
				pos.y = float.Parse(result[1]);
				break;
			case "Field_isLocked":
				f_isLocked = result[1].Split(new char[]{','});
				break;
			case "Field_item":
				f_item = result[1].Split(new char[]{','});
				break;
			}

			if(value == string.Empty) 
			{
				tmp = SetIcon(size, item, content);
				tmp.counter = counter;
				tmp.iconCountText.text = counter.ToString();
				tmp.isInside = true;
				tmp.iconCountObject.SetActive(true);
				tmp.rectTransform.localPosition = pos;
				pIcon.Add(tmp);
				item = string.Empty;
			}
		}

		for(int i = 0; i < cells.Length; i++)
		{
			if(f_item[i] == "null") cells[i].item = string.Empty; else cells[i].item = f_item[i];
			if(f_isLocked[i] == "0") cells[i].isLocked = false; else cells[i].isLocked = true;
		}

		reader.Close();
	}

	string Crypt(string text)
	{
		if(!isCrypt) return text;

		string result = string.Empty;
		foreach (char j in text)
		{
			result += (char)((int)j ^ 47);
		}
		return result;
	}

	void SaveSettings()
	{
		if(pIcon.Count == 0) return;

		StreamWriter writer = new StreamWriter(Path());

		foreach(InventoryIcon i in pIcon)
		{
			writer.WriteLine(Crypt("[" + i.item + "]"));
			writer.WriteLine(Crypt("IconSize=" + i.size));
			writer.WriteLine(Crypt("IconCounter=" + i.counter));
			writer.WriteLine(Crypt("IconPositionX=" + i.rectTransform.localPosition.x));
			writer.WriteLine(Crypt("IconPositionY=" + i.rectTransform.localPosition.y));
			writer.WriteLine(string.Empty); // пустая строка, обязательный ключ, запускает создание иконки
		}

		string t_item = string.Empty;
		string t_isLocked = string.Empty;
		foreach(InventoryCells i in cells)
		{
			if(i.item == string.Empty) t_item += "null,"; else t_item += (i.item + ",");
			if(i.isLocked) t_isLocked += "1,"; else t_isLocked += "0,";
		}

		writer.WriteLine(Crypt("Field_isLocked=" + t_isLocked));
		writer.WriteLine(Crypt("Field_item=" + t_item));

		writer.Close();
		Debug.Log(this + " сохранение инвентаря игрока: " + Path());
	}

	string Path() // путь сохранения
	{
		return Application.dataPath + "/" + fileName;
	}

	public void BeginDrag(InventoryIcon curIcon)
	{
		lastPosition = curIcon.rectTransform.position;
		icon = curIcon;
		SetUnlock(curIcon.item);
		icon.rectTransform.SetParent(overlap);
	}

	void SetUnlock(string item)
	{
		lastCell = new List<InventoryCells>();
		foreach(InventoryCells cell in cells)
		{
			if(cell.item == item)
			{
				lastCell.Add(cell);
				cell.item = string.Empty;
				cell.isLocked = false;
			}
		}
	}

	void RemoveItem(string item)
	{
		int j = 0;
		foreach(InventoryIcon i in pIcon)
		{
			if(i.item == item)
			{
				pIcon.RemoveAt(j);
				var obj = items_list[j];
				obj.gameObject.SetActive(true);
				obj.gameObject.transform.SetParent(items);
				obj.gameObject.transform.position=Camera.main.ScreenToWorldPoint(Input.mousePosition);
				obj.gameObject.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0f);

				items_list.RemoveAt(j);
				return;
			}
			j++;
		}
	}

	void Awake()
	{
		inventory.SetActive(false);
		_Active = false;
		_inv = this;
		pIcon = new List<InventoryIcon>();
		items_list = new List<GameObject>();
		field = new InventoryCells[width, height];
		int j = 0;
		for(int y = 0; y < height; y++)
		{
			for(int x = 0; x < width; x++)
			{
				cells[j].item = string.Empty;
				field[x, y] = cells[j];
				j++;
			}
		}

		LoadSettings();
	}

	InventoryComponent GetCurrent() // запрос компонента с объекта
	{
		Vector3[] worldCorners = new Vector3[4];
		content.GetWorldCorners(worldCorners);
		if(PointInside(Input.mousePosition, worldCorners)) {
			return null;
		}

		Transform tr = null;

		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if(hit.transform != null) tr = hit.transform;

		if(tr == null) return null;

		InventoryComponent t = tr.GetComponent<InventoryComponent>();
		if(t != null) return t;

		return null;
	}

	InventoryIcon SetIcon(InventoryEnum size, string item, RectTransform targetRect) // создание и настройка новой иконки
	{
		InventoryIcon target = null;

		foreach(InventoryIcon i in icons)
		{
			if(i.size == size) target = i;
		}

		if(target)
		{
			InventoryIcon clone = Instantiate(target) as InventoryIcon;
			clone.iconImage.sprite = Resources.Load<Sprite>(iconPath + "/" + item);
			clone.iconCountObject.SetActive(false);
			clone.gameObject.name = item;
			clone.item = item;
			clone.counter = 1;
			clone.iconCountText.text = "1";
			clone.rectTransform.SetParent(targetRect);
			clone.rectTransform.localScale = Vector3.one;
			clone.rectTransform.position = Input.mousePosition;
			clone.gameObject.SetActive(true);
			return clone;
		}

		return null;
	}

	void Control() 
	{
		if(icon) icon.rectTransform.position = Input.mousePosition;

		if(Input.GetMouseButtonDown(0))
		{
			/* foreach(var i in items_list)
			{
				Debug.Log("ITEMS: " + i.ToString());
			}*/
			current = GetCurrent();
			if(!current) return;
			Debug.Log(current.ToString());
			icon = SetIcon(current.size, current.item, overlap);
			current.gameObject.SetActive(false);
		}
		else if((Input.GetMouseButtonUp(0) && icon))
		{
			if(!IsInside(icon.element))
			{
				Debug.Log("...");
				if(OnEndDrag != null && icon.isInside) { OnEndDrag(icon.item, icon.counter); }// выполняем событие
				RemoveItem(icon.item);
				ResetCurrent();
				return;
			}

			if(!icon.isInside && IsSimilar(icon.item))
			{
				Destroy(icon.gameObject);
				Destroy(current.gameObject);	
				return;
			}

			if(!icon.isInside && IsOverlap(icon.element))
			{
				ResetCurrent();
				return;
			}
			else if(icon.isInside && IsOverlap(icon.element))
			{
				ResetDrag();
				return;
			}

			AddCurrentIcon();
		}
		else if(Input.GetMouseButtonDown(1))
		{
			current = GetCurrent();
			if(current) AddQuickly();
		}
	}

	void Update()
	{
		if(!_Active && Input.GetKeyDown(key))
		{
			_Active = true;
			inventory.SetActive(true);
		}
		else if(_Active && Input.GetKeyDown(key))
		{
			_Active = false;
			inventory.SetActive(false);
			SaveSettings();
		}

		if(_Active) Control();
	}

	bool IsSimilar(string item) // поиск похожей иконки
	{
		for(int i = 0; i < pIcon.Count; i++)
		{
			if(pIcon[i].item == item)
			{
				pIcon[i].counter++;
				pIcon[i].iconCountText.text = pIcon[i].counter.ToString();
				return true;
			}
		}

		return false;
	}

	void AddQuickly() // быстрое добавление
	{
		 if(IsSimilar(current.item))
		{
			Destroy(current.gameObject);
			return;
		}

		icon = SetIcon(current.size, current.item, content);
		if(!icon) return;

		int xx = 0, yy = 0; // Размер иконки 1х1		

		targetCell = new List<InventoryCells>();
		for(int y = 0; y < height-yy; y++)
		{
			for(int x = 0; x < width-xx; x++)
			{
				switch(icon.size) // ищем место для новой иконки
				{
				case InventoryEnum.size1x1:
					if(!field[x, y].isLocked)
					{
						targetCell.Add(field[x, y]);
						AddCurrentIcon();
						return;
					}
					break;
				}
			}
		}
	}

	void ResetDrag()
	{
		foreach(InventoryCells cell in lastCell)
		{
			cell.item = icon.item;
			cell.isLocked = true;
		}

		icon.rectTransform.SetParent(content);
		icon.rectTransform.position = lastPosition;
		icon = null;
	}

	void ResetCurrent()
	{
		Destroy(icon.gameObject);
		if(!current) return;
		current.gameObject.SetActive(true);
		current = null;
	}

	void AddCurrentIcon() // добавление иконки
	{
		Vector3 p1 = targetCell[0].rectTransform.position;
		Vector3 p2 = targetCell[targetCell.Count-1].rectTransform.position;
		Vector3 offset = (p1 - p2)/2;
		icon.transform.position = p2 + offset;

		foreach(InventoryCells cell in targetCell)
		{
			cell.item = icon.item;
			cell.isLocked = true;
		}

		if(!icon.isInside) {pIcon.Add(icon); items_list.Add(current.gameObject);}
		icon.rectTransform.SetParent(content);
		icon.isInside = true;
		icon.iconCountObject.SetActive(true);
		icon = null;
		if(current) {current.transform.SetParent(objects); current.gameObject.SetActive(false);}
	}

	bool PointInside(Vector3 position, Vector3[] worldCorners)
	{
		if(position.x > worldCorners[0].x && position.x < worldCorners[2].x 
			&& position.y > worldCorners[0].y && position.y < worldCorners[2].y)
		{
			return true;
		}
		return false;
	}

	bool IsInside(RectTransform[] rectTransform) // проверка, объект внутри поля или нет
	{
		Vector3[] worldCorners = new Vector3[4];
		content.GetWorldCorners(worldCorners);
		foreach(RectTransform tr in icon.element)
		{
			if(!PointInside(tr.position, worldCorners)) return false;
		}
		return true;
	}

	bool IsOverlap(RectTransform[] rectTransform) // проверка на перекрытие
	{
		targetCell = new List<InventoryCells>();
		foreach(RectTransform tr in rectTransform)
		{
			foreach(InventoryCells cell in cells)
			{
				if(Vector2.Distance(cell.rectTransform.position, tr.position) < cellsSize/2)
				{
					if(cell.isLocked) return true;
					targetCell.Add(cell);
				}
			}
		}

		if(targetCell.Count == 0) return true;

		return false;
	}

	#if UNITY_EDITOR
	public void BuildGrid() // для создания сетки
	{
		foreach(InventoryCells cell in cells)
		{
			if(cell) DestroyImmediate(cell.gameObject);
		}
		Vector2 delta = new Vector2(cellsSize * width, cellsSize * height);
		grid.sizeDelta = delta;
		content.sizeDelta = delta;
		overlap.sizeDelta = delta;	
		RectTransform sample = new GameObject().AddComponent<RectTransform>();
		sample.gameObject.AddComponent<InventoryCells>().SetRectTransform(sample);
		sample.sizeDelta = new Vector2(cellsSize, cellsSize);
		float posX = -cellsSize * width/2 - cellsSize/2;
		float posY = cellsSize * height/2 + cellsSize/2;
		float Xreset = posX;
		int i = 0;
		cells = new InventoryCells[width*height];
		for(int y = 0; y < height; y++)
		{
			posY -= cellsSize;
			for(int x = 0; x < width; x++)
			{
				posX += cellsSize;
				RectTransform tr = Instantiate(sample) as RectTransform;
				tr.SetParent(grid);
				tr.localScale = Vector3.one;
				tr.anchoredPosition = new Vector2(posX, posY);
				tr.name = "Cell_" + i;
				cells[i] = tr.GetComponent<InventoryCells>();
				i++;
			}
			posX = Xreset;
		}

		DestroyImmediate(sample.gameObject);
	}
	#endif
}
