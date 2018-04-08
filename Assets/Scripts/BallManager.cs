using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Interfaces;
public class BallManager : MonoBehaviour {

    public Transform a;
    public Transform b;
    public Transform c;
    public Transform ac;
    public float aSpeed;
    public float bSpeed;
    public float cSpeed;
    public float acSpeed;

    public Transform Prototype;
    public Camera cam;
    public Transform Background;

    private List<Trajectory> Trajectoryes = new List<Trajectory>();
    private List<StateTransform> Balls = new List<StateTransform>();

    public float pk;
    private float complexy;
    private float checkDist;
    private float deleteH;

    public bool isplay;
    private int Lap;
    //-------
    private int score;
    private int record = -1;
    private int PlayedRounds = 0;
    //-------
    private int TouchBorder;

    private IAd Ad;


    public int Score {
        get
        {
            return score;
        }
        private set
        {
            score = value;
            RedrawMenu(true);
        }

    }
    public int Record {
        get
        {
            if(record == -1) record = LoadRecord();
            return record;
        }
        private set
        {
            record = value;
            RedrawMenu(false);
            SaveRecord(record);
        }
        }

    private const string RecordPath = "R.cd";

    public int lCount;

    private StateTransform last;

    public Transform menu;
    public Text ScoreText;
    public Text RecordText;

    

    private void InitBall(StateTransform res)
    {
        res.transform.position = a.position;
        float bsize = checkDist / 4;
        res.transform.localScale = new Vector3(bsize, bsize, bsize);
        last = res;
    }

    private void DeleteBalls()
    {
        foreach (var ball in Balls)
        {
            Destroy(ball.transform.gameObject);
        }
        Balls.Clear();
    }

    private StateTransform CreateBall()
    {
        var res = new StateTransform(Instantiate(Prototype));
        InitBall(res);
        Balls.Add(res);
        SetComplexy();
        return res;
    }

    private void SetComplexy()
    {
        complexy = Mathf.Sqrt(Mathf.Sqrt(Balls.Count));
    }

    private bool ChechHeight(StateTransform ball)
    {
        if (ball.transform.position.y <= deleteH) return true;
        return false;
    }

    private float GetDt(int id)
    {
        if (id == 0) return Time.deltaTime * pk * complexy;
        else return Time.deltaTime * pk;
    }

    private void Moving(StateTransform ball)
    {
        int id = ball.State;
        ball.transform.position = Trajectoryes[id].Get(ball.transform.position, GetDt(id));
    }

    private bool CheckStart(StateTransform ball)
    {
        if (Vector2.Distance(a.position, ball.transform.position) <= checkDist) return true;
        return false;
    }

    private bool CheckEnd(StateTransform ball)
    {
        if (Vector2.Distance(c.position, ball.transform.position) <= checkDist) return true;
        return false;
    }

    private void SetUpCam()
    {
        cam.orthographicSize = Screen.height / 2;
        TouchBorder = cam.pixelWidth / 2;
    }

    private void SetUpBackGround()
    {
        Background.position = new Vector3(0, 0, 10);
        Background.localScale = new Vector3(cam.pixelHeight, cam.pixelHeight, 0);
    }

    private void SetUpParams()
    {
        pk = -cam.pixelWidth;
        deleteH = GetPos(new Vector2(0, 0)).y - 20;
        checkDist = cam.pixelHeight < cam.pixelWidth ? cam.pixelHeight / 3 : cam.pixelWidth / 3;
    }

