using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using My_Track.Resources;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using System.Device.Location;
using System.Windows.Media;
using Microsoft.Phone.Maps.Toolkit;
using System.IO;
using System.IO.IsolatedStorage;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;

namespace My_Track
{
    public partial class MainPage : PhoneApplicationPage
    {
        RouteQuery routeQuery = new RouteQuery();
        List<GeoCoordinate> waypoints = new List<GeoCoordinate>();
        MapRoute myMapRoute;

        private const float KM_METERS = 0.001f;

        GeoCoordinateWatcher gcw;
        bool isRunning = false;
        int count = 0;

        MapPolyline polyline = new MapPolyline() { StrokeThickness = 5};
        MapLayer UserLayer = new MapLayer();
        List<GeoPosition<GeoCoordinate>> positions = new List<GeoPosition<GeoCoordinate>>();

        StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

        public MainPage()
        {
            InitializeComponent();
            RouteDetails.DataContext = this;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.Current.Resources.Contains("FileSelected"))
            {
                RouteDetails.Visibility = System.Windows.Visibility.Visible;
                int selection = Convert.ToInt32(App.Current.Resources["FileSelected"].ToString());
                await OpenFile(App.DataModel[selection].FileName);
                routeQuery.TravelMode = TravelMode.Driving;
                routeQuery.QueryCompleted += rq_QueryCompleted;
                routeQuery.Waypoints = waypoints;
                routeQuery.QueryAsync();
                App.Current.Resources.Remove("FileSelected");
            }
            else
            {
                RouteDetails.Visibility = System.Windows.Visibility.Collapsed;
                if (gcw == null)
                {
                    TrackMap.MapElements.Clear();
                    TrackMap.ColorMode = MapColorMode.Light;
                    TrackMap.CartographicMode = MapCartographicMode.Road;
                    TrackMap.LandmarksEnabled = true;
                    TrackMap.PedestrianFeaturesEnabled = true;
                    TrackMap.Center = new GeoCoordinate(11.0134, -74.8023);
                    TrackMap.ZoomLevel = 16;
                    TrackMap.Layers.Add(UserLayer);

                    gcw = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                    gcw.PositionChanged += gcw_PositionChanged;
                    gcw.StatusChanged += gcw_StatusChanged;

                    if (!App.InBackground)
                    {
                        
                        TrackMap.MapElements.Add(polyline);
                    }
                }
            }
        }

        private void gcw_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            if (e.Status == GeoPositionStatus.Ready)
            {
                TrackMap.Center = gcw.Position.Location;
                polyline.Path.Clear();
                Mileage = 0;
                Runtime = TimeSpan.Zero;
            }
        }

        private void gcw_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (!waypoints.Contains(e.Position.Location))
            {
                Dispatcher.BeginInvoke(() => CenterLocation(e.Position.Location));
                count++;

                polyline.Path.Add(e.Position.Location);

                positions.Add(e.Position);
                waypoints.Add(e.Position.Location);
            }
        }

        private void rq_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                if (myMapRoute!=null)
                {
                    TrackMap.RemoveRoute(myMapRoute);
                }
                Route myRoute = e.Result;
                myMapRoute = new MapRoute(myRoute);
                TrackMap.AddRoute(myMapRoute);


                TimeSpan timeDiff = positions.Last().Timestamp.TimeOfDay - positions.First().Timestamp.TimeOfDay;
                TrackMap.Center = positions.First().Location;
                Mileage = float.Parse(e.Result.LengthInMeters.ToString()) * KM_METERS;
                Runtime = timeDiff;
            }
            else
            {
                MessageBox.Show("An error has ocurred. Details: " + e.Error.Message);
            }
        }

        private void TrackButtonClick(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            ApplicationBarIconButton btnSave = (ApplicationBarIconButton)ApplicationBar.Buttons[1];

            if (!isRunning)
            {
                btnSave.IsEnabled = false;
                btn.Text = "Stop";
                btn.IconUri = new Uri("/Assets/AppBar/transport.pause.png", UriKind.RelativeOrAbsolute);
                gcw.Start();
                polyline.Path.Clear();
                waypoints.Clear();
                positions.Clear();
                Mileage = 0;
                Runtime = TimeSpan.Zero;
                isRunning = true;
            }
            else
            {
                btnSave.IsEnabled = true;
                btn.Text = "Record";
                btn.IconUri = new Uri("/Assets/AppBar/transport.play.png", UriKind.RelativeOrAbsolute);
                gcw.Stop();
                isRunning = false;
            }
            

        }

        private async void SaveFileButtonClick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            String FileName = "Track " + now.Day + "-" + now.Month + "-" + now.Year + "_" + now.TimeOfDay.Hours + "-" + now.TimeOfDay.Minutes+".txt";
            await WriteFile(FileName);
            ApplicationBarIconButton btnSave = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            btnSave.IsEnabled = false;
        }

        private async Task WriteFile(String FileName)
        {
            var dataFolder = await local.CreateFolderAsync("Tracks", CreationCollisionOption.OpenIfExists);
            var file = await dataFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

            using (StreamWriter writer = new StreamWriter(file.Path))
            {
                foreach (var item in positions)
                {
                    writer.WriteLine(item.Location.Latitude + ";" + item.Location.Longitude + ";" + item.Timestamp);
                }
                writer.Close();
            }
        }

        private void OpenDocuments(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Documents.xaml", UriKind.RelativeOrAbsolute));
        }

        private void CenterLocation(GeoCoordinate gc)
        {
            UserLayer.Clear();
            UserLayer.Add(new MapOverlay() { GeoCoordinate = gc, Content = new UserLocationMarker() });
            TrackMap.Center = gc;
            TrackMap.Heading = gc.Course;             
        }

        private async Task OpenFile(String FilePath)
        {
            var dataFolder = await local.CreateFolderAsync("Tracks", CreationCollisionOption.OpenIfExists);
            var FileCheck = await dataFolder.OpenStreamForReadAsync(FilePath);
            if (FileCheck!=null)
            {
                waypoints.Clear();
                using (StreamReader reader = new StreamReader(FileCheck))
                {
                    do
                    {
                        String[] Coordinate = reader.ReadLine().Split(';');
                        double latitude = Convert.ToDouble(Coordinate[0]);
                        double longitude = Convert.ToDouble(Coordinate[1]);
                        DateTimeOffset TimeStampLoc = DateTimeOffset.Parse(Coordinate[2]);
                        waypoints.Add(new GeoCoordinate(latitude,longitude));
                        positions.Add(new GeoPosition<GeoCoordinate>() { Location = new GeoCoordinate(latitude, longitude), Timestamp = TimeStampLoc });
                    } while (reader.Peek()!=-1);
                    reader.Close();
                }
            }
        }


        //Distance and Runtime Dependency Properties.

        public float Mileage
        {
            get { return (float)GetValue(MileageProperty); }
            set { SetValue(MileageProperty, value); }
        }

        public static readonly DependencyProperty MileageProperty =
            DependencyProperty.Register("Mileage", typeof(float), typeof(MainPage), new PropertyMetadata(0.0f));


        public TimeSpan Runtime
        {
            get { return (TimeSpan)GetValue(RuntimeProperty); }
            set { SetValue(RuntimeProperty, value); }
        }

        public static readonly DependencyProperty RuntimeProperty =
            DependencyProperty.Register("Runtime", typeof(TimeSpan), typeof(MainPage), new PropertyMetadata(TimeSpan.Zero));
    }
}