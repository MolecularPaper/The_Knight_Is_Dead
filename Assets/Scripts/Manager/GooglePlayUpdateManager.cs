using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class GooglePlayUpdateManager : MonoBehaviour
{
    public static GooglePlayUpdateManager updateManager;

    AppUpdateManager appUpdateManager;

    void Awake()
    {
        if(GooglePlayUpdateManager.updateManager == null)
        {
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        appUpdateManager = new AppUpdateManager();

#if UNITY_ANDROID
        CheckUpdateCycle();
#endif
    }

    public async void CheckUpdateCycle()
    {
        while (true)
        {
            await CheckForUpdate();
            await Task.Delay(60000);
        }
    }

    public async Task CheckForUpdate()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

        if(appUpdateInfoOperation == null)

        appUpdateInfoOperation = await Task.Run(() => {
            return appUpdateInfoOperation;
        });

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions(allowAssetPackDeletion: true);

            if (appUpdateInfoResult.IsUpdateTypeAllowed(appUpdateOptions))
            {
                var startUpdateRequest = await Task.Run(() =>
                {
                    return appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);
                });

                print(startUpdateRequest.Error);
                Application.Quit();
            }
        }
        else
        {
            print(appUpdateInfoOperation.Error);
        }
    }
}
