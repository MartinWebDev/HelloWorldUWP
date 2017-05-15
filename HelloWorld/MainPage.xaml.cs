using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Extra usings
using Windows.Devices.Geolocation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HelloWorld
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Geolocator geolocator;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ClickMeButton_Click(object sender, RoutedEventArgs e)
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    txtGeolocationPermissionStatus.Text = "Permission Granted";
                    txtPosition.Text = "Waiting for update...";

                    geolocator = new Geolocator { DesiredAccuracyInMeters = 20, ReportInterval = 1000 };

                    geolocator.StatusChanged += StatusChanged;
                    geolocator.PositionChanged += PositionChanged;

                    Geoposition pos = await geolocator.GetGeopositionAsync();

                    UpdateLocationData(pos);
                    break;
                case GeolocationAccessStatus.Denied:
                    txtGeolocationPermissionStatus.Text = "Permission denied by user!";
                    txtPosition.Text = "Cannot get position!";

                    UpdateLocationData(null);
                    break;
                case GeolocationAccessStatus.Unspecified:
                    txtGeolocationPermissionStatus.Text = "Unspecified error!";
                    txtPosition.Text = "Cannot get position!";
                    break;
            }
        }

        private void PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            UpdateLocationData(args.Position);
        }

        private async void UpdateLocationData(Geoposition pos)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (pos != null)
                {
                    txtPosition.Text = string.Format("Position: {0}, {1}", pos.Coordinate.Point.Position.Latitude.ToString(), pos.Coordinate.Point.Position.Longitude.ToString());
                }
                else
                {
                    txtPosition.Text = "Position was null!";
                }
            });
        }

        private async void StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
            {
                switch (args.Status)
                {
                    case PositionStatus.Ready:
                        txtGeolocationStatus.Text = "Ready";
                        break;
                    case PositionStatus.Initializing:
                        txtGeolocationStatus.Text = "Initialising...";
                        break;
                    case PositionStatus.NoData:
                        txtGeolocationStatus.Text = "No data recieved!";
                        break;
                    case PositionStatus.Disabled:
                        txtGeolocationStatus.Text = "Disabled!";
                        UpdateLocationData(null);
                        break;
                    case PositionStatus.NotInitialized:
                        txtGeolocationStatus.Text = "Not initialised";
                        break;
                    case PositionStatus.NotAvailable:
                        txtGeolocationStatus.Text = "Location not available on this device";
                        break;
                    default:
                        txtGeolocationStatus.Text = "Unknown!";
                        break;
                }
            });
        }

        private async void btnUpdatePosition_Click(object sender, RoutedEventArgs e)
        {
            UpdateLocationData(await geolocator.GetGeopositionAsync());
        }

        private async void btnAccessLocationSettings_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
        }
    }
}
