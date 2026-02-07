using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VectorLab.Infrastructure;
using VectorLab.Services;

namespace VectorLab.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private BitmapSource? _currentImage;
        public BitmapSource? CurrentImage
        {
            get => _currentImage;
            set => SetProperty(ref _currentImage, value);
        }

        public ICommand OpenGeoTiffCommand { get; }

        public MainViewModel()
        {
            OpenGeoTiffCommand = new RelayCommand(OpenGeoTiff);
        }

        private void OpenGeoTiff()
        {
            var dig = new OpenFileDialog
            {
                Filter = "GeoTIFF (*.tif;*.tiff)|*.tif;*.tiff|All files (*.*)|*.*"
            };

            if (dig.ShowDialog() != true) return;

            CurrentImage = GeoTiffLoaderGdal.LoadAsBitmapSource(dig.FileName);
        }
    }
}
