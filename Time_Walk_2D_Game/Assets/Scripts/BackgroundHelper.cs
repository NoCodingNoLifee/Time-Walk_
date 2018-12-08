using UnityEngine;

using UnityEngine.UI; // обязательно добавляем библиотеку пользовательского интерфейса, без нее кино не будет

using System.Collections;

public class BackgroundHelper : MonoBehaviour {

    public float speed = 0; //эта публичная переменная отобразится в инспекторе, там же мы ее можем и менять. Это очень удобно, чтобы настроить скорость разным слоям картинки

    float pos = 0; //переменная для позиции картинки

    private RawImage image; //создаем объект нашей картинки

      

       void Start () {

        image = GetComponent<RawImage>();//в старте получаем ссылку на картинку

        }    

      

        void Update () {

        //в апдейте прописываем как, с какой скоростью и куда мы будем двигать нашу картинку

          pos += speed;

            if (pos > 1.0F)

                pos -= 1.0F;

            image.uvRect = new Rect(pos, 0, 1, 1);

       }

   

}
