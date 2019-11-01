using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum Colors
{
    OnlineUserNormal,
    OnlineUserHighlighted,
    OnlineUserPressed,
    OnlineSelectedUserNormal,
    OnlineSelectedUserHighlighted,
    OnlineSelectedUserPressed
}

public class ColorUtilityManager : MonoBehaviour
{
    public static ColorUtilityManager Instance;

    //This materials for the avatar
    public Material onlineUserMaterial;
    public Material selectedUserMaterial;

    private void Awake()
    {
        Instance = this;
    }

    public Color GetColor(int colorIndex)
    {
        Color color;
        switch (colorIndex)
        {
            case (int)Colors.OnlineUserNormal:
                color = new Color(0.01176471f, 0.2627451f, 0.08235294f);
                break;
            case (int)Colors.OnlineUserHighlighted:
                color = new Color(0.03083838f, 0.5943396f, 0.2917185f);
                break;
            case (int)Colors.OnlineUserPressed:
                color = new Color(0.05473477f, 0.7735849f, 0.3875357f);
                break;
            case (int)Colors.OnlineSelectedUserNormal:
                color = Color.red;
                break;
            case (int)Colors.OnlineSelectedUserHighlighted:
                color = new Color(0.990566f, 0.2850213f, 0.3630024f);
                break;
            case (int)Colors.OnlineSelectedUserPressed:
                color = new Color(0.9716981f, 0.4629316f, 0.5173451f);
                break;
            default:
                color = Color.black;
                break;                
        }
        return color;
    }
    //Not working completely true because of the prefab problems
    public void SetColorofAvatars(User selectedUser)
    {
        GameObject[] userAvatars = GameObject.FindGameObjectsWithTag("UserAvatar");
        GameObject avatar;
        Renderer[] avatarRenderer;
        for (int i = 0; i < userAvatars.Length; i++)
        {
            if (selectedUser != null)
            {
                if (userAvatars[i].name.ToLower().Equals(selectedUser.Username.ToLower()))
                {

                    avatar = GameObject.Find("/" + selectedUser.Username);
                    //Debug.Log("Selected:" + avatar.name);
                    avatarRenderer = avatar.GetComponentsInChildren<Renderer>();

                    //Call SetColor using the shader property name "_Color" and setting the color to red
                    foreach (Renderer renderer in avatarRenderer)
                    {
                        renderer.material = selectedUserMaterial;

                    }
                    continue;
                }

            }
            avatar = GameObject.Find("/" + userAvatars[i].name);
            avatarRenderer = avatar.GetComponentsInChildren<Renderer>();

            //Call SetColor using the shader property name "_Color" and setting the color to yellow
            foreach (Renderer renderer in avatarRenderer)
            {
                renderer.material = onlineUserMaterial;
            }
        }
    }

    public void SetColorofOnlineUserButtons(User selectedUser, string allUserText = " ")
    {
        GameObject[] onlineButtons = GameObject.FindGameObjectsWithTag("OnlineButtons");
        ColorBlock colors;
        foreach (GameObject gObj in onlineButtons)
        {
            string text = gObj.GetComponentInChildren<TextMeshProUGUI>().text;
            if ((selectedUser != null && text.ToLower().Equals(selectedUser.Username.ToLower())) || text.ToLower().Equals(allUserText.ToLower()))
            {
                colors = gObj.GetComponentInChildren<Button>().colors;
                colors.normalColor = GetColor((int)Colors.OnlineSelectedUserNormal);
                colors.highlightedColor = GetColor((int)Colors.OnlineSelectedUserHighlighted);
                colors.pressedColor = GetColor((int)Colors.OnlineSelectedUserPressed);
                gObj.GetComponentInChildren<Button>().colors = colors;
                continue;
            }

            colors = gObj.GetComponentInChildren<Button>().colors;
            colors.normalColor = GetColor((int)Colors.OnlineUserNormal);
            colors.highlightedColor = GetColor((int)Colors.OnlineUserHighlighted);
            colors.pressedColor = GetColor((int)Colors.OnlineUserPressed);
            gObj.GetComponentInChildren<Button>().colors = colors;
        }
    }

    public void SetColorofCamMobilityButtons(GameObject selectedButton)
    {
        GameObject[] camMobilityButtons = GameObject.FindGameObjectsWithTag("CamMobility");
        ColorBlock colors;

        foreach (GameObject gObj in camMobilityButtons)
        {

            if (gObj.Equals(GameObject.Find("/Canvas/AddOrNavigate/"+ selectedButton.name)))
            {
                colors = gObj.GetComponentInChildren<Button>().colors;
                colors.normalColor = GetColor((int)Colors.OnlineSelectedUserNormal);
                colors.highlightedColor = GetColor((int)Colors.OnlineSelectedUserHighlighted);
                colors.pressedColor = GetColor((int)Colors.OnlineSelectedUserPressed);
                gObj.GetComponentInChildren<Button>().colors = colors;
                continue;
            }
            colors = gObj.GetComponentInChildren<Button>().colors;
            colors.normalColor = GetColor((int)Colors.OnlineUserNormal);
            colors.highlightedColor = GetColor((int)Colors.OnlineUserHighlighted);
            colors.pressedColor = GetColor((int)Colors.OnlineUserPressed);
            gObj.GetComponentInChildren<Button>().colors = colors;            
        }
    }

}
