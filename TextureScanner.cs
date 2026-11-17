using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScanner : MonoBehaviour
{
    private Texture2D myTexture;
    Sprite mySprite;
    public Color COMPARE_COLOR;
    Rect sprtRect;
    
    void Start()
    {
        mySprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        myTexture = mySprite.texture;

        sprtRect = mySprite.rect;
        
        print("HEIGHT: " + myTexture.height + " WIDTH: " + myTexture.width);
        Color MyPixel;
        for(int x = (int)sprtRect.x; x < (int)sprtRect.x + (int)sprtRect.width; x++)
        {
            for(int y = (int)sprtRect.y; y < (int)sprtRect.y + (int)sprtRect.height; y++)
            {
                
                MyPixel = myTexture.GetPixel(x, y);
                if (MyPixel.Equals(COMPARE_COLOR))
                {
                    print(MyPixel + "," + COMPARE_COLOR);
                }
            }
        }
        
    }

    
}
