﻿using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using ReactiveUI;
using SIT.Manager.Avalonia.Interfaces;
using SIT.Manager.Avalonia.Models.Installation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.ViewModels.Installation;

public partial class ConfigureSitViewModel : InstallationViewModelBase
{
    private readonly IInstallerService _installerService;
    private readonly IPickerDialogService _pickerDialogService;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private SitInstallVersion? _selectedVersion;

    [ObservableProperty]
    private KeyValuePair<string, string>? _selectedMirror;

    [ObservableProperty]
    public bool _isConfigurationValid = false;

    [ObservableProperty]
    private bool _hasVersionsAvailable = false;

    [ObservableProperty]
    private bool _hasMirrorsAvailable = false;

    public ObservableCollection<SitInstallVersion> AvailableVersions { get; } = [];
    public ObservableCollection<KeyValuePair<string, string>> AvailableMirrors { get; } = [];

    public IAsyncRelayCommand ChangeEftInstallLocationCommand { get; }

    public ConfigureSitViewModel(IInstallerService installerService, IPickerDialogService pickerDialogService) : base()
    {
        _installerService = installerService;
        _pickerDialogService = pickerDialogService;

        ChangeEftInstallLocationCommand = new AsyncRelayCommand(ChangeEftInstallLocation);

        this.WhenActivated(async (CompositeDisposable disposables) => await FetchVersionAndMirrorMatrix());
    }

    private async Task ChangeEftInstallLocation()
    {
        IStorageFolder? directorySelected = await _pickerDialogService.GetDirectoryFromPickerAsync();
        if (directorySelected != null)
        {
            if (directorySelected.Path.LocalPath == CurrentInstallProcessState.BsgInstallPath)
            {
                // TODO show an error of some kind as we don't want the legit install to be the same as the SIT install.
            }
            else
            {
                CurrentInstallProcessState.EftInstallPath = directorySelected.Path.LocalPath;
                ValidateConfiguration();
            }
        }
    }

    /// <summary>
    /// Fetch the available versions and the download mirrors for these versions so we can populate the necesarry selections
    /// and save loading time later
    /// </summary>
    /// <returns></returns>
    private async Task FetchVersionAndMirrorMatrix()
    {
        IsLoading = true;

        // Clear the collections
        HasVersionsAvailable = false;
        HasMirrorsAvailable = false;

        AvailableVersions.Clear();
        AvailableMirrors.Clear();

        List<SitInstallVersion> availableVersions = await _installerService.GetAvailableSitReleases(CurrentInstallProcessState.EftVersion);
        if (CurrentInstallProcessState.RequestedInstallOperation == RequestedInstallOperation.UpdateSit)
        {
            // TODO filter results for updating SIT to versions higher than currently
        }

        // Make sure we only offer versions which are actually available to use to maximize the chances the install will work
        AvailableVersions.AddRange(availableVersions.Where(x => x.IsAvailable));
        if (AvailableVersions.Any())
        {
            SelectedVersion = AvailableVersions[0];
            HasVersionsAvailable = true;
        }

        // TODO add some logging here and an alert somehow in case it fails to load any versions or something

        IsLoading = false;
    }

    [RelayCommand]
    private void Back()
    {
        RegressInstall();
    }

    [RelayCommand]
    private void Start()
    {
        ProgressInstall();
    }

    private void ValidateConfiguration()
    {
        IsConfigurationValid = true;

        if (CurrentInstallProcessState.UsingBsgInstallPath)
        {
            if (CurrentInstallProcessState.BsgInstallPath == CurrentInstallProcessState.EftInstallPath)
            {
                IsConfigurationValid = false;
                return;
            }
        }

        if (string.IsNullOrEmpty(CurrentInstallProcessState.EftInstallPath))
        {
            IsConfigurationValid = false;
            return;
        }

        if (AvailableMirrors.Count != 0 && string.IsNullOrEmpty(CurrentInstallProcessState.DownloadMirrorUrl))
        {
            IsConfigurationValid = false;
            return;
        }

        if (IsLoading)
        {
            IsConfigurationValid = false;
            return;
        }
    }

    partial void OnSelectedVersionChanged(SitInstallVersion? value)
    {
        if (value != null)
        {
            CurrentInstallProcessState.RequestedVersion = value.Release;

            AvailableMirrors.Clear();
            if (value.DownloadMirrors.Count > 0)
            {
                AvailableMirrors.AddRange(value.DownloadMirrors);
                SelectedMirror = AvailableMirrors[0];
                HasMirrorsAvailable = true;
            }
            else
            {
                SelectedMirror = null;
                HasMirrorsAvailable = false;
            }
        }
        ValidateConfiguration();
    }

    partial void OnSelectedMirrorChanged(KeyValuePair<string, string>? value)
    {
        CurrentInstallProcessState.DownloadMirrorUrl = value?.Value ?? string.Empty;
        ValidateConfiguration();
    }
}