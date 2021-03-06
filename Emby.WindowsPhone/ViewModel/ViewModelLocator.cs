﻿using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.ApiInteraction;
using MediaBrowser.ApiInteraction.Cryptography;
using MediaBrowser.ApiInteraction.Data;
using MediaBrowser.ApiInteraction.Playback;
using MediaBrowser.ApiInteraction.Sync;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using Emby.WindowsPhone.Design;
using Emby.WindowsPhone.Extensions;
using Emby.WindowsPhone.Helpers;
using Emby.WindowsPhone.Interfaces;
using Emby.WindowsPhone.Logging;
using Emby.WindowsPhone.Messaging;
using Emby.WindowsPhone.Model.Connection;
using Emby.WindowsPhone.Model.Security;
using Emby.WindowsPhone.Model.Sync;
using Emby.WindowsPhone.Services;
using Emby.WindowsPhone.ViewModel.Channels;
using Emby.WindowsPhone.ViewModel.Playlists;
using Emby.WindowsPhone.ViewModel.Predefined;
using Emby.WindowsPhone.ViewModel.Remote;
using Emby.WindowsPhone.ViewModel.Settings;
using Emby.WindowsPhone.ViewModel.Sync;
using Microsoft.Practices.ServiceLocation;
using Emby.WindowsPhone.Model;
using MediaBrowser.Model;
using INavigationService = Emby.WindowsPhone.Model.Interfaces.INavigationService;
using NavigationService = Emby.WindowsPhone.Services.NavigationService;
using Emby.WindowsPhone.ViewModel.LiveTv;
using GalaSoft.MvvmLight.Messaging;

