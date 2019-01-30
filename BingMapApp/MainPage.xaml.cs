using BingMapApp.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Storage.Streams;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Popups;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace BingMapApp
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {
        public Library Library = new Library();
        private int indexPosition = 1;
        private Geopoint myPoint;
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //base.OnNavigatedTo(e);
            myPoint = await Library.Position();
            myMap.MapServiceToken = "dJ22pIV4NdbmraunFP7b~_IZL-4m9DzLM4vXxLxCzbQ~AoBMOdQDuqq5Dbhq6vN5ZpyXy5_33mP4z3q0y_HkAYqI6drveh7wz93Vz_Xje9BC";
            myMap.ZoomLevel = 16;
            myMap.Center = myPoint;
            addIconToLocation(myPoint, "Posición: " + indexPosition);
            indexPosition++;

        }
        private void addIconToLocation (Geopoint location, String name)
        {
            MapIcon mapIcon = new MapIcon();
            mapIcon.Location =location;
            mapIcon.Title = String.Format("{0}\nLatLng:{1}\nLongLng:{2}", name, location.Position.Latitude, location.Position.Longitude);
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/location.png"));
            mapIcon.ZIndex = 0;
            myMap.MapElements.Add(mapIcon);
        }

        private async void ShowRouteOnMap(Geopoint startPoint, Geopoint endPoint)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(
                startPoint,
                endPoint,
                MapRouteOptimization.Time,
                MapRouteRestrictions.None);

            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                myMap.Routes.Clear(); // Retira del mapa rutas previamente dibujadas
                MapRouteView routeView = new MapRouteView(routeResult.Route);
                routeView.RouteColor = Colors.Blue;
                routeView.OutlineColor = Colors.Gray;
                myMap.Routes.Add(routeView);
                await myMap.TrySetViewBoundsAsync(routeResult.Route.BoundingBox, null, MapAnimationKind.None);
            }
            else
            {
                var message = new MessageDialog("No se puede encontrar ninguna ruta");
                await message.ShowAsync();
            }
        }

        // Presenta la ruta más rápida desde la ubicación actual del usuario hasta donde éste ha hecho "tap" dentro del mapa
        private async void AppBarButton_Click(Object sender, RoutedEventArgs e)
        {
            Geopoint position = await Library.Position();
            DependencyObject marker = Library.Marker();
            myMap.Children.Add(marker);
            MapControl.SetLocation(marker, position);
            MapControl.SetNormalizedAnchorPoint(marker, new Point(0.5, 0.5));
            myMap.ZoomLevel = 12;
            myMap.Center = position;
        }

        private async void myMap_Tapped(MapControl sender, MapInputEventArgs args)
        {
            addIconToLocation(args.Location, "Position: " + indexPosition);
            indexPosition++;
            Geopoint startPoint = await Library.Position();
            ShowRouteOnMap(startPoint, args.Location);

        }

        private async void Buscador_Click(object sender, RoutedEventArgs e)
        {
            MapService.ServiceToken = "dJ22pIV4NdbmraunFP7b~_IZL-4m9DzLM4vXxLxCzbQ~AoBMOdQDuqq5Dbhq6vN5ZpyXy5_33mP4z3q0y_HkAYqI6drveh7wz93Vz_Xje9BC";
            string addressToGeocode = AddressInput.Text;
            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(
                           addressToGeocode,
                           myPoint,
                           1);
            if (result.Status == MapLocationFinderStatus.Success)
            {
                BasicGeoposition targetLocation = new BasicGeoposition();
                targetLocation.Longitude = double.Parse(result.Locations[0].Point.Position.Longitude.ToString());
                targetLocation.Latitude = double.Parse(result.Locations[0].Point.Position.Latitude.ToString());
                Geopoint targetPosition = new Geopoint(targetLocation);
                this.ShowRouteOnMap(myPoint, targetPosition);
                addIconToLocation(targetPosition, "Destino: " + addressToGeocode);
            }
        }
    }
}
