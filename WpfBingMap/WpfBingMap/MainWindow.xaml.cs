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

            var pushpins = new List<PushpinModel>();
            pushpins.Add(new PushpinModel { ID=1, Location = new Location(39.8151940395589, 116.411970893135),Title="和义东里社区" });
            pushpins.Add(new PushpinModel { ID = 2, Location = new Location(39.9094878843105, 116.33299936282) ,Title="中国水科院南小区"});
            pushpins.Add(new PushpinModel { ID = 3, Location = new Location(39.9181518802641, 116.203328913478),Title="石景山山姆会员超市" });
            pushpins.Add(new PushpinModel { ID = 4, Location = new Location(39.9081417418219, 116.331244439925), Title = "茂林居小区" });
            PushpinArray = pushpins;
        }

       

        private void Pushpin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var model = sender as Pushpin;
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
    public class OpenstreetmapTileSource: TileSource
    {
        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            //var UriFormat = "https://www.openstreetmap.org/#map={z}/{x}/{y}";
            //var url = string.Format("https://www.openstreetmap.org/#map={z}/{x}/{y}",zoomLevel,x,y);
            //return new Uri(url, UriKind.Absolute);
            var uri= new Uri(UriFormat.
                       Replace("{x}", x.ToString()).
                       Replace("{y}", y.ToString()).
                       Replace("{z}", zoomLevel.ToString()));
            Console.WriteLine(uri);
            return uri;
        }
    }
    public class OpenstreetmapTileLayer : MapTileLayer 
    {
        public OpenstreetmapTileLayer()
        {
            TileSource = new OpenstreetmapTileSource();
        }

        public string UriFormat
        {
            get { return TileSource.UriFormat; }
            set { TileSource.UriFormat = value; }
        }
    }
    public class CustomTileSource : TileSource
    {
        public override Uri GetUri(int x, int y, int zoomLevel) =>
            new Uri($"http://mt0.google.com/vt/x={x}&amp;y={y}&amp;z={zoomLevel}.png");
        //new Uri($"https://tile.openstreetmap.org/{zoomLevel}/{x}/{y}.png");
    }
    public class OwnDrawTileSource : TileSource
    {
        public OwnDrawTileSource()
        {
            this.DirectImage = this.TileRender;
        }

        public BitmapImage TileRender(long x, long y, int zoomLevel)
        {
            var rawData = new byte[256 * 256 * 4];
            var a = (byte)0x80;
            var r = ((x + y) % 3) == 0 ? (byte)0xFF : (byte)0;
            var g = ((x + y + 1) % 3) == 0 ? (byte)0xFF : (byte)0;
            var b = ((x + y + 2) % 3) == 0 ? (byte)0xFF : (byte)0;

            for (var yy = 0; yy < 256; yy++)
            {
                for (var xx = 0; xx < 256; xx++)
                {
                    var index = (yy * 256 + xx) * 4;
                    rawData[index] = r;
                    rawData[index + 1] = g;
                    rawData[index + 2] = b;
                    rawData[index + 3] = a;
                }
            }


            var source = BitmapSource.Create(256, 256, 96, 96, PixelFormats.Pbgra32, null, rawData, 256 * 4);

            var result = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(stream);

                stream.Position = 0;

                result.BeginInit();
                result.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.UriSource = null;
                result.StreamSource = stream;
                result.EndInit();
            }
            result.Freeze();

            return result;
        }
    }

}
