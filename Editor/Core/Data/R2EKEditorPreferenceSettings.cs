﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace RoR2.Editor
{
    [PublicFilePath("RoR2EditorKit/R2EKEditorPreferenceSettings.asset", PublicFilePathAttribute.Location.PreferencesFolder)]
    public sealed class R2EKEditorPreferenceSettings : ScriptableSingleton<R2EKEditorPreferenceSettings>, EditorSettingManager.IEditorSettingProvider
    {

        [ReadOnly, SerializeField]
        private List<EditorSetting> _projectSettings = new List<EditorSetting>();

        List<EditorSetting> EditorSettingManager.IEditorSettingProvider.editorSettings => _projectSettings;
        public void SaveSettings() => Save(true);

        private void Awake()
        {
            EditorApplication.quitting += EditorApplication_quitting;
        }

        private void EditorApplication_quitting()
        {
            Save(true);
        }
    }
}
