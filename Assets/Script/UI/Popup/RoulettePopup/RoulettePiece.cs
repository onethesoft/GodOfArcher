using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoulettePiece : MonoBehaviour
{
  
    [SerializeField]
    private Image imageIcon;

    [SerializeField]
    private Text Description;

   
    public void Setup(RoulettePieceData pieceData)
    {
        imageIcon.sprite = pieceData.Icon;
        Description.text = pieceData.description;
        
    }

    

    private void Update()
    {
        transform.rotation = Quaternion.identity;
    }

   

}
