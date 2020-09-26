using GalaSoft.MvvmLight;
using Serilog;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WpfScreenHelper;

namespace MouseBuddy.Services
{
    public class MouseService : ObservableObject
    {
        public bool DisableStickyCorners
        {
            get
            {
                return disableStickyCorners;
            }

            set
            {
                disableStickyCorners = value;
                RaisePropertyChanged();
            }
        }
        private bool relativeScreenCrossing = true;

        public bool RelativeScreenCrossing
        {
            get { return relativeScreenCrossing = true; }
            set { relativeScreenCrossing = value; }
        }

        int StickyCornersHeight { get; set; } = 6; //Sticky corners are 5 pixels high.
        public MouseService()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    GetCursorPos(out var point);

                    //DisableStickyCorners Task
                    if (DisableStickyCorners && (point.X != x || point.Y != y)) //If mouse is still, we don't need to check another time
                    {
                        x = point.X;
                        y = point.Y;
                        MouseX = x;
                        MouseY = y;
                        //Find what screen we are on and see if we should move the mouse cursor ++ or --
                        foreach (var screen in Screen.AllScreens)
                        {
                            var sc = screen.Bounds;
                            if (point.Y >= sc.Top - StickyCornersHeight || point.Y <= sc.Bottom + StickyCornersHeight)
                                if (point.X == sc.Left && point.X != 0)
                                {
                                    LocalInfo += ($"(Corner! : {point.X},{point.Y})");
                                    x--;
                                    SetCursorPos(x, y);
                                    break;
                                }
                                else if (point.X == sc.Right - 1)
                                {
                                    LocalInfo += ($"(Corner! : {point.X},{point.Y})");
                                    x++;
                                    SetCursorPos(x, y);
                                    break;
                                }
                        }
                    }



                    await Task.Delay(20);
                }
            });


        }

        static int x, y;
        private bool disableStickyCorners = true;
        private string localInfo = "";

        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public string LocalInfo
        {
            get
            {
                return localInfo;
            }
            private set
            {
                if (SilentMode) return;
                if (localInfo.Length > 5000) localInfo = localInfo.Remove(0, localInfo.Length - 5000);  //Trim of length so not too long
                localInfo = value;
                RaisePropertyChanged();
            }
        }

        public bool SilentMode { get; set; } = false;

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        const int ENUM_CURRENT_SETTINGS = -1;

        public struct POINT
        {
            public int X;
            public int Y;
        }
    }
}
