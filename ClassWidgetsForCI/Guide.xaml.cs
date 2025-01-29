using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ClassIsland.Core;
using MahApps.Metro.Controls;

namespace ClassWidgetsForCI {
    public partial class Guide {
        public Guide() {
            InitializeComponent();
            DoubleAnimation valueInitAnimation = new DoubleAnimation {
                From = 100,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(10))
            };
            Storyboard.SetTarget(valueInitAnimation, ProgressBar);
            Storyboard.SetTargetProperty(valueInitAnimation, new PropertyPath("Value"));
            _initStoryboard = new Storyboard {
                Children = [valueInitAnimation],
                FillBehavior = FillBehavior.Stop
            };
        }

        readonly Storyboard _initStoryboard;
        
        void Guide_OnLoaded(object sender,RoutedEventArgs e) {
            _initStoryboard.Completed += Swapper;
            InfoLabel.Content = "切换程序将在10秒后启动...";
            _initStoryboard.Begin();
        }
        void Swapper(object? sender,EventArgs eventArgs) {
            Swapper();
        }
        void Swapper() {
            this.BeginInvoke(() => {
                if (CheckExistence()) {
                    InfoLabel.Content = "正在拉起 Class Widgets...";
                    ProgressBar.Value = 100;
                    Startup();
                } else {
                    InfoLabel.Content = "没有检测到由本插件安装的CW实例存在";
                    InfoLabel.Content = "开始安装...";
                    #pragma warning disable CS0618 // Type or member is obsolete
                        Install();
                    #pragma warning restore CS0618 // Type or member is obsolete
                }
            });
        }
        
        static readonly string PackUrl = "https://gh.api.99988866.xyz/https://github.com/Class-Widgets/Class-Widgets/releases/download/v1.1.7/ClassWidgets_v1.1.7_win_x64.zip";

        public static void Startup() {
            ProcessStartInfo psi = new ProcessStartInfo("cmd", $"/c start {Path.Combine(GetPath(),"ClassWidgets_v1.1.7_win_x64\\ClassWidgets.exe")}");
            Process.Start(psi);
            AppBase.Current.Shutdown();
        }

        public static string GetPath() {
            return Directory.Exists("D:\\") ? @"D:\Class-Widgets-CI\"
                : @"C:\Users\Public\Class-Widgets-CI\";
        }

        public static bool CheckExistence() {
            return File.Exists(Path.Combine(GetPath(),@"ClassWidgets_v1.1.7_win_x64\ClassWidgets.exe"));
        }

        [Obsolete("Obsolete")]
        public void Install() {
            string path = GetPath();
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            try {
                WebClient client = new WebClient();
                client.DownloadFileCompleted += (_,_) => {
                    new Thread(() => {
                        try {
                            this.BeginInvoke(() => {
                                InfoLabel.Content = "正在解压,这通常需要一会儿...";
                            });
                            ZipFile.ExtractToDirectory("ClassWidgets.zip",GetPath());
                            this.BeginInvoke(() => {
                                InfoLabel.Content = "解压完成,准备启动...";
                                Swapper();
                            });
                        }
                        catch {
                            this.BeginInvoke(() => {
                                InfoLabel.Content = "下载文件出现错误"; 
                                InfoLabel.Foreground = Brushes.Red;
                            });
                        }
                    }).Start();
                };
                client.DownloadProgressChanged += (_,e) => {
                    this.BeginInvoke(() => {
                        ProgressBar.Value = e.ProgressPercentage;
                        InfoLabel.Content = $"正在下载 {e.ProgressPercentage}%";
                    });
                };
                client.DownloadFileAsync(new Uri(PackUrl),"ClassWidgets.zip");
            }
            catch {
                this.BeginInvoke(() => {
                    InfoLabel.Content = "下载时遇到错误";
                });
            }
        }
        
        bool _isLocked = true;
        void Guide_OnClosing(object? sender,CancelEventArgs e) {
            e.Cancel = _isLocked;
        }
        void ButtonBase_OnClick(object sender,RoutedEventArgs e) {
            _isLocked = false;
            Close();
        }
    }
}