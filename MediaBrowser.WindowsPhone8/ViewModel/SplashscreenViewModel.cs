﻿using System.Threading;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Photo;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using Microsoft.Phone.Controls;
using INavigationService = MediaBrowser.WindowsPhone.Model.Interfaces.INavigationService;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SplashscreenViewModel : ViewModelBase
    {
        private readonly IApplicationSettingsService _applicationSettings;

        /// <summary>
        /// Initializes a new instance of the SplashscreenViewModel class.
        /// </summary>
        public SplashscreenViewModel(IConnectionManager connectionManager, INavigationService navigationService, IApplicationSettingsService applicationSettings)
            : base(navigationService, connectionManager)
        {
            _applicationSettings = applicationSettings;
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.SplashAnimationFinishedMsg))
                {
                    App.Settings.ConnectionDetails = new ConnectionDetails
                    {
                        PortNo = 8096
                    };

                    var doNotShowFirstRun = _applicationSettings.Get(Constants.Settings.DoNotShowFirstRun, false);

                    if (!doNotShowFirstRun)
                    {
                        NavigationService.NavigateTo(Constants.Pages.FirstRun.WelcomeView);
                        return;
                    }

                    SetProgressBar(AppResources.SysTrayLoadingSettings);

#if !DEBUG
                    //try
                    //{
                    //    if (!ApplicationManifest.Current.App.Title.ToLower().Contains("beta"))
                    //    {
                    //        var marketPlace = new MarketplaceInformationService();
                    //        var appInfo = await marketPlace.GetAppInformationAsync(ApplicationManifest.Current.App.ProductId);

                    //        if (new Version(appInfo.Entry.Version) > new Version(ApplicationManifest.Current.App.Version) &&
                    //            MessageBox.Show("There is a newer version, would you like to install it now?", "Update Available", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    //        {
                    //            new MarketplaceDetailService().Show();
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    Log.ErrorException("GetAppInformationAsync()", ex);
                    //}
#endif
                    ConnectionResult result = null;

                    // Get settings from storage 
                    var connectionDetails = _applicationSettings.Get<ConnectionDetails>(Constants.Settings.ConnectionSettings);
                    if (connectionDetails != null)
                    {
                        result = await ConnectionManager.Connect(connectionDetails.ServerAddress, default(CancellationToken));
                    }
                    //    var messageBox = new CustomMessageBox
                    //    {
                    //        Caption = AppResources.ErrorConnectionDetailsTitle,
                    //        Message = AppResources.ErrorConnectionDetailsMessage,
                    //        LeftButtonContent = AppResources.LabelYes,
                    //        RightButtonContent = AppResources.LabelNo,
                    //        IsFullScreen = false
                    //    };

                    //    messageBox.Dismissed += (sender, args) =>
                    //    {
                    //        if (args.Result == CustomMessageBoxResult.LeftButton)
                    //        {
                    //            NavigationService.NavigateTo(Constants.Pages.SettingsViewConnection);
                    //        }
                    //    };

                    //    messageBox.Show();
                    //}
                    //else
                    //{
                    //App.Settings.ConnectionDetails = connectionDetails;

                    // Get and set the app specific settings 
                    var specificSettings = _applicationSettings.Get<SpecificSettings>(Constants.Settings.SpecificSettings);
                    if (specificSettings != null) Utils.CopyItem(specificSettings, App.SpecificSettings);

                    var uploadSettings = _applicationSettings.Get<UploadSettings>(Constants.Settings.PhotoUploadSettings);
                    if (uploadSettings != null) Utils.CopyItem(uploadSettings, App.UploadSettings);

                    // See if we can find and communicate with the server
                    SetProgressBar(AppResources.SysTrayGettingServerDetails);

                    if (result == null || result.State == ConnectionState.Unavailable)
                    {
                        result = await ConnectionManager.Connect(default(CancellationToken));
                    }

                    switch (result.State)
                    {
                        case ConnectionState.Unavailable:
                            App.ShowMessage(AppResources.ErrorCouldNotFindServer);
                            NavigationService.NavigateTo(Constants.Pages.SettingsViewConnection);
                            break;
                        case ConnectionState.ServerSelection:

                            break;
                        case ConnectionState.ServerSignIn:
                            await Utils.CheckProfiles(NavigationService, Log, ApiClient);
                            break;
                        case ConnectionState.SignedIn:
                            AuthenticationService.Current.SetAuthenticationInfo();
                            break;
                    }

                    SetProgressBar();
                }
            });
        }

        public RelayCommand TestConnectionCommand { get; set; }
    }
}