using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Storage;

namespace My_Track
{
    public partial class Documents : PhoneApplicationPage
    {
        public Documents()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (local != null)
            {

                try
                {
                    // Get the DataFolder folder.
                    var dataFolder = await local.GetFolderAsync("Tracks");
                    var files = await dataFolder.GetFilesAsync();
                    App.DataModel.Clear();
                    foreach (var file in files)
                    {
                        App.DataModel.Add(new TrackReport() { DateCreated = file.DateCreated.DateTime.ToLongDateString(), FileName = file.Name });
                    }
                    DataContext = App.DataModel;
                    feedbackRoutes.Visibility = Visibility.Collapsed;
                    documentSelector.Visibility = Visibility.Visible;
                }
                catch (Exception)
                {
                    feedbackRoutes.Visibility = Visibility.Visible;
                    documentSelector.Visibility = Visibility.Collapsed;
                }
               
            }

            //base.OnNavigatedTo(e);
        }

        private void documentSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (documentSelector.SelectedIndex!=-1)
	        {
                App.Current.Resources.Add("FileSelected", documentSelector.SelectedIndex);
                NavigationService.GoBack();
                documentSelector.SelectedIndex = -1;
	        }        
        }
    }
}