using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace BingMapApp.Common
{
    public class Library
    {
        // Obtiene posición actual
        public async Task<Geopoint> Position() {
            return (await new Geolocator().GetGeopositionAsync()).Coordinate.Point;
        }

        // Dibuja Marcador en posición obtenida
        public UIElement Marker ()
        {
            Canvas marker = new Canvas();
            Ellipse outerEllipse = new Ellipse();
            outerEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            outerEllipse.Margin = new Thickness(-10, -10, 0, 0);
            Ellipse innerEllipse = new Ellipse() { Width = 30, Height = 30 };
            innerEllipse.Margin = new Thickness(-12, -12, 0, 0);
            Ellipse coreEllipse = new Ellipse() { Width = 10, Height = 10 };
            coreEllipse.Fill = new SolidColorBrush(Colors.Red);
            coreEllipse.Margin = new Thickness(-5, -5, 0, 0);
            marker.Children.Add(outerEllipse);
            marker.Children.Add(innerEllipse);
            marker.Children.Add(coreEllipse);

            return marker;
        }

        // Dibuja texto de marcador
        public UIElement MarkerText (string Text)
        {
            Canvas marker = new Canvas();
            TextBlock text = new TextBlock();
            text.Foreground = new SolidColorBrush(Colors.Red);
            marker.Background = new SolidColorBrush(Colors.Black);
            text.FontSize = 16;
            text.Text = Text;
            marker.Children.Add(text);
            return marker;
        }
    }
}
