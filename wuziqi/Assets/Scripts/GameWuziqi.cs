using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWuziqi : MonoBehaviour
{
    private bool begin;  // 控制游戏开始
    private bool win;    // 判断胜利与否
    private bool starIsBlack;
    private bool orangeIsBlack;
    private bool ChessColor; // true 则下黑子; false 则下白子
    private float gridWidth=0.5f;  // 棋盘网格大小
    // --- Prefabs ----
    public GameObject black_pieces; // 预制体-黑色棋子
    public GameObject white_pieces; // 预制体-白色棋子
    // --- UI image ----
    public GameObject image_star1;
    public GameObject image_star2;
    public GameObject image_orange1;
    public GameObject image_orange2;
    public GameObject image_blackchess;
    public GameObject image_whitechess;
    public GameObject image_win_star;
    public GameObject image_win_orange;
    //-----------------
    private Vector3 posi_left; // 提示棋子图片在左侧的位置
    private Vector3 posi_right; // 提示棋子图片在右侧的位置
    // ----- record the prefabs(chess pieces) ---
    //List<GameObject> pieces = new List<GameObject>();
    Stack<GameObject> pieces = new Stack<GameObject>();
    Stack<int[]> piecePosi = new Stack<int[]>();  // 记录场上所有子的位置
    // ----- record the chessboard situation ----
    int[,] array = new int[19,19]; // 2d array

    // ===== Start is called before the first frame update ======
    void Start()
    {
        begin = false;
        win = false;
        starIsBlack = false;
        orangeIsBlack = false;
        ChessColor = false;
        posi_left = image_blackchess.transform.position;
        posi_right = image_whitechess.transform.position;
        // --- UI initialization ---
        UIControl();
    }

    // ======= Update is called once per frame ===================
    void Update()
    {
        // --- 鼠标点击则放置黑白棋 --
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 mousePositionOnScreen = Input.mousePosition; 
		mousePositionOnScreen.z = screenPosition.z;
		Vector3 mousePositionInWorld =  Camera.main.ScreenToWorldPoint(mousePositionOnScreen);
        if (Input.GetMouseButtonDown(0) && begin == true){
            int x = Mathf.RoundToInt(mousePositionInWorld.x/gridWidth);
            int y = Mathf.RoundToInt(mousePositionInWorld.y/gridWidth);
            if (x <= 9 && x >= -9 && y <= 9 && y >= -9 && array[x+9,y+9]==0){ // 确认落子在棋盘内
                Vector3 chessPosition = mousePositionInWorld;
                chessPosition.x = x*gridWidth;
                chessPosition.y = y*gridWidth;
                chessPosition.z = mousePositionInWorld.z;
                if (ChessColor){ // 轮到黑子下
                    GameObject piece = Instantiate(black_pieces,chessPosition,Quaternion.identity); // 生成棋子
                    //pieces.Add(piece); // 记录预制体
                    pieces.Push(piece);
                    piecePosi.Push(new int[]{x+9,y+9});
                    array[x+9,y+9] = 1; // 记录落子位置
                    // ----- 计算是否有五子连线 ------
                    CalculateChess(array);
                    if (!win){
                        ChessColor = false;
                        UIControl();
                    }
                }
                else{  // 轮到白子下
                    GameObject piece = Instantiate(white_pieces,chessPosition,Quaternion.identity);
                    //pieces.Add(piece);
                    pieces.Push(piece);
                    piecePosi.Push(new int[]{x+9,y+9});
                    array[x+9,y+9] = -1;
                    // ----- 计算是否有五子连线 ------
                    CalculateChess(array);
                    if (!win){
                        ChessColor = true;
                        UIControl();
                    }
                }
            }
        }
    }

    // ========== 控制游戏开始 =========
    public void StartGame(){
        begin = true;
        win = false;
        UIControl();
    }

    // =========== 计算五子棋连子情况 ========
    public void CalculateChess(int[,] array){
        int n = 0;
        // --- 垂直方向 ----
        for (int i=0; i<19; i++){
            n = 1;
            for (int j=0; j<18; j++){
                if (array[i,j]!=0 && array[i,j+1]!=0 && array[i,j] == array[i,j+1]){
                    n += 1;
                }
                if (array[i,j]!=0 && array[i,j+1]!=0 && array[i,j+1] != array[i,j] && n < 5){
                    n = 1;
                }
            }
            if ( n >= 5 ){ win = true; }
        }
        // ----- 水平方向 ----
        for (int j=0; j<19; j++){
            n = 1;
            for (int i=0; i<18; i++){
                if (array[i,j]!=0 && array[i+1,j]!=0 && array[i+1,j] == array[i,j]){
                    n += 1;
                }
                if (array[i,j]!=0 && array[i+1,j]!=0 && array[i+1,j] != array[i,j] && n < 5){
                    n = 1;
                }
            }
            if (n >= 5){ win = true; }
        }
        // ----- 斜边 45 度 ------
        for (int i=0;i<15;i++){
            n = 1;
            for (int k=i;k<18;k++){
                if (array[k,k-i]!=0 && array[k+1,k-i+1]!=0 && array[k+1,k-i+1] == array[k,k-i]){
                    n += 1;
                }
                if (array[k,k-i]!=0 && array[k+1,k-i+1]!=0 && array[k+1,k-i+1] != array[k,k-i] && n < 5){
                    n = 1;
                }
            }
            if (n >= 5){ win = true; }
        }
        for (int j=1;j<15;j++){
            n = 1;
            for (int k=0;k<18-j;k++){
                if (array[k,k+j]!=0 && array[k+1,k+j+1]!=0 && array[k+1,k+j+1] == array[k,k+j]){
                    n += 1;
                }
                if (array[k,k+j]!=0 && array[k+1,k+j+1]!=0 && array[k+1,k+j+1] != array[k,k+j] && n < 5){
                    n = 1;
                }
            }
            if (n >= 5){ win = true; }
        }
        // ------ 斜边 135 度 -------
        for (int i=4;i<19;i++){
            n = 1;
            for (int k=i;k>0;k--){
                if (array[k,i-k]!=0 && array[k-1,i-k+1]!=0 && array[k-1,i-k+1] == array[k,i-k]){
                    n += 1;
                }
                if (array[k,i-k]!=0 && array[k-1,i-k+1]!=0 && array[k-1,i-k+1] != array[k,i-k] && n < 5){
                    n = 1;
                }
            }
            if (n >= 5){ win = true; }
        }
        for (int j=1;j<15;j++){
            n = 1;
            for (int k=18;k>j;k--){
                if (array[k,18-k+j]!=0 && array[k-1,19-k+j]!=0 && array[k-1,19-k+j] == array[k,18-k+j]){
                    n += 1;
                }
                if (array[k,18-k+j]!=0 && array[k-1,19-k+j]!=0 && array[k-1,19-k+j] != array[k,18-k+j] && n < 5){
                    n = 1;
                }
            }
            if (n >= 5){ win = true; }
        }
        //  -------- 胜利 --------
        if (win){
            UIControl();
            begin = false; // 游戏结束
        }
    }

    // ============= 重新开始 ============
    public void Reset(){
        // ---  destroy prefabs ---
        //for (int n = 0; n < pieces.Count; n++){
            //Destroy(pieces[n]);
        //}
        int n = pieces.Count;
        for (int i = 0; i < n; i++){
            Destroy(pieces.Pop());
            piecePosi.Pop();
        }
        // --- array 归零 ---
        for (int i=0; i<19; i++){
            for (int j=0; j<19; j++){
                array[i,j] = 0;
            }
        }
        begin = false;
        win = false;
        starIsBlack = false;
        orangeIsBlack = false;
        ChessColor = false;
        UIControl();
    }
    // =============== 悔棋 ==========
    public void Remake(){
        if(begin){
            Destroy(pieces.Pop());
            int[] posi = piecePosi.Pop();
            array[posi[0],posi[1]] = 0;
            ChessColor = !ChessColor;
        }
    }
    
    // ============ 游戏初始化 确定黑子和先手 ========
    public void pickStarIsBlack(){
        starIsBlack = true;
        // ---- set position of UI chess -----
        image_blackchess.transform.position = posi_left;
        image_whitechess.transform.position = posi_right;
    }
    public void pickOrangeIsBlack(){
        orangeIsBlack = true;
        // ---- set position of UI chess -----
        image_blackchess.transform.position = posi_right;
        image_whitechess.transform.position = posi_left;
    }
    public void pickStarFirst(){
        if (starIsBlack){
            ChessColor = true;
        }
        else{
            ChessColor = false;
        }
    }
    public void pickOrangeFirst(){
        if (orangeIsBlack){
            ChessColor = true;
        }
        else{
            ChessColor = false;
        }
    }

    // ================= UI image control ====================
    public void UIControl(){
        if (!win && !begin){ // 游戏尚未开始时
            image_star1.SetActive(false);
            image_star2.SetActive(false);
            image_orange1.SetActive(false);
            image_orange2.SetActive(false);
            image_blackchess.SetActive(false);
            image_whitechess.SetActive(false);
            image_win_star.SetActive(false);
            image_win_orange.SetActive(false);
        }
        // ---- 落子提示切换 ------
        if (begin){
            if (ChessColor && starIsBlack){
                image_blackchess.SetActive(true);
                image_whitechess.SetActive(false); 
                image_star1.SetActive(false);
                image_star2.SetActive(true);
                image_orange1.SetActive(true);
                image_orange2.SetActive(false);
            }
            if (ChessColor && orangeIsBlack){
                image_blackchess.SetActive(true);
                image_whitechess.SetActive(false); 
                image_star1.SetActive(true);
                image_star2.SetActive(false);
                image_orange1.SetActive(false);
                image_orange2.SetActive(true);
            }
            if (!ChessColor && starIsBlack){
                image_blackchess.SetActive(false);
                image_whitechess.SetActive(true);
                image_star1.SetActive(true);
                image_star2.SetActive(false);
                image_orange1.SetActive(false);
                image_orange2.SetActive(true);
            }
            if (!ChessColor && orangeIsBlack){
                image_blackchess.SetActive(false);
                image_whitechess.SetActive(true);
                image_star1.SetActive(false);
                image_star2.SetActive(true);
                image_orange1.SetActive(true);
                image_orange2.SetActive(false);
            }
        }
        // --- 胜利提示 -----
        if (win && ChessColor == true){
            print("黑子胜!");
            if (starIsBlack){
                image_win_star.SetActive(true);
            }
            if (orangeIsBlack){
                image_win_orange.SetActive(true);
            }
            image_star1.SetActive(true);
            image_star2.SetActive(false);
            image_orange1.SetActive(true);
            image_orange2.SetActive(false);
            image_blackchess.SetActive(true);
            image_whitechess.SetActive(false);            
        }
        if (win && ChessColor == false){
            print("白子胜！");
            if (starIsBlack){
                image_win_orange.SetActive(true);
            }
            if (orangeIsBlack){
                image_win_star.SetActive(true);
            }
            image_star1.SetActive(true);
            image_star2.SetActive(false);
            image_orange1.SetActive(true);
            image_orange2.SetActive(false);
            image_blackchess.SetActive(false);
            image_whitechess.SetActive(true);            
        }
    }
}
