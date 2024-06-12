using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Services.Maps;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MapaNawigacja
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void powMape(object sender, RoutedEventArgs e)
        {
            mojaMapa.ZoomLevel++;
        }

        private void pomMape(object sender, RoutedEventArgs e)
        {
            mojaMapa.ZoomLevel--;
        }

        private void trybMapy(object sender, RoutedEventArgs e)
        {
            var aBB = sender as AppBarButton;

            // jak zmienić:     <FontIcon   FontFamily="auto" Glyph="S" />
            var fIcon = new FontIcon()
            {
                FontFamily = FontFamily.XamlAutoFontFamily // należy pamiętać, że tu nie ma średnika!
            };

            if (mojaMapa.Style == Windows.UI.Xaml.Controls.Maps.MapStyle.AerialWithRoads)
            {
                mojaMapa.Style = Windows.UI.Xaml.Controls.Maps.MapStyle.Road;
                aBB.Label = "satelita";
                fIcon.Glyph = "S";
            }
            else
            {
                mojaMapa.Style = Windows.UI.Xaml.Controls.Maps.MapStyle.AerialWithRoads;
                aBB.Label = "mapa";
                fIcon.Glyph = "M";
            }
            aBB.Icon = fIcon;
        }

        private void koordynaty(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Koordynaty));
        }



        // zanim strona stanie się widoczna
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DaneGeograficzne.flagaPoczątku)
            {
                return;
            }

            try
            {
                var znacznikStartu = new MapIcon()
                {
                    Title = "Tu jestem",
                    Location = new Windows.Devices.Geolocation.Geopoint(DaneGeograficzne.pktStartowy)
                };
                mojaMapa.MapElements.Add(znacznikStartu);

                var znacznikDocelowy = new MapIcon()
                {
                    Title = "Cel podróży",
                    Location = new Windows.Devices.Geolocation.Geopoint(DaneGeograficzne.pktDocelowy)
                };
                mojaMapa.MapElements.Add(znacznikDocelowy);

                var route = await DaneGeograficzne.Trasa();

                var trasaLotem = new MapPolyline()
                {
                    StrokeColor = Windows.UI.Colors.Black,
                    StrokeThickness = 3,
                    StrokeDashed = true,
                    Path = new Geopath(route.Path.Positions)
                };
                mojaMapa.MapElements.Add(trasaLotem);

                await mojaMapa.TrySetViewBoundsAsync(route.BoundingBox, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);

                double totalDistance = route.LengthInMeters / 1000;
                var estimatedDuration = route.EstimatedDuration;

                var maneuvers = "Trzy początkowe manewry:\n";
                int maneuverCount = 0;
                foreach (var leg in route.Legs)
                {
                    foreach (var maneuver in leg.Maneuvers.Take(3))
                    {
                        maneuverCount++;
                        maneuvers += $"{maneuverCount}. {maneuver.InstructionText}\n";
                    }
                    break;
                }

                var dialog = new ContentDialog()
                {
                    Title = "Informacje o trasie",
                    Content = $"Przewidywany łączny czas przejazdu: {estimatedDuration}\nDystans: {totalDistance:F2} km\n\n{maneuvers}",
                    CloseButtonText = "Zamknij",
                    Background = new SolidColorBrush(Windows.UI.Colors.LightBlue),
                    FontSize = 16,
                    FontWeight = Windows.UI.Text.FontWeights.Bold
                };
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog($"Wystąpił błąd: {ex.Message}");
                await dialog.ShowAsync();
            }
        }
    }
}