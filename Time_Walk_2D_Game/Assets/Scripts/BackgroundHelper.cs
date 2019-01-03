using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundHelper : MonoBehaviour {
    
    [SerializeField] private float speed = 0;
    [SerializeField] private float maxPosX = 0;
    
    private float posLayer = 0;
    [SerializeField] private float posNow = 0;
    private float lastPlayerX = 0;
    private float nowPlayerX = 0;
    private float horBound;

    private RawImage image;
    private Transform player;
    private RectTransform Paralax;
    private RectTransform rect;
    private Camera cam;
//7.574187
    void Start() {
        image = GetComponent<RawImage>(); //get link to back image
        player = GameObject.FindGameObjectWithTag("Player").transform; //get link to player
        //GetComponent<RectTransform>().position
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>() as Camera;
        Paralax = GameObject.FindGameObjectWithTag("Paralax").GetComponent<RectTransform>();
        horBound = cam.orthographicSize / Screen.height * Screen.width;
    }

    void FixedUpdate() {
        //maxPosX = horBound;
        nowPlayerX = player.position.x;
        //maxPosX = cam.WorldToScreenPoint(cam.transform.position).x;
        if (nowPlayerX != lastPlayerX) {
            layerMove(nowPlayerX < lastPlayerX);
            
            lastPlayerX = nowPlayerX;
        }
    }

    void layerMove(bool left) {
        if (left && posLayer - speed >= 0) {
            posLayer -= speed;
            maxPosX--;
        } else if (!left && 
            (image.rectTransform.rect.width * (posLayer + speed)) 
                < image.rectTransform.rect.width - Paralax.rect.width) {
            posLayer += speed;
            maxPosX++;
        } else { return; }
        //Update layer
        image.uvRect = new Rect(posLayer, 0, 1, 1);
        posNow = Paralax.rect.width;
    }
}
