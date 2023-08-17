using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Roulette : MonoBehaviour
{
    [SerializeField]
    private Transform piecePrefab;

    [SerializeField]
    private RoulettePieceData[] roulettePieceDatas;

    [SerializeField]
    private Transform pieceParent;

    [SerializeField]
    private Transform Offset;

    [SerializeField]
    private int spinDuration;

    [SerializeField]
    private Transform Indicator;

    [SerializeField]
    private Transform IndicatorInit;



    [SerializeField]
    Transform spinningRoulette;




    private float pieceAngle;
    private int accumulateWeight;
    private int selectedIndex = 0;
    private float halfPieceAngleWithPaddings;

    public bool IsSpinning { get; private set; }
    

    



    // Start is called before the first frame update
    void Start()
    {
        pieceAngle = 360 / roulettePieceDatas.Length;

        halfPieceAngleWithPaddings = (pieceAngle * 0.5f) - ((pieceAngle * 0.5f) * 0.25f);
        Indicator.gameObject.SetActive(false);
        SpawnPieces();
        CalculateWeightsAndIndices();

    }

    private void SpawnPieces()
    {
        for(int i=0; i<roulettePieceDatas.Length;i++)
        {
            float prefabAngle = pieceAngle * i;

            Transform piece = Instantiate(piecePrefab, Offset.transform.position, Quaternion.identity, pieceParent);

            Util.GetOrAddComponent<RoulettePiece>(piece.gameObject).Setup(roulettePieceDatas[i]);
            
          
            piece.RotateAround(pieceParent.position, Vector3.back, (pieceAngle * i));
           
        }
    }
    private void CalculateWeightsAndIndices()
    {
        for(int i=0; i<roulettePieceDatas.Length;++i)
        {
            roulettePieceDatas[i].index = i;

            if (roulettePieceDatas[i].change <= 0)
                roulettePieceDatas[i].change = 1;

            accumulateWeight += roulettePieceDatas[i].change;
            roulettePieceDatas[i].weight = accumulateWeight;
        }
    }

    private int GetRandomIndex()
    {
        int weight = Random.Range(0, accumulateWeight);
        for (int i = 0; i < roulettePieceDatas.Length; ++i)
        {
            if (roulettePieceDatas[i].weight > weight)
                return i;
        }

        return 0;
    }

    public RoulettePieceData Spin(UnityAction<RoulettePieceData> action = null)
    {

        if (IsSpinning)
            return null;

        IsSpinning = true;
        selectedIndex = GetRandomIndex();

        float angle = pieceAngle * selectedIndex;
      
        float leftOffset = (angle   - halfPieceAngleWithPaddings) % 360;
        float rightOffset = (angle  + halfPieceAngleWithPaddings) % 360;


        float randomAngle = Random.Range(leftOffset, rightOffset);
        float fixedRandmAngle = randomAngle ;
       

        int rotateSpeed = 2;
        float targetAngle = (fixedRandmAngle + 360 * spinDuration * rotateSpeed);
        Debug.Log($"selected Index = {selectedIndex}");
        Debug.Log($"selected angle = {targetAngle}");
        spinningRoulette.rotation = Quaternion.identity;
        spinningRoulette.DORotate(new Vector3(0, 0, targetAngle), spinDuration, RotateMode.FastBeyond360).
            SetEase(Ease.OutQuad).
            OnComplete(() => 
            {
                Indicator.transform.position = IndicatorInit.position;
                Indicator.transform.rotation = IndicatorInit.rotation;

              
           
                Indicator.RotateAround(pieceParent.position,Vector3.back , angle - randomAngle);
                Indicator.gameObject.SetActive(true);
                IsSpinning = false;
                action?.Invoke(roulettePieceDatas[selectedIndex]); 
            }) ;


        return roulettePieceDatas[selectedIndex];

        //StartCoroutine(OnSpin(targetAngle, action));

    }

   
}
