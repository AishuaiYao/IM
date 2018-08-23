using System.Net;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class IM : MonoBehaviour
{
    //数据包
    public struct Message
    {
        public string username;
        public string headportrait;
        //public IPAddress ipaddress;
        //public string identity;
        public string message;
        public bool online;

    };

    //每个消息标签高度宽度，与边缘的距离
    private const int labelHeight = 50;
    private const int labelWidth = 260;
    private const int Margins = 2;
    //文字大小
    private const int fontSize = 13;
    private const int fontCapacity = 14;
    private const int nameHeight = 20;


    //生成消息labal的位置记录
    private static float contentSize = 0;
    //生成好友label标签的预制体计数
    private static int i = 0;
    //未读消息记录
    private int unDealMessages = 0;

    private bool CHECKEDFRIENDS;
    private bool CHECKEDMESSAGES;

 

    //聊天区域
    [SerializeField]
    private GameObject MESSAGEPANEL;
    private RectTransform rtMESSAGEPANEL;
    [SerializeField]
    private GameObject FRIENDSPANEL;
    private RectTransform rtFRIENDSPANEL;
    [SerializeField]
    private GameObject messagePanelContent;
    private RectTransform rtMessagePanelContent;
    [SerializeField]
    private GameObject friendsPanelContent;
    private RectTransform rtFriendsPanelContent;
    //聊天框
    [SerializeField]
    private InputField inputField;


    //导航栏
    [SerializeField]
    private Text textOfMessages;
    [SerializeField]
    private Text textOfFriends;
    [SerializeField]
    private Image underline;
    [SerializeField]
    private Text onlineUsers;


    //获得提醒
    [SerializeField]
    private Image remind;
    [SerializeField]
    private Text remindText;
    private RectTransform rtRemind;


    //top部位信息
    [SerializeField]
    private GameObject identityData;
    private RectTransform rtIdentityData;
    [SerializeField]
    private Text myName;
    [SerializeField]
    private Image myHeadPortrait;
    [SerializeField]
    private Text myNewName;
    
    public delegate void HaveMessage(object sender, Message message);
    public event HaveMessage MessageEvent;

    private Queue<Message> queue = new Queue<Message>();

    private void Start()
    {
        
        MessageEvent += new HaveMessage(ShowMessages);             //将委托绑定到事件上
        GetComponentsRT();
        identityData.SetActive(false);
        remind.enabled = false;
    }

    private void Update()
    {
        //if(Input.anyKeyDown)
        //{
        //foreach(KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        //{
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Debug.Log("Current Key is : " + keyCode.ToString());
            //if (keyCode.ToString() == "Return")
            Send();
        }
        // }
        //}
        
        
    }

    private void ShowMessages(object obj, Message message)
    {
        GameObject friendMessage = Resources.Load("Prefabs/FriendMessage") as GameObject;

        friendMessage = Instantiate(friendMessage);
        friendMessage.transform.SetParent(messagePanelContent.transform);
        
        Text messageContent = GameObject.FindObjectOfType<Text>();  //获得刚刚实例化的文本
        RectTransform rtFriendMessage = friendMessage.GetComponent<RectTransform>();
        Image bubble = GameObject.FindObjectOfType<Image>();        //获得刚刚实例化的气泡
        RectTransform rtBubble = bubble.GetComponent<RectTransform>();

        //修改聊天label
        Text[] text = GameObject.FindObjectsOfType<Text>();
        //foreach(Text t in text)
        //    Debug.Log(t.name);
        text[1].text = message.username;
        //Debug.Log("--------------------------");
        Image[] image = GameObject.FindObjectsOfType<Image>();
        //foreach(Image i in image)
        //    Debug.Log(i.name);
        Sprite sprite = Resources.Load(message.headportrait, typeof(Sprite)) as Sprite;
        image[2].sprite = sprite;
        messageContent.text = message.message;    //消息赋给气泡的文本上
        
        //label的对齐方式
        float bubbleHeight = 40;
        bubbleHeight = (message.message.Length / fontCapacity) * fontSize + bubbleHeight; //计算一下聊天气泡需要变动的高度
        //修改label高度，增加间隙等
        rtFriendMessage.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, contentSize + Margins, bubbleHeight + nameHeight);
        rtFriendMessage.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, labelWidth);
        //修改content大小
        contentSize = contentSize + Margins + bubbleHeight + nameHeight;
        //修改气泡高度
        rtBubble.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 15, bubbleHeight);
        rtBubble.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 58, 180);

        KeepOnBottom();
    }

    //public void Dialog()
    //{
    //    GameObject dialog = Resources.Load("Prefabs/Dialog") as GameObject;
    //    GameObject canvas = GameObject.Find("Canvas");

    //    dialog = Instantiate(dialog);                       //实例化预制体
    //    dialog.transform.parent = canvas.transform;          //为预制体分配父类

    //    RectTransform rt = dialog.GetComponent<RectTransform>();

    //    rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 50, 400);
    //    rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 50, 400);
    //}

    public void Messages()
    {
        Debug.Log("You click on the MESSAGES button");
        ClickOnTheMessages();
    }
    public void Friends()
    {
        Debug.Log("You chick on the FRIENDS button");
        ClickOnTheFriends();

        GameObject contacts = Resources.Load("Prefabs/Contacts") as GameObject;

        contacts = Instantiate(contacts);                       //实例化预制体
        contacts.transform.SetParent(rtFriendsPanelContent.transform);
        
        i++;

        rtFriendsPanelContent.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, i * labelHeight);
        
        onlineUsers.text = "成员(" + i + ")";

        //预留以后点击出现对话框
        //Button button = contacts.GetComponent<Button>();
        //button.onClick.AddListener(Dialog);
    }

    public void Receive()
    {
        Message message = new Message();
        message.username = "小明";
        //message.ipaddress=
        //message.identity=
        message.message = "老师第二段是什么！";
        message.headportrait = "Monkey";
        queue.Enqueue(message);             //存入队列

        MessageEvent(this, message);        //产生事件显示到对话框

        //未读消息提示计数
        if (CHECKEDFRIENDS) {
            ++unDealMessages;
            if (unDealMessages == 100) unDealMessages = 99;
            remindText.text = unDealMessages.ToString();
            
            remind.enabled = true;
        }


    }
    public void Send()
    {
        GameObject myselfMessage = Resources.Load("Prefabs/MyselfMessage") as GameObject;

        myselfMessage = Instantiate(myselfMessage);
        myselfMessage.transform.SetParent(messagePanelContent.transform);

        Text message = GameObject.FindObjectOfType<Text>();         //获得刚实例化的文本
        Image bubble = GameObject.FindObjectOfType<Image>();        //获得刚实例化的的气泡
        RectTransform rtMyselfMessage = myselfMessage.GetComponent<RectTransform>();
        RectTransform rtBubble = bubble.GetComponent<RectTransform>();

        //修改聊天label
        Text[] text = GameObject.FindObjectsOfType<Text>();
        text[1].text = myName.text;
        Image[] image = GameObject.FindObjectsOfType<Image>();
        image[2].sprite = myHeadPortrait.sprite;
        message.text = inputField.text;      //输入框中的消息赋给气泡的文本上

        //label的对齐方式
        float bubbleHeight = 40;
        bubbleHeight = (message.text.Length / fontCapacity) * fontSize + bubbleHeight;

        rtMyselfMessage.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, contentSize + Margins, bubbleHeight + nameHeight);
        rtMyselfMessage.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, labelWidth);

        contentSize = contentSize + Margins + bubbleHeight + nameHeight;

        rtBubble.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 15, bubbleHeight);
        rtBubble.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 58, 180);

        KeepOnBottom();

        inputField.text = null;

    }

    private void KeepOnBottom()
    {
        if (contentSize < 400)
        {
            rtMessagePanelContent.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, contentSize);
        }
        else
        {
            rtMessagePanelContent.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, contentSize);
        }
    }
    
    private void GetComponentsRT()
    {
        rtRemind                = remind.GetComponent<RectTransform>();
        rtMESSAGEPANEL          = MESSAGEPANEL.GetComponent<RectTransform>();
        rtMessagePanelContent   = messagePanelContent.GetComponent<RectTransform>();
        rtFRIENDSPANEL          = FRIENDSPANEL.GetComponent<RectTransform>();
        rtFriendsPanelContent   = friendsPanelContent.GetComponent<RectTransform>();
        rtIdentityData          = identityData.GetComponent<RectTransform>();
    }

    private void ClickOnTheMessages()
    {
        //改变导航栏文字明暗
        textOfFriends.color = new Color(0, 0, 0, 0.5f);
        textOfMessages.color = new Color(0, 0, 0, 1f);
        //改变下划线位置
        RectTransform rt = underline.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 1, 130);
        
        remind.enabled = false;
        rtMESSAGEPANEL.SetAsLastSibling();

        CHECKEDMESSAGES = true;
        CHECKEDFRIENDS = false;

        unDealMessages = 0;
    }
    private void ClickOnTheFriends()
    {
        //改变导航栏文字明暗
        textOfFriends.color = new Color(0, 0, 0, 1f);
        textOfMessages.color = new Color(0, 0, 0, 0.5f);
        //改变下划线位置
        RectTransform rt = underline.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 1, 130);

        rtFRIENDSPANEL.SetAsLastSibling();

        CHECKEDFRIENDS = true;
        CHECKEDMESSAGES = false;
    }

    /// <summary>
    /// 修改个人信息的三个函数
    /// </summary>
    public void EditIdentityData()
    {
        identityData.SetActive(true);
        rtIdentityData.SetAsLastSibling();
    }
    //点击头像修改信息
    private void onClick(Transform trans)
    {
        Sprite sprite = Resources.Load(trans.name, typeof(Sprite)) as Sprite;
        myHeadPortrait.sprite = sprite;
    }
    private void Close()
    {
        identityData.SetActive(false);
        Text name = myName;
        Text newName = myNewName;
        if (newName.text!="")
        {
            name.text = newName.text;
        }
        
    }

}













