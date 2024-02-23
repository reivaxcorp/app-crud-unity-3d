using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinksCompany : MonoBehaviour
{
    private const string YOUTUBE_TUTORIALS =
                                            "https://youtube.com/playlist?list=PLsvltDspdJcfiiWy2baA2MCNzBm32USjv&si=q7dTsZltYs-d3eOI";
    private const string GIT_HUB = 
                                "https://github.com/reivaxcorp/app-crud-unity-3d";

    public void OpenYoutubeTutorial()
    {
        Application.OpenURL(YOUTUBE_TUTORIALS);
    }

    public void OpenGitHubCode()
    {
        Application.OpenURL(GIT_HUB);
    }
}