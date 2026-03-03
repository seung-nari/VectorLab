using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VectorLab.Infrastructure;
using VectorLab.Services;
using VectorLab.Models;
using System.Collections.ObjectModel;


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

        private LabelRect? _selectedLabel;
        
        // ListBox의 SelectedItem과 바인딩될 속성
        public LabelRect? SelectedLabel
        {
            get => _selectedLabel;
            // SetProperty는 ViewModelBase에 있는 함수
            // 값이 바뀌면 UI에게 "값 바뀌었다"라고 알려줌 (INotifyPropertyChanged)
            set => SetProperty(ref _selectedLabel, value);
        }

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
