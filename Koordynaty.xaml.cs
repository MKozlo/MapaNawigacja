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
//
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MapaNawigacja
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Koordynaty : Page
    {
        public Koordynaty()
        {
            this.InitializeComponent();
            DaneGeograficzne.flagaPoczątku = true;
            GdzieJestem();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        async void GdzieJestem()
        {
            var mojGPS = new Geolocator();
            mojGPS.DesiredAccuracy = PositionAccuracy.High;
            var mojeZGPSa = await mojGPS.GetGeopositionAsync();
            double dlg = mojeZGPSa.Coordinate.Point.Position.Longitude; // dlg = dlugosc
            double szer = mojeZGPSa.Coordinate.Point.Position.Latitude; // szer = szerokosc
            // teraz do textBlock
            tbGPS.Text = string.Format("dłg.: {0,10:f4}, szer.: {1,10:f4}", dlg, szer);
            DaneGeograficzne.pktStartowy = mojeZGPSa.Coordinate.Point.Position;
        }

        private async void btSzukajCelu_Click(object sender, RoutedEventArgs e)
        {
            var gdzieJedziemy = await MapLocationFinder.FindLocationsAsync(txAdres.Text, new Geopoint(DaneGeograficzne.pktStartowy));

            if (gdzieJedziemy != null && gdzieJedziemy.Locations.Count > 0)
            {
                // jest wynik
                var lokalizacja = gdzieJedziemy.Locations[0];
                double dlg = lokalizacja.Point.Position.Longitude;
                double szer = lokalizacja.Point.Position.Latitude;
                tbDlg.Text = string.Format("{0,10:f4}", dlg);
                tbSzer.Text = string.Format("{0,10:f4}", szer);
                DaneGeograficzne.pktDocelowy = lokalizacja.Point.Position;
                DaneGeograficzne.opisCelu = txAdres.Text;
                DaneGeograficzne.flagaPoczątku = false;
            }
            else
            {
                // nic nie znaleziono lub błąd
                var dialog = new MessageDialog("Wystąpił błąd. / Nie znaleziono lokalizacji.");
                await dialog.ShowAsync();
            }
        }
    }
}
