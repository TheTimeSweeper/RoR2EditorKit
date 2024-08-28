﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

namespace RoR2.Editor
{
    /// <summary>
    /// A HelpBox is a custom VisualElement that works as a replacement for creating <see cref="IMGUIContainer"/> and calling <see cref="EditorGUILayout.HelpBox(GUIContent, bool)"/>
    /// <para>The HelpBox element works by imitating the behaviour of the EditorGUILayout method, but with a few extra utilities, such as making messages implicit (showing in the icon's tooltip), and allowing for easy creation of ContextualMenus</para>
    /// </summary>
    public class HelpBox : VisualElement
    {
        /// <summary>
        /// The Message that this HelpBox displays
        /// </summary>
        public string message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                if (messageIsExplicit)
                {
                    label.text = _message;
                }
                else
                {
                    tooltip = _message;
                }
            }
        }
        private string _message;

        /// <summary>
        /// The type of message for this helpBox, different values change the Icon that's used in the Help Box.
        /// <para><see cref="MessageType.None"/> will display a little page document icon</para>
        /// <para><see cref="MessageType.Info"/> will display a round information bubble</para>
        /// <para><see cref="MessageType.Warning"/> will display a yellow warning triangle</para>
        /// <para><see cref="MessageType.Error"/> will display an red error octagon</para>
        /// </summary>
        public MessageType messageType
        {
            get
            {
                return _messageType;
            }
            set
            {
                _messageType = value;
                if (icon == null)
                    return;

                switch (_messageType)
                {
                    case MessageType.None:
                        icon.style.backgroundImage = null;
                        break;
                    case MessageType.Info:
                        icon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("console.infoicon.sml@2x").image;
                        break;
                    case MessageType.Warning:
                        icon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("console.warnicon.sml@2x").image;
                        break;
                    case MessageType.Error:
                        icon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("console.erroricon.sml@2x").image;
                        break;
                }
            }
        }
        private MessageType _messageType;
        /// <summary>
        /// Wether the string inputed in <see cref="message"/> is Explicit.
        /// <para>When <see cref="messageIsExplicit"/> is set to true, a <see cref="UnityEngine.UIElements.Label"/> will be displayed alongside the icon, where the text in <see cref="message"/> is displayed in the label</para>
        /// <para>When <see cref="messageIsExplicit"/> is set to false, the text in <see cref="message"/> will be displayed as a tooltip when hovering over the HelpBox's Icon</para>
        /// </summary>
        public bool messageIsExplicit
        {
            get
            {
                return _messageIsExplicit;
            }
            set
            {
                _messageIsExplicit = value;
                if (_messageIsExplicit)
                {
                    contentContainer.style.width = StyleKeyword.Auto;
                }
                else
                {
                    contentContainer.style.width = new StyleLength(new Length(32f, LengthUnit.Pixel));
                }

                icon.tooltip = _messageIsExplicit ? String.Empty : message;
                label.text = _messageIsExplicit ? message : String.Empty;
                label.SetDisplay(_messageIsExplicit);
                label.SetEnabled(_messageIsExplicit);
            }
        }
        private bool _messageIsExplicit;

        public bool isDismissable
        {
            get => _isDismissable;
            set
            {
                _isDismissable = value;
                dismissButton.parent.SetDisplay(value);
            }
        }
        private bool _isDismissable;
        /// <summary>
        /// An Acton to populate a context menu when the HelpBox is clicked
        /// </summary>
        public Action<ContextualMenuPopulateEvent> ContextualMenuPopulateEvent
        {
            set
            {
                _contextualMenuPopulateEvent = value;
                if (_contextualMenuPopulateEvent == null && _manipulator != null)
                {
                    this.RemoveManipulator(_manipulator);
                    _manipulator = null;
                }
                if (_contextualMenuPopulateEvent != null && _manipulator == null)
                {
                    _manipulator = new ContextualMenuManipulator(_contextualMenuPopulateEvent);
                    this.AddManipulator(_manipulator);
                }
            }
        }
        private Action<ContextualMenuPopulateEvent> _contextualMenuPopulateEvent;
        private ContextualMenuManipulator _manipulator;
        /// <summary>
        /// The Icon visual element
        /// </summary>
        public VisualElement icon { get; }
        /// <summary>
        /// A Container element, this element gets resized whenever <see cref="messageIsExplicit"/> changes value
        /// </summary>
        public override VisualElement contentContainer { get; }
        /// <summary>
        /// The Label of the HelpBox
        /// </summary>
        public Label label { get; }

        /// <summary>
        /// A button that can be clicked to dismiss this helpbox.
        /// </summary>
        public Button dismissButton { get; }
        
        private void Init(AttachToPanelEvent evt)
        {
            dismissButton.clicked += Detach;
        }

        private void Detach()
        {
            parent.Remove(this);
        }

        /// <summary>
        /// Constructor for HelpBox
        /// </summary>
        public HelpBox()
        {
            VisualElementTemplateDictionary.instance.GetTemplateInstance(nameof(HelpBox), this);

            contentContainer = this.Q<VisualElement>("Container");
            icon = this.Q<VisualElement>("Icon");
            label = this.Q<Label>("Label");
            dismissButton = this.Q<Button>();

            RegisterCallback<AttachToPanelEvent>(Init);
        }

        ~HelpBox()
        {
            UnregisterCallback<AttachToPanelEvent>(Init);
        }


        /// <summary>
        /// Constructor for HelpBox
        /// </summary>
        public HelpBox(Action<ContextualMenuPopulateEvent> contextMenuPopulateEvent)
        {
            VisualElementTemplateDictionary.instance.GetTemplateInstance(nameof(HelpBox), this);

            contentContainer = this.Q<VisualElement>("Container");
            icon = this.Q<VisualElement>("Icon");
            label = this.Q<Label>("Label");
            dismissButton = this.Q<Button>();

            ContextualMenuPopulateEvent = contextMenuPopulateEvent;
            RegisterCallback<AttachToPanelEvent>(Init);
        }

        /// <summary>
        /// Constructor for HelpBox
        /// </summary>
        public HelpBox(string message, MessageType messageType, bool messageIsExplicit, bool canBeDismissed)
        {
            VisualElementTemplateDictionary.instance.GetTemplateInstance(nameof(HelpBox), this);

            contentContainer = this.Q<VisualElement>("Container");
            icon = this.Q<VisualElement>("Icon");
            label = this.Q<Label>("Label");
            dismissButton = this.Q<Button>();

            this.message = message;
            this.messageType = messageType;
            this.messageIsExplicit = messageIsExplicit;
            this.isDismissable = canBeDismissed;
            RegisterCallback<AttachToPanelEvent>(Init);
        }

        /// <summary>
        /// Constructor for HelpBox
        /// </summary>
        public HelpBox(string message, MessageType messageType, bool messageIsExplicit, bool canBeDismissed, Action<ContextualMenuPopulateEvent> contextMenuPopulateEvent)
        {
            VisualElementTemplateDictionary.instance.GetTemplateInstance(nameof(HelpBox), this);

            contentContainer = this.Q<VisualElement>("Container");
            icon = this.Q<VisualElement>("Icon");
            label = this.Q<Label>("Label");
            dismissButton = this.Q<Button>();

            this.message = message;
            this.messageType = messageType;
            this.messageIsExplicit = messageIsExplicit;
            this.isDismissable = canBeDismissed;
            ContextualMenuPopulateEvent = contextMenuPopulateEvent;
            RegisterCallback<AttachToPanelEvent>(Init);
        }

        public new class UxmlFactory : UxmlFactory<HelpBox, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription m_Message = new UxmlStringAttributeDescription
            {
                name = VisualElementUtil.NormalizeNameForUXMLTrait(nameof(message)),
                defaultValue = string.Empty
            };
            private UxmlEnumAttributeDescription<MessageType> m_MessageType = new UxmlEnumAttributeDescription<MessageType>
            {
                name = VisualElementUtil.NormalizeNameForUXMLTrait(nameof(messageType)),
                defaultValue = MessageType.None
            };
            private UxmlBoolAttributeDescription m_MessageIsExplicit = new UxmlBoolAttributeDescription
            {
                name = VisualElementUtil.NormalizeNameForUXMLTrait(nameof(messageIsExplicit)),
                defaultValue = true
            };
            private UxmlBoolAttributeDescription m_Dismissable = new UxmlBoolAttributeDescription
            {
                name = VisualElementUtil.NormalizeNameForUXMLTrait(nameof(isDismissable)),
                defaultValue = false
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var helpBox = (HelpBox)ve;
                helpBox.message = m_Message.GetValueFromBag(bag, cc);
                helpBox.messageIsExplicit = m_MessageIsExplicit.GetValueFromBag(bag, cc);
                helpBox.messageType = m_MessageType.GetValueFromBag(bag, cc);
                helpBox.isDismissable = m_Dismissable.GetValueFromBag(bag, cc);
            }
        }
    }
}