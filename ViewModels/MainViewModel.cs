using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VectorLab.Infrastructure;
using VectorLab.Services;
using VectorLab.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace VectorLab.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        // 현재 화면에 표시되는 GeoTIFF 이미지
        private BitmapSource? _currentImage;
        public BitmapSource? CurrentImage
        {
            get => _currentImage;
            set => SetProperty(ref _currentImage, value);
        }

        // 라벨 목록(사각형 라벨들)
        public ObservableCollection<LabelRect> Labels { get; }

        // 버튼에서 사용할 Command
        public ICommand OpenGeoTiffCommand { get; }

        public MainViewModel()
        {
            Labels = new ObservableCollection<LabelRect>();
            OpenGeoTiffCommand = new RelayCommand(OpenGeoTiff);
        }

        private void OpenGeoTiff()
        {
            var dig = new OpenFileDialog
            {
                Filter = "GeoTIFF (*.tif;*.tiff)|*.tif;*.tiff|All files (*.*)|*.*"
            };

            if (dig.ShowDialog() != true) return;

            // GDAL 서비스로 이미지 로드
            var image = GeoTiffLoaderGdal.LoadAsBitmapSource(dig.FileName);

            CurrentImage = image;
        }
    }
}
