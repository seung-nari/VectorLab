using System.Windows;
using OSGeo.GDAL;

namespace VectorLab
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Gdal.AllRegister(); // 드라이버 등록(GeoTIFF 포함)


        }
    }

}
