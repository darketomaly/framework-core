/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using UnityEngine;
using System.Collections;

namespace Framework
{
    /// <summary>
    /// Fades the screen from black after a new scene is loaded. Fade can also be controlled mid-scene using SetUIFade and SetFadeLevel
    /// </summary>
    [HelpURL("https://developer.oculus.com/reference/unity/latest/class_o_v_r_screen_fade")]
    public class ScreenFade : MonoBehaviour
    {
        public static ScreenFade Instance { get; private set; }

        /// <summary>
        /// Raised when a fade begins.
        /// </summary>
        /// <param name="isFadingIn">True if fading in, false if fading out.</param>
        public Action<bool> OnFadeStart;

        [Tooltip("Fade duration")]
        public float m_FadeTime = 2.0f;

        [Tooltip("Screen color at maximum fade")]
        public Color m_FadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);

        public bool m_FadeOnStart = true;

        /// <summary>
        /// The render queue used by the fade mesh. Reduce this if you need to render on top of it.
        /// </summary>
        public int m_RenderQueue = 5000;

        /// <summary>
        /// Renders the current alpha value being used to fade the screen.
        /// </summary>
        public float CurrentAlpha => Mathf.Max(m_explicitFadeAlpha, m_animatedFadeAlpha, m_uiFadeAlpha);

        private float m_explicitFadeAlpha;
        private float m_animatedFadeAlpha;
        private float m_uiFadeAlpha;

        private MeshRenderer m_fadeRenderer;
        private MeshFilter m_fadeMesh;
        private Material m_fadeMaterial;
        private bool m_isFading;

        /// <summary>
        /// Automatically starts a fade in
        /// </summary>
        private void Start()
        {
            if (gameObject.name.StartsWith("OculusMRC_"))
            {
                Destroy(this);
                return;
            }

            // create the fade material
            m_fadeMaterial = new Material(Shader.Find("Framework/Unlit Transparent Color"));
            m_fadeMesh = gameObject.AddComponent<MeshFilter>();
            m_fadeRenderer = gameObject.AddComponent<MeshRenderer>();

            var mesh = new Mesh();
            m_fadeMesh.mesh = mesh;

            Vector3[] vertices = new Vector3[4];

            float width = 2f;
            float height = 2f;
            float depth = 1f;

            vertices[0] = new Vector3(-width, -height, depth);
            vertices[1] = new Vector3(width, -height, depth);
            vertices[2] = new Vector3(-width, height, depth);
            vertices[3] = new Vector3(width, height, depth);

            mesh.vertices = vertices;

            int[] tri = new int[6];

            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            mesh.triangles = tri;

            Vector3[] normals = new Vector3[4];

            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;

            mesh.normals = normals;

            Vector2[] uv = new Vector2[4];

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            mesh.uv = uv;

            m_explicitFadeAlpha = 0.0f;
            m_animatedFadeAlpha = 0.0f;
            m_uiFadeAlpha = 0.0f;

            if (m_FadeOnStart)
            {
                FadeIn();
            }

            Instance = this;
        }

        /// <summary>
        /// Start a fade in
        /// </summary>
        public void FadeIn()
        {
            StopAllCoroutines();
            StartCoroutine(Fade(1.0f, 0.0f));
            OnFadeStart?.Invoke(true);
        }

        /// <summary>
        /// Start a fade out
        /// </summary>
        public void FadeOut()
        {
            StopAllCoroutines();
            StartCoroutine(Fade(0, 1));
            OnFadeStart?.Invoke(false);
        }

        /// <summary>
        /// Starts a fade in when a new level is loaded
        /// </summary>
        private void OnLevelFinishedLoading(int level)
        {
            FadeIn();
        }

        private void OnEnable()
        {
            if (!m_FadeOnStart)
            {
                m_explicitFadeAlpha = 0.0f;
                m_animatedFadeAlpha = 0.0f;
                m_uiFadeAlpha = 0.0f;
            }
        }

        /// <summary>
        /// Cleans up the fade material
        /// </summary>
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            if (m_fadeRenderer != null)
                Destroy(m_fadeRenderer);

            if (m_fadeMaterial != null)
                Destroy(m_fadeMaterial);

            if (m_fadeMesh != null)
                Destroy(m_fadeMesh);
        }

        /// <summary>
        /// Set the UI fade level - fade due to UI in foreground
        /// </summary>
        public void SetUIFade(float level)
        {
            m_uiFadeAlpha = Mathf.Clamp01(level);
            SetMaterialAlpha();
        }

        /// <summary>
        /// Override current fade level
        /// </summary>
        /// <param name="level"></param>
        public void SetExplicitFade(float level)
        {
            m_explicitFadeAlpha = level;
            SetMaterialAlpha();
        }

        /// <summary>
        /// Fades alpha from 1.0 to 0.0
        /// </summary>
        private IEnumerator Fade(float startAlpha, float endAlpha)
        {
            float elapsedTime = 0.0f;
            
            while (elapsedTime < m_FadeTime)
            {
                elapsedTime += Time.deltaTime;
                m_animatedFadeAlpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / m_FadeTime));
                SetMaterialAlpha();
                yield return new WaitForEndOfFrame();
            }

            m_animatedFadeAlpha = endAlpha;
            SetMaterialAlpha();
        }

        /// <summary>
        /// Update material alpha. UI fade and the current fade due to fade in/out animations (or explicit control)
        /// both affect the fade. (The max is taken)
        /// </summary>
        private void SetMaterialAlpha()
        {
            Color color = m_FadeColor;
            color.a = CurrentAlpha;
            m_isFading = color.a > 0;
            
            if (m_fadeMaterial)
            {
                m_fadeMaterial.color = color;
                m_fadeMaterial.renderQueue = m_RenderQueue;
                m_fadeRenderer.material = m_fadeMaterial;
                m_fadeRenderer.enabled = m_isFading;
            }
        }
    }
}