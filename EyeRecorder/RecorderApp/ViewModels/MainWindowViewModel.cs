﻿using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using RecorderApp.Models;
using RecorderApp.Utility;
using RecorderApp.Views;
using System;
using System.IO;
using System.Windows.Input;

namespace RecorderApp.ViewModels
{
    public class MainWindowViewModel : BindableBase, IControlWindows
    {
        private string _title = "Eye Luminate";
        public string Title
        {
            get { return _title; }
            set 
            {
                SetProperty(ref _title, value);
            }
        }

        IEventAggregator _ea;
        public IView _view;
        public IView2 _view2;
        public IView3 _view3;
        public MainWindowViewModel(IView view, IView2 view2, IView3 view3, IEventAggregator ea)
        {
            _view = view;
            _view2 = view2;
            _view3 = view3;
            _ea = ea;
            this.OpenCommand = new RelayCommand(this.OpenFile);


        }

        #region Open New Window Methods
        private DelegateCommand _nextWindow;

        public DelegateCommand NextWindow =>
            _nextWindow ?? (_nextWindow = new DelegateCommand(ExecuteNextWindow));

        void ExecuteNextWindow()
        {
            Next?.Invoke();
            SendTrackingStatus(true);
            _view.Open(this.SelectedPath);
        }

        #endregion


        #region Open Generate Results Window

        private DelegateCommand _resultsWindow;
        public DelegateCommand ResultsWindow =>
            _resultsWindow ?? (_resultsWindow = new DelegateCommand(ShowResultsWindow));

        void ShowResultsWindow()
        {
            Next?.Invoke();
            _view2.OpenRes();
        }

        #endregion

        #region Open Generate Quick Results Window

        private DelegateCommand _quickResultsWindow;
        public DelegateCommand QuickResultsWindow =>
            _quickResultsWindow ?? (_quickResultsWindow = new DelegateCommand(ShowQuickResultsWindow));

        void ShowQuickResultsWindow()
        {
            Next?.Invoke();
            SendTrackingStatus(false);
            _view3.OpenQRes();
        }

        #endregion
        #region Get File Methods / Open File Dialog

        private string selectedPath;

        public string SelectedPath
        {
            get { return selectedPath; }
            set 
            {
                SetProperty(ref selectedPath, value);
            }
        }

        string getParent()
        {
            string currentPath = Directory.GetCurrentDirectory();
            string vidPath = Path.GetFullPath(Path.Combine(currentPath, @"..\..\..\..\..\videos"));
            Console.WriteLine(vidPath);
            return vidPath;
        }

        public ICommand OpenCommand { get; set; }

        private void OpenFile()
        {
            FileDialogViewModel fd = new FileDialogViewModel();
            fd.Extension = "*.mp4";
            fd.Filter = "(.mp4) |*.mp4";

            fd.InitialDirectory = getParent();
            fd.OpenCommand.Execute(null);

            if (fd.FileName == null)
            {
                selectedPath = "blank";
            }

            this.selectedPath = fd.FileName;

            KeepVidPath();
        }

        #endregion

        #region EventAggregators
        /// <summary>
        /// eventaggregator for video path
        /// </summary>
        private void KeepVidPath()
        {
            _ea.GetEvent<SavePathEvent>().Publish(SelectedPath);
            //Console.WriteLine("helo " + SelectedPath);
        }

        private void SendTrackingStatus(bool status)
        {
            _ea.GetEvent<RecStatusEvent>().Publish(status);
            Console.WriteLine("Recording triggered");
        }

        #endregion

        #region Close Window 

        private DelegateCommand _closeCommand;
        public DelegateCommand CloseCommand =>
            _closeCommand ?? (_closeCommand = new DelegateCommand(CloseWindow));

        void CloseWindow()
        {
            Close?.Invoke();
        }

        public Action Close { get; set; }

        public Action Next { get; set; }

        #endregion


        #region open calibration

        private DelegateCommand _calibrateWindow;

        public DelegateCommand CalibrateWindow =>
            _calibrateWindow ?? (_calibrateWindow = new DelegateCommand(ExecuteCalibration));

        void ExecuteCalibration()
        {
            //GazeTrackerViewModel gazeVm = new GazeTrackerViewModel();
            //gazeVm.startCalibration();
            SendCalibrationSignal(true);
        }
        private void SendCalibrationSignal(bool start)
        {
            _ea.GetEvent<RecStatusEvent>().Publish(start);
            Console.WriteLine("Calibration triggered");
        }
        #endregion


    }

    public interface IControlWindows
    {
        Action Close { get; set; }

        Action Next { get; set; }
    }



}