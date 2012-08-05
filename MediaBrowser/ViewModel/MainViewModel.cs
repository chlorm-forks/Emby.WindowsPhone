﻿using GalaSoft.MvvmLight;
using MediaBrowser.Model;
using System.Net;
using Newtonsoft.Json;
using GalaSoft.MvvmLight.Command;
using SharpGIS;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Windows;
using MediaBrowser.Model.Entities;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;

namespace MediaBrowser.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private INavigationService NavService;
        private bool hasLoaded;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(INavigationService navService)
        {
            Folders = new ObservableCollection<ApiBaseItemWrapper<ApiBaseItem>>();
            RecentItems = new ObservableCollection<ApiBaseItemWrapper<ApiBaseItem>>();
            if (IsInDesignMode)
            {
                RecentItems.Add(new ApiBaseItemWrapper<ApiBaseItem> { Item = new ApiBaseItem() { Id = new Guid("2fc6f321b5f8bbe842fcd0eed089561d"), Name = "A Night To Remember" } });
            }
            else
            {
                NavService = navService;                
                WireCommands();
                App.Settings.HostName = "192.168.0.2"; App.Settings.PortNo = "8096";
                App.Settings.LoggedInUser = new Model.Users.User { Id = new Guid("c0eeed038863422d9efc61d4b65506fc") };
                DummyFolder = new ApiBaseItemWrapper<ApiBaseItem>
                                  {
                                      Type= "folder",
                                      Item = new ApiBaseItem
                                                 {
                                                     Name= "recent"
                                                 }
                                  };
            }
        }

        private void WireCommands()
        {
            PageLoaded = new RelayCommand(async () =>
            {
                if (NavService.IsNetworkAvailable && App.Settings.CheckHostAndPort() && !hasLoaded)
                {
                    ProgressIsVisible = true;
                    ProgressText = "Loading folders...";
                    string url = App.Settings.ApiUrl + "item?userid=" + App.Settings.LoggedInUser.Id;
                    string recentUrl = App.Settings.ApiUrl + "recentlyaddeditems?userid=" + App.Settings.LoggedInUser.Id;
                    try
                    {
                        string folderjson = await new GZipWebClient().DownloadStringTaskAsync(url);
                        var item = JsonConvert.DeserializeObject<ApiBaseItemWrapper<ApiBaseItem>>(folderjson);
                        Folders.Clear();
                        item.Children.ToList().ForEach(folder => Folders.Add(folder));
                    }
                    catch {}
                    try
                    {
                        string recentjson = await new GZipWebClient().DownloadStringTaskAsync(recentUrl);
                        var recent = JsonConvert.DeserializeObject<List<ApiBaseItemWrapper<ApiBaseItem>>>(recentjson);
                        RecentItems.Clear();
                        recent.OrderBy(x => x.Item.DateCreated).Take(6).ToList().ForEach(recentItem => RecentItems.Add(recentItem));
                    }
                    catch (Exception ex)
                    {
                        App.ShowMessage("", "Error connecting to service");
                    }
                    
                    ProgressIsVisible = false;
                    hasLoaded = true;
                }
            });

            NavigateToPage = new RelayCommand<ApiBaseItemWrapper<ApiBaseItem>>(NavService.NavigateTopage);
        }

        // UI properties
        public bool ProgressIsVisible { get; set; }
        public string ProgressText { get; set; }

        public RelayCommand PageLoaded { get; set; }
        public RelayCommand<ApiBaseItemWrapper<ApiBaseItem>> NavigateToPage { get; set; }
        public ObservableCollection<ApiBaseItemWrapper<ApiBaseItem>> Folders { get; set; }
        public ObservableCollection<ApiBaseItemWrapper<ApiBaseItem>> RecentItems { get; set; }
        public ApiBaseItemWrapper<ApiBaseItem> DummyFolder { get; set; }
    }
}