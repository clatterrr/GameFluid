using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Convection : MonoBehaviour
{
    public GameObject prefab;
    private float PlaneSize = 10.0f;
    private const int QuadNumber = 16; //箭头在一个方向上的数量
    private GameObject[,] Arrow = new GameObject[QuadNumber, QuadNumber];
    private float[] ArrowRotation = new float[QuadNumber * QuadNumber];
    private float[] ArrowScale = new float[QuadNumber * QuadNumber];

    public Material ConvectionMat;//用于绘制平流
    public Material CheckerInit;//用于绘制初始棋盘格
    public Material CheckerMove;//用于绘制初始棋盘格

    public RenderTexture RtPreChecker;//棋盘格前一帧
    public RenderTexture RtNowChecker;//棋盘格这一帧

    public RenderTexture RtPreFrame;//前一帧图像
    public RenderTexture RtNowFrame;//本帧图像
    void Start()
    {
        Application.targetFrameRate = 10;
        float QuadSize = PlaneSize / QuadNumber;

        for (int j = 0; j < QuadNumber; j++)
        {
            for (int i = 0; i < QuadNumber; i++)
            {
                float posy = j * QuadSize - PlaneSize / 2.0f + QuadSize / 2.0f;
                float posx = i * QuadSize - PlaneSize / 2.0f + QuadSize / 2.0f;
                Arrow[i, j] = Instantiate(prefab, new Vector3(posx, 0.1f, posy), Quaternion.identity);
                Arrow[i, j].transform.localScale = new Vector3(2.0f,2.0f,2.0f);
                Arrow[i, j].transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                Arrow[i, j].transform.Rotate(new Vector3(0.0f, 0.0f, -90.0f));
                ArrowScale[j * QuadNumber + i] = 1.0f;

                ArrowRotation[j * QuadNumber + i] = 0.0f;
                int HalfNumber = QuadNumber / 2;
                if (i <= HalfNumber + 1 && i >= HalfNumber && j <= HalfNumber + 1 && j >= HalfNumber)
                {
                    ArrowScale[j * QuadNumber + i] = 2.0f;
                }
                if (i <= HalfNumber + 2 && i >= HalfNumber + 1 && j <= HalfNumber + 2 && j >= HalfNumber + 1)//右上角
                {
                    ArrowRotation[j * QuadNumber + i] = 315.0f;
                }
                if (i <= HalfNumber && i >= HalfNumber - 1 && j <= HalfNumber + 2 && j >= HalfNumber + 1)//左上角
                {
                    ArrowRotation[j * QuadNumber + i] = 45.0f;
                }
                if (i <= HalfNumber && i >= HalfNumber - 1 && j <= HalfNumber && j >= HalfNumber - 1)//左下角
                {
                    ArrowRotation[j * QuadNumber + i] = 135.0f;
                }
                if (i <= HalfNumber + 2 && i >= HalfNumber + 1 && j <= HalfNumber && j >= HalfNumber - 1)//右下角
                {
                    ArrowRotation[j * QuadNumber + i] = 225.0f;
                }
                Arrow[i, j].transform.Rotate(new Vector3(0.0f, 0.0f, ArrowRotation[j * QuadNumber + i]));
                Arrow[i, j].transform.localScale = new Vector3(ArrowScale[j * QuadNumber + i], ArrowScale[j * QuadNumber + i], ArrowScale[j * QuadNumber + i]);
            }
        }
        Graphics.Blit(null, RtPreFrame, CheckerInit);//绘制初始的黑白棋盘格纹理
        Graphics.Blit(RtPreFrame, RtPreChecker);
    }
    private void Update()
    {
        ConvectionMat.SetFloatArray("_ArrowRotation", ArrowRotation);
        ConvectionMat.SetFloatArray("_ArrowScale", ArrowScale);
        ConvectionMat.SetTexture("_CheckerTex", RtNowChecker);
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(RtPreChecker, RtNowChecker, CheckerMove);//移动棋盘格
        Graphics.Blit(RtNowChecker, RtPreChecker);

        Graphics.Blit(RtPreFrame, RtNowFrame, ConvectionMat);//移动之前的纹理
        Graphics.Blit(RtNowFrame, RtPreFrame);
        Graphics.Blit(source, destination);
    }
}
