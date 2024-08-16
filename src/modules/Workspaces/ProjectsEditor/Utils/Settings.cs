﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.PowerToys.Settings.UI.Library;

namespace ProjectsEditor.Utils
{
    public class Settings
    {
        private const string ProjectsModuleName = "Workspaces";
        private static SettingsUtils _settingsUtils = new SettingsUtils();

        public static ProjectsSettings ReadSettings()
        {
            if (!_settingsUtils.SettingsExists(ProjectsModuleName))
            {
                var defaultProjectsSettings = new ProjectsSettings();
                defaultProjectsSettings.Save(_settingsUtils);
                return defaultProjectsSettings;
            }

            ProjectsSettings settings = _settingsUtils.GetSettingsOrDefault<ProjectsSettings>(ProjectsModuleName);
            return settings;
        }
    }
}