    private void SetUpObj()
    {
        a.localScale = new Vector3(checkDist, checkDist, checkDist);
        c.localScale = new Vector3(checkDist, checkDist, checkDist);

        a.position = GetPos(new Vector2(cam.pixelWidth, 0));
        b.position = GetPos(new Vector2(cam.pixelWidth / 2, cam.pixelHeight));
        c.position = GetPos(new Vector2(0, 0));
        ac.position = GetPos(new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 5));
    }

    private void SetUpScene()
    {
        SetUpParams();
        SetUpObj();
        
    }

    public Vector2 GetPos(Vector2 campos)
    {
        var res = cam.ScreenToWorldPoint(new Vector3(campos.x, campos.y, 0));
        return new Vector2(res.x, res.y);
    }

    private void SetUpTrajectories()
    {
        Trajectoryes.Add(
            new Trajectory(
                new Functions.Parabola(a.position, b.position, c.position),
                new Functions.Parabola(new Vector2(a.position.x, aSpeed), new Vector2(b.position.x, bSpeed), new Vector2(c.position.x, cSpeed))
         ));
        Trajectoryes.Add(
            new Trajectory(
                new Functions.Parabola(c.position, ac.position, a.position),
                new Functions.Const(-acSpeed)
         ));
    }
    void Start () {
        RedrawMenu(false);
        SetUpCam();
        SetUpScene();
        SetUpBackGround();
        SetUpTrajectories();

        try
        {
            Ad = new Ads("ca-app-pub-5544910402146685~6772042747", "ca-app-pub-5544910402146685/5963014640");
        }
        catch(Exception ex)
        {
            ScoreText.text = ex.Message;
        }
}

    

    public void Restart()
    {
        DeleteBalls();
        Lap = 0;
        Score = 0;
        isplay = true;
        CreateBall();
        
        menu.gameObject.SetActive(false);
    }

    private void ShowAdd()
    {
        if (PlayedRounds % 1 == 0) Ad.Show();
    }

    private void Stop()
    {
        PlayedRounds++;
        isplay = false;
        if (Score > Record) Record = Score;
        menu.gameObject.SetActive(true);
    }

    private void RedrawMenu(bool f)
    {
        if (f) ScoreText.text = "Score: " + Score;
        else RecordText.text = "Record: " + Record;
    }

    private void CheckLaps(StateTransform ball)
    {
        if (ball != last) return;
        Lap++;
        if (Lap % lCount != 0) return;
        CreateBall();
    }

    private void SaveRecord(int score)
    {
        File.WriteAllBytes(Application.persistentDataPath + "/" + RecordPath, BitConverter.GetBytes(score));
    }

    private int LoadRecord()
    {
        string path = Application.persistentDataPath + "/" + RecordPath;
        if (!File.Exists(path)) return 0;
        return BitConverter.ToInt32(File.ReadAllBytes(path), 0);
    }

    private bool CheckRightTouch()
    {
        foreach(var a in Input.touches)
        {
            if (a.phase == TouchPhase.Began && a.position.x <= TouchBorder) return true;
        }
        return false;
    }

    private bool CheckLeftTouch()
    {
        foreach (var a in Input.touches)
        {
            if (a.phase == TouchPhase.Began &&  a.position.x > TouchBorder) return true;
        }
        return false;
    }

    private bool CheckGlobalTap()
    {
        if (Input.GetKeyDown(KeyCode.Space)) return true;
        foreach (var a in Input.touches) if (a.phase == TouchPhase.Began) return true;
        return false;
    }

    void Update () {
        if (!isplay)
        {
            if (CheckGlobalTap()) Restart(); 
            return;
        }
        bool click = CheckLeftTouch() || Input.GetKeyDown(KeyCode.Space);
        bool clickt = CheckRightTouch() || Input.GetKeyDown(KeyCode.LeftControl);
        foreach (var ball in Balls)
        {
            if (ChechHeight(ball))
            {
                Stop();
                break;
            }
            if (clickt && ball.State == 0 && CheckEnd(ball))
            {
                CheckLaps(ball);
                Score++;
                ball.transform.position = c.position;
                ball.State = 1;
            }
            else if (click && ball.State == 1 && CheckStart(ball))
            {
                Score++;
                ball.transform.position = a.position;
                ball.State = 0;
            }
            Moving(ball);
        }
	}



}
