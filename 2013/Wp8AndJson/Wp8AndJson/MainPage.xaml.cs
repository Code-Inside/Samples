using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using Windows.Storage;

namespace Wp8AndJson
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateNewEntryButton.IsEnabled = false;
            try
            {
                var result = await CreateNewEntry();
            }
            finally
            {
                CreateNewEntryButton.IsEnabled = true;
            }
        }

        private async Task<List<Data>> CreateNewEntry()
        {
            Data demo = new Data();
            demo.Id = Guid.NewGuid();
            demo.Value = Guid.NewGuid().ToString();

            var existing = await Load();

            if (existing == null)
            {
                existing = new List<Data>();
            }

            existing.Add(demo);

            await Save(existing);

            return existing;
        }

        private async Task Save(List<Data> data)
        {
            IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder;

            IStorageFolder storageFolder = await applicationFolder.CreateFolderAsync("data", CreationCollisionOption.OpenIfExists);

            string fileName = "data.json";

            IStorageFile storageFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);

            using (Stream stream = await storageFile.OpenStreamForWriteAsync())
            using (var sw = new StreamWriter(stream))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;

                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jw, data);
                await stream.FlushAsync();
            }
        }

        private async Task<List<Data>> Load()
        {
            IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder;

            IStorageFolder storageFolder = await applicationFolder.CreateFolderAsync("data", CreationCollisionOption.OpenIfExists);

            string fileName = "data.json";

            IStorageFile storageFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);

            using (Stream stream = await storageFile.OpenStreamForReadAsync())
            using (var sr = new StreamReader(stream))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                var result = serializer.Deserialize<List<Data>>(jr);
                jr.Close();
                sr.Dispose();
                stream.Dispose();

                return result;
            }
        }
    }

    public class Data
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
    }
}