namespace Emby.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
    /// to this locator.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var device = new Device
            {
                DeviceName = SharedUtils.GetDeviceName(),
                DeviceId = SharedUtils.GetDeviceId()
            };

            var logger = new MBLogger();
            var network = new NetworkConnection();

            SimpleIoc.Default.RegisterIf<ILogger>(() => logger);
            SimpleIoc.Default.RegisterIf<IDevice>(() => device);
            SimpleIoc.Default.RegisterIf<INetworkConnection>(() => network);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.RegisterIf<INavigationService, NavigationService>();
                SimpleIoc.Default.RegisterIf<FolderViewModel>();
                SimpleIoc.Default.RegisterIf<MovieViewModel>();
                SimpleIoc.Default.RegisterIf<IApplicationSettingsService, ApplicationSettingsDesignService>();
                SimpleIoc.Default.RegisterIf<IStorageService, StorageDesignService>();
                SimpleIoc.Default.RegisterIf<ILocalAssetManager, NullAssetManager>();
            }
            else
            { 
                SimpleIoc.Default.RegisterIf<INavigationService, NavigationService>();
                SimpleIoc.Default.RegisterIf<ISettingsService, SettingsService>();
                SimpleIoc.Default.RegisterIf<IMessengerService, MessengerService>();
                SimpleIoc.Default.RegisterIf(() => Utils.CreateConnectionManager(device, logger, network));
                SimpleIoc.Default.RegisterIf<IApplicationSettingsService, ApplicationSettingsService>();
                SimpleIoc.Default.RegisterIf<IStorageService, StorageService>();
                SimpleIoc.Default.RegisterIf<IServerInfoService, ServerInfoService>();
                SimpleIoc.Default.RegisterIf<AuthenticationService>(true);
                SimpleIoc.Default.RegisterIf<LockScreenService>(true);
                SimpleIoc.Default.RegisterIf<TileService>(true);
                SimpleIoc.Default.RegisterIf<TrialHelper>(true);
                SimpleIoc.Default.RegisterIf<IMessagePromptService, MessagePromptService>();
                SimpleIoc.Default.RegisterIf<ITransferService, TransferService>();

                AddSyncInterfaces();

                SimpleIoc.Default.RegisterIf<IPlaybackManager>(() => new PlaybackManager(AssetManager, device, logger, network));
            }

            SimpleIoc.Default.RegisterIf<IMessageBoxService, MessageBoxService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<VideoPlayerViewModel>();
            SimpleIoc.Default.Register<SplashscreenViewModel>();
            SimpleIoc.Default.Register<ChooseProfileViewModel>();
            SimpleIoc.Default.Register<TvViewModel>();
            SimpleIoc.Default.Register<TrailerViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<MusicViewModel>();
            SimpleIoc.Default.Register<SearchViewModel>();
            SimpleIoc.Default.Register<NowPlayingViewModel>(true);
            SimpleIoc.Default.Register<NotificationsViewModel>();
            SimpleIoc.Default.Register<RemoteViewModel>();
            SimpleIoc.Default.Register<MovieCollectionViewModel>();
            SimpleIoc.Default.Register<TvCollectionViewModel>();
            SimpleIoc.Default.Register<MusicCollectionViewModel>();
            SimpleIoc.Default.Register<ActorViewModel>();
            SimpleIoc.Default.Register<GenericItemViewModel>();
            SimpleIoc.Default.Register<LiveTvChannelsViewModel>();
            SimpleIoc.Default.Register<GuideViewModel>();
            SimpleIoc.Default.Register<ScheduleViewModel>();
            SimpleIoc.Default.Register<LiveTvViewModel>();
            SimpleIoc.Default.Register<ScheduledSeriesViewModel>();
            SimpleIoc.Default.Register<ScheduledRecordingViewModel>();
            SimpleIoc.Default.Register<AllProgrammesViewModel>();
            SimpleIoc.Default.Register<ProgrammeViewModel>();
            SimpleIoc.Default.Register<RecordedTvViewModel>();
            SimpleIoc.Default.Register<ChannelViewModel>();
            SimpleIoc.Default.Register<ChannelsViewModel>();
            SimpleIoc.Default.Register<ServerPlaylistsViewModel>();
            SimpleIoc.Default.Register<AddToPlaylistViewModel>();
            SimpleIoc.Default.Register<PhotoUploadViewModel>();
            SimpleIoc.Default.Register<MbConnectViewModel>();
            SimpleIoc.Default.Register<SyncViewModel>();
            SimpleIoc.Default.Register<CurrentDownloadsViewModel>();
            SimpleIoc.Default.Register<SyncJobDetailViewModel>();
            SimpleIoc.Default.Register<UnlockFeaturesViewModel>();
            SimpleIoc.Default.Register<ConnectSignUpViewModel>();
        }

        private static void AddSyncInterfaces()
        {
            SimpleIoc.Default.RegisterIf<SyncRequestHelper>(true);
            SimpleIoc.Default.RegisterIf<IUserActionRepository, UserActionRepository>();
            SimpleIoc.Default.RegisterIf<IItemRepository, ItemRepository>();
            SimpleIoc.Default.RegisterIf<IFileRepository, FileRepository>();
            SimpleIoc.Default.RegisterIf<IFileTransferManager, FileTransferManager>();
            SimpleIoc.Default.RegisterIf<ICryptographyProvider, CryptographyProvider>();
            SimpleIoc.Default.RegisterIf<IUserRepository, UserRepository>();
            SimpleIoc.Default.RegisterIf<IImageRepository, ImageRepository>();
            SimpleIoc.Default.RegisterIf<ILocalAssetManager, LocalAssetManager>();
            SimpleIoc.Default.RegisterIf<IMultiServerSync, MultiServerSync>();

            SimpleIoc.Default.RegisterIf<SyncService>(true);
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public FolderViewModel Folder
        {
            get { return ServiceLocator.Current.GetInstance<FolderViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MovieViewModel Movie
        {
            get { return ServiceLocator.Current.GetInstance<MovieViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TvViewModel Tv
        {
            get { return ServiceLocator.Current.GetInstance<TvViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SplashscreenViewModel Splashscreen
        {
            get { return ServiceLocator.Current.GetInstance<SplashscreenViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ChooseProfileViewModel Profile
        {
            get { return ServiceLocator.Current.GetInstance<ChooseProfileViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public VideoPlayerViewModel Player
        {
            get { return ServiceLocator.Current.GetInstance<VideoPlayerViewModel>(); }
        }

        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TrailerViewModel Trailer
        {
            get { return ServiceLocator.Current.GetInstance<TrailerViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MusicViewModel Music
        {
            get { return ServiceLocator.Current.GetInstance<MusicViewModel>(); }
        }

        public SearchViewModel Search
        {
            get { return ServiceLocator.Current.GetInstance<SearchViewModel>(); }
        }

        public NowPlayingViewModel NowPlaying
        {
            get { return ServiceLocator.Current.GetInstance<NowPlayingViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public NotificationsViewModel Notifications
        {
            get { return ServiceLocator.Current.GetInstance<NotificationsViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MovieCollectionViewModel MovieCollection
        {
            get { return ServiceLocator.Current.GetInstance<MovieCollectionViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MusicCollectionViewModel MusicCollection
        {
            get { return ServiceLocator.Current.GetInstance<MusicCollectionViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TvCollectionViewModel TvCollection
        {
            get { return ServiceLocator.Current.GetInstance<TvCollectionViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public RemoteViewModel Remote
        {
            get { return ServiceLocator.Current.GetInstance<RemoteViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ActorViewModel Actor
        {
            get { return ServiceLocator.Current.GetInstance<ActorViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public GenericItemViewModel Generic
        {
            get { return ServiceLocator.Current.GetInstance<GenericItemViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public LiveTvChannelsViewModel LiveTvChannels
        {
            get { return ServiceLocator.Current.GetInstance<LiveTvChannelsViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public GuideViewModel Guide
        {
            get { return ServiceLocator.Current.GetInstance<GuideViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ScheduleViewModel Schedule
        {
            get { return ServiceLocator.Current.GetInstance<ScheduleViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public LiveTvViewModel LiveTv
        {
            get { return ServiceLocator.Current.GetInstance<LiveTvViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ScheduledSeriesViewModel ScheduledSeries
        {
            get { return ServiceLocator.Current.GetInstance<ScheduledSeriesViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ScheduledRecordingViewModel ScheduledRecording
        {
            get { return ServiceLocator.Current.GetInstance<ScheduledRecordingViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AllProgrammesViewModel AllProgrammes
        {
            get { return ServiceLocator.Current.GetInstance<AllProgrammesViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ProgrammeViewModel Programme
        {
            get { return ServiceLocator.Current.GetInstance<ProgrammeViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public RecordedTvViewModel RecordedTv
        {
            get { return ServiceLocator.Current.GetInstance<RecordedTvViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ChannelsViewModel Channels
        {
            get { return ServiceLocator.Current.GetInstance<ChannelsViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ChannelViewModel Channel
        {
            get { return ServiceLocator.Current.GetInstance<ChannelViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ServerPlaylistsViewModel ServerPlaylists
        {
            get { return ServiceLocator.Current.GetInstance<ServerPlaylistsViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public AddToPlaylistViewModel AddToPlaylist
        {
            get { return ServiceLocator.Current.GetInstance<AddToPlaylistViewModel>(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public PhotoUploadViewModel PhotoUpload
        {
            get { return ServiceLocator.Current.GetInstance<PhotoUploadViewModel>(); }
        }

        public MbConnectViewModel MbConnect
        {
            get { return ServiceLocator.Current.GetInstance<MbConnectViewModel>(); }
        }

        public SyncViewModel Sync
        {
            get { return ServiceLocator.Current.GetInstance<SyncViewModel>(); }
        }

        public CurrentDownloadsViewModel CurrentDownloads
        {
            get { return ServiceLocator.Current.GetInstance<CurrentDownloadsViewModel>(); }
        }

        public SyncJobDetailViewModel SyncJobDetail
        {
            get { return ServiceLocator.Current.GetInstance<SyncJobDetailViewModel>(); }
        }

        public UnlockFeaturesViewModel UnlockFeatures
        {
            get { return ServiceLocator.Current.GetInstance<UnlockFeaturesViewModel>(); }
        }

        public ConnectSignUpViewModel SignUp
        {
            get { return ServiceLocator.Current.GetInstance<ConnectSignUpViewModel>(); }
        }

        public TrialHelper Trial
        {
            get { return TrialHelper.Current; }
        }

        public static TvViewModel GetTvViewModel(string itemId)
        {
            return ServiceLocator.Current.GetInstance<TvViewModel>(itemId);
        }

        public static ChannelViewModel GetChannelViewModel(string itemId)
        {
            return ServiceLocator.Current.GetInstance<ChannelViewModel>(itemId);
        }

        public static IConnectionManager ConnectionManager
        {
            get { return ServiceLocator.Current.GetInstance<IConnectionManager>(); }
        }

        public static INavigationService NavigationService
        {
            get { return ServiceLocator.Current.GetInstance<INavigationService>(); }
        }

        public static AuthenticationService Auth
        {
            get { return ServiceLocator.Current.GetInstance<AuthenticationService>(); }
        }

        public static ILocalAssetManager AssetManager
        {
            get { return ServiceLocator.Current.GetInstance<ILocalAssetManager>(); }
        }

        public static IServerInfoService ServerInfo
        {
            get { return ServiceLocator.Current.GetInstance<IServerInfoService>(); }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            CleanupInternal<MainViewModel>();
            CleanupInternal<MovieCollectionViewModel>();
            CleanupInternal<TvCollectionViewModel>();
            CleanupInternal<MusicCollectionViewModel>();
        }

        private static void CleanupInternal<T>() where T : ScottIsAFool.WindowsPhone.ViewModel.ViewModelBase
        {
            var item = ServiceLocator.Current.GetInstance<T>();
            if (item != null)
            {
                item.Cleanup();
            }
        }
    }
}