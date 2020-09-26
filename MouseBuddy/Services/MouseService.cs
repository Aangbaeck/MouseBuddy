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

                            if (RelativeScreenCrossing)
                            {
                                if (point.X == sc.Left && point.X != 0)
                                {
                                    //LocalInfo += ($"(Side left! : {point.X},{point.Y})");
                                    var heightRightScreen = sc.Bottom;
                                    var relativePosRight = point.Y / heightRightScreen;
                                    var heightLeftScreen = FindScreenHeight(x - 1);
                                    x--;
                                    y = (int)(heightLeftScreen * relativePosRight);
                                    SetCursorPos(x, y);
                                    break;
                                }
                                else if (point.X == sc.Right - 1)
                                {
                                    //LocalInfo += ($"(Side right! : {point.X},{point.Y})");
                                    var heightLeftScreen = sc.Bottom;
                                    var relativePosLeft = point.Y / heightLeftScreen;
                                    var heightRightScreen = FindScreenHeight(x + 1);
                                    x++;
                                    y = (int)(heightRightScreen * relativePosLeft);
                                    SetCursorPos(x, y);
                                    break;
                                }
                            }
                            else if (DisableStickyCorners)
                            {
                                //DisableStickyCorners Task
                                if (point.Y >= sc.Bottom - StickyCornersHeight || point.Y <= sc.Top + StickyCornersHeight)
                                {
                                    if (point.X == sc.Left && point.X != 0)
                                    {
                                        //LocalInfo += ($"(Corner left! : {point.X},{point.Y})");
                                        x--;
                                        SetCursorPos(x, y);
                                        break;
                                    }
                                    else if (point.X == sc.Right - 1)
                                    {
                                        //LocalInfo += ($"(Corner right! : {point.X},{point.Y})");
                                        x++;
                                        SetCursorPos(x, y);
                                        break;
                                    }
                                }
                            }



                        }
                    }



                    await Task.Delay(0);
                }
            });


        }

        private double FindScreenHeight(int x)
        {
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.Bounds.Left <= x && x < screen.Bounds.Right)
                    return screen.Bounds.Bottom;
            }
            return -1;
        }

        static int x, y;
        private bool disableStickyCorners = true;
        private string localInfo = "";
        private int mouseX;
        private int mouseY;

        public int MouseX
        {
            get => mouseX; set
            {
                mouseX = value;
                RaisePropertyChanged();
            }
        }
        public int MouseY
        {
            get => mouseY; set
            {
                mouseY = value;
                RaisePropertyChanged();
            }
        }
        public string LocalInfo
        {
            get
            {
                return localInfo;
            }
            private set
            {
                if (SilentMode) return;
                if (localInfo.Length > 1000) localInfo = localInfo.Remove(0, localInfo.Length - 1000);  //Trim of length so not too long
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
