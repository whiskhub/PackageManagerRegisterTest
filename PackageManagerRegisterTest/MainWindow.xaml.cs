using Microsoft.UI.Xaml;
using Microsoft.Windows.Management.Deployment;
using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;


namespace PackageManagerRegisterTest
{
    public sealed partial class MainWindow : Window
    {
        public string PackageFullName { get; }
        public string NewerIsRegistered { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();

            PackageFullName = Package.Current.Id.FullName;
        }

        private ObservableCollection<string> _logs = new();

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.IsEnabled = false;

            try
            {
                _logs.Add("Starting add");
                var packageDeploymentManager = PackageDeploymentManager.GetDefault();

                var options = new AddPackageOptions();
                options.DeferRegistrationWhenPackagesAreInUse = true;

                // #### THIS IS THE OFFENDING LINE OF CODE
                // #### REMOVE IT AND THE UPDATE WORKS FINE
                options.DependencyPackageUris.Add(new Uri("https://aka.ms/Microsoft.VCLibs.x64.14.00.Desktop.appx"));

                var uri = new Uri(FilePathField.Text);

                _logs.Add("Starting operation");
                var operation = packageDeploymentManager.AddPackageByUriAsync(uri, options);
                _logs.Add($"Operation started: {operation}");
                var results1 = operation.GetResults();
                _logs.Add($"Operation started: {results1?.ActivityId} {results1?.ErrorText} {results1?.ExtendedError}");

                var results2 = await operation;

                _logs.Add($"Operation finished: {results2?.ActivityId} {results2?.ErrorText} {results2?.ExtendedError}");
            }
            catch (Exception ex)
            {
                _logs.Add($"Operation failed: {ex}");
            }
            finally
            {
                myButton.IsEnabled = true;
            }
        }

        private async void FilePathPickerButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            picker.FileTypeFilter.Add("*");

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                FilePathField.Text = file.Path;
            }
        }
    }
}
