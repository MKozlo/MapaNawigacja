using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace MapaNawigacja
{
    internal class DaneGeograficzne
    {
        internal static BasicGeoposition pktStartowy;
        internal static BasicGeoposition pktDocelowy;
        internal static string opisCelu = "";
        internal static bool flagaPoczątku = true;

        internal static async Task<MapRoute> Trasa()
        {
            var startPoint = new Geopoint(pktStartowy);
            var endPoint = new Geopoint(pktDocelowy);
            var routeResult = await MapRouteFinder.GetDrivingRouteAsync(startPoint, endPoint, MapRouteOptimization.Time, MapRouteRestrictions.None);

            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                return routeResult.Route;
            }
            else
            {
                throw new Exception("Nie udało się znaleźć trasy.");
            }
        }
    }
}
