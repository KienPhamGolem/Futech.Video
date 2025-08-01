using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futech.Video
{

    public class ResizeConfigModel
    {
        private int _LocationX;
        private int _LocationY;
        private int _ResizeWidth;
        private int _ResizeHeight;
        private string _Resolution;
        private int _WindowWidth;
        private int _WindowHeight;
        private int _ResolutionHeight = 0;
        private int _ResolutionWidth = 0;

        //vị trí theo trục X điểm góc trái trên của hình chữ nhật resize ( tính theo độ phân giải ở dưới)
        public int LocationX { get => _LocationX; set => _LocationX = value; }
        //vị trí theo trục Y điểm góc trái trên của hình chữ nhật resize ( tính theo độ phân giải ở dưới)
        public int LocationY { get => _LocationY; set => _LocationY = value; }
        //Độ dài hình chữ nhật resize ( tính theo độ phân giải)
        public int ResizeWidth { get => _ResizeWidth; set => _ResizeWidth = value; }
        //cHiều cao HCn (tính theo độ phân giải)
        public int ResizeHeight { get => _ResizeHeight; set => _ResizeHeight = value; } 
        public string Resolution { get => _Resolution; set 
            { 
                _Resolution = value; 
                GetResolution(); 
            } }    //độ phân giải
        public int WindowWidth { get => _WindowWidth; set => _WindowWidth = value; }    //chiều dài của control hiển thị camera
        public int WindowHeight { get => _WindowHeight; set => _WindowHeight = value; } //chiều cao của control hiển thị camera
        public int ResolutionHeight { get => _ResolutionHeight; set => _ResolutionHeight = value; }
        public int ResolutionWidth { get => _ResolutionWidth; set => _ResolutionWidth = value; }
       
        private void GetResolution()
        {
            //cat chuoi lay do phan giai cua luong video
            Char Symbol;
            if (_Resolution.Contains('x'))
            {
                Symbol = 'x';
            }
            else if (_Resolution.Contains('*'))
            {
                Symbol = '*';
            }  
            else if (_Resolution.Contains(':'))
            {
                Symbol = ':';
            }
            else
            {
                return;
            }
            int.TryParse(_Resolution.Substring(0, _Resolution.IndexOf(Symbol)), out _ResolutionWidth);
            int.TryParse(_Resolution.Substring(_Resolution.IndexOf(Symbol) + 1, _Resolution.Length - _Resolution.IndexOf(Symbol) - 1), out _ResolutionHeight);
        }

    }
}
