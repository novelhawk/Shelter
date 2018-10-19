using System.Collections.Generic;
using UnityEngine;

namespace NGUI.UI
{
    public class UIGeometry
    {
        private readonly List<Vector3> _rtpVertices = new List<Vector3>();
        public readonly List<Color32> _colors = new List<Color32>();
        public readonly List<Vector2> _uvs = new List<Vector2>();
        public readonly List<Vector3> _vertices = new List<Vector3>();
        private Vector3 mRtpNormal;
        private Vector4 mRtpTan;

        public void ApplyOffset(Vector3 pivotOffset)
        {
            for (int i = 0; i < _vertices.Count; i++)
            {
                this._vertices[i] += pivotOffset;
            }
        }

        public void ApplyTransform(Matrix4x4 matrix)
        {
            if (this._vertices.Count > 0)
            {
                this._rtpVertices.Clear();
                foreach (var vertex in _vertices)
                {
                    _rtpVertices.Add(matrix.MultiplyPoint3x4(vertex));
                }
                mRtpNormal = matrix.MultiplyVector(Vector3.back).normalized;
                Vector3 normalized = matrix.MultiplyVector(Vector3.right).normalized;
                this.mRtpTan = new Vector4(normalized.x, normalized.y, normalized.z, -1f);
            }
            else
            {
                this._rtpVertices.Clear();
            }
        }

        public void Clear()
        {
            this._vertices.Clear();
            this._uvs.Clear();
            this._colors.Clear();
            this._rtpVertices.Clear();
        }

        public void WriteToBuffers(List<Vector3> v, List<Vector2> u, List<Color32> c, List<Vector3> n, List<Vector4> t)
        {
            if (this._rtpVertices != null && this._rtpVertices.Count > 0)
            {
                if (n == null)
                {
                    for (int i = 0; i < this._rtpVertices.Count; i++)
                    {
                        v.Add(this._rtpVertices[i]);
                        u.Add(this._uvs[i]);
                        c.Add(this._colors[i]);
                    }
                }
                else
                {
                    for (int j = 0; j < this._rtpVertices.Count; j++)
                    {
                        v.Add(this._rtpVertices[j]);
                        u.Add(this._uvs[j]);
                        c.Add(this._colors[j]);
                        n.Add(this.mRtpNormal);
                        t.Add(this.mRtpTan);
                    }
                }
            }
        }

        public bool HasVertices => _vertices.Count > 0;
    }
}

