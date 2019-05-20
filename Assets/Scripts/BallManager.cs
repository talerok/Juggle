using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Interfaces;

public enum BallsUpdateStatus
{
    Ok,
    Fail,
    NewBall
}

public class BallManager : MonoBehaviour {

    public Style Style;

    public float aSpeed;
    public float bSpeed;
    public float cSpeed;
    public float acSpeed;

    public Camera cam;

    public float pk;
    public int LCount;

    public Transform Menu;
    public Text ScoreText;
    public Text RecordText;
    public Text StartText;

    public bool isplay { get; private set; }

    public int Score {
        get
        {
            return _score;
        }
        private set
        {
            _score = value;
            _redrawMenu(true);
        }

    }

    public int Record
    {
        get
        {
            return _recordManager.Record;
        }
        private set
        {
            _recordManager.Record = value;
            _redrawMenu(false);
        }

    }

    private Vector2 aPos;
    private Vector2 bPos;
    private Vector2 cPos;
    private Vector2 acPos;

    private List<Trajectory> _trajectoryes = new List<Trajectory>();
    private List<StateTransform> _balls = new List<StateTransform>();

    private float _complexy;
    private float _checkDist;
    private float _deleteH;

    private int _lap = 0;

    //-------
    private int _score;
    private int _playedRounds = 0;
    //-------
    private int _touchBorder;

    private RecordManager _recordManager = new RecordManager();

    private void _initBall(StateTransform res)
    {
        res.transform.position = new Vector3(aPos.x, aPos.y, 0);
        float bsize = _checkDist / 4;
        res.transform.localScale = new Vector3(bsize, bsize, bsize);
    }

    private void _deleteBalls()
    {
        foreach (var ball in _balls)
        {
            Destroy(ball.transform.gameObject);
        }
        _balls.Clear();
    }

    private StateTransform _createBall()
    {
        var res = new StateTransform(Instantiate(Style.Ball));
        _initBall(res);
        _balls.Insert(0, res);
        _setComplexy();
        return res;
    }

    private void _setComplexy()
    {
        _complexy = Mathf.Sqrt(Mathf.Sqrt(_balls.Count));
    }

    private bool _chechHeight(StateTransform ball)
    {
        if (ball.transform.position.y <= _deleteH) return true;
        return false;
    }

    private float _getDt(int id)
    {
        if (id == 0) return Time.deltaTime * pk * _complexy;
        else return Time.deltaTime * pk;
    }

    private void _moving(StateTransform ball)
    {
        int id = ball.State;
        ball.transform.position = _trajectoryes[id].Get(ball.transform.position, _getDt(id));
    }

    private bool _checkStart(StateTransform ball)
    {
        if (Vector2.Distance(aPos, ball.transform.position) <= _checkDist) return true;
        return false;
    }

    private bool _checkEnd(StateTransform ball)
    {
        if (Vector2.Distance(cPos, ball.transform.position) <= _checkDist) return true;
        return false;
    }

    private void _setUpCam()
    {
        cam.orthographicSize = Screen.height / 2;
        _touchBorder = cam.pixelWidth / 2;
    }

    private void _setUpBackGround()
    {
        var background = Instantiate(Style.Background);

        background.position = new Vector3(0, 0, 10);
        background.localScale = new Vector3(cam.pixelHeight, cam.pixelHeight, 0);
    }

    private void _setUpParams()
    {
        pk = -cam.pixelWidth;
        _deleteH = _getPos(new Vector2(0, 0)).y - 20;
        _checkDist = cam.pixelHeight < cam.pixelWidth ? cam.pixelHeight / 3 : cam.pixelWidth / 3;
    }

