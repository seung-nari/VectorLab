using System.IO;
using System.Windows;
using OSGeo.GDAL;
using VectorLab.ViewModels;

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

            // 네이티브dll / 데이터 폴더가 있는 위치를 "실행 폴더 기준"으로 잡는다
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // 예: bin 폴더 아래에 native 모아두는 전략
            var nativeDir = Path.Combine(baseDir, "gdal", "x64");
            var dataDir = Path.Combine(baseDir, "gdal", "data");
            var projDir = Path.Combine(baseDir, "gdal", "share");

            // PATH에 nativeDir 추가 (gdal_wrap.dll / gdal.dll등을 찾기 위함)
            Environment.SetEnvironmentVariable(
                "PATH", nativeDir + ";" + Environment.GetEnvironmentVariable("PATH")
            );

            // GDAL 데이터/PROJ 설정 (좌표계/리샘플링/GeoTIFF 메타 처리에 중요)
            Environment.SetEnvironmentVariable("GDAL_DATA", dataDir);
            Environment.SetEnvironmentVariable("PROJ_LIB", projDir);

            // 드라이버 등록
            Gdal.AllRegister();
        }
    }
}
