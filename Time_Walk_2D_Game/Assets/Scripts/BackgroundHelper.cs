using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundHelper : MonoBehaviour {
    
    [SerializeField] private float speed = 0;
    [SerializeField] private float maxPosX;
    
    private float posLayer = 0;
    private float lastPlayerX = 0;
    private float nowPlayerX = 0;

    private RawImage image;
    private Transform player;

    void Start() {
        image = GetComponent<RawImage>(); //get link to back image
        player = GameObject.FindGameObjectWithTag("Player").transform; //get link to player
    }

    void FixedUpdate() {
        nowPlayerX = player.position.x;
        if (nowPlayerX != lastPlayerX) {
            layerMove(nowPlayerX < lastPlayerX);
            
            lastPlayerX = nowPlayerX;
        }
    }

    void layerMove(bool left) {
        if (left && posLayer - speed >= 0) {
            posLayer -= speed;
        } else if (posLayer + speed <= maxPosX) {
            posLayer += speed;
        } else { return; }
        //Update layer
        image.uvRect = new Rect(posLayer, 0, 1, 1);
    }
}
