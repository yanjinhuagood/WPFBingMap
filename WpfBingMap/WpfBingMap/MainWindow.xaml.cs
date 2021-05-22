using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfBingMap
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private LocationCollection _polyLocations;
        private MapPolyline mapPolyline;
        private Pushpin carPushpin;
        private TranslateTransform translateTransform;

        public IEnumerable PushpinArray
        {
            get { return (IEnumerable)GetValue(PushpinArrayProperty); }
            set { SetValue(PushpinArrayProperty, value); }
        }

        public static readonly DependencyProperty PushpinArrayProperty =
            DependencyProperty.Register("PushpinArray", typeof(IEnumerable), typeof(MainWindow), new PropertyMetadata(null));


        public MainWindow()
        {
            InitializeComponent();
            //this.map.Mode = new MercatorMode();
            //this.map.Children.Add(new AMapTitleLayer());
            //this.map.MouseDown += Map_MouseDown;
            var pushpins = new List<PushpinModel>();
            pushpins.Add(new PushpinModel { ID=1, Location = new Location(39.8151940395589, 116.411970893135),Title="和义东里社区" });
            pushpins.Add(new PushpinModel { ID = 2, Location = new Location(39.9094878843105, 116.33299936282) ,Title="中国水科院南小区"});
            pushpins.Add(new PushpinModel { ID = 3, Location = new Location(39.9219204792284, 116.203500574855),Title="石景山山姆会员超市" });
            pushpins.Add(new PushpinModel { ID = 4, Location = new Location(39.9081417418219, 116.331244439925), Title = "茂林居小区" });
            PushpinArray = pushpins;

             _polyLocations = new LocationCollection();
            _polyLocations.Add(new Location(39.9082973053021, 116.63105019548));
            _polyLocations.Add(new Location(39.9155572462212, 116.192505993178));
            _polyLocations.Add(new Location(39.8065773542251, 116.276113341099));

            mapPolyline = new MapPolyline 
            {
                Stroke = Brushes.Green,
                StrokeThickness = 2,
                Locations = _polyLocations,
            };
            CarLayer.Children.Add(mapPolyline);
            carPushpin = new Pushpin
            {
                Template = this.Resources["CarTemplate"] as ControlTemplate,
                Location = _polyLocations[0],
                PositionOrigin = PositionOrigin.Center,
                RenderTransformOrigin = new Point(0.5,0.5)
            };
            translateTransform = new TranslateTransform();
            carPushpin.RenderTransform = translateTransform;
            CarLayer.Children.Add(carPushpin);
        }
        private void BtnCar_Click(object sender, RoutedEventArgs e)
        {
            //var storyboard = new Storyboard();
            var doubleAnimation = new DoubleAnimation()
            {
                From = _polyLocations[0].Longitude,
                To = _polyLocations[1].Longitude,
                Duration = new Duration(TimeSpan.FromSeconds(1.5))
            };
            //storyboard.Children.Add(doubleAnimation);
            //Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            //carPushpin.BeginAnimation(, doubleAnimation);
            //translateTransform.BeginAnimation();

        }
        private void Map_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);
            Location pinLocation = this.map.ViewportPointToLocation(mousePosition);
            
            Console.WriteLine(pinLocation);

        }

        private void Pushpin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var model = sender as Pushpin;
            map.Center = model.Location;
            map.ZoomLevel = 16;
        }

        private void PART_Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            var model = PushpinArray.OfType<PushpinModel>().FirstOrDefault(x=>x.ID.Equals(grid.Tag));
            map.Center = model.Location;
            map.ZoomLevel = 16;
        }

       
    }
    public class PushpinModel
    {
        public Location Location { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
    }
    //public class OpenstreetmapTileSource: TileSource
    //{
    //    public override Uri GetUri(int x, int y, int zoomLevel)
    //    {
    //        //var url = string.Format("https://www.openstreetmap.org/#map={z}/{x}/{y}",zoomLevel,x,y);
    //        //return new Uri(url, UriKind.Absolute);
    //        var uri= new Uri(UriFormat.
    //                   Replace("{x}", x.ToString()).
    //                   Replace("{y}", y.ToString()).
    //                   Replace("{z}", zoomLevel.ToString()));
    //        Console.WriteLine(uri);
    //        return uri;
    //    }
    //}
    //public class OpenstreetmapTileLayer : MapTileLayer 
    //{
    //    public OpenstreetmapTileLayer()
    //    {
    //        TileSource = new OpenstreetmapTileSource();
    //    }

    //    public string UriFormat
    //    {
    //        get { return TileSource.UriFormat; }
    //        set { TileSource.UriFormat = value; }
    //    }
    //}
    public class AMapTitleLayer : MapTileLayer
    {
        public AMapTitleLayer()
        {
            TileSource = new AMapTileSource();
        }

        public string UriFormat
        {
            get { return TileSource.UriFormat; }
            set { TileSource.UriFormat = value; }
        }
    }
    public class AMapTileSource: TileSource
    {
        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            string url = "http://webrd01.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x=" + x + "&y=" + y + "&z=" + zoomLevel;
            return new Uri(url, UriKind.Absolute);
        }
    }


}
