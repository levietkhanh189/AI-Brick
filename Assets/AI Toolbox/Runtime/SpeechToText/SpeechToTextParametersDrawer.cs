﻿#if UNITY_EDITOR
using System;
using AiToolbox;
using UnityEditor;
using UnityEngine;

namespace AiToolbox {
[CustomPropertyDrawer(typeof(SpeechToTextParameters))]
public class SpeechToTextParametersDrawer : PropertyDrawer {
    private float _height;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        label.text = "Speech to Text Parameters";
        label.tooltip = "Settings for the backend of the OpenAI Speech to Text requests.";
        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded =
            EditorGUI.Foldout(new Rect(position.x, position.y, position.width - 20, EditorGUIUtility.singleLineHeight),
                              property.isExpanded, label, true);
        _height = EditorGUIUtility.singleLineHeight;

        // Question mark button
        {
            var buttonRect = new Rect(position.x + position.width - 20, position.y + 2, 20,
                                      EditorGUIUtility.singleLineHeight);
            EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);
            var icon = EditorGUIUtility.IconContent("_Help");
            icon.tooltip = "Open documentation";
            if (GUI.Button(buttonRect, icon, EditorStyles.label)) {
                Application.OpenURL("https://ai-toolbox.dustyroom.com/speech-to-text");
            }
        }

        if (property.isExpanded) {
            EditorGUI.indentLevel++;
            var width = position.width - EditorGUIUtility.standardVerticalSpacing * 2f;

            // Space
            {
                var rect = new Rect(position.x, position.y + _height, width, EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.LabelField(rect, GUIContent.none);
                _height += rect.height + EditorGUIUtility.standardVerticalSpacing;
            }

            // API Key
            {
                var apiKeyProperty = property.FindPropertyRelative(nameof(SpeechToTextParameters.apiKey));
                var apiKey = apiKeyProperty.stringValue;
                if (string.IsNullOrEmpty(apiKey)) {
                    const string m = "API Key is required.";
                    var helpBoxHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(m),
                                                                        width - EditorGUIUtility.labelWidth - 24);
                    var helpBoxRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y + _height,
                                               width - EditorGUIUtility.labelWidth, helpBoxHeight);
                    EditorGUI.HelpBox(helpBoxRect, m, MessageType.Error);
                    _height += helpBoxRect.height + EditorGUIUtility.standardVerticalSpacing;

                    var buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y + _height,
                                              width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                    _height += buttonRect.height + EditorGUIUtility.standardVerticalSpacing;
                    if (GUI.Button(buttonRect, "Open API Keys page", EditorStyles.miniButton)) {
                        Application.OpenURL("https://platform.openai.com/account/api-keys");
                    }
                }

                var rect = new Rect(position.x, position.y + _height, width, EditorGUIUtility.singleLineHeight);
                _height += rect.height + EditorGUIUtility.standardVerticalSpacing;
                var encryptionProperty = property.FindPropertyRelative(nameof(SpeechToTextParameters.apiKeyEncryption));
                var isRemote = encryptionProperty.enumValueIndex == (int)ApiKeyEncryption.RemoteConfig;
                var apiKeyTitle = isRemote ? "API Key (fallback)" : "API Key";
                var apiKeyLabel = new GUIContent(apiKeyTitle, "Your API Key from the OpenAI platform.");
                var isEncrypted = encryptionProperty.enumValueIndex == (int)ApiKeyEncryption.LocallyEncrypted;
                var passwordProperty =
                    property.FindPropertyRelative(nameof(SpeechToTextParameters.apiKeyEncryptionPassword));
                if (isEncrypted && !string.IsNullOrEmpty(apiKey) && !apiKey.StartsWith("sk-") &&
                    !string.IsNullOrEmpty(passwordProperty.stringValue)) {
                    apiKey = Key.B(apiKey, passwordProperty.stringValue);
                }

                var apiKeyEdited = EditorGUI.TextField(rect, apiKeyLabel, apiKey);
                if (apiKeyEdited != apiKey) {
                    apiKeyProperty.stringValue = isEncrypted && !string.IsNullOrEmpty(apiKeyEdited)
                        ? Key.A(apiKeyEdited, passwordProperty.stringValue) : apiKeyEdited;
                }
            }

