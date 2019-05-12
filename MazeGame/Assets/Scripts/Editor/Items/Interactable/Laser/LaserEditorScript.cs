using System.Collections;
using System.Collections.Generic;
using Game.Items.Activators.Button;
using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Intractable.Laser
{
    [CustomEditor(typeof(Game.Items.Intractable.Laser.Laser))]
    public class LaserEditorScript : Editor, ITem, ITemButtonInteraction
    {
        private Game.Items.Intractable.Laser.Laser _laser;
        private bool _buttonMappingMode;

        private void OnEnable()
        {
            Init();
        }
        
        private void OnSceneGUI()
        {
            DrawAddButtonLinkHandle();
            DrawDeletionHandle();
            DrawLaserActivationHandles();
            if (_buttonMappingMode)
            {
                DrawAllUnUsedButtonHandles();
            }
        }
        
        public void DrawAddButtonLinkHandle()
        {
            if (!_buttonMappingMode)
            {
                Handles.color = new Color(0.14f, 0.93f, 1f);
            }
            else
            {
                Handles.color = new Color(1f, 0.73f, 0.18f);                
            }
            if (Handles.Button(_laser.transform.position + _laser.transform.forward, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _buttonMappingMode = !_buttonMappingMode;
            }
        }
        
        private void DrawDeletionHandle()
        {
            Handles.color = new Color(1f, 0f, 0.07f);
            if (Handles.Button(_laser.transform.position + _laser.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                if (!EditorUtility.DisplayDialog("Warning!!",
                    "This will delete the item permanently", "Cancel", "Continue"))
                {
                    RemoveItem();
                }
            }
        }

        private void DrawLaserActivationHandles()
        {
            //drawing the activated laser wings
            if (_laser.right)
            {
                Handles.color = new Color(1f, 0.55f, 0.45f);
            }
            else
            {
                Handles.color = new Color(0.55f, 0.64f, 1f);                
            }
            if (Handles.Button(_laser.transform.position + _laser.transform.forward * 0.6f - _laser.transform.up * 0.3f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _laser.right = !_laser.right;
            }

            if (_laser.left)
            {
                Handles.color = new Color(1f, 0.55f, 0.45f);
            }
            else
            {
                Handles.color = new Color(0.55f, 0.64f, 1f);                
            }
            if (Handles.Button(_laser.transform.position + _laser.transform.forward * 0.6f + _laser.transform.up * 0.3f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _laser.left = !_laser.left;           
            }

            if (_laser.forward)
            {
                Handles.color = new Color(1f, 0.55f, 0.45f);
            }
            else
            {
                Handles.color = new Color(0.55f, 0.64f, 1f);                
            }
            if (Handles.Button(_laser.transform.position + _laser.transform.forward * 0.6f - _laser.transform.right * 0.3f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _laser.forward = !_laser.forward;
            }

            if (_laser.back)
            {
                Handles.color = new Color(1f, 0.55f, 0.45f);
            }
            else
            {
                Handles.color = new Color(0.55f, 0.64f, 1f);                
            }
            if (Handles.Button(_laser.transform.position + _laser.transform.forward * 0.6f + _laser.transform.right * 0.3f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _laser.back = !_laser.back;
            }
            
            _laser.transform.GetChild(1).gameObject.SetActive(_laser.right);
            _laser.transform.GetChild(2).gameObject.SetActive(_laser.left);
            _laser.transform.GetChild(3).gameObject.SetActive(_laser.forward);
            _laser.transform.GetChild(4).gameObject.SetActive(_laser.back);
        }
        
        public void DrawAllUnUsedButtonHandles()
        {
            foreach (var activator in LevelEditor.AllItems[(int)ItemCategories.Activator])
            {
                var button = activator.GetComponent<Button>();
                Handles.color = new Color(0.25f, 1f, 0.67f);                
                if (!button.itemSet)
                {
                    if (Handles.Button(activator.transform.position + activator.transform.up * 0.2f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
                    {
                        AddButtonLink(button);
                    }
                }
            }
        }
        
        public void Init()
        {
            _laser = (Game.Items.Intractable.Laser.Laser)target;
            _laser.name = "Laser";
        }

        public void AddItem()
        {
            throw new System.NotImplementedException();
        }

        public void EditItem()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveItem()
        {
            LevelEditor.AllItems[(int) ItemCategories.Intractable].Remove(_laser.gameObject);
            DestroyImmediate(_laser.gameObject);
            LevelEditor.ReCalculateAllItems();
        }

        public bool CheckValuesSet()
        {
            throw new System.NotImplementedException();
        }

        public void AddButtonLink(Button button)
        {
            throw new System.NotImplementedException();
        }

        public void EditButtonLink()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveButtonLink(int buttonId)
        {
            throw new System.NotImplementedException();
        }
    }
}
