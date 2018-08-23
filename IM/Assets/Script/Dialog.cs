using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour {
    
    GameObject text;                        //获取notice提示文本，存在这个变量里面进行操作  
    public static Vector3 vec3, pos;        //用于存放坐标，其变量类型也是Vector3，从官方文档抄来的，我也不知道为什么  

    //初始化函数  
    void Start()
    {
        text = GameObject.Find("notice");//获取提示文本  
        text.SetActive(false);//让提示文本隐藏  
        gameObject.SetActive(false);//让自己隐藏，也就是这个image  
    }

    // Update is called once per frame  
    void Update()
    {

    }

    //按下鼠标将会被触发的事件  
    public void PointerDown()
    {
        vec3 = Input.mousePosition;//获取当前鼠标位置  
        pos = transform.GetComponent<RectTransform>().position;//获取自己所在的位置  
    }

    //鼠标拖拽时候会被触发的事件  
    public void Drag()
    {
        Vector3 off = Input.mousePosition - vec3;
        //此处Input.mousePosition指鼠标拖拽结束的新位置  
        //减去刚才在按下时的位置，刚好就是鼠标拖拽的偏移量  
        vec3 = Input.mousePosition;//刷新下鼠标拖拽结束的新位置，用于下次拖拽的计算  
        pos = pos + off;//原来image所在的位置自然是要被偏移的  
        transform.GetComponent<RectTransform>().position = pos;//直接将自己刷新到新坐标  
    }

    //此函数接口将赋予给“弹出对话框”按钮的onClick事件  
    public void onShow()
    {
        gameObject.SetActive(true);//显示自己  
    }

    //此函数接口将赋予给“确认”按钮的onClick事件  
    public void onOK()
    {
        text.SetActive(true);//让提示文本显示  
        GameObject.Find("notice").GetComponent<Text>().text = "你点击了确定！";//并更改内容  
        gameObject.SetActive(false);//让自己隐藏  
    }

    //此函数接口将赋予给“取消”按钮的onClick事件  
    public void onCancel()
    {
        text.SetActive(true);//让提示文本显示  
        GameObject.Find("notice").GetComponent<Text>().text = "你点击了取消！";//并更改内容  
        gameObject.SetActive(false);//让自己隐藏  
    }

}