            // API Key encryption
            {
                EditorGUI.indentLevel++;
                var encryptionProperty = property.FindPropertyRelative(nameof(SpeechToTextParameters.apiKeyEncryption));

                // Warning
                if (encryptionProperty.enumValueIndex == (int)ApiKeyEncryption.None &&
                    !string.IsNullOrEmpty(property.FindPropertyRelative(nameof(SpeechToTextParameters.apiKey))
                                              .stringValue)) {
                    const string m = "API Key is not encrypted. A malicious actor could steal your API Key by " +
                                     "decompiling your build.";
                    var helpBoxHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(m),
                                                                        width - EditorGUIUtility.labelWidth - 24);
                    var helpBoxRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y + _height,
                                               width - EditorGUIUtility.labelWidth, helpBoxHeight);
                    EditorGUI.HelpBox(helpBoxRect, m, MessageType.Warning);
                    _height += helpBoxRect.height + EditorGUIUtility.standardVerticalSpacing;
                }

                var rect = new Rect(position.x, position.y + _height, width, EditorGUIUtility.singleLineHeight);
                _height += rect.height + EditorGUIUtility.standardVerticalSpacing;
                var encryptionLabel = new GUIContent("Encryption",
                                                     "Encrypt API Key to prevent malicious actors from stealing it.");
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(rect, encryptionProperty, encryptionLabel);
                if (EditorGUI.EndChangeCheck()) {
                    var apiKeyProperty = property.FindPropertyRelative(nameof(SpeechToTextParameters.apiKey));
                    var passwordProperty =
                        property.FindPropertyRelative(nameof(SpeechToTextParameters.apiKeyEncryptionPassword));
                    var isEncrypted = encryptionProperty.enumValueIndex == (int)ApiKeyEncryption.LocallyEncrypted;
                    if (isEncrypted) {
                        if (string.IsNullOrEmpty(passwordProperty.stringValue)) {
                            passwordProperty.stringValue = Guid.NewGuid().ToString();
                        }

                        if (apiKeyProperty.stringValue.StartsWith("sk-") &&
                            !string.IsNullOrEmpty(apiKeyProperty.stringValue)) {
                            var encryptedApiKey = Key.A(apiKeyProperty.stringValue, passwordProperty.stringValue);
                            apiKeyProperty.stringValue = encryptedApiKey;
                        }
                    } else {
                        if (!apiKeyProperty.stringValue.StartsWith("sk-") &&
                            !string.IsNullOrEmpty(passwordProperty.stringValue) &&
                            !string.IsNullOrEmpty(apiKeyProperty.stringValue)) {
                            var decryptedApiKey = Key.B(apiKeyProperty.stringValue, passwordProperty.stringValue);
                            apiKeyProperty.stringValue = decryptedApiKey;
                        }
                    }
                }

                var showPasswordField = encryptionProperty.enumValueIndex == (int)ApiKeyEncryption.LocallyEncrypted;
                if (showPasswordField) {
                    var passwordProperty =
                        property.FindPropertyRelative(nameof(SpeechToTextParameters.apiKeyEncryptionPassword));

                    if (string.IsNullOrEmpty(passwordProperty.stringValue)) {
                        const string m = "Password is required.";
                        var helpBoxHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(m),
                                                                            width - EditorGUIUtility.labelWidth - 24);
                        var helpBoxRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y + _height,
                                                   width - EditorGUIUtility.labelWidth, helpBoxHeight);
                        EditorGUI.HelpBox(helpBoxRect, m, MessageType.Error);
                        _height += helpBoxRect.height + EditorGUIUtility.standardVerticalSpacing;
                    }

                    var passwordRect = new Rect(position.x, position.y + _height, width,
                                                EditorGUIUtility.singleLineHeight);
                    var passwordLabel = new GUIContent("Password", "Password used to encrypt API Key.");
                    var passwordEdited = EditorGUI.TextField(passwordRect, passwordLabel, passwordProperty.stringValue);
                    if (passwordEdited != passwordProperty.stringValue) {
                        var apiKeyProperty = property.FindPropertyRelative(nameof(SpeechToTextParameters.apiKey));
                        var decryptedApiKey = string.IsNullOrEmpty(passwordProperty.stringValue)
                            ? apiKeyProperty.stringValue
                            : Key.B(apiKeyProperty.stringValue, passwordProperty.stringValue);
                        var encryptedApiKey = string.IsNullOrEmpty(passwordEdited) ? decryptedApiKey
                            : Key.A(decryptedApiKey, passwordEdited);
                        apiKeyProperty.stringValue = encryptedApiKey;
                        passwordProperty.stringValue = passwordEdited;
                    }

                    _height += passwordRect.height + EditorGUIUtility.standardVerticalSpacing;
                }

                var showRemoteConfigField = encryptionProperty.enumValueIndex == (int)ApiKeyEncryption.RemoteConfig;
                if (showRemoteConfigField) {
                    var remoteConfigKeyProperty =
                        property.FindPropertyRelative(nameof(SpeechToTextParameters.apiKeyRemoteConfigKey));
                    var remoteConfigKeyRect = new Rect(position.x, position.y + _height, width,
                                                       EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(remoteConfigKeyRect, remoteConfigKeyProperty,
                                            new GUIContent("Remote Config Key"));
                    _height += remoteConfigKeyRect.height + EditorGUIUtility.standardVerticalSpacing;
                }

                EditorGUI.indentLevel--;
            }

            // Space
            {
                var rect = new Rect(position.x, position.y + _height, width, EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.LabelField(rect, GUIContent.none);
                _height += rect.height + EditorGUIUtility.standardVerticalSpacing;
            }

            // Model
            {
                var modelProperty = property.FindPropertyRelative(nameof(SpeechToTextParameters.model));
                var rect = new Rect(position.x, position.y + _height, width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(rect, modelProperty);
                _height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            // Language
            {
                var languageProperty = property.FindPropertyRelative(nameof(SpeechToTextParameters.language));
                var rect = new Rect(position.x, position.y + _height, width - 20, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(rect, languageProperty);
                _height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Question mark button
                {
                    var buttonRect = new Rect(position.x + width - 20, position.y + _height - 20, 20,
                                              EditorGUIUtility.singleLineHeight);
                    EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);
                    var icon = EditorGUIUtility.IconContent("_Help");
                    icon.tooltip = "Open language codes";
                    if (GUI.Button(buttonRect, icon, EditorStyles.label)) {
                        Application.OpenURL("https://en.wikipedia.org/wiki/List_of_ISO_639_language_codes");
                    }
                }
            }

            // Prompt
            {
                var promptProperty = property.FindPropertyRelative(nameof(SpeechToTextParameters.prompt));
                var rect = new Rect(position.x, position.y + _height, width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(rect, promptProperty);
                _height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            // Temperature
            {
                var temperatureProperty = property.FindPropertyRelative(nameof(SpeechToTextParameters.temperature));
                var rect = new Rect(position.x, position.y + _height, width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(rect, temperatureProperty);
                _height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            // Space
            {
                var rect = new Rect(position.x, position.y + _height, width, EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.LabelField(rect, GUIContent.none);
                _height += rect.height + EditorGUIUtility.standardVerticalSpacing;
            }

            // Timeout
            {
                var timeoutProperty = property.FindPropertyRelative(nameof(SpeechToTextParameters.timeout));
                var useTimeout = timeoutProperty.intValue > 0;
                var toggleRect = new Rect(position.x, position.y + _height, EditorGUIUtility.labelWidth,
                                          EditorGUIUtility.singleLineHeight);

                EditorGUI.BeginChangeCheck();
                var timeoutLabel = new GUIContent(" Timeout", "The maximum number of seconds to wait for a response.");
                useTimeout = EditorGUI.ToggleLeft(toggleRect, timeoutLabel, useTimeout);
                if (EditorGUI.EndChangeCheck()) {
                    if (useTimeout && timeoutProperty.intValue == 0) {
                        timeoutProperty.intValue = 60;
                    }

                    if (!useTimeout && timeoutProperty.intValue > 0) {
                        timeoutProperty.intValue = 0;
                    }
                }

                var rect = new Rect(position.x + EditorGUIUtility.labelWidth - 15, position.y + _height,
                                    width - EditorGUIUtility.labelWidth + 15, EditorGUIUtility.singleLineHeight);

                EditorGUI.BeginDisabledGroup(!useTimeout);

                if (useTimeout) {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(rect, timeoutProperty, GUIContent.none);
                    if (EditorGUI.EndChangeCheck()) {
                        if (timeoutProperty.intValue < 1) {
                            timeoutProperty.intValue = 1;
                        }
                    }
                } else {
                    EditorGUI.LabelField(rect, "Disabled", EditorStyles.textField);
                }

                EditorGUI.EndDisabledGroup();

                _height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            // Throttle
            {
                var throttleProperty = property.FindPropertyRelative(nameof(SpeechToTextParameters.throttle));
                var useThrottle = throttleProperty.intValue > 0;
                var toggleRect = new Rect(position.x, position.y + _height, EditorGUIUtility.labelWidth,
                                          EditorGUIUtility.singleLineHeight);

                EditorGUI.BeginChangeCheck();
                var throttleLabel = new GUIContent(" Throttle", "The maximum number of concurrent requests.");
                useThrottle = EditorGUI.ToggleLeft(toggleRect, throttleLabel, useThrottle);
                if (EditorGUI.EndChangeCheck()) {
                    if (useThrottle && throttleProperty.intValue == 0) {
                        throttleProperty.intValue = 10;
                    }

                    if (!useThrottle && throttleProperty.intValue > 0) {
                        throttleProperty.intValue = 0;
                    }
                }

                var rect = new Rect(position.x + EditorGUIUtility.labelWidth - 15, position.y + _height,
                                    width - EditorGUIUtility.labelWidth + 15, EditorGUIUtility.singleLineHeight);

                EditorGUI.BeginDisabledGroup(!useThrottle);

                if (useThrottle) {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(rect, throttleProperty, GUIContent.none);
                    if (EditorGUI.EndChangeCheck()) {
                        if (throttleProperty.intValue < 1) {
                            throttleProperty.intValue = 1;
                        }
                    }
                } else {
                    EditorGUI.LabelField(rect, "Disabled", EditorStyles.textField);
                }

                EditorGUI.EndDisabledGroup();

                _height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            // Draw box around parameters
            {
                var indentWidth = EditorGUI.indentLevel * 15 - 5;
                var rect = new Rect(position.x + indentWidth, position.y + EditorGUIUtility.singleLineHeight,
                                    position.width - indentWidth,
                                    _height - EditorGUIUtility.singleLineHeight +
                                    EditorGUIUtility.standardVerticalSpacing);
                GUI.backgroundColor = Color.gray;
                EditorGUI.HelpBox(rect, null, MessageType.None);
            }

            // Space
            {
                var rect = new Rect(position.x, position.y + _height, width, EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.LabelField(rect, GUIContent.none);
                _height += rect.height + EditorGUIUtility.standardVerticalSpacing * 2;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return _height;
    }
}
}

#endif