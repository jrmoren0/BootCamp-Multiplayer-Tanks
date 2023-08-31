using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using TMPro;

public class RestAPITest : MonoBehaviour
{

    [SerializeField] private TMP_InputField _inputUsername;
    [SerializeField] private TMP_InputField _inputPassword;

    [SerializeField] private TMP_InputField _inputScore;


    [SerializeField] private TMP_Text _textUserName;
    [SerializeField] private TMP_Text _textResponseMessge;

    [SerializeField] private GameObject _highScoreElement;
    [SerializeField] private GameObject _registarPanel;
    [SerializeField] private Transform _scoreboard;


    [SerializeField] private GameObject _buttonRegistar;
    [SerializeField] private GameObject _buttonLogin;







    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TestAPI());
    }

    IEnumerator TestAPI()
    {

        UnityWebRequest webRequest = UnityWebRequest.Get("https://bootcamp-restapi-practice.xrcourse.com");

        yield return webRequest.SendWebRequest();


        Debug.Log("Response Code " + webRequest.responseCode);
        Debug.Log("Response errors " + webRequest.error);
        Debug.Log(webRequest.downloadHandler.text);
    }

    public void RegistarUserCall()
    {
        StartCoroutine(RegistarUser(_inputUsername.text, _inputPassword.text));

        _buttonRegistar.SetActive(false);
    }

    public void LoginUserCall()
    {
        StartCoroutine(LoginUser(_inputUsername.text, _inputPassword.text));
        _buttonRegistar.SetActive(false);
        _buttonLogin.SetActive(false);
    }


    public void SubmitScoreCall()
    {
       StartCoroutine(SubmitScore(int.Parse(_inputScore.text)));
    }

    public void UpdateScoreBoardCall()
    {

    }

    IEnumerator UpdateScoreBoard()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://bootcamp-restapi-practice.xrcourse.com/top-scores");

        yield return webRequest.SendWebRequest();


        Debug.Log("Response Code " + webRequest.responseCode);
        Debug.Log("Response errors " + webRequest.error);

        Debug.Log(webRequest.downloadHandler.text);

        // parse high scores.

    }


    IEnumerator SubmitScore(int score)
    {
        Score ScoreData = new Score();

        ScoreData.score = score;

        string dataToUpload = JsonUtility.ToJson(ScoreData);


        UnityWebRequest submitScoreRequest = UnityWebRequest.Post("https://bootcamp-restapi-practice.xrcourse.com/submit-score", dataToUpload, "application/json");


        yield return submitScoreRequest.SendWebRequest();

        //Add header



        Debug.Log("Response Code " + submitScoreRequest.responseCode);
        Debug.Log("Response errors " + submitScoreRequest.error);

        Debug.Log(submitScoreRequest.downloadHandler.text);


        //Update Scoreboard Call

    }

    ///public void Su

    IEnumerator LoginUser(string username, string password)
    {

        User user = new User();

        user.username = username;

        user.password = password;


        string dataToUpload = JsonUtility.ToJson(user);


        UnityWebRequest loginUserWebRequest = UnityWebRequest.Post("https://bootcamp-restapi-practice.xrcourse.com/login", dataToUpload, "application/json" ); // "application/json"
      


        yield return loginUserWebRequest.SendWebRequest();


        Debug.Log("Response Code " + loginUserWebRequest.responseCode);
        Debug.Log("Response errors " + loginUserWebRequest.error);

        Debug.Log(loginUserWebRequest.downloadHandler.text);


        Login loginData = JsonUtility.FromJson<Login>(loginUserWebRequest.downloadHandler.text);

        PlayerPrefs.SetString("token", loginData.token);

        _textUserName.text = username;

        _registarPanel.SetActive(false);

    }



    IEnumerator RegistarUser( string username, string password)
    {
        User user = new User();

        user.username = username;

        user.password = password;


        string dataToUpload = JsonUtility.ToJson(user);


        UnityWebRequest registerUserWebRequest = UnityWebRequest.Post("https://bootcamp-restapi-practice.xrcourse.com/register", dataToUpload,"application/json"); // "application/json"



        yield return registerUserWebRequest.SendWebRequest();


        Debug.Log("Response Code " + registerUserWebRequest.responseCode);
        Debug.Log("Response errors " + registerUserWebRequest.error);

        Debug.Log(registerUserWebRequest.downloadHandler.text);

        
    }


}
