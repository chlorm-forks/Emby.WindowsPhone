﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Devices;
using MediaBrowser.Model.Net;
using Microsoft.Xna.Framework.Media;
using ScottIsAFool.WindowsPhone.Logging;

namespace Emby.WindowsPhone.Model
{
    public class Device : IDevice
    {
        private readonly static ILog Log = new WPLogger(typeof(Device));

        public async Task<IEnumerable<LocalFileInfo>> GetLocalPhotos()
        {
            var items = GetPhotos(library => library.Pictures);
            Log.Info("Photos found: {0}", items.Count());
            return items;
        }

        private IEnumerable<LocalFileInfo> GetPhotos(Func<MediaLibrary, PictureCollection> albumFunc)
        {
            using (var mediaLibrary = new MediaLibrary())
            {
                var pictureLibrary = albumFunc.Invoke(mediaLibrary);
                var pictures = pictureLibrary.Where(CanUpload).Select(x => new LocalFileInfo
                {
                    Name = x.Name,
                    Album = x.Album != null ? x.Album.Name : string.Empty,
                    Id = string.Format("{0}.{1}", x.Album != null ? x.Album.Name : string.Empty, x.Name),
                    MimeType = GetMimeType(x)
                }).ToList();

                return pictures;
            }
        }

        private bool CanUpload(Picture x)
        {
            if (UploadAll)
            {
                return true;
            }

            return x.Date.Date >= AfterDateTime.Date;
        }

        private static string GetMimeType(Picture x)
        {
            var ext = Path.GetExtension(x.Name);
            return "image/" + (string.IsNullOrEmpty(ext) ? "jpeg" : ext.Replace(".", string.Empty));
        }

        public async Task<IEnumerable<LocalFileInfo>> GetLocalVideos()
        {
            return new List<LocalFileInfo>();
        }

        public async Task UploadFile(LocalFileInfo file, IApiClient apiClient, CancellationToken cancellationToken)
        {
            using (var mediaLibrary = new MediaLibrary())
            {
                var picture = mediaLibrary.Pictures.FirstOrDefault(x => (x.Album == null || x.Album.Name == file.Album) && x.Name == file.Name);

                if (picture != null)
                {
                    Log.Info("Uploading file '{0}'", file.Id);
                    using (var stream = picture.GetImage())
                    {
                        try
                        {
                            await apiClient.UploadFile(stream, file, cancellationToken);
                        }
                        catch (HttpException ex)
                        {
                            Log.ErrorException("Uploadfile(" + file.Id + ")", ex);
                        }
                    }
                }
            }
        }

        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public bool UploadAll { get; set; }
        public DateTime AfterDateTime { get; set; }
        public event EventHandler<EventArgs> ResumeFromSleep;
    }
}
