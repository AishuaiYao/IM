using System.IO;
using System.Drawing;
using UnityEngine;
using System.Threading;
using System;
public class Live : MonoBehaviour {

    private Texture2D screen;
    private bool success = false;
    // Use this for initialization
    private void Start () {
        screen = new Texture2D(Screen.width, Screen.height);
        MicphoneCapture();
    }
	
	// Update is called once per frame
	private void Update () {
        
        Draw(PCScreen());
        
    }

    public byte[] CameraScreen()
    {
        Camera camera = GameObject.Find("Camera").GetComponent<Camera>();
        Rect rect = new Rect(0, 0, camera.pixelWidth, camera.pixelHeight);

        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);

        camera.targetTexture = rt;
        camera.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);

        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        camera.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(rt);

        byte[] bytes = screenShot.EncodeToJPG();  //bytes数组
        
        string filename = Application.streamingAssetsPath + "/CameraScreen.jpg";
        System.IO.File.WriteAllBytes(filename, bytes);

        return bytes;
    }
    public byte[] PCScreen()
    {
        //Thread.Sleep(200);
        System.Drawing.Bitmap bitmap = new Bitmap(Screen.width, Screen.height);

        System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);//创建画笔

        graphics.CopyFromScreen(new Point(0, 0), new Point(0, 0), bitmap.Size);//截屏
        
        string fileName = Application.streamingAssetsPath  + "/PCScreen.jpg";
        bitmap.Save(fileName);

        //将图片读回成数组
        FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length);

        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;
        bitmap.Dispose();
        graphics.Dispose();

        return bytes;
    }

    
    public bool Draw(byte[] bytes)
    {
        
        success = screen.LoadImage(bytes);
        //screen = RotateTexture(screen,180);
        gameObject.GetComponent<Renderer>().material.mainTexture = screen;
        
        return success;
    }
    
    
    public float[] MicphoneCapture()
    {
        AudioClip clip = new AudioClip();
        const int smaplingRate = 8000;
        float[] samples=new float[smaplingRate];

        string[] devices;
        devices = Microphone.devices;

        if (devices.Length > 0)
        {
            Debug.Log("开始录音！");
            Microphone.End(null);
            
            clip = Microphone.Start(null, false, 10, smaplingRate);
         
            Microphone.End(devices[0]);
            Debug.Log("结束录音！");
            clip.GetData(samples, 0);
            int i = 0;
            while (i < samples.Length)
            {
                samples[i] = samples[i] * 0.5F;
                ++i;
            }
        }
        else
        {
            Debug.Log("未检测到麦克风！");
        }
        
        //clip.SetData(samples, 0);
        //clip.Play();

        return samples;
    }
}


