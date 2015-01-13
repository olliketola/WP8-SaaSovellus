using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp3.Resources;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Services;
using System.Device.Location;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using System.Globalization;




namespace PhoneApp3
{
   

    public partial class MainPage : PhoneApplicationPage
    {

        public string paikkakunta;
        public string phaku;
        public string lampo;
        public string paikka;
        public string tuuli;
        public string ikoni;
        public string ikosteus;
        public string anousu;
        public string alasku;
        public Uri kuvadata;
        public bool valmis;
        


        // Constructor
        public MainPage()
        {
            
            InitializeComponent();
            testi();
          
 
        }

     
        public async void testi()
        {
           
            // Oman paikan hakeminen
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 100;
            Geoposition geoposition = await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(5));
            ReverseGeocodeQuery query = new ReverseGeocodeQuery();
            query.GeoCoordinate = new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);
            lataus.IsEnabled = true;
            lataus.Visibility = Visibility.Visible;
            query.QueryCompleted += (s, ev) =>
            {
                if (ev.Error == null && ev.Result.Count > 0)
                {
                    MapAddress address = ev.Result[0].Information.Address;
                    //t1.Text = "Olet tällä hetkellä : " + address.City + ", " + address.Country;
                    h1.Header = address.City;
                    paikkakunta = address.City;
                    lataus.IsEnabled = false;
                    lataus.Visibility = Visibility.Collapsed;
                    LatausTeksti.Text = "";
                        HaeData();
                        HaeData2();
                        HaeData3();
                    
                }
            };
            query.QueryAsync();

         

        }

        //Sää Tällä hetkellä
        public async Task HaeData()
        {
         


            var client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
            {
               

                if (e.Error == null)
                {
                    
                    KasitteleData(e.Result.ToString());
                  
                  
                }
            };

            client.DownloadStringAsync(new Uri("http://api.openweathermap.org/data/2.5/weather?q=" + paikkakunta + "&mode=xml&units=metric"));
        }


        //Kuluvan päivän tiedot
        public async Task HaeData3()
        {

            var client = new WebClient();
            
            client.DownloadStringAsync(new Uri("http://api.openweathermap.org/data/2.5/forecast/hourly?q=" + paikkakunta + "&mode=xml&units=metric&cnt=1"));
            client.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    KasitteleData3(e.Result.ToString());
                }
                else
                {

                    MessageBox.Show("Virhe haettaessa päivän tietoja:" + e);
                }
            };

        }

        //Viikon säätiedot
        public async Task HaeData2()
        {
            

            string data = "";
            var client = new WebClient();

            client.DownloadStringAsync(new Uri("http://api.openweathermap.org/data/2.5/forecast/daily?q="+paikkakunta+"&mode=xml&units=metric&cnt=7"));
                client.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    data = e.Result.ToString();
                    KasitteleData2(data);
                }
                else
                {

                    MessageBox.Show("Virhe:" + e);
                }
            };

        }


        public void KasitteleData2(string data)
        {
            try
            {
                XDocument xdoc = XDocument.Parse(data);
                string kuvadata;
                List<ViewModel> datalista = new List<ViewModel>();
                var foo = xdoc.Root.Element("forecast").Elements("time").Count();

                for (int i = 0; i < foo; i++)
                {

                    kuvadata = xdoc.Root.Element("forecast").Elements("time").Elements("symbol").ElementAt(i).Attribute("var").Value.ToString();
                    BitmapImage kuva = new BitmapImage(new Uri("http://openweathermap.org/img/w/" + kuvadata + ".png", UriKind.Absolute));

                    datalista.Add(new ViewModel()
                    {
                        pvm = xdoc.Root.Element("forecast").Elements("time").ElementAt(i).Attribute("day").Value.ToString(),
                        temp = xdoc.Root.Element("forecast").Elements("time").Elements("temperature").ElementAt(i).Attribute("day").Value.ToString() + " °C",
                        kuva = kuva
                    });
                }
                lstDemo.ItemsSource = datalista;
            }
            catch (Exception e)
            {
                MessageBox.Show("Virhe2:" + e);
            }
        }

        //Data XML muotoon
        public void KasitteleData(string data)
        {
            try
            {
                XDocument xdoc = XDocument.Parse(data);
                
                paikka = xdoc.Root.Elements().First(node => node.Name.LocalName == "city").Attribute("name").Value;
                lampo = xdoc.Root.Elements().First(node => node.Name.LocalName == "temperature").Attribute("value").Value;
                tuuli = xdoc.Root.Element("wind").Element("speed").Attribute("value").Value;
                ikoni = xdoc.Root.Element("weather").Attribute("icon").Value;
                ikosteus = xdoc.Root.Element("humidity").Attribute("value").Value;
                anousu = xdoc.Root.Element("city").Element("sun").Attribute("rise").Value;
                alasku = xdoc.Root.Element("city").Element("sun").Attribute("set").Value;

                TuntiMuunnos muunna = new TuntiMuunnos();
                string anousu_muunnettu = muunna.MuunnaTunnit(anousu);
                string alasku_muunnettu = muunna.MuunnaTunnit(alasku);

                double num;
                string lt = "";
               var  style = NumberStyles.Float | NumberStyles.AllowThousands;
               var  culture = CultureInfo.InvariantCulture;
                
                bool result = double.TryParse(lampo, style, culture, out num);
              
                if (true == result)
                {
                    
                    lt = Math.Round(num,0).ToString();

                }else{
                    lt = "yolo";
                }



                    kuvadata = new Uri("http://openweathermap.org/img/w/" + ikoni + ".png", UriKind.Absolute);
                    BitmapImage kuva = new BitmapImage(kuvadata);
                    img1.Source = kuva;
                    t1.Text = "Säätiedot paikkakunnalta : " + paikka;
                    t2.Text = lt + "°C";
                    t3.Text = "Tuulen nopeus : " + tuuli + "m/s ";
                    t4.Text = "Ilman kosteus : " + ikosteus + "% ";
                    t5.Text = "Aurinko nousee: " + anousu_muunnettu;
                    t6.Text = "Aurinko laskee : " + alasku_muunnettu;
                
            }
            catch (Exception e)
            {
                MessageBox.Show("Virhe1:" + e);
            }
        }

        public void KasitteleData3(string data)
        {
            TuntiMuunnos muunna = new TuntiMuunnos();
          
            try
            {
                XDocument xdoc = XDocument.Parse(data);
                string kuvadata;
               
                List<PaivanSaa> datalista = new List<PaivanSaa>();
                var foo = xdoc.Root.Element("forecast").Elements("time").Count();
                

                for (int i = 0; i < foo; i++)
                {
                    kuvadata = xdoc.Root.Element("forecast").Elements("time").Elements("symbol").ElementAt(i).Attribute("var").Value.ToString();
                    BitmapImage kuva = new BitmapImage(new Uri("http://openweathermap.org/img/w/" + kuvadata + ".png", UriKind.Absolute));
                 
                    datalista.Add(new PaivanSaa()
                    {
                        time =  muunna.MuunnaAika(  xdoc.Root.Element("forecast").Elements("time").ElementAt(i).Attribute("from").Value.ToString()),
                        temp = xdoc.Root.Element("forecast").Elements("time").Elements("temperature").ElementAt(i).Attribute("value").Value.ToString() + " °C",
                        kuva = kuva
                    });
                }
                lstDemo2.ItemsSource = datalista;
            }
            catch (Exception e)
            {
                MessageBox.Show("Virhe3:" + e);
            }

        }

        //private void paivitaTile(string paikka, string lampo, Uri kuva)
        //{
        //    ShellTile PinnedTile = ShellTile.ActiveTiles.First();
        //    FlipTileData UpdatedTileData = new FlipTileData
        //    {

        //        Title = "SääSovellus",
        //        Count = 0,
        //        BackContent = paikka + " " + lampo + "°C",
        //        SmallBackgroundImage = kuva,
        //        BackgroundImage = kuva,
        //        BackBackgroundImage = kuva,

        //    };
        //    PinnedTile.Update(UpdatedTileData);
        //}

        private void Button_Click1(object sender, RoutedEventArgs e)
        {

            phaku = tb1.Text;
            paikkakunta = phaku;
            HaeData();
            HaeData2();
            HaeData3();
            h1.Header = tb1.Text;
            pivot.SelectedItem = h1;


        }

      

    }
}

    



