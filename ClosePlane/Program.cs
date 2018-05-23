using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json.Linq;


namespace ClosePlaneNM {

    class ClosePlane {

        static HttpClient client = new HttpClient();

        static void Main() {

            getParseAndDisplay();
            Console.ReadKey();
        }

        async static void getParseAndDisplay(){

            string URL = "https://opensky-network.org/api/states/all?";

            Console.WriteLine("Enter a latitude, positive and negative values represent N and S, respectively");
            double latitude = Double.Parse(Console.ReadLine());
            Console.WriteLine("Enter a longitude, positive and negative values represent E and W, respectively");
            double longitude = Double.Parse(Console.ReadLine());

            string lomin = (longitude - 1.5).ToString();
            string lomax = (longitude + 1.5).ToString();
            string lamin = (latitude - 1.5).ToString();
            string lamax = (latitude + 1.5).ToString();

            URL = URL + "lamin=" + lamin + "&lomin=" + lomin + "&lamax=" + lamax + "&lomax=" + lomax;

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(URL);
            HttpContent content = response.Content;

            string apiContent = await content.ReadAsStringAsync();

            var jo = JObject.Parse(apiContent);
            //Console.WriteLine(jo["states"]);

            var tupList = new List<Tuple<double, JArray>>();

            foreach (JArray plane in jo["states"]){
                double long1 = Double.Parse(plane[5].ToString());
                double lat1 = Double.Parse(plane[5].ToString());
                var tup = Tuple.Create<double, JArray>(eucDistance(long1, lat1, longitude, latitude), plane);
                tupList.Add(tup);     
            }

            tupList = tupList.OrderBy(i => i.Item1).ToList();

            if (tupList.Count > 0)
            {
                JArray dispPlane = tupList[0].Item2;

                Console.WriteLine("Displaying info for closest plane to Latitude -> " + latitude.ToString() + " and Longitude -> " + longitude.ToString());
                Console.WriteLine();
                Console.WriteLine("Aircraft Callsign: " + dispPlane[1].ToString());
                Console.WriteLine("Country of Origin: " + dispPlane[2].ToString());
                Console.WriteLine("Altitude: " + dispPlane[7].ToString());
                Console.WriteLine("Velocity: " + dispPlane[9].ToString());
                Console.WriteLine("Heading: " + dispPlane[10].ToString());
                Console.WriteLine("Latitude: " + dispPlane[6].ToString());
                Console.WriteLine("Longitude: " + dispPlane[5].ToString());
            }

            else
            {
                Console.WriteLine("No Planes were found near Latitude -> " + latitude.ToString() + " and Longitude -> " + longitude.ToString());
            }

        }

        static double eucDistance(double x1, double y1, double x2, double y2){

            double dist = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            return dist;
        }
        



    }

}

