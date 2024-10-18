using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ScoreDatas
{
    public int highScore;
    public List<int> highScores = new List<int>();

}


public class Rocket : MonoBehaviour
{
    public GameObject ground;

    private Rigidbody2D _rb2d;
    private float fuel = 100f; //연료

    private readonly float SPEED = 5f;
    private readonly float FUELPERSHOOT = 10f;
    private readonly int POWER = 50;

    [SerializeField] private TextMeshProUGUI currentScoreTxt;
    [SerializeField] private TextMeshProUGUI HighScoreTxt;

    private Rigidbody2D rb;

    public Button ShootBtn;

    ScoreDatas scoreDatas = new ScoreDatas();
    public int HighScore => scoreDatas.highScore;
    public List<int> Scores => scoreDatas.highScores;
    private string fileName = "Scores.json";
    private int currentScore;

    void Awake()
    {
        // TODO : Rigidbody2D 컴포넌트를 가져옴(캐싱) 
        LoadScores();
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("저장경로" + Path.Combine(Application.persistentDataPath, fileName));
    }

    private void Update()
    {
        DisplayInfo();
    }

    public void Shoot()
    {
        if (fuel > 0)
        {
            // TODO : fuel이 넉넉하면 윗 방향으로 SPEED만큼의 힘으로 점프, 모자라면 무시
            rb.AddForce(Vector2.up * SPEED, ForceMode2D.Impulse); //힘/질량 값으로 속도를 변경
            //Addforce만으로 올라가기
            fuel -= FUELPERSHOOT;
            Debug.Log("남은연료: " + fuel);
            if (fuel <= 0)
            {
                fuel = 0;
            }
        }
    }

    public void DisplayInfo()
    {
        //현재점수 = 내위치 - ground위치
        currentScore = (int)(transform.position.y - ground.transform.position.y);
        currentScoreTxt.text = $"{currentScore}M";

        HighScoreTxt.text = $"HIGH: {HighScoreLogic()}M";
    }

    public int HighScoreLogic()
    {
        int highScore = Scores.Count > 0 ? Scores[0] : currentScore;
        if (currentScore > highScore)
        {
            highScore = currentScore;
            if(fuel <= 0)
            {
                UpdateHighScore();
            }

        }
        return highScore;
    }

    public void UpdateHighScore()
    {
        Scores.Add(currentScore);
        Scores.Sort((a, b) => b.CompareTo(a));
        if (Scores.Count > 1)
        {
            Scores.RemoveRange(1, Scores.Count - 1);
        }

        SaveScores(); 

    }

    public void SaveScores()
    {
        string json = JsonUtility.ToJson(scoreDatas, true);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, json);
        Debug.Log("저장");
    }

    public void LoadScores()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            Debug.Log("로드완");
            string json = File.ReadAllText(path);
            scoreDatas = JsonUtility.FromJson<ScoreDatas>(json);
        }
        else Debug.Log("경로에 없음");
    }


}
