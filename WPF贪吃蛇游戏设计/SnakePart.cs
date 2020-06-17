using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF贪吃蛇游戏设计
{
    public  class SnakePart
    {
        public UIElement  uIElement { get; set; }
        public Point point { get; set; }
        public Boolean isHead { get; set; }
    }
}
