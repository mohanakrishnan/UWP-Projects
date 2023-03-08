using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;


namespace WatamanEcho
{
    class OpenWeatherMap
    {
        public async static Task<Root> GetWeather(double lat, double lon)
        {
            var data = new Root();
            try { 
                var http = new HttpClient();
                var response = await http.GetAsync("https://api.openweathermap.org/data/2.5/weather?lat=10.45&lon=77.51&units=metric&appid=d2097b9c40836d752e6dd167c6ff20b2");
                var result = await response.Content.ReadAsStringAsync();
                var serializer = new DataContractJsonSerializer(typeof(Root));
               
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
                data = (Root) serializer.ReadObject(ms);
            
            } catch(Exception e)
            {

            }
            return data;
        }
        
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [DataContract]
    public class Clouds
    {

        [DataMember]
        public int all { get; set; }
    }

    [DataContract]
    public class Coord
    {

        [DataMember]
        public double lon { get; set; }

        [DataMember]
        public double lat { get; set; }
    }

    [DataContract]
    public class Main
    {

        [DataMember]
        public double temp { get; set; }

        [DataMember]
        public double feels_like { get; set; }

        [DataMember]
        public double temp_min { get; set; }

        [DataMember]
        public double temp_max { get; set; }

        [DataMember]
        public int pressure { get; set; }

        [DataMember]
        public int humidity { get; set; }

        [DataMember]
        public int sea_level { get; set; }

        [DataMember]
        public int grnd_level { get; set; }
    }

    [DataContract]
    public class Root
    {

        [DataMember]
        public Coord coord { get; set; }

        [DataMember]
        public List<Weather> weather { get; set; }

        [DataMember]
        public string @base { get; set; }

        [DataMember]
        public Main main { get; set; }

        [DataMember]
        public int visibility { get; set; }

        [DataMember]
        public Wind wind { get; set; }

        [DataMember]
        public Clouds clouds { get; set; }

        [DataMember]
        public int dt { get; set; }

        [DataMember]
        public Sys sys { get; set; }

        [DataMember]
        public int timezone { get; set; }

        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public int cod { get; set; }
    }

    [DataContract]
    public class Sys
    {

        [DataMember]
        public string country { get; set; }

        [DataMember]
        public int sunrise { get; set; }

        [DataMember]
        public int sunset { get; set; }
    }

    [DataContract]
    public class Weather
    {

        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string main { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public string icon { get; set; }
    }

    [DataContract]
    public class Wind
    {

        [DataMember]
        public double speed { get; set; }

        [DataMember]
        public int deg { get; set; }

        [DataMember]
        public double gust { get; set; }
    }


}
