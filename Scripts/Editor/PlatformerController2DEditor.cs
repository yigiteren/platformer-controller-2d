using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Yern.PlatformerController2D.Editor
{
    [CustomEditor(typeof(PlatformerController2D))]
    public class PlatformerController2DEditor : UnityEditor.Editor
    {
        private PlatformerController2D _platformerController2D;

        private SerializedProperty _horizontalVelocityLimit;
        private SerializedProperty _horizontalAccelerationRate;
        private SerializedProperty _horizontalDecelerationRate;
        private SerializedProperty _flipXScaleOnDirectionChange;

        private SerializedProperty _gravity;
        private SerializedProperty _clampFallSpeed;
        private SerializedProperty _maximumFallSpeed;

        private SerializedProperty _enableJumping;
        private SerializedProperty _allowFirstJumpWhileFalling;
        private SerializedProperty _enableVariableJumpHeight;
        private SerializedProperty _jumpHeight;
        private SerializedProperty _jumpCount;
        private SerializedProperty _coyoteTime;
        private SerializedProperty _maxJumpBufferTime;

        private SerializedProperty _enableDashing;
        private SerializedProperty _dashPower;
        private SerializedProperty _dashTime;
        private SerializedProperty _allowMovementWhileDash;
        private SerializedProperty _dashMovementModifier;
        private SerializedProperty _allowDashWhileGrounded;
        private SerializedProperty _disableGravityWhileDashing;

        private SerializedProperty _groundCheckMask;
        private SerializedProperty _groundCheckRayCount;
        private SerializedProperty _groundCheckRayDistance;
        private SerializedProperty _enableGroundCheckStuckProtection;
        private SerializedProperty _groundCheckUnstuckThreshold;
        private SerializedProperty _groundCheckUnstuckVelocity;

        private SerializedProperty _ceilingCheckLayerMask;
        private SerializedProperty _ceilingCheckIgnoreTags;
        private SerializedProperty _ceilingCheckRayCount;
        private SerializedProperty _ceilingCheckRayDistance;

        private SerializedProperty _horizontalCheckLayerMask;
        private SerializedProperty _horizontalCheckRayCount;
        private SerializedProperty _horizontalCheckRayDistance;

        private GUIStyle _titleStyle;
        private GUIStyle _subtitleStyle;

        private void OnEnable()
        {
            _platformerController2D = serializedObject.targetObject as PlatformerController2D;

            _horizontalVelocityLimit = serializedObject.FindProperty("horizontalVelocityLimit");
            _horizontalAccelerationRate = serializedObject.FindProperty("horizontalAccelerationRate");
            _horizontalDecelerationRate = serializedObject.FindProperty("horizontalDecelerationRate");
            _flipXScaleOnDirectionChange = serializedObject.FindProperty("flipXScaleOnDirectionChange");
            _gravity = serializedObject.FindProperty("gravity");
            _clampFallSpeed = serializedObject.FindProperty("clampFallSpeed");
            _maximumFallSpeed = serializedObject.FindProperty("maximumFallSpeed");
            _enableJumping = serializedObject.FindProperty("enableJumping");
            _allowFirstJumpWhileFalling = serializedObject.FindProperty("allowFirstJumpWhileFalling");
            _enableVariableJumpHeight = serializedObject.FindProperty("enableVariableJumpHeight");
            _jumpHeight = serializedObject.FindProperty("jumpHeight");
            _jumpCount = serializedObject.FindProperty("jumpCount");
            _coyoteTime = serializedObject.FindProperty("coyoteTime");
            _maxJumpBufferTime = serializedObject.FindProperty("maxJumpBufferTime");
            _enableDashing = serializedObject.FindProperty("enableDashing");
            _dashPower = serializedObject.FindProperty("dashPower");
            _dashTime = serializedObject.FindProperty("dashTime");
            _allowMovementWhileDash = serializedObject.FindProperty("allowMovementWhileDash");
            _dashMovementModifier = serializedObject.FindProperty("dashMovementModifier");
            _allowDashWhileGrounded = serializedObject.FindProperty("allowDashWhileGrounded");
            _disableGravityWhileDashing = serializedObject.FindProperty("disableGravityWhileDashing");
            _groundCheckMask = serializedObject.FindProperty("groundCheckMask");
            _groundCheckRayCount = serializedObject.FindProperty("groundCheckRayCount");
            _groundCheckRayDistance = serializedObject.FindProperty("groundCheckRayDistance");
            _enableGroundCheckStuckProtection = serializedObject.FindProperty("enableGroundCheckStuckProtection");
            _groundCheckUnstuckThreshold = serializedObject.FindProperty("groundCheckUnstuckThreshold");
            _groundCheckUnstuckVelocity = serializedObject.FindProperty("groundCheckUnstuckVelocity");
            _ceilingCheckLayerMask = serializedObject.FindProperty("ceilingCheckLayerMask");
            _ceilingCheckIgnoreTags = serializedObject.FindProperty("ceilingCheckIgnoreTags");
            _ceilingCheckRayCount = serializedObject.FindProperty("ceilingCheckRayCount");
            _ceilingCheckRayDistance = serializedObject.FindProperty("ceilingCheckRayDistance");
            _horizontalCheckLayerMask = serializedObject.FindProperty("horizontalCheckLayerMask");
            _horizontalCheckRayCount = serializedObject.FindProperty("horizontalCheckRayCount");
            _horizontalCheckRayDistance = serializedObject.FindProperty("horizontalCheckRayDistance");

            _titleStyle = new GUIStyle
            {
                fontSize = 22,
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = GUI.color
                }
            };
            
            _subtitleStyle = new GUIStyle
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = GUI.color
                }
            };

            if (_platformerController2D)
                _platformerController2D.OnStateChange += HandleControllerStateChange;
        }

        private void OnDisable()
        {
            if (_platformerController2D)
                _platformerController2D.OnStateChange -= HandleControllerStateChange;
        }

        private void HandleControllerStateChange(PlatformerController2DState state)
        {
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("<b>Yern's Platformer Controller 2D</b>", _titleStyle);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField($"Controller State: {_platformerController2D.State}", _subtitleStyle);
            
            EditorGUILayout.Space();

            #region Horizontal Movement Settings
            EditorGUILayout.LabelField("Horizontal Movement Settings", EditorStyles.boldLabel);
            _horizontalVelocityLimit.floatValue = EditorGUILayout.FloatField("Horizontal Velocity Limit", _horizontalVelocityLimit.floatValue);
            _horizontalAccelerationRate.floatValue = EditorGUILayout.Slider("Horizontal Acceleration Rate", _horizontalAccelerationRate.floatValue, 0, 1f);
            _horizontalDecelerationRate.floatValue = EditorGUILayout.Slider("Horizontal Deceleration Rate", _horizontalDecelerationRate.floatValue, 0f, 1f);
            _flipXScaleOnDirectionChange.boolValue = EditorGUILayout.Toggle("Flip X Scale On Direction Change", _flipXScaleOnDirectionChange.boolValue);
            #endregion
            
            EditorGUILayout.Space();

            #region Gravity Settings
            EditorGUILayout.LabelField("Gravity Settings", EditorStyles.boldLabel);
            _gravity.floatValue = EditorGUILayout.FloatField("Gravity", _gravity.floatValue);
            _clampFallSpeed.boolValue = EditorGUILayout.Toggle("Clamp Fall Speed", _clampFallSpeed.boolValue);

            if (_clampFallSpeed.boolValue)
            {
                EditorGUI.indentLevel = 1;
                _maximumFallSpeed.floatValue = EditorGUILayout.FloatField("└ Maximum Fall Speed", _maximumFallSpeed.floatValue);
                EditorGUI.indentLevel = 0;
            }
            #endregion
            
            EditorGUILayout.Space();

            #region Jump Settings
            EditorGUILayout.LabelField("Jump Settings", EditorStyles.boldLabel);
            _enableJumping.boolValue = EditorGUILayout.Toggle("Enable Jumping", _enableJumping.boolValue);
            
            if (_enableJumping.boolValue)
            {
                EditorGUI.indentLevel = 1;
                _enableVariableJumpHeight.boolValue = EditorGUILayout.Toggle("└ Enable Variable Jump Height", _enableVariableJumpHeight.boolValue);
                _jumpHeight.floatValue = EditorGUILayout.FloatField("└ Jump Height", _jumpHeight.floatValue);
                _jumpCount.intValue = EditorGUILayout.IntField("└ Jump Count", _jumpCount.intValue);
                
                _allowFirstJumpWhileFalling.boolValue = EditorGUILayout.Toggle("└ Allow First Jump While Falling", _allowFirstJumpWhileFalling.boolValue);

                if (!_allowFirstJumpWhileFalling.boolValue)
                {
                    EditorGUI.indentLevel = 2;
                    _coyoteTime.floatValue = EditorGUILayout.FloatField("└ Coyote Time", _coyoteTime.floatValue);
                    EditorGUI.indentLevel = 1;
                }

                _maxJumpBufferTime.floatValue = EditorGUILayout.FloatField("└ Jump Buffer Time", _maxJumpBufferTime.floatValue);
                EditorGUI.indentLevel = 0;
            }
            #endregion
            
            EditorGUILayout.Space();

            #region Dash Settings
            EditorGUILayout.LabelField("Dash Settings", EditorStyles.boldLabel);
            _enableDashing.boolValue = EditorGUILayout.Toggle("Enable Dashing", _enableDashing.boolValue);

            if (_enableDashing.boolValue)
            {
                EditorGUI.indentLevel = 1;
                _dashPower.floatValue = EditorGUILayout.FloatField("└ Dash Power", _dashPower.floatValue);
                _dashTime.floatValue = EditorGUILayout.FloatField("└ Dash Duration", _dashTime.floatValue);
                
                EditorGUI.indentLevel = 2;
                EditorGUILayout.LabelField($"└ Calculated Dash Distance: {_dashPower.floatValue * _dashTime.floatValue}m");
                EditorGUI.indentLevel = 1;
                
                _allowMovementWhileDash.boolValue = EditorGUILayout.Toggle("└ Allow Movement While Dashing", _allowMovementWhileDash.boolValue);

                if (_allowMovementWhileDash.boolValue)
                {
                    EditorGUI.indentLevel = 2;
                    _dashMovementModifier.floatValue = EditorGUILayout.FloatField("└ Dash Movement Modifier", _dashMovementModifier.floatValue);
                    EditorGUI.indentLevel = 1;
                }
                
                _allowDashWhileGrounded.boolValue = EditorGUILayout.Toggle("└ Allow Dash While Grounded", _allowDashWhileGrounded.boolValue);
                _disableGravityWhileDashing.boolValue = EditorGUILayout.Toggle("└ Disable Gravity While Dashing", _disableGravityWhileDashing.boolValue);
                EditorGUI.indentLevel = 0;
            }
            #endregion
            
            EditorGUILayout.Space();

            #region Ground Check Settings
            EditorGUILayout.LabelField("Ground Check Settings", EditorStyles.boldLabel);
            _groundCheckMask.intValue = EditorGUILayout.MaskField("Ground Check Mask", _groundCheckMask.intValue, InternalEditorUtility.layers);
            _groundCheckRayCount.intValue = EditorGUILayout.IntField("Ground Check Ray Count", _groundCheckRayCount.intValue);
            _groundCheckRayDistance.floatValue = EditorGUILayout.FloatField("Ground Check Ray Distance", _groundCheckRayDistance.floatValue);
            _enableGroundCheckStuckProtection.boolValue = EditorGUILayout.Toggle("Enable Ground Check Stuck Protection", _enableGroundCheckStuckProtection.boolValue);

            if (_enableGroundCheckStuckProtection.boolValue)
            {
                EditorGUI.indentLevel = 1;
                _groundCheckUnstuckThreshold.floatValue = EditorGUILayout.Slider("└ Ground Check Unstuck Threshold", _groundCheckUnstuckThreshold.floatValue, 0f, _groundCheckRayDistance.floatValue);
                _groundCheckUnstuckVelocity.floatValue = EditorGUILayout.FloatField("└ Ground Check Unstuck Velocity", _groundCheckUnstuckVelocity.floatValue);
                EditorGUI.indentLevel = 0;
            }
            #endregion
            
            EditorGUILayout.Space();

            #region Ceiling Check Settings
            EditorGUILayout.LabelField("Ceiling Check Settings", EditorStyles.boldLabel);
            _ceilingCheckLayerMask.intValue = EditorGUILayout.MaskField("Ceiling Check Mask", _ceilingCheckLayerMask.intValue, InternalEditorUtility.layers);
            _ceilingCheckRayCount.intValue = EditorGUILayout.IntField("Ceiling Check Ray Count", _ceilingCheckRayCount.intValue);
            _ceilingCheckRayDistance.floatValue = EditorGUILayout.FloatField("Ceiling Check Ray Distance", _ceilingCheckRayDistance.floatValue);
            EditorGUILayout.PropertyField(_ceilingCheckIgnoreTags, true);
            #endregion
            
            EditorGUILayout.Space();

            #region Horizontal Check Settings
            EditorGUILayout.LabelField("Horizontal Check Settings", EditorStyles.boldLabel);
            _horizontalCheckLayerMask.intValue = EditorGUILayout.MaskField("Horizontal Check Mask", _horizontalCheckLayerMask.intValue, InternalEditorUtility.layers);
            _horizontalCheckRayCount.intValue = EditorGUILayout.IntField("Horizontal Check Ray Count", _horizontalCheckRayCount.intValue);
            _horizontalCheckRayDistance.floatValue = EditorGUILayout.FloatField("Horizontal Check Ray Distance", _horizontalCheckRayDistance.floatValue);
            #endregion

            #region Value Controls

            if (_groundCheckRayCount.intValue < 2) _groundCheckRayCount.intValue = 2;
            if (_ceilingCheckRayCount.intValue < 2) _ceilingCheckRayCount.intValue = 2;
            if (_horizontalCheckRayCount.intValue < 2) _horizontalCheckRayCount.intValue = 2;

            #endregion
            
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}