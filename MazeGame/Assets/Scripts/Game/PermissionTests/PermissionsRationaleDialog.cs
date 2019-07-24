using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class PermissionsRationaleDialog : MonoBehaviour
{
    const int KDialogWidth = 300;
    const int KDialogHeight = 100;
    private bool _windowOpen = true;

    void DoMyWindow(int windowID)
    {
        GUI.Label(new Rect(10, 20, KDialogWidth - 20, KDialogHeight - 50), "Please give permission to save and load game data");
        GUI.Button(new Rect(10, KDialogHeight - 30, 100, 20), "No");
        if (GUI.Button(new Rect(KDialogWidth - 110, KDialogHeight - 30, 100, 20), "Yes"))
        {
            #if PLATFORM_ANDROID
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            #endif
            
            _windowOpen = false;
        }
    }

    void OnGUI ()
    {
        if (_windowOpen)
        {
            Rect rect = new Rect((Screen.width / 2) - (KDialogWidth / 2), (Screen.height / 2) - (KDialogHeight / 2), KDialogWidth, KDialogHeight);
            GUI.ModalWindow(0, rect, DoMyWindow, "Permissions Request Dialog");
        }
    }
}