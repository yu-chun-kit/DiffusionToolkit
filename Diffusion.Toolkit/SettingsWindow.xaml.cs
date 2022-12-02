﻿using System;
using Diffusion.Database;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Diffusion.Toolkit.Classes;

namespace Diffusion.Toolkit
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly DataStore _dataStore;
        private readonly SettingsModel _model;

        public SettingsWindow()
        {
            InitializeComponent();

        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        public SettingsWindow(DataStore dataStore, Settings settings)  :this()
        {
            _dataStore = dataStore;
            
            _model = new SettingsModel();
            _model.ImagePaths = new ObservableCollection<string>(settings.ImagePaths);
            _model.ModelRootPath = settings.ModelRootPath;

            DataContext = _model;
            
            Closing += (sender, args) =>
            {
                settings.ImagePaths = _model.ImagePaths.ToList();
                settings.ModelRootPath = _model.ModelRootPath;
            };
        }

        private void AddFolder_OnClick(object sender, RoutedEventArgs e)
        {
            using var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.ShowDialog(this);
            if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
            {
                _model.ImagePaths.Add(dialog.FileName);
            }

        }

        private void RemoveFolder_OnClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(this,
                "Are you sure you want to remove this folder?",
                "Remove folder", MessageBoxButton.YesNo,
                MessageBoxImage.Question, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                _model.ImagePaths.RemoveAt(_model.SelectedIndex);
            }
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BrowseModelPath_OnClick(object sender, RoutedEventArgs e)
        {
            using var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if(dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
            {
                _model.ModelRootPath = dialog.FileName;
            }
        }
    }
}