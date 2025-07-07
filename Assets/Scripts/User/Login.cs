using Iterum.Scripts.Utils;
using Iterum.DTOs;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public TMP_InputField ifUsername;
    public TMP_InputField ifPassword;

    public Button btnRegister;
    public Button btnLogin;

    public TMP_Text lblError;

    void Start()
    {
        lblError.enabled = false;

        btnLogin.onClick.AddListener(TryLogin);
        btnRegister.onClick.AddListener(() =>
        {
            lblError.enabled = false;
            UserManager.Instance.CreateUser(new RegisterForm(ifUsername.text, ifPassword.text), LoginSuccess, SetErrorMessage);
        });
    }

    private void TryLogin() {
        lblError.enabled = false;
        UserManager.Instance.Login(
            new LoginForm(ifUsername.text, ifPassword.text),
            LoginSuccess,
            SetErrorMessage
        );
    }

    private void LoginSuccess(LoginResponse response)
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void SetErrorMessage(string error)
    {
        lblError.enabled = true;
        if (error.Contains("401"))
        {
            lblError.text = "Bad credentials";
        }
        else
        {
            lblError.text = error;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (ifPassword.isFocused)
            {
                ifUsername.Select();
            }
            else if (ifUsername.isFocused)
            { 
                ifPassword.Select();
            }
        }
    }
}
