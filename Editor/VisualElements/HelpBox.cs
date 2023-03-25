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

namespace RoR2EditorKit.VisualElements
{
    public class HelpBox : VisualElement
    {
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

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var helpBox = (HelpBox)ve;
                helpBox.message = m_Message.GetValueFromBag(bag, cc);
                helpBox.messageIsExplicit = m_MessageIsExplicit.GetValueFromBag(bag, cc);
                helpBox.messageType = m_MessageType.GetValueFromBag(bag, cc);
            }
        }

        public string message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                if(messageIsExplicit)
                {
                    Label.text = _message;
                }
                else
                {
                    tooltip = _message;
                }
            }
        }
        private string _message;
        public MessageType messageType
        {
            get
            {
                return _messageType;
            }
            set
            {
                _messageType = value;
                if (Icon == null)
                    return;

                switch (_messageType)
                {
                    case MessageType.None:
                        Icon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow@2x").image;
                        break;
                    case MessageType.Info:
                        Icon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("console.infoicon.sml@2x").image;
                        break;
                    case MessageType.Warning:
                        Icon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("console.warnicon.sml@2x").image;
                        break;
                    case MessageType.Error:
                        Icon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("console.erroricon.sml@2x").image;
                        break;
                }
            }
        }
        private MessageType _messageType;
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
                    Container.style.width = StyleKeyword.Auto;
                }
                else
                {
                    Container.style.width = new StyleLength(new Length(32f, LengthUnit.Pixel));
                }
                
                Icon.tooltip = _messageIsExplicit ? String.Empty : message;
                Label.text = _messageIsExplicit ? message : String.Empty;
                Label.SetDisplay(_messageIsExplicit);
                Label.SetEnabled(_messageIsExplicit);
            }
        }
        private bool _messageIsExplicit;
        public Action<ContextualMenuPopulateEvent> ContextualMenuPopulateEvent
        {
            set
            {
                _contextualMenuPopulateEvent = value;
                if(_contextualMenuPopulateEvent == null && _manipulator != null)
                {
                    this.RemoveManipulator(_manipulator);
                    _manipulator = null;
                }
                if(_contextualMenuPopulateEvent != null && _manipulator == null)
                {
                    _manipulator = new ContextualMenuManipulator(_contextualMenuPopulateEvent);
                    this.AddManipulator(_manipulator);
                }
            }
        }
        private Action<ContextualMenuPopulateEvent> _contextualMenuPopulateEvent;
        private ContextualMenuManipulator _manipulator;
        public VisualElement Icon { get; }
        public VisualElement Container { get; }
        public Label Label { get; }

        public HelpBox()
        {
            ThunderKit.Core.UIElements.TemplateHelpers.GetTemplateInstance(nameof(HelpBox), this, (_) => true);

            Container = this.Q<VisualElement>(nameof(Container));
            Icon = this.Q<VisualElement>(nameof(Icon));
            Label = this.Q<Label>(nameof(Label));
        }
        public HelpBox(Action<ContextualMenuPopulateEvent> contextMenuPopulateEvent)
        {
            ThunderKit.Core.UIElements.TemplateHelpers.GetTemplateInstance(nameof(HelpBox), this, (_) => true);

            Container = this.Q<VisualElement>(nameof(Container));
            Icon = this.Q<VisualElement>(nameof(Icon));
            Label = this.Q<Label>(nameof(Label));

            ContextualMenuPopulateEvent = contextMenuPopulateEvent;
        }

        public HelpBox(string message, MessageType messageType, bool messageIsExplicit)
        {
            ThunderKit.Core.UIElements.TemplateHelpers.GetTemplateInstance(nameof(HelpBox), this, (_) => true);

            Container = this.Q<VisualElement>(nameof(Container));
            Icon = this.Q<VisualElement>(nameof(Icon));
            Label = this.Q<Label>(nameof(Label));

            this.message = message;
            this.messageType = messageType;
            this.messageIsExplicit = messageIsExplicit;
        }

        public HelpBox(string message, MessageType messageType, bool messageIsExplicit, Action<ContextualMenuPopulateEvent> contextMenuPopulateEvent)
        {
            ThunderKit.Core.UIElements.TemplateHelpers.GetTemplateInstance(nameof(HelpBox), this, (_) => true);

            Container = this.Q<VisualElement>(nameof(Container));
            Icon = this.Q<VisualElement>(nameof(Icon));
            Label = this.Q<Label>(nameof(Label));

            this.message = message;
            this.messageType = messageType;
            this.messageIsExplicit = messageIsExplicit;
            ContextualMenuPopulateEvent = contextMenuPopulateEvent;
        }
    }
}
