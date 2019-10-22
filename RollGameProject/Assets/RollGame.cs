using UnityEngine;
using UnityEngine.UI;

public class RollGame : MonoBehaviour
{
    private int col = 5;
    private int row = 5;
    public Transform[] Col0;
    public Transform[] Col1;
    public Transform[] Col2;
    public Transform[] Col3;
    public Transform[] Col4;
    public Transform[] ColPos;
    public int Speed;
    private int[] colState = new int[5]{2,2,2,2,2}; //1旋转 2停止 3即将停止
    private int[] colDelay = new int[5];
    private int startPos = 33;
    private int endPos = -30;
    private Transform[][] Items;
    public Button Btn_Start;
    public Button Btn_Stop;
    private Object[] list_Sprites;

    // Start is called before the first frame update
    void Start()
    {
        Items = new Transform[][]{Col0,Col1,Col2,Col3,Col4};
        Btn_Start.onClick.AddListener(()=>{
            StartSpin();
            Btn_Start.gameObject.SetActive(false);
            Btn_Stop.gameObject.SetActive(true);
            Btn_Stop.interactable = true;
        });

        Btn_Stop.onClick.AddListener(()=>{
            StopSpin();
        });

        list_Sprites = Resources.LoadAll<Sprite>("Sprites");
        for (int i =0; i<col;i++){
            for (int j=0; j<row;j++){
                Items[i][j].GetComponent<Image>().sprite = (Sprite)list_Sprites[(int)Random.Range(0,list_Sprites.Length-1)];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i< col; i++){
            if (colState[i] == 1){
                ColPos[i].localPosition = new Vector3(ColPos[i].localPosition.x,ColPos[i].localPosition.y - Speed,ColPos[i].localPosition.z);
                if (ColPos[i].localPosition.y <= endPos){
                    ColPos[i].localPosition = new Vector3(ColPos[i].localPosition.x,startPos,ColPos[i].localPosition.z);
                } 
                for (int j = row -1;j>=1;j--){
                    Items[i][j].GetComponent<Image>().sprite = Items[i][j-1].GetComponent<Image>().sprite;
                }
                Items[i][0].GetComponent<Image>().sprite = (Sprite)list_Sprites[(int)Random.Range(0,list_Sprites.Length-1)];
                if (colDelay[i] >= 0)
                {
                    colDelay[i]--;
                    if (colDelay[i] <= 0)
                    {
                        colState[i] = 3;
                    }
                }
            }
            else if (colState[i] == 3){
                ColPos[i].localPosition = new Vector3(ColPos[i].localPosition.x,ColPos[i].localPosition.y - Speed,ColPos[i].localPosition.z);
                ColPos[i].localPosition = new Vector3(ColPos[i].localPosition.x,startPos,ColPos[i].localPosition.z);
                for (int j = row -1;j>=1;j--){
                    Items[i][j].GetComponent<Image>().sprite = Items[i][j-1].GetComponent<Image>().sprite;
                }
                Items[i][0].GetComponent<Image>().sprite = (Sprite)list_Sprites[(int)Random.Range(0,list_Sprites.Length-1)];
                colState[i] = 2;
                if (i == 4){
                    Btn_Start.gameObject.SetActive(true);
                    Btn_Stop.gameObject.SetActive(false);
                }
            }
        }
    }

    void StartSpin()
    {
        for (int i =0; i<col ;i++)
        {
            colState[i] = 1;
            colDelay[i] = 60 + i * 20;
        }        
    }

    void StopSpin()
    {
        for (int i = 0; i < col; i++)
        {
            colDelay[i] = i * 20;
        }
        Btn_Stop.interactable = false;
    }
}
