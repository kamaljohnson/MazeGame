using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

    public class FileStoragePermissionTest : MonoBehaviour 
    {
        GameObject dialog = null;
        
        void Awake ()
        {
            #if PLATFORM_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) && !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
                {
                    Permission.RequestUserPermission(Permission.Microphone);
                    dialog = new GameObject();
                }
            #endif
        }

        void OnGUI ()
        {
    #if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) && !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                // The user denied permission to use the microphone.
                // Display a message explaining why you need it with Yes/No buttons.
                // If the user says yes then present the request again
                // Display a dialog here.
                dialog.AddComponent<PermissionsRationaleDialog>();
            }
            else if (dialog != null)
            {
                Destroy(dialog);
            }
    #endif

            // Now you can do things with the microphone
        }
}