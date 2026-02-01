using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VectorLab.Infrastructure
{
    internal class ViewModelBase : INotifyPropertyChanged
    {
        // 바인딩된 값이 바뀔 때 PropertyChanged 이벤트를 발생시키는 것을 감지하여 UI 자동 갱신
        // 즉 UI 갱신 트리거
        public event PropertyChangedEventHandler PropertyChanged;

        /* 
        OnPropertyChanged() 왜 메서드로 뺐냐?
        프로퍼티가 10개면 10개마다 Invoke 함수를 10개 사용하게되는데
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Title")); 이게 반복되고 오타나 리팩터링 시 깨지기 쉬움
        그래서 공통 동작을 메서드로 뺌

        [CallerMemberName]은 컴파일러가 "호출한 멤버 이름"을 자동으로 넣어주는 속성
        예를 들어 OnPropertyChanged() => OnPropertyChanged("Title") 호출한 프로퍼티 이름이 Title이라면
        오타 방지, 리팩터링해도 자동으로 따라오고, 코드가 짧아지는 효과

        string name = null 에서 = null은 왜 붙어있냐?
        이건 옵션 파라미터로 만들기 위해서
        = null이 없으면 매번 인자를 줘야함
        예를 들어 OnPropertyChanged("Title") 처럼 "Title"을 줘야하는데 그럴 필요 없이 OnPropertyChanged()만 넣어도 됨
        */
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            // 실제로 바뀌었어요 하고 알리는 동작
            // ?. 사용 이유 : 리스너가 없다면 PropertyChanged는 null인데 그 상태에서 함수를 실행시키면 NullReferenceException 발생하여 리스너가 있을때만 호출
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /*
        SetProperty<T>(ref T field, T value, ...) 이건 왜 쓰나?
        MVVM에서 프로퍼티는 보통 이렇게 생김:
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        근데 이런 프로퍼티가 20개면 똑같은 패턴을 20번 쓰게 됨
        그래서 “대부분의 set 로직(비교→대입→알림)”을 한 방에 처리해주는 메서드가 SetProperty

        ref T field는 왜 ref일까
        C#에서 메서드 인자는 기본적으로 값 복사로 전달됨
        SetProperty(field, value)처럼 넘기면 메서드 안에서 field = value; 해도 원본 필드가 바뀌지 않음
        그래서 “원본 필드 자체를 바꾸기 위해” 참조로 넘기는 ref가 필요함
        SetProperty(ref _title, value);
        이렇게 하면 메서드 안에서 _title을 직접 바꾸는 효과가 생겨.

        **** ref를 쓰면 “복사 말고 주소를 넘겨라” ref는 값이 아니라 “그 변수 자체”를 넘김 ****
        a ----┐
        x ----┘  (같은 메모리)
         */
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            // if (Equals(field, value)) return false; 는 왜 비교하나?
            // 값이 안 바뀌었는데도 계속 PropertyChanged를 발생시키지 않으려고
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }
    }
}
