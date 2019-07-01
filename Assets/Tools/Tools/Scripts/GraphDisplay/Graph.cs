using System.Collections.Generic;
using UnityEngine;

namespace Tools.GraphDisplay
{
    public class Graph
    {


        private static Shader shaderFull;

        public Shader ShaderFull
        {
            get
            {
                if (shaderFull == null)
                    shaderFull = Shader.Find("Tools/Graph Standard");
                return shaderFull;
            }
        }

        private float limit = 1 / 5f;

        public float Limit
        {
            get
            {
                return limit;
            }
            set
            {
                if (value != limit)
                {
                    limit = value;
                    graphShader.GoodThreshold = limit / max;
                    graphShader.CautionThreshold = max;
                    graphShader.UpdateThresholds();
                }
            }
        }

        private float max = 1;

        public float Max
        {
            get
            {
                return max;
            }
            set
            {
                if (value != max)
                {
                    max = value;
                    graphShader.GoodThreshold = limit / max;
                    graphShader.CautionThreshold = max;
                    graphShader.UpdateThresholds();
                }
            }
        }

        private int nextDataId;
        private int m_graphDataSize;
        private float[] dataArray;
        private Queue<float> dataQueue = new Queue<float>();

        private GraphShader graphShader = new GraphShader();

        public Graph(UnityEngine.UI.Image image, int graphDataSize = 50)
        {
            graphShader.Image = image;
            if (graphDataSize > 1023)
                graphDataSize = 1023;
            this.m_graphDataSize = graphDataSize;
            graphShader.ArrayMaxSize = m_graphDataSize;
            dataArray = new float[m_graphDataSize];
            graphShader.Image.material = new Material(ShaderFull);
            graphShader.InitializeShader();
            graphShader.Array = dataArray;
            graphShader.UpdateArray();
            graphShader.GoodColor = Color.green;
            graphShader.CautionColor = Color.red;
            graphShader.CriticalColor = Color.red - new Color(0.2f, 0.2f, 0.2f);
            graphShader.UpdateColors();
            graphShader.GoodThreshold = limit / max;
            graphShader.CautionThreshold = max;
            graphShader.UpdateThresholds();
        }

        public void AddData(long data)
        {
            dataArray[nextDataId] = data / Max;
            nextDataId = (nextDataId + 1) % m_graphDataSize;
        }

        public void AddData(float data)
        {
            dataArray[nextDataId] = data / Max;
            nextDataId = (nextDataId + 1) % m_graphDataSize;
        }

        public void PushData(long data)
        {
            dataQueue.Enqueue(data / Max);
            if (dataQueue.Count > m_graphDataSize)
                dataQueue.Dequeue();
            dataArray = dataQueue.ToArray();
        }

        public void PushData(float data)
        {
            dataQueue.Enqueue(data / Max);
            if (dataQueue.Count > m_graphDataSize)
                dataQueue.Dequeue();
            dataArray = dataQueue.ToArray();
        }

        public void UpdateGraph()
        {
            graphShader.Array = dataArray;
            graphShader.UpdatePoints();
        }
    }
}
