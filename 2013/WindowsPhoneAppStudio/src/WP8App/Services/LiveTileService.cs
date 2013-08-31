using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Phone.Shell;
using WPAppStudio.Services.Interfaces;
using WPAppStudio.Entities.Base;
using WPAppStudio.Localization;

namespace WPAppStudio.Services
{
    /// <summary>
    /// Implementation of a Live Tile service.
    /// </summary>
    public class LiveTileService : ILiveTileService
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        private const string LiveTileFolder = "/shared/shellcontent/";
        private const string ResourcePath = "Images/";
        private const string IsoStorePreffix = "isostore:";

        public LiveTileService(IDialogService dialogService, INavigationService navigationService)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;
        }

        /// <summary>
        /// Pins a tile to start page.
        /// </summary>
        /// <param name="viewModelType">The source view-model.</param>
        /// <param name="tileInfo">The tile information.</param>
        public void PinToStart(Type viewModelType, TileInfo tileInfo)
        {
            var uri = _navigationService.GetUri(viewModelType);
            uri += string.Format("?currentID={0}", HttpUtility.UrlEncode(tileInfo.CurrentId) ?? string.Empty);
			if (TileExists(uri))
                UpdateTile(uri, tileInfo);
            else
                CreateTile(uri, tileInfo);
        }

        private async void CreateTile(string navSource, TileInfo tileInfo)
        {
            await CheckRemoteImages(tileInfo);
            var tileData = GetStandardTileData(tileInfo);

            ShellTile.Create(new Uri(navSource, UriKind.Relative), tileData);
        }

        private async void UpdateTile(string uri, TileInfo tileInfo)
        {
            await CheckRemoteImages(tileInfo);
            var tileData = GetStandardTileData(tileInfo);

            var activeTile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri.ToString().Contains(uri));
            if (activeTile != null)
                activeTile.Update(tileData);
        }

        private static StandardTileData GetStandardTileData(TileInfo tileInfo)
        {
            var tileData = new StandardTileData
            {
                Title = HtmlUtil.CleanHtml(tileInfo.Title) ?? string.Empty,
                Count = tileInfo.Count,
                BackTitle = HtmlUtil.CleanHtml(tileInfo.BackTitle) ?? string.Empty,
                BackContent = HtmlUtil.CleanHtml(tileInfo.BackContent) ?? string.Empty
            };
            if (!string.IsNullOrEmpty(tileInfo.BackgroundImagePath))
                tileData.BackgroundImage = new Uri(tileInfo.BackgroundImagePath, UriKind.RelativeOrAbsolute);
            if (!string.IsNullOrEmpty(tileInfo.BackgroundImagePath))
                tileData.BackBackgroundImage = new Uri(tileInfo.BackBackgroundImagePath, UriKind.RelativeOrAbsolute);
            return tileData;
        }

        private bool TileExists(string navSource)
        {
            var tile = ShellTile.ActiveTiles.FirstOrDefault(o => o.NavigationUri.ToString().Contains(navSource));
            return tile != null;
        }

        private async Task CheckRemoteImages(TileInfo tileInfo)
        {
            if (!string.IsNullOrEmpty(tileInfo.BackBackgroundImagePath))
                tileInfo.BackBackgroundImagePath = await SetTileImagePath(tileInfo, tileInfo.BackBackgroundImagePath);
            if (!string.IsNullOrEmpty(tileInfo.BackgroundImagePath))
                tileInfo.BackgroundImagePath = await SetTileImagePath(tileInfo, tileInfo.BackgroundImagePath);
        }
		
		private async Task<string> SetTileImagePath(TileInfo tileInfo, string image)
        {
            try
            {
                var transferUri = new Uri(Uri.EscapeUriString(image), UriKind.RelativeOrAbsolute);
                if (transferUri.ToString().StartsWith("http"))
                {
                    var localUri = new Uri(LiveTileFolder + Path.GetFileName(transferUri.LocalPath), UriKind.RelativeOrAbsolute);
                    await DownloadFile(transferUri, localUri);
					return IsoStorePreffix + localUri;                    
                }
                return ResourcePath +
                       Path.GetFileName(!string.IsNullOrEmpty(image) ? image : tileInfo.LogoPath);
            }
            catch (Exception ex)
            {
				Debug.WriteLine("{0} {1}{2}", AppResources.TileError, AppResources.Error, ex.ToString());
                return string.Empty;
            }
        }

        private async Task DownloadFile(Uri transferUri, Uri localUri)
        {
            var request = WebRequest.CreateHttp(transferUri);
            var task = Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse,
                request.EndGetResponse,
                null);
            await task.ContinueWith(t =>
            {
                using (var responseStream = task.Result.GetResponseStream())
                {
                    using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        try
                        {
                            using (var isoStoreFile = isoStore.OpenFile(localUri.ToString(),
                                FileMode.Create,
                                FileAccess.ReadWrite))
                            {
                                var dataBuffer = new byte[1024];
                                while (responseStream.Read(dataBuffer, 0, dataBuffer.Length) > 0)
                                {
                                    isoStoreFile.Write(dataBuffer, 0, dataBuffer.Length);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("{0} {1} from: {2}. {3}{4}", AppResources.TileError, localUri, transferUri, AppResources.Error, ex);
                        }
                    }
                }
            });
        }
    }
}