    private void _setUpCoords()
    {
        aPos = _getPos(new Vector2(cam.pixelWidth, 0));
        bPos = _getPos(new Vector2(cam.pixelWidth / 2, cam.pixelHeight));
        cPos = _getPos(new Vector2(0, 0));
        acPos = _getPos(new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 3));
    }

    private void _setUpObjects()
    {
        var leftHand = Instantiate(Style.LeftHand);
        leftHand.position = new Vector3(cPos.x, cPos.y, 0);
        leftHand.localScale = new Vector3(_checkDist, _checkDist, _checkDist); 

        var rightHand = Instantiate(Style.RightHand);
        rightHand.position = new Vector3(aPos.x, aPos.y, 0);
        rightHand.localScale = new Vector3(_checkDist, _checkDist, _checkDist);

        ScoreText.material = Style.ScoreMaterial;
        RecordText.material = Style.RecordMaterial;
        StartText.material = Style.TextMaterial;
    }

    private void _setUpScene()
    {
        _setUpParams();
        _setUpCoords();
        _setUpObjects();
    }

    public Vector2 _getPos(Vector2 campos)
    {
        var res = cam.ScreenToWorldPoint(new Vector3(campos.x, campos.y, 0));
        return new Vector2(res.x, res.y);
    }

    private void _setUpTrajectories()
    {
        _trajectoryes.Add(
            new Trajectory(
                new Functions.Parabola(aPos, bPos, cPos),
                new Functions.Parabola(new Vector2(aPos.x, aSpeed), new Vector2(bPos.x, bSpeed), new Vector2(cPos.x, cSpeed))
         ));
        _trajectoryes.Add(
            new Trajectory(
                new Functions.Parabola(aPos, acPos, cPos),
                new Functions.Const(-acSpeed)
         ));
    }

    public void Start () {
        _redrawMenu(false);
        _setUpCam();
        _setUpScene();
        _setUpBackGround();
        _setUpTrajectories();
    }

    public void Restart()
    {
        _deleteBalls();
        Score = 0;
        _lap = 0;
        isplay = true;
        _createBall();

        Menu.gameObject.SetActive(false);
    }

    private void _showAdd()
    {
        
    }

    private void _stop()
    {
        _playedRounds++;
        isplay = false;
        if(Record < Score)
            Record = Score;
        Menu.gameObject.SetActive(true);
    }

    private void _redrawMenu(bool f)
    {
        if (f)
            ScoreText.text = Score.ToString();
        else
            RecordText.text = "Record: " + Record;
    }

    private bool _checkRightTouch()
    {
        foreach(var a in Input.touches)
        {
            if (a.phase == TouchPhase.Began && a.position.x <= _touchBorder) return true;
        }
        return false;
    }

    private bool _checkLeftTouch()
    {
        foreach (var a in Input.touches)
        {
            if (a.phase == TouchPhase.Began &&  a.position.x > _touchBorder) return true;
        }
        return false;
    }

    private bool _checkGlobalTap()
    {
        if (Input.GetKeyDown(KeyCode.Space)) return true;
        foreach (var a in Input.touches) if (a.phase == TouchPhase.Began) return true;
        return false;
    }

    private BallsUpdateStatus _moveBalls(bool leftClick, bool rightClick)
    {
        bool newBall = false;

        foreach (var ball in _balls)
        {
            if (_chechHeight(ball))
                return BallsUpdateStatus.Fail;

            if (rightClick && ball.State == 0 && _checkEnd(ball))
            {
                Score++;
                ball.transform.position = new Vector3(cPos.x, cPos.y, 0);
                ball.State = 1;

                if(ball == _balls[0])
                {
                    _lap++;
                    if(_lap % 3 == 0)
                        newBall = true;
                }
            }
            else if (leftClick && ball.State == 1 && _checkStart(ball))
            {
                Score++;
                ball.transform.position = new Vector3(aPos.x, aPos.y, 0);
                ball.State = 0;
            }
            _moving(ball);
        }
        return newBall ? BallsUpdateStatus.NewBall : BallsUpdateStatus.Ok;
    }

    void Update () {
        bool click = _checkLeftTouch() || Input.GetKeyDown(KeyCode.Space);
        bool clickt = _checkRightTouch() || Input.GetKeyDown(KeyCode.LeftControl);

        if (!isplay)
        {
            if (_checkGlobalTap()) Restart(); 
            return;
        }

        var status = _moveBalls(click, clickt);
        if (status == BallsUpdateStatus.Fail)
            _stop();
        else if (status == BallsUpdateStatus.NewBall && _balls.Count < 6)
            _createBall();
	}



}
