using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class LoginSendData
{
    public AuthParameters AuthParameters;
    public string AuthFlow;
    public string ClientId;
}

[System.Serializable]
public class AuthParameters
{
    public string USERNAME;
    public string PASSWORD;
}

[System.Serializable]
public class AuthenticationResult
{
    public string IdToken;
}

[System.Serializable]
public class LoginResultData
{
    public AuthenticationResult AuthenticationResult;
}

[System.Serializable]
public class SendPostData
{
    public string message;
}

[System.Serializable]
public class ResultPostData
{
    public string answer;
    public string presigned_url;
}